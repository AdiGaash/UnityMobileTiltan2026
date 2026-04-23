using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform playerTransform;

     void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
    }
}
