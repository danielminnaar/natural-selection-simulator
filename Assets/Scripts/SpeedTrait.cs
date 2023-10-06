using System.Runtime.CompilerServices;
using UnityEngine;
public class SpeedTrait : Trait
{
    public float MoveSpeed = 0.0f;
    private float reduceDifference;
    private float increaseDifference;
    public SpeedTrait(float speed)
    {
        MoveSpeed = speed;
        type = TraitType.SPEED;
        reduceDifference = 0.1f;
        increaseDifference = 0.15f;
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
                MoveSpeed += MoveSpeed * increaseDifference;
            else
                MoveSpeed -= MoveSpeed * reduceDifference;
        }
        // cap the value
        MoveSpeed = Mathf.Clamp(MoveSpeed, 0, 10.0f);
    }
}