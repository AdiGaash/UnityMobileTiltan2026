using System.Collections;
using UnityEngine;


public class StartCoroutine : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("Game Starting");

        yield return new WaitForSeconds(2f);

        Debug.Log("2 seconds later");
    }
}
