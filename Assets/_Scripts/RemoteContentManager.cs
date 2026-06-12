
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class RemoteContentManager : MonoBehaviour
{
    [SerializeField] private string remoteContentLabel = "RemoteContent";

    public async Task InitializeRemoteSystem()
    {
        Debug.Log("Checking for catalog updates...");
        
        try 
        {
            var handle = Addressables.UpdateCatalogs();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Catalog updated successfully.");
            }
            Addressables.Release(handle);
        }
        catch (Exception e)
        {
            // We catch the exception because "Content update not available" 
            // is considered an exception by Unity, but it's actually a normal state.
            if (e.Message.Contains("Content update not available"))
            {
                Debug.Log("Catalog is already up to date.");
            }
            else
            {
                Debug.LogError($"Unexpected error updating catalog: {e.Message}");
            }
        }
    }

    public async Task<List<string>> GetAvailableRemoteAssets()
    {
        // 2. Get all resource locations associated with the specific label
        var locationsHandle = Addressables.LoadResourceLocationsAsync(remoteContentLabel);
        await locationsHandle.Task;

        if (locationsHandle.Status == AsyncOperationStatus.Succeeded)
        {
            List<string> assetKeys = new List<string>();
            foreach (var location in locationsHandle.Result)
            {
                assetKeys.Add(location.PrimaryKey);
            }
            Addressables.Release(locationsHandle);
            return assetKeys;
        }

        Addressables.Release(locationsHandle);
        return new List<string>();
    }

    public async Task DownloadAssetAsync(string key, Action<float> onProgress)
    {
        // 3. Check the download size first
        var sizeHandle = Addressables.GetDownloadSizeAsync(key);
        await sizeHandle.Task;
        long downloadSize = sizeHandle.Result;
        Addressables.Release(sizeHandle);

        if (downloadSize <= 0)
        {
            Debug.Log($"Asset {key} is already downloaded or local.");
            onProgress?.Invoke(1.0f);
            return;
        }

        Debug.Log($"Downloading {key}... Size: {downloadSize} bytes");

        // 4. Download the dependencies into the local cache
        var downloadHandle = Addressables.DownloadDependenciesAsync(key);
        
        while (!downloadHandle.IsDone)
        {
            float progress = downloadHandle.PercentComplete;
            onProgress?.Invoke(progress);
            await Task.Yield();
        }

        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Successfully downloaded {key}");
        }
        else
        {
            Debug.LogError($"Failed to download {key}");
        }

        Addressables.Release(downloadHandle);
    }

    public async Task<GameObject> LoadAssetAsync(string key)
    {
        // Final step: Instantiate the asset from the cache
        var handle = Addressables.InstantiateAsync(key);
        await handle.Task;
        return handle.Result;
    }
    
    // New method to instantiate assets at specific positions
    public async Task SpawnRemoteAsset(string key, Vector3 position, Quaternion rotation)
    {
        var handle = Addressables.InstantiateAsync(key, position, rotation);
        await handle.Task;
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Spawned {key} at {position}");
        }
        else
        {
            Debug.LogError($"Failed to spawn {key}");
        }
    }
}
