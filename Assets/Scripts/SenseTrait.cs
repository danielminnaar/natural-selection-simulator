public class SenseTrait : Trait
{
    public float SenseRadius = 0.0f;
    public int improvementChance = 30; // 30% chance
    public float improvementIncrease = 0.15f; // 15% increase
    public SenseTrait(float radius)
    {
        SenseRadius = radius;
        type = TraitType.SENSE;
    }

}