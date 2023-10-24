using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class OrganismSpecies
{
    public string Name;
    public Color Color;
    public List<Trait> StartingTraits;

    public OrganismSpecies(string name, Color color, List<Trait> startTraits)
    {
        Name = name;
        Color = color;
        StartingTraits = startTraits;
    }

    public OrganismSpecies(List<Trait> startTraits)
    {
        Name = GenerateRandomName();
        Color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        StartingTraits = startTraits;
    }

    private static readonly char[] Vowels = { 'a', 'e', 'i', 'o', 'u' };
    private static readonly char[] Consonants =
    {
        'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm',
        'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'
    };

    private static System.Random _random = new System.Random();

    public static string GenerateRandomName()
    {
        StringBuilder name = new StringBuilder();

        // Start with a consonant
        name.Append(RandomConsonant());

        // Generate up to 6 more characters alternating between vowels and consonants
        for (int i = 1; i < 7; i++)
        {
            if (i % 2 == 0)
                name.Append(RandomConsonant());
            else
                name.Append(RandomVowel());
        }

        // Make the name plural
        if (name.ToString().EndsWith("y"))
        {
            name.Remove(name.Length - 1, 1); // Remove the 'y'
            name.Append("ies"); // Append "ies"
        }
        else
        {
            name.Append("s");
        }

        return name.ToString();
    }

    private static char RandomVowel()
    {
        return Vowels[_random.Next(Vowels.Length)];
    }

    private static char RandomConsonant()
    {
        return Consonants[_random.Next(Consonants.Length)];
    }
}