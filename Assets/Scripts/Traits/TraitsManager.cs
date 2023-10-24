using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Security.Cryptography;

public enum TraitType
{
    NONE = 0,
    SPEED = 1,
    SENSE = 2,
    SLOW_DIGESTION = 3,
    PREDATOR = 4
}
public static class TraitsManager
{
    public static List<TraitType> allowedTraits;
    public static List<Trait> GenerateParentTraits(List<Trait> parentTraits)
    {
        if (parentTraits == null)
        {
            parentTraits = new List<Trait>();
            parentTraits.Add(GenerateRandomTrait());
        }
        else
        {
            foreach(var t in parentTraits)
            {
                t.Mutate();
            }
        }
        return parentTraits;
    }

    public static List<Trait> GenerateChildTraits(List<Trait> parent1Traits, List<Trait> parent2Traits)
    {
        List<Trait> childTraits = new List<Trait>();
        HashSet<TraitType> existingTraitTypes = new HashSet<TraitType>();
        Random random = new Random();

        // Check common traits between parents and add them to childTraits without duplication.
        foreach (Trait trait1 in parent1Traits)
        {
            foreach (Trait trait2 in parent2Traits)
            {
                if (trait1.type == trait2.type && !existingTraitTypes.Contains(trait1.type))
                {
                    childTraits.Add(trait1.Variation());
                    existingTraitTypes.Add(trait1.type);
                }
            }
        }

        // If there are no common traits, add a random trait from either parent to childTraits.
        if (childTraits.Count == 0)
        {
            Trait randomTrait = random.Next(0, 2) == 0 ? parent1Traits[random.Next(parent1Traits.Count)] : parent2Traits[random.Next(parent2Traits.Count)];
            childTraits.Add(randomTrait.Variation());
            existingTraitTypes.Add(randomTrait.type);
        }

        // 15% chance to introduce a new random trait not already inherited from the parents.
        if (RandomChance(15))
        {
            TraitType[] allTraitTypes = allowedTraits.ToArray(); //{ TraitType.SPEED, TraitType.SENSE, TraitType.SLOW_DIGESTION };
            List<TraitType> availableTraitTypes = new List<TraitType>(allTraitTypes);

            // Remove the traits that the child already has
            foreach (TraitType traitType in existingTraitTypes)
            {
                availableTraitTypes.Remove(traitType);
            }

            if (availableTraitTypes.Count > 0)
            {
                int randomIndex = random.Next(0, availableTraitTypes.Count);
                TraitType newTraitType = availableTraitTypes[randomIndex];

                switch (newTraitType)
                {
                    case TraitType.SPEED:
                        childTraits.Add(new SpeedTrait()); // Default value
                        break;
                    case TraitType.SENSE:
                        childTraits.Add(new SenseTrait()); // Default value
                        break;
                    case TraitType.SLOW_DIGESTION:
                        childTraits.Add(new SlowDigestionTrait()); // Default value
                        break;
                }
            }
        }

        TraitType firstType = (childTraits.Count > 1 ? childTraits[0].type : TraitType.NONE);
        TraitType secondType = (childTraits.Count == 2 ? childTraits[1].type : TraitType.NONE);
        if (firstType != TraitType.NONE && firstType == secondType)
        {
            Debug.Print("dupe");
        }
        return childTraits;
    }
    public static Trait GenerateRandomTrait()
    {
        TraitType selectedTraitType = allowedTraits[UnityEngine.Random.Range(0, allowedTraits.Count)];

        // Create a list containing the selected trait.
        switch (selectedTraitType)
        {
            case TraitType.SENSE:
                {
                    var t = new SenseTrait();
                    t.Mutate();
                    return t;
                }
            case TraitType.SPEED:
                {
                    var t = new SpeedTrait();
                    t.Mutate();
                    return t;
                }
            case TraitType.SLOW_DIGESTION:
                {
                    var t = new SlowDigestionTrait();
                    t.Mutate();
                    return t;
                }
            default:
                return null;
        }
    }

    private static bool RandomChance(int percentage)
    {
        Random random = new Random();
        return random.Next(0, 100) < percentage;
    }
}
