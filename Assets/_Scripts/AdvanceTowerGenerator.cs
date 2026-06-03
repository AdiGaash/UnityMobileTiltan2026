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

    protected override void SpawnNewSegment()
    {
        
        
        GameObject newSeg = TaggedObjectPooler.Instance.GetPooledObject("Platfroms"); // Get a segment from the pool.

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
                populator.AttachLaddersToFace(0, ChoosenLadders());
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
            TaggedObjectPooler.Instance.ReturnObject(oldSeg,"Platfroms");  // Return the segment to the pool.
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