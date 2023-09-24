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
    public static Trait GenerateTrait(Trait parentTrait)
    {
        if (parentTrait == null || parentTrait.type == TraitType.NONE)
        {
            if (RandomChance(50))
                return new SenseTrait(3.0f);
            else
                return new SpeedTrait(6.0f);
        }
        switch (parentTrait.type)
        {
            case TraitType.SPEED:
                {
                    SpeedTrait trait = (SpeedTrait)parentTrait;
                    if (RandomChance(trait.improvementChance))
                        trait.MoveSpeed += trait.MoveSpeed * trait.improvementIncrease;
                    return trait;
                }
            case TraitType.SENSE:
                {
                    SenseTrait trait = (SenseTrait)parentTrait;
                    if (RandomChance(trait.improvementChance))
                        trait.SenseRadius += trait.SenseRadius * trait.improvementIncrease;
                    return trait;
                }
            default: // None - randomly select a new trait
                {
                    return parentTrait;
                }
        }

    }

    private static bool RandomChance(int percentage)
    {
        Random random = new Random();
        return random.Next(0, 100) < percentage;
    }
}
