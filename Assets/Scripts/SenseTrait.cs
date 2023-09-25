public class SenseTrait : Trait
{
    public float SenseRadius = 0.0f;
    public SenseTrait(float radius)
    {
        SenseRadius = radius;
        type = TraitType.SENSE;
    }

}