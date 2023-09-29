
using System;

public abstract class Trait
{
    public TraitType type = TraitType.NONE;
    public int changeChance = 30; // 30% chance
    public float changeDifference = 0.15f; // 15% change

    public abstract Trait Variation();
    public abstract void Mutate();

    protected static bool RandomChance(int percentage)
    {
        Random random = new Random();
        return random.Next(0, 100) < percentage;
    }

    public virtual void OnEat(Organism organism) { }
    public virtual void OnMate(Organism organism) { }
}