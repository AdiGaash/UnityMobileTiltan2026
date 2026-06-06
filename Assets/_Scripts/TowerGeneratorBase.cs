using System.Collections.Generic;
using UnityEngine;

public abstract class TowerGeneratorBase : MonoBehaviour
{
    
    public Transform cameraTransform;
    public float segmentHeight = 2.0f;
    public int drawDistance = 6;
    protected List<GameObject> activeSegments = new List<GameObject>();
    protected float nextSpawnY;

    protected virtual void Start()
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

    private void Update()
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
            ReturnSegment();
        }
    }
    protected abstract void ReturnSegment();
    protected abstract void SpawnNewSegment();

    protected virtual void OnDestroy()
    {
        throw new System.NotImplementedException();
    }
}