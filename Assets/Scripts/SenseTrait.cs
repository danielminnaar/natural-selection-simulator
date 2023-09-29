public class SenseTrait : Trait
{
    public float SenseRadius = 0.0f;

    public SenseTrait(float radius)
    {
        SenseRadius = radius;
        type = TraitType.SENSE;
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
                SenseRadius += SenseRadius * changeDifference;
            else
                SenseRadius -= SenseRadius * changeDifference;
        }
    }
}