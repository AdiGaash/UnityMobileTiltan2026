using UnityEngine;

public class ResourceToMemory : MonoBehaviour {
    
    GameObject loadedObj;
            // Keep a reference to the loaded asset (prefab) so it can be unloaded from memory later
    GameObject loadedPrefab;
    public string resourcePath = "LargeAsset"; // Path to the resource in the Resources folder
    
    public void LoadResource()
    {
        // Loads asset into RAM. Memory usage goes UP.
        // Correct usage: first load the asset (prefab) then Instantiate it.
        loadedPrefab = Resources.Load<GameObject>(resourcePath);
        if (loadedPrefab == null) {
            Debug.LogError($"Failed to load resource at '{resourcePath}'");
            return;
        }
        // Instantiate a runtime instance of the prefab
        loadedObj = Instantiate(loadedPrefab);
    }

    public void UnloadResource() {
        // Destroy the instantiated object first
        if (loadedObj != null) {
            Destroy(loadedObj);
            loadedObj = null;
        }

        // If we previously loaded the prefab via Resources.Load, unload that asset from memory too
        if (loadedPrefab != null) {
            Resources.UnloadAsset(loadedPrefab);
            loadedPrefab = null;
        }

       
    }
}
