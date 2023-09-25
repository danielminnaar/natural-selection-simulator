using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class OrganismController : MonoBehaviour
{

    public float moveSpeed = 3.0f;        // Speed at which the capsule moves.
    public float senseRadius = 0.0f; // Radius that the capsule can detect objects
    public GameObject plane;              // Reference to the ground plane GameObject.
    public float sphereRemoveRadius = 0.5f; // Radius within which spheres will be removed.
    public bool debugDrawSphere = true;
    public Text text;
    private Bounds planeBounds;
    private Vector3 targetPosition;
    public int foodConsumed = 0;
    public bool canMove = false;
    public Vector3 startPosition;
    private bool movingToFood = false;
    public bool lookingForMate = false;
    public bool movingToMate = false;
    public List<Trait> traits;
    private GameObject targetFood = null;
    private GameObject targetMate = null;
    public GameObject mate = null;
    public List<GameObject> parents = new List<GameObject>();
    private static readonly string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public string id = "";



    public void ApplyTraits(List<Trait> traitsToApply)
    {
        traits = traitsToApply;
        Renderer renderer = GetComponentInChildren<Renderer>();
        foreach (Trait t in traits)
        {
            switch (t.type)
            {
                case TraitType.SENSE:
                    {
                        SenseTrait sense = (SenseTrait)t;
                        senseRadius = sense.SenseRadius;
                        //renderer.material = senseMaterial;
                        break;
                    }
                case TraitType.SPEED:
                    {
                        SpeedTrait speed = (SpeedTrait)t;
                        moveSpeed = speed.MoveSpeed;
                        //renderer.material = speedMaterial;
                        break;
                    }
            }
        }
        // Create a new material
        Material randomMaterial = new Material(Shader.Find("Standard"));
        // Generate a random color
        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        // Assign the random color to the material's main color property
        randomMaterial.color = randomColor;
        // Assign the new material to the targetRenderer
        renderer.material = randomMaterial;

    }

    void Start()
    {

        GenerateId();
        // Get the bounds of the ground plane.
        Renderer groundRenderer = plane.GetComponent<Renderer>();
        if (groundRenderer != null)
        {
            planeBounds = groundRenderer.bounds;
        }
        else
        {
            Debug.LogError("CapsuleController: Renderer not found on the ground plane GameObject.");
        }

        // Initialize the target position.
        ChooseNewTargetPosition();
    }

    private void GenerateId()
    {
        System.Random random = new System.Random();
        char[] result = new char[5];

        for (int i = 0; i < 5; i++)
        {
            result[i] = characters[random.Next(characters.Length)];
        }
        var idString = new string(result);
        id = idString;
    }

    void Update()
    {

        if (canMove)
        {
            // Move the capsule towards the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            // if we haven't located food, not moving to a mate and dont have a mate, then find something else to do
            if (!movingToFood && !movingToMate && mate == null)
            {
                DetectAndChangeTargetPosition();
                // Check if the capsule has reached the target position.
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    // Choose a new random target position within the plane.
                    ChooseNewTargetPosition();
                }
            }
            // Check for spheres / mates within the collect radius
            CheckCollisions();
        }
        UpdateText();
    }

    private void UpdateText()
    {
        string traitsAndVals = "";
        foreach (Trait t in traits)
        {
            switch (t.type)
            {
                case TraitType.SENSE:
                    traitsAndVals += " SENSE (" + senseRadius + ") ";
                    break;

                case TraitType.SPEED:
                    traitsAndVals += " SPEED (" + moveSpeed + ") ";
                    break;
            }
        }
        string food = "FOOD: " + foodConsumed.ToString();
        text.text = traitsAndVals + Environment.NewLine + food;
    }

    private bool RequestMate(OrganismController requestingMate)
    {
        // If we dont have a mate but are looking for one (or if we're already moving to the mate requesting)
        if (mate == null && lookingForMate && (!movingToMate || targetMate == requestingMate.gameObject) && foodConsumed > 0)
        {
            targetMate = requestingMate.gameObject;
            targetPosition = targetMate.transform.position;
            startPosition = requestingMate.startPosition; // go home with the mate
            movingToMate = true;
            movingToFood = false;
            return true;
        }
        else
            return false;
    }

    // mating thoughts
    // if the org has food, then it will continue to look for food, but also a mate
    // if the mate has also eaten, then that mate is priority.
    // we need to store info about the mate - need to make sure that only 1 reproduces.
    private void DetectAndChangeTargetPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.GetChild(0).position, sphereRemoveRadius + senseRadius);
        float closestDistance = Mathf.Infinity;
        GameObject closestFood = null;
        foreach (Collider collider in colliders)
        {
            var colliderOrg = collider.GetComponentInParent<OrganismController>();
            // find a mate if we've eaten
            if (collider.CompareTag("Capsule") && foodConsumed > 0 && mate == null && colliderOrg.id != this.id)
            {

                // make sure this potential mate is not any of the parents (MIGHT HAVE TO REVISE THIS IF WE ONLY SPAWN 2 AT START)
                if (parents.Count == 2)
                {
                    var p1 = parents[0].GetComponentInParent<OrganismController>().id;
                    var p2 = parents[1].GetComponentInParent<OrganismController>().id;
                    if (colliderOrg.id == p1 || colliderOrg.id == p2)
                        break;
                }

                // check that the collider is a different org, that it has eaten and has no other mate yet (and not moving to one)
                if (colliderOrg.foodConsumed > 0 && !movingToMate && (colliderOrg.RequestMate(this))) // Assuming mates have a "Capsule" tag.
                {
                    // mate accepted
                    movingToFood = false;
                    movingToMate = true;
                    targetPosition = collider.gameObject.transform.parent.position;

                    return; //ignore other colliders
                }

            }
            if (collider.CompareTag("Sphere")) // Assuming food have a "Sphere" tag.
            {
                Transform foodTransform = collider.transform;

                // Calculate the distance to the food object
                float distance = Vector3.Distance(transform.position, foodTransform.position);

                // Check if this food object is closer than the current closest
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFood = collider.gameObject;
                }

            }

        }
        if (closestFood != null && !movingToMate)
        {
            // change the target 
            float newX = closestFood.transform.position.x;
            float newZ = closestFood.transform.position.z;
            targetPosition = new Vector3(newX, transform.position.y, newZ);
            targetFood = closestFood;
            movingToFood = true;
        } // no more food in range

    }


    void OnDrawGizmos()
    {
        if (debugDrawSphere)
        {

            // Calculate the center of the transparent sphere to align with the capsule's center.
            Vector3 sphereCenter = transform.GetChild(0).position;

            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red color with transparency.
            Gizmos.DrawWireSphere(sphereCenter, sphereRemoveRadius + senseRadius);
        }
    }


    // Choose a new random target position within the plane.
    private void ChooseNewTargetPosition()
    {
        float newX = 0;
        float newZ = 0;
        // determine if sense picks up any food first


        newX = UnityEngine.Random.Range(planeBounds.min.x, planeBounds.max.x);
        newZ = UnityEngine.Random.Range(planeBounds.min.z, planeBounds.max.z);


        targetPosition = new Vector3(newX, transform.position.y, newZ);
    }

    // Remove spheres within the specified radius around the capsule.
    private void CheckCollisions()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.GetChild(0).position, sphereRemoveRadius);
        bool hasFood = false;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Sphere")) // Assuming spheres have a "Sphere" tag.
            {
                // whether we've targeted food or not we eat it
                foodConsumed++;
                hasFood = true;
                Destroy(collider.gameObject);
                // if we reach the targeted food
                if (movingToFood && collider.gameObject == targetFood)//(collider.transform.position.x == targetPosition.x && collider.transform.position.z == targetPosition.z))
                {
                    targetFood = null;
                    movingToFood = false;
                }
                if (mate == null)
                {
                    lookingForMate = true;
                }
                return; // dont concern ourselves about food anymore
            }
            else if (collider.CompareTag("Capsule"))
            {
                // make sure the collider is the same organism as our target mate
                OrganismController otherOrg = collider.GetComponentInParent<OrganismController>();
                if (otherOrg.id != id && targetMate != null)
                {
                    OrganismController targetOrg = targetMate.GetComponent<OrganismController>();
                    if (targetOrg.id == otherOrg.id && mate == null)
                    {
                        // we're close enough, assign the mate and go back to this starting position with the mate
                        mate = collider.gameObject;
                        movingToMate = false;
                        targetMate = null;
                        targetOrg.targetPosition = startPosition;
                        targetPosition = startPosition;
                    }

                }



            }
        }
        // if we dont find the food anymore (someone else got it first) then keep searching
        if (!hasFood && movingToFood && targetFood == null)
        {
            ChooseNewTargetPosition();
            movingToFood = false;
        }

    }
}
