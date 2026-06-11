using UnityEngine;
using System.Collections;

public class AssetLoaderAsync : MonoBehaviour
{
    private IEnumerator Start()
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>("Enemy");
        
        yield return request;

        GameObject enemyPrefab = request.asset as GameObject;

        Instantiate(enemyPrefab);
    }
}