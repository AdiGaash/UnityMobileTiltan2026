using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TaggedObjectPooler : Singleton<TaggedObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;  // The tag to identify the pool.
        public GameObject prefab;  // The prefab to pool.
        public int initialPoolSize = 10;  // Initial number of objects in the pool.
        public bool canExtend = true;  // Whether the pool can extend dynamically.
    }

    public List<Pool> pools;  // A list of different pools.

    private Dictionary<string, Queue<GameObject>> pooledObjects;  // A dictionary to hold pooled objects by tags.

    public static TaggedObjectPooler Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        pooledObjects = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            pooledObjects[pool.tag] = new Queue<GameObject>();

            for (int i = 0; i < pool.initialPoolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);  // Deactivate initially to save resources.
                pooledObjects[pool.tag].Enqueue(obj);
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (pooledObjects.ContainsKey(tag) && pooledObjects[tag].Count > 0)
        {
            GameObject obj = pooledObjects[tag].Dequeue();
            obj.SetActive(true);  // Activate the object when retrieved.
            return obj;
        }
    
        // If no objects are available, instantiate a new one.
        Pool pool = pools.Find(p => p.tag == tag);
        if (pool != null && pool.canExtend)
        {
            GameObject newObj = Instantiate(pool.prefab);
            return newObj;
        }

        Debug.LogWarning($"No pool or available object found for tag: {tag}");
        return null;
    }

    
 
    public GameObject GetPooledObjectWithAutoReturn(string tag)
    {
        GameObject obj = GetPooledObject(tag);
        
        if (obj != null)
        {
            var poolableComponent = obj.GetComponent<IPoolableObject>();
            if (poolableComponent != null)
            {
                // Set up auto-return action
                poolableComponent.SetReturnAction(() => ReturnObject(obj,tag));
            }
        }
        
        return obj;
    }
    
    
    public void ReturnObject(GameObject obj, string tag)
    {
        if (obj != null && pooledObjects.ContainsKey(tag))
        {
            obj.SetActive(false);  // Deactivate the object when returned to pool.
            obj.transform.parent = null; // Detach from any parent to avoid unintended transformations.
            pooledObjects[tag].Enqueue(obj);
        }
        else
        {
            Debug.Log($"Attempted to return object with tag '{tag}' that does not belong to any pool or is null.");
        }
    }
    
    
    public void InitializePool(Pool pool)
    {
        if (pooledObjects.ContainsKey(pool.tag))
        {
            Debug.LogWarning($"Pool with tag '{pool.tag}' already exists!");
            return;
        }

        pooledObjects[pool.tag] = new Queue<GameObject>();

        for (int i = 0; i < pool.initialPoolSize; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            pooledObjects[pool.tag].Enqueue(obj);
        }

        Debug.Log($"Initialized pool '{pool.tag}' with {pool.initialPoolSize} objects");
    }

  
    public bool HasPool(string tag)
    {
        return pooledObjects.ContainsKey(tag);
    }
}