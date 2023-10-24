using System;
using System.Collections.Generic;
using UnityEngine;
public class Organism
{
    public float moveSpeed = 3.0f;        // Default speed at which the capsule moves.
    public float senseRadius = 0.0f; // Default sense radius that the capsule can detect objects
    public float sphereRemoveRadius = 1.3f; // Default radius within which objects will be detected
    public int maxEnergy = 8;
    public int energyUsed = 0;
    public int foodConsumed = 0;
    public bool canMove = false;
    public List<Trait> traits;
    public Vector3 startPosition;
    public string mateId = "";
    public string id = "";
    public bool hasMated = false;
    public Tuple<string, string> parentIds;
    public Color color;

}
