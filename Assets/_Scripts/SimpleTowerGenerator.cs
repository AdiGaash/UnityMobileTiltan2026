using UnityEngine;
using System.Collections.Generic;

public class SimpleTowerGenerator : MonoBehaviour
{
    public BasicObjectPooler objectPooler;  // Reference to the Object Pooler.
    public Transform cameraTransform; 
    public float segmentHeight = 2.0f;
    public int drawDistance = 6;   

    private List<GameObject> activeSegments = new List<GameObject>();
    private float nextSpawnY;

    void Start()
    {
        // 1. Calculate the start position to be below the player/camera
        // This ensures the player is already standing on a tower segment at start.
        nextSpawnY = -(drawDistance * segmentHeight);

        // 2. Populate the initial tower (half below player, half above)
        // We spawn double the drawDistance to cover both directions.
        for (int i = 0; i < drawDistance * 2; i++)
        {
            SpawnNewSegment();
        }
    }

    void Update()
    {
        // 3. Keep adding segments above as the camera climbs
        if (cameraTransform.position.y + (drawDistance * segmentHeight) > nextSpawnY)
        {
            SpawnNewSegment();
        }

        // 4. Clean up segments that are now far below the camera
        // We use 'drawDistance * 1.5' to ensure they don't pop out of view too early.
        float removalThreshold = cameraTransform.position.y - (drawDistance * segmentHeight);
        
        if (activeSegments.Count > 0 && activeSegments[0].transform.position.y < removalThreshold)
        {
            GameObject oldest = activeSegments[0];
            activeSegments.RemoveAt(0);
            objectPooler.ReturnObject(oldest);  // Return the segment to the pool.
        }
    }

    void SpawnNewSegment()
    {
        GameObject newSeg = objectPooler.GetPooledObject();  // Get a segment from the pool.
        if (newSeg != null)
        {
            newSeg.transform.position = new Vector3(0, nextSpawnY, 0);
            activeSegments.Add(newSeg);
            nextSpawnY += segmentHeight;
        }
    }
}