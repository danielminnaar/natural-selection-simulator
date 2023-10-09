using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject groundPlane;      // Reference to the ground plane GameObject.
    public List<GameObject> capsulePrefabs;
    public GameObject foodPrefab;   // Reference to food prefab.
    public int numberOfFood = 10;    // Number of spheres to spawn.
    public int initialPopulation = 20; // Initial number of capsules.
    public float generationTime = 5.0f; // The time for each generation in seconds.
    public int worldSize = 5;
    private Text generationText;
    private bool isGenerationInProgress = false; // Flag to check if a generation is in progress.
    public List<TraitType> allowedTraits = new List<TraitType>();
    public int currentGeneration = 1;
    public List<GameObject> currentGenGameObjects;
    public List<Organism> currentGenOrganisms;
    private List<GameObject> currentGenFood;
    private bool pause = false;
    public float timer = 0f;
    private ChartController chartController;
    private Dictionary<int, List<float>> simulationStats;

    public static GameController Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {


    }
    public void InitializeSimulation()
    {
        if (groundPlane == null)
            groundPlane = GameObject.Find("Plane");
        if (foodPrefab == null)
            foodPrefab = Resources.Load<GameObject>("Prefabs/Sphere");
        if (capsulePrefabs.Count == 0)
        {
            capsulePrefabs.Add(Resources.Load<GameObject>("Prefabs/Slime_03"));
            capsulePrefabs.Add(Resources.Load<GameObject>("Prefabs/Slime_03 Leaf"));
            capsulePrefabs.Add(Resources.Load<GameObject>("Prefabs/Slime_03 Sprout"));
        }
        groundPlane.transform.localScale = new Vector3((float)worldSize, (float)worldSize, (float)worldSize);
        chartController = FindObjectOfType<ChartController>();
        currentGenOrganisms = new List<Organism>();
        currentGenGameObjects = new List<GameObject>();
        currentGenFood = new List<GameObject>();
        simulationStats = new Dictionary<int, List<float>>();
        generationText = GameObject.Find("Stats").GetComponent<Text>();
        TraitsManager.allowedTraits = allowedTraits;
        Cursor.visible = false;
        ScatterFoodSources();
        SpawnInitialPopulation();
        StartCoroutine(GenerationTimer());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // pause game
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
        if (Input.GetKeyDown(KeyCode.F)) // toggle fast foward
        {
            if(Time.timeScale == 1.0f)
                Time.timeScale = 5.0f;
            else
                Time.timeScale = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

            // If you're testing in the editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if (Input.GetKeyDown(KeyCode.E)) // toggle fast foward
        {
            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSimulation();
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }


    private void ResetSimulation()
    {
        isGenerationInProgress = false;
        timer = 0;
        StopAllCoroutines();
        Time.timeScale = 1;
        currentGeneration = 1;
        foreach (GameObject obj in currentGenGameObjects)
            Destroy(obj);
        foreach (GameObject obj in currentGenFood)
            Destroy(obj);
        currentGenGameObjects = new List<GameObject>();
        currentGenOrganisms = new List<Organism>();
        currentGenFood = new List<GameObject>();
        simulationStats = new Dictionary<int, List<float>>();
        chartController.Reset();
    }


    // Coroutine to run the generation timer.
    IEnumerator GenerationTimer()
    {
        isGenerationInProgress = true;
        timer = generationTime;
        for (int i = 0; i < currentGenGameObjects.Count; i++)
        {
            currentGenGameObjects[i].GetComponent<OrganismController>().organism.canMove = true;
        }
        while (timer > 0)
        {
            string updateText = "GENERATION " + currentGeneration + " (" + timer + ")";
            generationText.text =  updateText;
            yield return new WaitForSeconds(1.0f);
            timer--;

        }

        // Generation is complete.
        isGenerationInProgress = false;

        for (int i = 0; i < currentGenGameObjects.Count; i++)
        {
            currentGenGameObjects[i].GetComponent<OrganismController>().organism.canMove = false;

        }
        UpdateChart();
        // calculate 
        StartNewGeneration();
        currentGeneration++;
        // Perform generation-related actions here.
    }

    private void UpdateChart()
    {
        // calculate median values for each trait type and update the chart bars
        List<float> speedValues = new List<float>();
        List<float> senseValues = new List<float>();
        List<float> digestValues = new List<float>();
        currentGenOrganisms.ForEach(org =>
        {
            org.traits.ForEach(orgTrait =>
            {
                if (orgTrait.type == TraitType.SPEED)
                    speedValues.Add(org.moveSpeed);
                if (orgTrait.type == TraitType.SENSE)
                    senseValues.Add(org.senseRadius);
                if (orgTrait.type == TraitType.SLOW_DIGESTION)
                    digestValues.Add(org.maxEnergy);
            });
        });

        float medianSpeed = (speedValues.Count > 0 ? CalculateMedian(speedValues.ToArray()) : 0.0f);
        float medianSense = (senseValues.Count > 0 ? CalculateMedian(senseValues.ToArray()) : 0.0f);
        float medianDigest = (digestValues.Count > 0 ? CalculateMedian(digestValues.ToArray()) : 0.0f);
        simulationStats.Add(currentGeneration, new List<float>() { medianSpeed, medianSense, medianDigest });
        //simulationStats.Add(currentGeneration, )
        //chartController.UpdateBars(medianSpeed, medianSense, medianDigest);
        chartController.UpdateBars(simulationStats);

    }

    private float CalculateMedian(float[] values)
    {
        if (values.Length == 0)
        {
            throw new InvalidOperationException("Cannot calculate median for an empty set.");
        }

        var sortedValues = values.OrderBy(v => v).ToArray();

        int middle = sortedValues.Length / 2;

        // If even, average the two middle values
        if (sortedValues.Length % 2 == 0)
        {
            return (sortedValues[middle - 1] + sortedValues[middle]) / 2.0f;
        }
        // If odd, return the middle value
        else
        {
            return sortedValues[middle];
        }
    }

    private void CalculateReplications()
    {
        // those that ate remain
        // those that mated add 1 child
        List<int> orgIndexesToRemove = new List<int>();
        List<int> gameObjIndexesToRemove = new List<int>();
        List<OrganismController> orgsToAdd = new List<OrganismController>();

        List<(OrganismController Parent1, OrganismController Parent2)> parentMatches = new List<(OrganismController, OrganismController)>();

        for (int i = currentGenGameObjects.Count - 1; i >= 0; i--)
        {
            // Remove all organism game objects from the game
            Destroy(currentGenGameObjects[i]);
            gameObjIndexesToRemove.Add(i);
            // Remove organisms that haven't eaten
            var org = currentGenGameObjects[i].GetComponent<OrganismController>();
            if (org.organism.foodConsumed == 0)
            {
                orgIndexesToRemove.Add(i);
            }
            else
            {

                org.organism.foodConsumed = 0;
                // copy and replicate this organism if it's with child
                // assumption that only 1 parent has a reference to the other
                if (org.organism.mateId != "")
                {
                    var parent1 = org;
                    OrganismController parent2 = null;
                    currentGenGameObjects.ForEach(p2 =>
                    {
                        var p2Org = p2.GetComponent<OrganismController>();
                        if (p2Org.organism.id == org.organism.mateId)
                            parent2 = p2Org;

                    });

                    if (parent2 != null)
                    {
                        // Add the pair of parents to the list
                        parentMatches.Add((parent1, parent2));
                    }
                }

                // Update parent traits (child inherits previous gen traits)
                org.ApplyTraits(TraitsManager.GenerateParentTraits(org.organism.traits));
                currentGenOrganisms[i] = org.organism;
            }
        }

        foreach (var o in orgIndexesToRemove)
        {
            currentGenOrganisms.RemoveAt(o);
        }
        foreach (var go in gameObjIndexesToRemove)
        {
            currentGenGameObjects.RemoveAt(go);
        }


        // Generate children
        foreach (var parentMatch in parentMatches)
        {
            var childOrg = new Organism();
            childOrg.parentIds = new Tuple<string, string>(parentMatch.Parent1.organism.id, parentMatch.Parent2.organism.id);
            //childOrg.startPosition = GetRandomSpawnPosition(); // parentMatch.Parent1.organism.startPosition; // spawn with parents
            childOrg.id = "";
            childOrg.traits = DetermineChildTraits(parentMatch.Parent1, parentMatch.Parent2);
            currentGenOrganisms.Add(childOrg);
        }


    }

    private List<Trait> DetermineChildTraits(OrganismController parent1, OrganismController parent2)
    {
        return TraitsManager.GenerateChildTraits(parent1.organism.traits, parent2.organism.traits);
    }


    private void StartNewGeneration()
    {
        CalculateReplications();
        SpawnPopulation();
        ScatterFoodSources();
        if (!isGenerationInProgress)
        {
            StartCoroutine(GenerationTimer());
        }
    }

    private GameObject GetOrganismPrefab()
    {
        int randIndex = UnityEngine.Random.Range(0, capsulePrefabs.Count);
        return capsulePrefabs[randIndex];
    }

    private void SpawnPopulation()
    {

        for (int i = 0; i < currentGenOrganisms.Count; i++)
        {
            GameObject prefab = GetOrganismPrefab();
            Vector3 spawnPosition = GetRandomSpawnPosition(prefab);
            // randomy select a pref
            GameObject capsule = Instantiate(prefab, spawnPosition, Quaternion.identity);
            // Collider capsuleRenderer = capsule.GetComponent<Collider>();
            // float capsuleHeight = capsuleRenderer.bounds.size.y;
            OrganismController orgController = capsule.GetComponent<OrganismController>();
            // update the spawn position to be on top of plane
            spawnPosition = new Vector3(capsule.transform.position.x, groundPlane.transform.position.y, capsule.transform.position.z);
            capsule.transform.position = spawnPosition;
            orgController.plane = groundPlane;
            orgController.organism = new Organism();
            orgController.organism.startPosition = spawnPosition;
            orgController.organism.id = currentGenOrganisms[i].id;
            orgController.organism.foodConsumed = 0;
            orgController.organism.parentIds = currentGenOrganisms[i].parentIds;
            orgController.organism.startPosition = currentGenOrganisms[i].startPosition;
            orgController.ApplyTraits(currentGenOrganisms[i].traits);
            currentGenGameObjects.Add(capsule);
        }
    }

    void SpawnInitialPopulation()
    {
        currentGenOrganisms.Clear();
        currentGenGameObjects.Clear();
        bool testing = false; // todo: create a better test scenario framework
        // Spawn capsules at random positions as your initial population.
        for (int i = 0; i < initialPopulation; i++)
        {
            GameObject prefab = GetOrganismPrefab();
            Vector3 spawnPosition = GetRandomSpawnPosition(prefab);
            GameObject capsule = Instantiate(prefab, spawnPosition, Quaternion.identity);
            //Renderer capsuleRenderer = capsule.GetComponentInChildren<Renderer>();
            //float capsuleHeight = capsuleRenderer.bounds.size.y; 
            // update the spawn position to be on top of plane
            spawnPosition = new Vector3(capsule.transform.position.x, groundPlane.transform.position.y, capsule.transform.position.z);
            capsule.transform.position = spawnPosition;
            // Set the "plane" reference in the capsule script.
            OrganismController orgController = capsule.GetComponent<OrganismController>();
            orgController.organism = new Organism();
            orgController.organism.startPosition = spawnPosition;
            orgController.plane = groundPlane;

            if (testing)
            {
                orgController.ApplyTraits(new List<Trait>() { TestSenseOrganisms() });
            }
            else
            {
                orgController.ApplyTraits(TraitsManager.GenerateParentTraits(null));
            }

            currentGenOrganisms.Add(orgController.organism);
            currentGenGameObjects.Add(capsule);
        }
    }

    private SenseTrait TestSenseOrganisms()
    {

        return new SenseTrait();

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
        for (int i = 0; i < numberOfFood; i++)
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

    Vector3 GetRandomSpawnPosition(GameObject capsulePrefab)
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
