using UnityEngine;

public class PlatformTriggerDetector : MonoBehaviour
{
    private PlayerControl playerControl;
    
    void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        playerControl.OnPlatformTriggerEnter(other);
    }
    
    void OnTriggerExit(Collider other)
    {
        playerControl.OnPlatformTriggerExit(other);
    }
}