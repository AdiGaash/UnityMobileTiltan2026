using UnityEngine;

public class AdvanceTowerGenerator : TowerGeneratorBase
{
    private bool[] lastSegmentUpperLadders = new bool[3];
    private bool isFirstSegment = true;

    [Header("Difficulty Management")]
    public TowerParametersManager parametersManager;
    
    // Keep the old reference for backward compatibility
    public TowerParameters towerParameters;

    // Event for when difficulty changes
    public System.Action<string> OnDifficultyChanged;

    private bool DidEnsureCollectiblesInPool = false;
    
        protected override void Start()
        {
            base.Start();
            // Subscribe to our own difficulty changed event to ensure collectibles are pooled
            OnDifficultyChanged += EnsureCollectiblesInPool;
            
            // Ensure initial collectibles are in pool
            EnsureCollectiblesInPool(GetCurrentDifficultyName());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Unsubscribe to prevent memory leaks
            if (OnDifficultyChanged != null)
                OnDifficultyChanged -= EnsureCollectiblesInPool;
        }

        /// <summary>
        /// Ensures all collectible prefabs from the current difficulty are available in the object pool
        /// </summary>
        /// <param name="difficultyName">The name of the difficulty level (for logging purposes)</param>
        private void EnsureCollectiblesInPool(string difficultyName)
        {
            DidEnsureCollectiblesInPool = true;
            var currentParams = GetCurrentTowerParameters();
            if (currentParams?.Collectables == null || currentParams.Collectables.Length == 0)
            {
                Debug.Log($"No collectibles defined for difficulty: {difficultyName}");
                return;
            }

            Debug.Log($"Ensuring collectibles are available in pool for difficulty: {difficultyName}");

            foreach (var collectableParam in currentParams.Collectables)
            {
                if (collectableParam.CollectablePrefab != null)
                {
                    EnsurePrefabInPool(collectableParam.CollectablePrefab);
                }
            }
        }

        /// <summary>
        /// Ensures a specific collectible prefab is available in the TaggedObjectPooler
        /// </summary>
        /// <param name="prefab">The collectible prefab to ensure is in the pool</param>
        private void EnsurePrefabInPool(GameObject prefab)
        {
            string poolTag = prefab.name;
            
            // Check if a pool with this tag already exists
            bool poolExists = TaggedObjectPooler.Instance.pools.Exists(p => p.tag == poolTag);
            
            if (!poolExists)
            {
                // Create a new pool for this collectible type
                var newPool = new TaggedObjectPooler.Pool(prefab, 5, true);
                TaggedObjectPooler.Instance.pools.Add(newPool);
                
                // Initialize the pool manually since Awake has already been called
                TaggedObjectPooler.Instance.InitializePool(newPool);
                
                Debug.Log($"Created new pool for collectible: {prefab.name} with tag: {poolTag}");
            }
            else
            {
                // Pool exists, check if it has the correct prefab
                var existingPool = TaggedObjectPooler.Instance.pools.Find(p => p.tag == poolTag);
                if (existingPool.prefab != prefab)
                {
                    Debug.LogWarning($"Pool with tag {poolTag} exists but has different prefab. Expected: {prefab.name}, Found: {existingPool.prefab.name}");
                }
            }
        }

    
      
    protected override void SpawnNewSegment()
    {
        
        
        GameObject newSeg = TaggedObjectPooler.Instance.GetPooledObject("WoodenBaseClear"); // Get a segment from the pool.

        if (newSeg != null)
        {
            newSeg.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            
            // Use parameters from manager or fallback to old system
            var currentParams = GetCurrentTowerParameters();
            if (currentParams != null)
            {
                newSeg.GetComponent<Renderer>().material = currentParams.SegMaterial;
            }
            
            newSeg.transform.position = new Vector3(0, nextSpawnY, 0);
            activeSegments.Add(newSeg);
            nextSpawnY += segmentHeight;

            SegmentPopulator populator = newSeg.GetComponent<SegmentPopulator>();
            if (populator != null)
            {
                var chosenLadders = ChoosenLadders();
                populator.AttachLaddersToFace(0, chosenLadders);
                    
                // Spawn collectibles on top of ladders based on current tower parameters
                if (currentParams?.Collectables != null && currentParams.Collectables.Length > 0 && DidEnsureCollectiblesInPool)
                {
                    populator.SpawnCollectiblesOnLadders(0, chosenLadders, currentParams.Collectables);
                }
            }
            
            
            
            
            // Check for difficulty progression
            if (parametersManager != null && parametersManager.OnSegmentGenerated())
            {
                OnDifficultyChanged?.Invoke(parametersManager.CurrentDifficultyName);
            }
        }
    }

