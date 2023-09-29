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

    for (int i = 0; i < parent1Traits.Count; i++)
    {
        Trait parent1Trait = parent1Traits[i];
        Trait parent2Trait = parent2Traits[i];

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

    return childTraits;
}

    private static bool RandomChance(int percentage)
    {
        Random random = new Random();
        return random.Next(0, 100) < percentage;
    }
}
