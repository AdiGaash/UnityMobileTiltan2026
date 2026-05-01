using UnityEngine;
using System.Collections.Generic;

public class SimpleTowerGenerator : TowerGeneratorBase
{
    protected override void SpawnNewSegment()
    {
        GameObject newSeg = TaggedObjectPooler.Instance.GetPooledObject("Platfroms");  // Get a segment from the pool.
        
        if (newSeg != null)
        {
            newSeg.transform.position = new Vector3(0, nextSpawnY, 0);
            activeSegments.Add(newSeg);
            nextSpawnY += segmentHeight;
            
            SegmentPopulator populator = newSeg.GetComponent<SegmentPopulator>();
            if (populator != null)            {
                populator.AttachSubObjects();  // Populate the segment with sub-objects.
            }
        }
    }
    
    protected override void ReturnSegment()
    {
        if (activeSegments.Count > 0)
        {
            GameObject oldSeg = activeSegments[0];
            activeSegments.RemoveAt(0);
            TaggedObjectPooler.Instance.ReturnObject(oldSeg,"Platfroms");  // Return the segment to the pool.
        }
    }
}