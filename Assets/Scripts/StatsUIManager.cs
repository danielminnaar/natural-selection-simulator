using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class StatsUIManager : MonoBehaviour
{
    public Text text;
    public GameController game;
    private Dictionary<int, int> speedFoodEaten = new Dictionary<int, int>();
    private Dictionary<int, int> senseFoodEaten = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // string heading = "SIMULATION STATS";
        string generation = "GENERATION " + game.currentGeneration.ToString() + " (" + game.timer + ")";
        // string organisms = "ORGANISMS: " + game.currentGenOrganisms.Count.ToString();
        // organisms += Environment.NewLine;
        // int speedTraits = 0;
        // int senseTraits = 0;
        // string senseOrgs = "SENSERS: ";
        // string speedOrgs = "SPEEDERS: ";
        // int totalSenseFood = 0;
        // int totalSpeedFood = 0;
        // foreach (GameObject obj in game.currentGenOrganisms)
        // {
        //     OrganismController org = obj.GetComponent<OrganismController>();
        //     if (org.trait.type == TraitType.SENSE)
        //     {
        //         if (!senseFoodEaten.ContainsKey(game.currentGeneration))
        //             senseFoodEaten.Add(game.currentGeneration, org.foodConsumed);
        //         senseTraits++;
        //         totalSenseFood += org.foodConsumed;
        //         senseOrgs += org.senseRadius.ToString() + ", ";
        //     }

        //     else if (org.trait.type == TraitType.SPEED)
        //     {
        //         if (!speedFoodEaten.ContainsKey(game.currentGeneration))
        //             speedFoodEaten.Add(game.currentGeneration, org.foodConsumed);
        //         totalSpeedFood += org.foodConsumed;
        //         speedTraits++;
        //         speedOrgs += org.moveSpeed.ToString() + ", ";
        //     }


        // }
        // speedFoodEaten[game.currentGeneration] = totalSpeedFood;
        // senseFoodEaten[game.currentGeneration] = totalSenseFood;

        // string currentGenSenseFood = "G" + game.currentGeneration + ": " + senseFoodEaten[game.currentGeneration];
        // string currentGenSpeedFood = "G" + game.currentGeneration + ": " + speedFoodEaten[game.currentGeneration];
        // if (game.currentGeneration > 0)
        // {
        //     currentGenSenseFood += " (G" + (game.currentGeneration - 1).ToString() + ": " + senseFoodEaten[game.currentGeneration - 1] + ")";
        //     currentGenSpeedFood += " (G" + (game.currentGeneration - 1).ToString() + ": " + speedFoodEaten[game.currentGeneration - 1] + ")";
        // }

        // organisms += "SENSE ORGS: " + senseTraits.ToString() + " - " + senseOrgs + currentGenSenseFood + Environment.NewLine + "SPEED ORGS: " + speedTraits.ToString() + " = " + speedOrgs + currentGenSpeedFood;

        text.text =  generation;
    }
}
