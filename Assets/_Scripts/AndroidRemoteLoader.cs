using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks; 

public class AndroidRemoteLoader : MonoBehaviour
{
    public string assetKey = "MyAndroidPrefab"; 

    async void Start()
    {
        await LoadRemoteAsset();
    }

    async Task LoadRemoteAsset()
    {
        Debug.Log("Downloading from GitHub...");
        
        // Unity 6 handles the remote path automatically based on your Profile settings
        var handle = Addressables.LoadAssetAsync<GameObject>(assetKey);
        
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Instantiate(handle.Result);
            Debug.Log("Successfully downloaded asset on Android!");
        }
        else
        {
            Debug.LogError($"Error loading asset: {handle.OperationException}");
        }
    }
}