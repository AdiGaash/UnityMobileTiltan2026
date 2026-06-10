using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Pseudorandom
{
    [SerializeField] private int randomSeed = -1;
    private System.Random seededRandom;
    private bool isInitialized = false;

    // Properties for external access
    public int CurrentSeed => randomSeed;
    public bool IsInitialized => isInitialized;

    // Constructors
    public Pseudorandom()
    {
        InitializeSeededRandom();
    }

    public Pseudorandom(int seed)
    {
        randomSeed = seed;
        InitializeSeededRandom();
    }

    public void InitializeSeededRandom()
    {
        // Create a new seed if the value is -1
        if (randomSeed == -1)
        {
            randomSeed = Environment.TickCount;
        }

        seededRandom = new System.Random(randomSeed);
        isInitialized = true;
        
        Debug.Log($"Pseudorandom initialized with seed: {randomSeed}");
    }

    /// <summary>
    /// Initialize with a specific seed
    /// </summary>
    public void InitializeWithSeed(int seed)
    {
        randomSeed = seed;
        seededRandom = new System.Random(randomSeed);
        isInitialized = true;
        
        Debug.Log($"Pseudorandom initialized with custom seed: {randomSeed}");
    }

   

   

    

  

    // Random calculation methods for other classes to use
    
    /// <summary>
    /// Returns a random float between 0.0 and 1.0
    /// </summary>
    public float NextFloat()
    {
        if (!isInitialized) InitializeSeededRandom();
        return (float)seededRandom.NextDouble();
    }

    /// <summary>
    /// Returns a random float between min and max
    /// </summary>
    public float NextFloat(float min, float max)
    {
        if (!isInitialized) InitializeSeededRandom();
        return min + (float)seededRandom.NextDouble() * (max - min);
    }

    /// <summary>
    /// Returns a random integer between min (inclusive) and max (exclusive)
    /// </summary>
    public int NextInt(int min, int max)
    {
        if (!isInitialized) InitializeSeededRandom();
        return seededRandom.Next(min, max);
    }

    /// <summary>
    /// Returns a random integer between 0 and max (exclusive)
    /// </summary>
    public int NextInt(int max)
    {
        if (!isInitialized) InitializeSeededRandom();
        return seededRandom.Next(max);
    }

    /// <summary>
    /// Returns a random integer (full range)
    /// </summary>
    public int NextInt()
    {
        if (!isInitialized) InitializeSeededRandom();
        return seededRandom.Next();
    }

    /// <summary>
    /// Returns a random boolean value
    /// </summary>
    public bool NextBool()
    {
        if (!isInitialized) InitializeSeededRandom();
        return seededRandom.NextDouble() > 0.5;
    }

    /// <summary>
    /// Returns a random boolean with specified probability of being true
    /// </summary>
    public bool NextBool(float probability)
    {
        if (!isInitialized) InitializeSeededRandom();
        return seededRandom.NextDouble() < probability;
    }

    /// <summary>
    /// Returns a random Vector3 with components between min and max
    /// </summary>
    public Vector3 NextVector3(float min, float max)
    {
        return new Vector3(
            NextFloat(min, max),
            NextFloat(min, max),
            NextFloat(min, max)
        );
    }

    /// <summary>
    /// Returns a random Vector2 with components between min and max
    /// </summary>
    public Vector2 NextVector2(float min, float max)
    {
        return new Vector2(
            NextFloat(min, max),
            NextFloat(min, max)
        );
    }

    /// <summary>
    /// Returns a random Vector3 within a unit sphere
    /// </summary>
    public Vector3 NextInsideUnitSphere()
    {
        Vector3 randomPoint;
        do
        {
            randomPoint = NextVector3(-1f, 1f);
        } while (randomPoint.sqrMagnitude > 1f);
        
        return randomPoint;
    }

    /// <summary>
    /// Returns a random Vector2 within a unit circle
    /// </summary>
    public Vector2 NextInsideUnitCircle()
    {
        Vector2 randomPoint;
        do
        {
            randomPoint = NextVector2(-1f, 1f);
        } while (randomPoint.sqrMagnitude > 1f);
        
        return randomPoint;
    }

    /// <summary>
    /// Returns a random angle in radians
    /// </summary>
    public float NextAngle()
    {
        return NextFloat(0f, 2f * Mathf.PI);
    }

    /// <summary>
    /// Returns a random angle in degrees
    /// </summary>
    public float NextAngleDegrees()
    {
        return NextFloat(0f, 360f);
    }

    /// <summary>
    /// Shuffles an array using Fisher-Yates algorithm
    /// </summary>
    public void Shuffle<T>(T[] array)
    {
        if (!isInitialized) InitializeSeededRandom();
        
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = seededRandom.Next(i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Shuffles a list using Fisher-Yates algorithm
    /// </summary>
    public void Shuffle<T>(List<T> list)
    {
        if (!isInitialized) InitializeSeededRandom();
        
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = seededRandom.Next(i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Returns a random element from an array
    /// </summary>
    public T GetRandomElement<T>(T[] array)
    {
        if (array == null || array.Length == 0) return default(T);
        return array[NextInt(array.Length)];
    }

    /// <summary>
    /// Returns a random element from a list
    /// </summary>
    public T GetRandomElement<T>(List<T> list)
    {
        if (list == null || list.Count == 0) return default(T);
        return list[NextInt(list.Count)];
    }

    /// <summary>
    /// Returns multiple random elements from a list without repetition
    /// </summary>
    public List<T> GetRandomElements<T>(List<T> list, int count)
    {
        if (list == null || list.Count == 0 || count <= 0) return new List<T>();
        
        count = Mathf.Min(count, list.Count);
        List<T> shuffled = new List<T>(list);
        Shuffle(shuffled);
        
        return shuffled.GetRange(0, count);
    }

    /// <summary>
    /// Weighted random selection from arrays
    /// </summary>
    public T GetWeightedRandom<T>(T[] items, float[] weights)
    {
        if (items == null || weights == null || items.Length != weights.Length || items.Length == 0)
            return default(T);

        float totalWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
            totalWeight += weights[i];

        float randomValue = NextFloat(0f, totalWeight);
        float currentWeight = 0f;

        for (int i = 0; i < items.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
                return items[i];
        }

        return items[items.Length - 1]; // Fallback
    }

    /// <summary>
    /// Reset the random generator with a new seed
    /// </summary>
    public void ResetWithNewSeed()
    {
        randomSeed = Environment.TickCount;
        InitializeSeededRandom();
    }

    /// <summary>
    /// Reset the random generator with current seed (restart sequence)
    /// </summary>
    public void ResetSequence()
    {
        if (randomSeed != -1)
        {
            seededRandom = new System.Random(randomSeed);
            isInitialized = true;
        }
    }

    [System.Serializable]
    private class SeedData
    {
        public int seed;
        public long timestamp;
    }
}