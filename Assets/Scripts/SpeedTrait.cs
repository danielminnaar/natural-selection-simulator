using System.Runtime.CompilerServices;

public class SpeedTrait : Trait
{
    public float MoveSpeed = 0.0f;

    public SpeedTrait(float speed)
    {
        MoveSpeed = speed;
        type = TraitType.SPEED;
    }

    public override Trait Variation()
    {
        if (UnityEngine.Random.Range(0, 100) < changeChance)
        {
            MoveSpeed *= (1 + UnityEngine.Random.Range(-changeDifference, changeDifference));
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
                MoveSpeed += MoveSpeed * changeDifference;
            else
                MoveSpeed -= MoveSpeed * changeDifference;
        }
    }
}