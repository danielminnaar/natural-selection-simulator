using System;
using UnityEngine;

public class SenseTrait : Trait
{
    public float SenseRadius = 0.0f;

    private float reduceDifference;
    private float increaseDifference;

    public SenseTrait(float radius)
    {
        SenseRadius = radius;
        type = TraitType.SENSE;
        // buff the increase rate and nerf the decrease rate
        reduceDifference = 0.1f;
        increaseDifference = 0.15f;
    }

    public override Trait Variation()
    {
        if (UnityEngine.Random.Range(0, 100) < changeChance)
        {
            SenseRadius *= (1 + UnityEngine.Random.Range(-changeDifference, changeDifference));
        }
        return this;
    }

    public override void Mutate()
    {
        bool chanceToChange = RandomChance(changeChance);
        bool chanceToIncrease = RandomChance(50);
        if (chanceToChange)
        {
            if (chanceToIncrease)
                SenseRadius += SenseRadius * increaseDifference;
            else
                SenseRadius -= SenseRadius * reduceDifference;
        }
        // cap the value
        SenseRadius = Mathf.Clamp(SenseRadius, 0, 10.0f);
    }
}