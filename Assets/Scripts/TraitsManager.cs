using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public enum TraitType
{
    NONE = 0,
    SPEED = 1,
    SENSE = 2,
    SLOW_DIGESTION = 3
}
public static class TraitsManager
{
public static List<Trait> GenerateParentTraits(List<Trait> parentTraits)
{
    if (parentTraits == null)
    {
        float randomTraitChoice = UnityEngine.Random.Range(0f, 1f);
        if (randomTraitChoice < 0.33f)
            parentTraits = new List<Trait>() { new SenseTrait(2.5f) };
        else if (randomTraitChoice < 0.66f)
            parentTraits = new List<Trait>() { new SpeedTrait(6.0f) };
        else
            parentTraits = new List<Trait>() { new SlowDigestionTrait(1) };  
    }
    foreach (Trait t in parentTraits)
    {
        t.Mutate();
    }
    return parentTraits;
}

public static List<Trait> GenerateChildTraits(List<Trait> parent1Traits, List<Trait> parent2Traits)
{
    List<Trait> childTraits = new List<Trait>();

    // Determine which parent has more traits and which has fewer
    List<Trait> longerList = (parent1Traits.Count > parent2Traits.Count) ? parent1Traits : parent2Traits;
    List<Trait> shorterList = (parent1Traits.Count > parent2Traits.Count) ? parent2Traits : parent1Traits;

    for (int i = 0; i < longerList.Count; i++)
    {
        if (i < shorterList.Count) // If the other parent has a corresponding trait
        {
            Trait parent1Trait = (longerList == parent1Traits) ? longerList[i] : shorterList[i];
            Trait parent2Trait = (longerList == parent1Traits) ? shorterList[i] : longerList[i];

            float randomChoice = UnityEngine.Random.Range(0f, 1f);

            if (randomChoice < 0.15f)
            {
                // Logic to generate an entirely new trait
                float newTraitChoice = UnityEngine.Random.Range(0f, 1f);
                if (newTraitChoice < 0.33f)
                    childTraits.Add(new SenseTrait(2.0f).Variation());
                else if (newTraitChoice < 0.66f)
                    childTraits.Add(new SpeedTrait(6.0f).Variation());
                else
                    childTraits.Add(new SlowDigestionTrait().Variation());
                
                // A bonus additional trait
                float bonusTraitChance = UnityEngine.Random.Range(0f, 1f);
                if (bonusTraitChance < 0.15f)
                {
                    List<Type> availableTraits = new List<Type> { typeof(SenseTrait), typeof(SpeedTrait), typeof(SlowDigestionTrait) };
                    foreach (Trait existingTrait in childTraits)
                    {
                        availableTraits.Remove(existingTrait.GetType());  // Remove the traits the child already has
                    }

                    if (availableTraits.Count > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, availableTraits.Count);
                        Type selectedTraitType = availableTraits[randomIndex];

                        if (selectedTraitType == typeof(SenseTrait))
                            childTraits.Add(new SenseTrait(2.0f).Variation());
                        else if (selectedTraitType == typeof(SpeedTrait))
                            childTraits.Add(new SpeedTrait(6.0f).Variation()); 
                        else if (selectedTraitType == typeof(SlowDigestionTrait))
                            childTraits.Add(new SlowDigestionTrait().Variation());
                    }
                }
            }
            else
            {
                switch (randomChoice)
                {
                    case float n when n < 0.33f:
                        childTraits.Add(parent1Trait.Variation());
                        break;

                    case float n when n < 0.66f:
                        childTraits.Add(parent1Trait.Variation());
                        break;

                    default:
                        childTraits.Add(parent2Trait.Variation());
                        break;
                }
            }
        }
        else // If the other parent doesn't have a corresponding trait
        {
            // Directly pass the trait from the parent with more traits, or introduce custom logic if desired
            childTraits.Add(longerList[i].Variation());
        }
    }

    return childTraits;
}

    private static bool RandomChance(int percentage)
    {
        Random random = new Random();
        return random.Next(0, 100) < percentage;
    }
}
