using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject groundPlane;      // Reference to the ground plane GameObject.
    public GameObject capsulePrefab; // Reference to your capsule prefab.
    public GameObject foodPrefab;   // Reference to your food prefab.
    public int numberOfSpheres = 10;    // Number of spheres to spawn.
    public int initialPopulation = 20; // Initial number of capsules.
    public int maxGenerations = 100;   // Maximum number of generations.
    public float maxOrganismSpeed = 15.0f;
    public float generationTime = 5.0f; // The time for each generation in seconds.
    private bool isGenerationInProgress = false; // Flag to check if a generation is in progress.

    public int currentGeneration = 1;
    public List<GameObject> currentGenOrganisms;
    private List<GameObject> currentGenFood;
    private bool pause = false;

    void Start()
    {
        currentGenOrganisms = new List<GameObject>();
        currentGenFood = new List<GameObject>();
        InitializeSimulation();
    }
    void InitializeSimulation()
    {
        currentGeneration = 0;
        ScatterFoodSources();
        SpawnInitialPopulation();
        StartCoroutine(GenerationTimer());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) // If player wants to move on X and Z axis only
        {
            if (pause)
            {
                Time.timeScale = 1f;
                pause = false;
            }

            else
            {
                Time.timeScale = 0f;
                pause = true;
            }

        }
    }


    // Coroutine to run the generation timer.
    IEnumerator GenerationTimer()
    {
        isGenerationInProgress = true;
        float timer = generationTime;
        for (int i = 0; i < currentGenOrganisms.Count; i++)
        {
            currentGenOrganisms[i].GetComponent<OrganismController>().canMove = true;
        }
        while (timer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timer--;

        }

        // Generation is complete.
        isGenerationInProgress = false;
        Debug.Log("Generation Complete");
        float avgGenSpeed = 0.0f;
        float avgGenSense = 0.0f;
        for (int i = 0; i < currentGenOrganisms.Count; i++)
        {
            currentGenOrganisms[i].GetComponent<OrganismController>().canMove = false;
            avgGenSpeed += currentGenOrganisms[i].GetComponent<OrganismController>().moveSpeed;
            avgGenSense += currentGenOrganisms[i].GetComponent<OrganismController>().senseRadius;
        }
        avgGenSpeed = avgGenSpeed / currentGenOrganisms.Count;
        avgGenSense = avgGenSense / currentGenOrganisms.Count;
        Debug.Log("Average speed: " + avgGenSpeed.ToString());
        Debug.Log("Average sense: " + avgGenSense.ToString());
        StartNewGeneration();
        currentGeneration++;
        // Perform generation-related actions here.
    }

    private OrganismController CopyTraits(OrganismController fromOrganism)
    {
        OrganismController newOrg = new OrganismController();
        newOrg.moveSpeed = fromOrganism.moveSpeed;
        return newOrg;
    }

    private void StartNewGeneration()
    {
        int fedCount = 0;
        for (int i = currentGenOrganisms.Count - 1; i >= 0; i--)
        {
            // Remove all organism game objects from the game
            Destroy(currentGenOrganisms[i]);
            // Remove organisms that haven't eaten
            if (currentGenOrganisms[i].GetComponent<OrganismController>().foodConsumed == 0)
            {
                currentGenOrganisms.RemoveAt(i);
            }
            else
            {
                // copy and replicate this organism
                currentGenOrganisms[i].GetComponent<OrganismController>().foodConsumed = 0;
                fedCount++;
                currentGenOrganisms.Add(currentGenOrganisms[i]);
            }
        }
        Debug.Log("Organisms fed: " + fedCount.ToString());
        SpawnPopulation();
        ScatterFoodSources();
        if (!isGenerationInProgress)
        {
            StartCoroutine(GenerationTimer());
        }
    }

    private void SpawnPopulation()
    {
        Debug.Log("Spawning " + (currentGenOrganisms.Count).ToString() + " organisms.");
        for (int i = 0; i < currentGenOrganisms.Count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject capsule = Instantiate(capsulePrefab, spawnPosition, Quaternion.identity);
            Renderer capsuleRenderer = capsule.GetComponentInChildren<Renderer>();
            float capsuleHeight = capsuleRenderer.bounds.size.y;
            capsule.transform.position = new Vector3(capsule.transform.position.x, groundPlane.transform.position.y + capsuleHeight + (capsuleHeight / 2.0f), capsule.transform.position.z);
            OrganismController organism = capsule.GetComponent<OrganismController>();
            if (organism != null)
            {
                organism.plane = groundPlane;
                organism.foodConsumed = 0;
                organism.ApplyTrait(TraitsManager.GenerateTrait(organism.trait));
                // organism.moveSpeed = ImproveMoveSpeed(currentGenOrganisms[i].GetComponent<OrganismController>().moveSpeed);
                // organism.senseRadius = ImproveSense(organism.senseRadius);
            }
            currentGenOrganisms[i] = capsule;
        }
    }

    private GameObject SetTraits(GameObject organism)
    {
        OrganismController org = organism.GetComponent<OrganismController>();
        org.trait = TraitsManager.GenerateTrait(org.trait);
        return organism;

    }

    void SpawnInitialPopulation()
    {
        bool testing = false; // todo: create a better test scenario framework
        // Spawn capsules at random positions as your initial population.
        for (int i = 0; i < initialPopulation; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(); // Implement this method.
            GameObject capsule = Instantiate(capsulePrefab, spawnPosition, Quaternion.identity);
            Renderer capsuleRenderer = capsule.GetComponentInChildren<Renderer>();
            float capsuleHeight = capsuleRenderer.bounds.size.y;
            capsule.transform.position = new Vector3(capsule.transform.position.x, groundPlane.transform.position.y + capsuleHeight + (capsuleHeight / 2.0f), capsule.transform.position.z);
            // Set the "plane" reference in the capsule script.
            OrganismController organism = capsule.GetComponent<OrganismController>();
            if (organism != null)
            {
                organism.plane = groundPlane;
                if (testing)
                {
                    organism.ApplyTrait(TestSenseOrganisms());
                }
                else
                {
                    organism.ApplyTrait(TraitsManager.GenerateTrait(null));
                }


                currentGenOrganisms.Add(capsule);
            }
        }
    }

    private SenseTrait TestSenseOrganisms()
    {

        return new SenseTrait(50);

    }

    void ScatterFoodSources()
    {
        // Reset food in game
        foreach (GameObject food in currentGenFood)
        {
            Destroy(food);
        }
        currentGenFood.Clear();
        // Scatter food sources randomly around the plane.
        // Implement the logic to spawn food sources similar to capsule spawning.
        // Ensure the ground plane and sphere prefab are assigned.
        if (groundPlane == null || foodPrefab == null)
        {
            Debug.LogError("GameController: Ground Plane or Food Prefab not assigned.");
            return;
        }

        // Get the ground plane's size and position.
        Renderer groundRenderer = groundPlane.GetComponent<Renderer>();
        Vector3 groundPosition = groundPlane.transform.position;
        Vector3 groundSize = groundRenderer.bounds.size;

        // Spawn the specified number of spheres on top of the ground plane.
        for (int i = 0; i < numberOfSpheres; i++)
        {
            // Calculate random positions on top of the ground plane.
            float randomX = UnityEngine.Random.Range(groundPosition.x - groundSize.x / 2f, groundPosition.x + groundSize.x / 2f);
            float randomZ = UnityEngine.Random.Range(groundPosition.z - groundSize.z / 2f, groundPosition.z + groundSize.z / 2f);

            // Ensure the spheres are placed on top of the ground plane.
            float terrainHeight = groundPosition.y + groundRenderer.bounds.extents.y + 0.3f;
            Vector3 spawnPosition = new Vector3(randomX, terrainHeight, randomZ);

            // Instantiate a sphere at the calculated position.
            GameObject food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            currentGenFood.Add(food);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (groundPlane == null)
        {
            Debug.LogError("Plane GameObject reference is missing.");
            return Vector3.zero;
        }

        Renderer planeRenderer = groundPlane.GetComponent<Renderer>();

        if (planeRenderer == null)
        {
            Debug.LogError("Plane does not have a Renderer component.");
            return Vector3.zero;
        }

        // Get the dimensions of the plane using the minimum bounds values.
        float planeMinX = planeRenderer.bounds.min.x;
        float planeMinZ = planeRenderer.bounds.min.z;
        float planeWidth = planeRenderer.bounds.size.x;
        float planeLength = planeRenderer.bounds.size.z;

        float x, z;

        // Determine which edge to spawn on (0, 1, 2, or 3)
        int edge = UnityEngine.Random.Range(0, 4);

        switch (edge)
        {
            case 0: // Spawn on the top edge
                x = UnityEngine.Random.Range(planeMinX, planeMinX + planeWidth);
                z = planeMinZ + planeLength;
                break;
            case 1: // Spawn on the bottom edge
                x = UnityEngine.Random.Range(planeMinX, planeMinX + planeWidth);
                z = planeMinZ;
                break;
            case 2: // Spawn on the left edge
                x = planeMinX;
                z = UnityEngine.Random.Range(planeMinZ, planeMinZ + planeLength);
                break;
            case 3: // Spawn on the right edge
                x = planeMinX + planeWidth;
                z = UnityEngine.Random.Range(planeMinZ, planeMinZ + planeLength);
                break;
            default:
                x = planeMinX;
                z = planeMinZ;
                break;
        }

        float y = groundPlane.transform.position.y + capsulePrefab.transform.localScale.y / 2;
        return new Vector3(x, y, z);
    }
}
