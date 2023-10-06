using System;
using UnityEngine;

public class SlowDigestionTrait : Trait
{
    public int digestionFactor = 1;

    public SlowDigestionTrait(int digestionRate = 1)
    {
        digestionFactor = digestionRate;
        type = TraitType.SLOW_DIGESTION;
    }


    // No intention to variate this trait yet
    public override Trait Variation()
    {
        return this;
    }

    public override void Mutate()
    {

    }

    public override void OnEat(Organism organism)
    {
        organism.maxEnergy += digestionFactor;
        organism.maxEnergy = Mathf.Clamp(organism.maxEnergy, 1, 20);
    }
}