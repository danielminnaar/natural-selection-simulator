using System.Collections;
using System.Collections.Generic;
using System;

public enum TraitType
{
    NONE = 0,
    SPEED = 1,
    SENSE = 2
}
public static class TraitsManager
{
    public static List<Trait> GenerateTraits(List<Trait> parentTraits)
    {
        if (parentTraits == null)
        {
            if (RandomChance(50)) // Randomly select one trait with default values
                return new List<Trait>() { new SenseTrait(2.0f) };
            else
                return new List<Trait>() { new SpeedTrait(6.0f) };
        }
        foreach (Trait t in parentTraits)
        {
            switch (t.type)
            {
                case TraitType.SPEED:
                    {
                        SpeedTrait trait = (SpeedTrait)t;
                        bool chanceToChange = RandomChance(trait.changeChance);
                        bool chanceToIncrease = RandomChance(50);
                        if (chanceToChange && chanceToIncrease)
                            trait.MoveSpeed += trait.MoveSpeed * trait.changeDifference;
                        else if (!chanceToIncrease)
                            trait.MoveSpeed -= trait.MoveSpeed * trait.changeDifference;
                        break;
                    }
                case TraitType.SENSE:
                    {
                        SenseTrait trait = (SenseTrait)t;
                        bool chanceToChange = RandomChance(trait.changeChance);
                        bool chanceToIncrease = RandomChance(50);
                        if (chanceToChange && chanceToIncrease)
                            trait.SenseRadius += trait.SenseRadius * trait.changeDifference;
                        else if (chanceToChange && !chanceToIncrease)
                            trait.SenseRadius -= trait.SenseRadius * trait.changeDifference;
                        break;
                    }
                default:
                    {
                        throw new Exception("No trait types found!");
                    }
            }
        }
        return parentTraits;


    }

    private static bool RandomChance(int percentage)
    {
        Random random = new Random();
        return random.Next(0, 100) < percentage;
    }
}
