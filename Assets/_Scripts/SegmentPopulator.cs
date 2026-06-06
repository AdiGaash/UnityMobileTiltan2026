
using UnityEngine;

public class SegmentPopulator : MonoBehaviour
{
    public Transform[] faceTransforms; // Array of references to the four faces, each with six children
  
    
     
  

    public void AttachSubObjects()
    {
        foreach (Transform face in faceTransforms)
        {
            for (int i = 0; i < face.childCount-1; i++) // Assuming six positions per face
            {
                Transform positionTransform = face.GetChild(i);
                GameObject subObject = TaggedObjectPooler.Instance.GetPooledObject("StairsShort"); // Get a sub-object from the pool
                subObject.transform.parent = positionTransform;
                subObject.transform.localPosition = Vector3.zero; // Reset local position to align with the parent
            }
        }
    }

    public void AttachLaddersToFace(int faceNum, bool[] laddersSlots)
    {
        Transform face = faceTransforms[faceNum];

        for (int i = 0; i < face.childCount-1; i++) // Assuming six positions per face
        {
            if (laddersSlots[i])
            {
                Transform positionTransform = face.GetChild(i);
                GameObject subObject = TaggedObjectPooler.Instance.GetPooledObject("StairsShort");

                subObject.transform.parent = positionTransform;
                subObject.transform.localPosition = Vector3.zero; // Reset local position to align with the parent
            }
        }
        
    }

    /// <summary>
    /// Spawns collectibles on top of ladders based on the CollectableLevelParameters
    /// </summary>
    /// <param name="faceNum">Face index to spawn collectibles on</param>
    /// <param name="laddersSlots">Array indicating which positions have ladders</param>
    /// <param name="collectableParameters">Array of collectible parameters with spawn chances</param>
    public void SpawnCollectiblesOnLadders(int faceNum, bool[] laddersSlots, CollectableLevelParameters[] collectableParameters)
    {
        if (collectableParameters == null || collectableParameters.Length == 0)
            return;

        Transform face = faceTransforms[faceNum];

        // Only check the upper ladder positions (indices 0-2) for collectible spawning
        for (int i = 0; i < 3; i++)
        {
            if (laddersSlots[i]) // Only spawn on positions that have ladders
            {
                // Try to spawn a collectible based on the parameters
                foreach (var collectableParam in collectableParameters)
                {
                    if (collectableParam.CollectablePrefab != null && Random.value <= collectableParam.SpawnChance)
                    {
                        SpawnCollectible(face.GetChild(i), collectableParam.CollectablePrefab);
                        break; // Only spawn one collectible per ladder position
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawns a specific collectible at the given position
    /// </summary>
    /// <param name="ladderPosition">The transform of the ladder position</param>
    /// <param name="collectiblePrefab">The collectible prefab to spawn</param>
    private void SpawnCollectible(Transform ladderPosition, GameObject collectiblePrefab)
    {
        
        GameObject collectible = TaggedObjectPooler.Instance.GetPooledObject(collectiblePrefab.name);
        
        if (collectible != null)
        {
            collectible.transform.parent = ladderPosition;
            // Position the collectible slightly above the ladder
            collectible.transform.localPosition = new Vector3(0, 0.2f, -0.2f); // Adjust Y offset as needed
        }
    }

    /// <summary>
    /// Determines the appropriate pool tag for the collectible based on its type
    /// </summary>
    /// <param name="collectiblePrefab">The collectible prefab</param>
    /// <returns>Pool tag string</returns>
    

    
    public void ClearSubObjects()
    {
        foreach (Transform face in faceTransforms)
        {
            for (int i = 0; i < face.childCount; i++)
            {
                Transform positionTransform = face.GetChild(i);
                if (positionTransform.childCount > 0)
                {
                    // Clear all child objects (ladders and collectibles)
                    for (int j = positionTransform.childCount - 1; j >= 0; j--)
                    {
                        GameObject childObject = positionTransform.GetChild(j).gameObject;
                        string childName = childObject.name.Replace("(Clone)", "").Trim(); // Get the base name of the child object
                        TaggedObjectPooler.Instance.ReturnObject(childObject, childName);
                    }
                }
            }
        }
    }
    
}