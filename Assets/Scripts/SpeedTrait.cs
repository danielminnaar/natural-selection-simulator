using System.Runtime.CompilerServices;

public class SpeedTrait : Trait
{

    public float MoveSpeed = 0.0f;

    public SpeedTrait(float speed)
    {
        MoveSpeed = speed;
        type = TraitType.SPEED;
    }

}