    bool[] ChoosenLadders()
    {
        bool[] ladderSlots = new bool[6];
    
        // Initialize lastSegmentUpperLadders randomly for the first segment
        if (isFirstSegment)
        {
            for (int i = 0; i < lastSegmentUpperLadders.Length; i++)
            {
                lastSegmentUpperLadders[i] = Random.value > 0.5f;
            }
            isFirstSegment = false;
        }
    
        // Set the lower ladders (indices 3-5) based on the upper ladders of the previous segment
        for (int i = 0; i < 3; i++)
        {
            ladderSlots[i + 3] = lastSegmentUpperLadders[i];
        }
    
        // Generate new upper ladders (indices 0-2) using MinLadders and MaxLadders parameters
        var currentParams = GetCurrentTowerParameters();
        int minLadders = currentParams?.MinLadders ?? 1;
        int maxLadders = currentParams?.MaxLadders ?? 3;
        int numLaddersToPlace = Random.Range(minLadders, maxLadders + 1);
    
        // First, set all upper ladders to false
        for (int i = 0; i < 3; i++)
        {
            ladderSlots[i] = false;
            lastSegmentUpperLadders[i] = false;
        }
    
        // Then randomly place the specified number of ladders
        for (int i = 0; i < numLaddersToPlace; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, 3); // Upper ladder positions (0, 1, 2)
            } while (ladderSlots[randomIndex]); // Ensure we don't place a ladder where one already exists
        
            ladderSlots[randomIndex] = true;
            lastSegmentUpperLadders[randomIndex] = true;
        }
    
        return ladderSlots;
    }

    protected override void ReturnSegment()
    {
        if (activeSegments.Count > 0)
        {
            GameObject oldSeg = activeSegments[0];
            SegmentPopulator populator = oldSeg.GetComponent<SegmentPopulator>();
            if (populator != null)
            {
                populator.ClearSubObjects();  // Clear sub-objects before returning to pool.
            }
    
            activeSegments.RemoveAt(0);
            TaggedObjectPooler.Instance.ReturnObject(oldSeg,"WoodenBaseClear");  // Return the segment to the pool.
        }
    }
    
    /// <summary>
    /// Gets current TowerParameters from manager or falls back to legacy system
    /// </summary>
    private TowerParameters GetCurrentTowerParameters()
    {
        // Try to get from parameters manager first
        if (parametersManager != null && parametersManager.CurrentTowerParameters != null)
        {
            return parametersManager.CurrentTowerParameters;
        }
        
        // Fallback to legacy system
        return towerParameters;
    }
    
    /// <summary>
    /// Manually reset difficulty to beginning
    /// </summary>
    public void ResetDifficulty()
    {
        parametersManager?.ResetDifficulty();
    }
    
    /// <summary>
    /// Get current difficulty info
    /// </summary>
    public string GetCurrentDifficultyName()
    {
        return parametersManager?.CurrentDifficultyName ?? "Legacy";
    }
    
    /// <summary>
    /// Get progress to next difficulty (0.0 to 1.0)
    /// </summary>
    public float GetDifficultyProgress()
    {
        return parametersManager?.GetProgressToNextDifficulty() ?? 1.0f;
    }
    
    /// <summary>
    /// Get segments remaining until next difficulty
    /// </summary>
    public int GetSegmentsUntilNextDifficulty()
    {
        return parametersManager?.GetSegmentsUntilNextDifficulty() ?? 0;
    }
}