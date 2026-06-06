using System;
using UnityEngine;

[Serializable]
public class CollectableLevelParameters
{
    public GameObject CollectablePrefab;
    [Range(0.01f, 1f)]
    public float SpawnChance; // Chance to spawn this collectable (0 to 1)
}