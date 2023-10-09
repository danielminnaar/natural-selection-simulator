using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OrganismController : MonoBehaviour
{

    public GameObject plane;              // Reference to the ground plane GameObject.
    public bool debugDrawSphere = true;
    public Text text;
    private Bounds planeBounds;
    private Vector3 targetPosition;
    private bool movingToFood = false;
    private bool lookingForMate = false;
    private bool movingToMate = false;
    private GameObject targetFood = null;
    private GameObject targetMate = null;
    private static readonly string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public Organism organism;
    private Animator animator;
    public string id;

    public void ApplyTraits(List<Trait> traitsToApply)
    {
        organism.traits = traitsToApply;
        Renderer renderer = GetComponentInChildren<Renderer>();
        foreach (Trait t in organism.traits)
        {
            switch (t.type)
            {
                case TraitType.SENSE:
                    {
                        SenseTrait sense = (SenseTrait)t;
                        organism.senseRadius = sense.SenseRadius;
                        //renderer.material = senseMaterial;
                        break;
                    }
                case TraitType.SPEED:
                    {
                        SpeedTrait speed = (SpeedTrait)t;
                        organism.moveSpeed = speed.MoveSpeed;
                        //renderer.material = speedMaterial;
                        break;
                    }
                case TraitType.SLOW_DIGESTION:
                    {
                        SlowDigestionTrait speed = (SlowDigestionTrait)t;
                        // not much to do here.
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
        id = organism.id;
    }

    void Start()
    {
        //
        animator = GetComponent<Animator>();

        if (organism.id == "")
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
        organism.hasMated = false;
        // Initialize the target position.
        ChooseNewTargetPosition();
    }

    private void RotateText()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        // Make the object point towards the camera, ignoring the vertical axis
        directionToCamera.y = 0;
        text.transform.rotation = Quaternion.LookRotation(-directionToCamera);
    }

    private void UpdateTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        transform.LookAt(targetPosition);
        RotateText();
        // Rotate the text to look at the camera
        //StartCoroutine(Animate(0.3f)); Fix this

        organism.energyUsed++;

    }

    private IEnumerator Animate(float time)
    {
        animator.SetFloat("Speed", time);
        float timer = time;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timer--;
        }
        animator.SetFloat("Speed", 0.0f);

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
        organism.id = idString;
    }

    private void CheckEnergy()
    {
        if (organism.energyUsed >= organism.maxEnergy)
        {
            organism.canMove = false;
         

        }

    }

    void Update()
    {

        CheckEnergy();
        if (organism.canMove)
        {
            // Move the capsule towards the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, organism.moveSpeed * Time.deltaTime);
            // if we haven't located food, not moving to a mate and dont have a mate, then find something else to do
            if (!movingToFood && !movingToMate)
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

    public void FoodEaten()
    {
        // Default behavior
        organism.maxEnergy++;

        foreach (Trait t in organism.traits)
        {
            t.OnEat(this.organism);
        }
    }

    private void Mated()
    {
        organism.energyUsed++;
    }

    private void UpdateText()
    {
        string traitsAndVals = "";
        bool multipleTraits = (organism.traits.Count > 1);
        foreach (Trait t in organism.traits)
        {
            switch (t.type)
            {
                case TraitType.SENSE:
                    traitsAndVals += " SENSE (" + organism.senseRadius.ToString("F2") + ") ";
                    break;

                case TraitType.SPEED:
                    traitsAndVals += " SPEED (" + organism.moveSpeed.ToString("F2") + ") ";
                    break;
                case TraitType.SLOW_DIGESTION:
                    traitsAndVals += " DIGESTION (" + organism.energyUsed.ToString() + "/" + organism.maxEnergy.ToString() + ") ";
                    break;
            }
            if (multipleTraits)
                traitsAndVals += Environment.NewLine;
        }
        // string food = "FOOD: " + organism.foodConsumed.ToString();

        // if (organism.hasMated)
        //     food += Environment.NewLine + "[ MATED ]";

        // if (organism.energyUsed >= organism.maxEnergy)
        //     food += Environment.NewLine + " [ NO ENERGY ]";

        text.text = traitsAndVals;
    }

    private bool RequestMate(OrganismController requestingMate)
    {
        // If we dont have a mate but are looking for one (or if we're already moving to the mate requesting)
        if (organism.mateId == "" && lookingForMate
        && (!movingToMate || targetMate == requestingMate.gameObject)
        && organism.foodConsumed > 0
        && organism.energyUsed < organism.maxEnergy)
        {
            targetMate = requestingMate.gameObject;
            UpdateTargetPosition(targetMate.transform.position);
            organism.startPosition = requestingMate.organism.startPosition; // go home with the mate
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
        var slimeTrfm = transform.Find("Slime");
        Collider[] colliders = Physics.OverlapSphere(slimeTrfm.position, organism.sphereRemoveRadius + organism.senseRadius);
        float closestDistance = Mathf.Infinity;
        GameObject closestFood = null;
        foreach (Collider collider in colliders)
        {
            var colliderOrg = collider.GetComponentInParent<OrganismController>();
            // find a mate if we've eaten
            if (collider.CompareTag("Capsule") && organism.foodConsumed > 0 && organism.mateId == "" && colliderOrg.organism.id != this.organism.id)
            {

                // make sure this potential mate is not any of the parents (MIGHT HAVE TO REVISE THIS IF WE ONLY SPAWN 2 AT START)
                if (organism.parentIds != null)
                {
                    if (colliderOrg.organism.id == organism.parentIds.Item1 || colliderOrg.organism.id == organism.parentIds.Item2)
                        break;
                }

                // check that the collider is a different org, that it has eaten and has no other mate yet (and not moving to one)
                if (colliderOrg.organism.foodConsumed > 0 && !movingToMate && (colliderOrg.RequestMate(this))) // Assuming mates have a "Capsule" tag.
                {
                    // mate accepted
                    movingToFood = false;
                    movingToMate = true;
                    UpdateTargetPosition(collider.gameObject.transform.parent.position);

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
            UpdateTargetPosition(new Vector3(newX, transform.position.y, newZ));
            targetFood = closestFood;
            movingToFood = true;
        } // no more food in range

    }


    void OnDrawGizmos()
    {
        if (debugDrawSphere)
        {
            var slimeTrfm = transform.Find("Slime");
            
            // Calculate the center of the transparent sphere to align with the capsule's center.
            //var cm = transform.position;
            
            //Vector3 sphereCenter = transform.GetComponentInChildren<Renderer>().transform.position;
              
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red color with transparency.
            Gizmos.DrawWireSphere(slimeTrfm.transform.position, organism.sphereRemoveRadius + organism.senseRadius);
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

        UpdateTargetPosition(new Vector3(newX, transform.position.y, newZ));
    }

    // Remove spheres within the specified radius around the capsule.
    private void CheckCollisions()
    {
        var slimeTrfm = transform.Find("Slime");
        Collider[] colliders = Physics.OverlapSphere(slimeTrfm.position, organism.sphereRemoveRadius);
        bool hasFood = false;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Sphere")) // Assuming spheres have a "Sphere" tag.
            {
                // whether we've targeted food or not we eat it
                organism.foodConsumed++;
                hasFood = true;
                FoodEaten();
                Destroy(collider.gameObject);
                // if we reach the targeted food
                if (movingToFood && collider.gameObject == targetFood)//(collider.transform.position.x == targetPosition.x && collider.transform.position.z == targetPosition.z))
                {
                    targetFood = null;
                    movingToFood = false;
                }
                if (organism.mateId == "")
                {
                    lookingForMate = true;
                }
                return; // dont concern ourselves about food anymore
            }
            else if (collider.CompareTag("Capsule"))
            {
                // make sure the collider is the same organism as our target mate
                OrganismController otherOrg = collider.GetComponentInParent<OrganismController>();
                if (otherOrg.organism.id != organism.id && targetMate != null)
                {
                    OrganismController targetOrg = targetMate.GetComponent<OrganismController>();
                    if (targetOrg.organism.id == otherOrg.organism.id && organism.mateId == "")
                    {
                        // we're close enough, assign the mate and  
                        // go back to this starting position with the mate

                        organism.mateId = otherOrg.organism.id;
                        organism.hasMated = true;
                        otherOrg.lookingForMate = false;
                        otherOrg.targetMate = null;
                        otherOrg.targetFood = null;
                        otherOrg.movingToFood = false;
                        otherOrg.movingToMate = false;
                        otherOrg.organism.hasMated = true;
                        otherOrg.Mated();
                        otherOrg.ChooseNewTargetPosition();
                        movingToMate = false;
                        Mated();
                        targetMate = null;
                        targetFood = null;
                        movingToFood = false;
                        movingToMate = false;

                        ChooseNewTargetPosition();
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
