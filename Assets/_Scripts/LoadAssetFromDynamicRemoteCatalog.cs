using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LoadAssetFromDynamicRemoteCatalog : MonoBehaviour
{
    async void Start()
    {
        await DynamicRemoteCatalog();
    }
    
    
     async Task DynamicRemoteCatalog()
    {
        RemoteContentManager manager = GetComponent<RemoteContentManager>();

        // Initialize and get the list of what's available remotely
        await manager.InitializeRemoteSystem();
        List<string> remoteAssets = await manager.GetAvailableRemoteAssets();

        foreach (var assetKey in remoteAssets)
        {
            Debug.Log($"Found available asset: {assetKey}");
        
            // Download a specific one when requested by user/logic
            await manager.DownloadAssetAsync(assetKey, progress => {
                Debug.Log($"Download progress for {assetKey}: {progress * 100}%");
            });
        }
    }
}
