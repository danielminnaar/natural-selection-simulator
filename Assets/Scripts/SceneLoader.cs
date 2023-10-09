using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TMP_InputField GenTimeInput;
    public TMPro.TMP_InputField InitialSpawnInput;
    public TMPro.TMP_InputField InitialFoodInput;
    public TMPro.TMP_InputField WorldSizeInput;
    public Toggle SpeedToggleInput;
    public Toggle SenseToggleInput;
    public Toggle DigestionToggleInput;

    public Button startButton;
    void Start()
    {
        Cursor.visible = true;
        startButton.onClick.AddListener(StartSimulator);
    }

    void StartSimulator()
    {
        GameController.Instance.initialPopulation = int.Parse(InitialSpawnInput.text);
        GameController.Instance.numberOfFood = int.Parse(InitialFoodInput.text);
        GameController.Instance.generationTime = int.Parse(GenTimeInput.text);
        GameController.Instance.worldSize = int.Parse(WorldSizeInput.text);
        GameController.Instance.allowedTraits = new List<TraitType>();
        if (SpeedToggleInput.isOn)
            GameController.Instance.allowedTraits.Add(TraitType.SPEED);
        if (SenseToggleInput.isOn)
            GameController.Instance.allowedTraits.Add(TraitType.SENSE);
        if (DigestionToggleInput.isOn)
            GameController.Instance.allowedTraits.Add(TraitType.SLOW_DIGESTION);
        if (GameController.Instance.allowedTraits.Count > 0)
        {
            SceneManager.LoadScene("SimulatorScene", LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameController.Instance.InitializeSimulation();
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Unsubscribe from the event
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartSimulator(); 
    }
}
