using System.Runtime.CompilerServices;

public class SpeedTrait : Trait
{

    public float MoveSpeed = 0.0f;
    public int improvementChance = 30; // 30% chance
    public float improvementIncrease = 0.15f; // 15% increase
    public SpeedTrait(float speed)
    {
        MoveSpeed = speed;
        type = TraitType.SPEED;
    }

}