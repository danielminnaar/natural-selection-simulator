using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrganismController : MonoBehaviour
{

    public float moveSpeed = 3.0f;        // Speed at which the capsule moves.
    public float senseRadius = 0.0f; // Radius that the capsule can detect objects
    public GameObject plane;              // Reference to the ground plane GameObject.
    public float sphereRemoveRadius = 0.5f; // Radius within which spheres will be removed.
    public bool debugDrawSphere = true;
    private Bounds planeBounds;
    private Vector3 targetPosition;
    public int foodConsumed = 0;
    public bool canMove = false;
    private bool movingToFood = false;

    // sense thoughts
    // the sense can be added to the sphere remove radius for detection
    // but can only remove the sphere if its within the sphereRemoveRadius (1.5f)
    // but the organism can change direction to intersect directly with the sphere
    // if its within the sense radius

    void Start()
    {

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

    void Update()
    {
        if (canMove)
        {
            // Move the capsule towards the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if(!movingToFood)
            {
                DetectFoodAndChangTargetPosition();
                // Check if the capsule has reached the target position.
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    // Choose a new random target position within the plane.
                    ChooseNewTargetPosition();
                }
            }
            // Check for spheres within the remove radius and remove them.
            RemoveSpheresWithinRadius();
        }
    }

    private void DetectFoodAndChangTargetPosition()
    {
        if (senseRadius > 0.0f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.GetChild(0).position, sphereRemoveRadius + senseRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Sphere")) // Assuming food have a "Sphere" tag.
                {
                    // change the target 
                    float newX = collider.transform.position.x;
                    float newZ = collider.transform.position.z;
                    targetPosition = new Vector3(newX, transform.position.y, newZ);
                    movingToFood = true;
                    break;
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        if (debugDrawSphere)
        {

            // Calculate the center of the transparent sphere to align with the capsule's center.
            Vector3 sphereCenter = transform.GetChild(0).position;

            // Draw a transparent sphere with the center aligned to the capsule's center.
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


        newX = Random.Range(planeBounds.min.x, planeBounds.max.x);
        newZ = Random.Range(planeBounds.min.z, planeBounds.max.z);


        targetPosition = new Vector3(newX, transform.position.y, newZ);
    }

    // Remove spheres within the specified radius around the capsule.
    private void RemoveSpheresWithinRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.GetChild(0).position, sphereRemoveRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Sphere")) // Assuming spheres have a "Sphere" tag.
            {
                foodConsumed++;
                Destroy(collider.gameObject);
                // if we reach the detected food
                if (movingToFood && (collider.transform.position.x == targetPosition.x && collider.transform.position.z == targetPosition.z))
                {
                    movingToFood = false;
                }
            }
        }
    }
}
