using _Scripts;
using UnityEngine;

public class AdvanceTowerGenerator : TowerGeneratorBase
{
    private bool[] lastSegmentUpperLadders = new bool[3];
    private bool isFirstSegment = true;


    public TowerParameters towerParameters;
    protected override void SpawnNewSegment()
    {
        GameObject newSeg = TaggedObjectPooler.Instance.GetPooledObject("Platfroms"); // Get a segment from the pool.

        if (newSeg != null)
        {
            newSeg.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            newSeg.GetComponent<Renderer>().material = towerParameters.SegMaterial;
            newSeg.transform.position = new Vector3(0, nextSpawnY, 0);
            activeSegments.Add(newSeg);
            nextSpawnY += segmentHeight;

            SegmentPopulator populator = newSeg.GetComponent<SegmentPopulator>();
            if (populator != null)
            {
                populator.AttachLaddersToFace(0, ChoosenLadders());
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
        int numLaddersToPlace = Random.Range(towerParameters.MinLadders, towerParameters.MaxLadders + 1);
        
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
}