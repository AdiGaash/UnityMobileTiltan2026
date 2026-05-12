

using UnityEngine;

public class LadderTriggerDetector : MonoBehaviour
{
    private PlayerControl playerControl;
    
    void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        playerControl.OnLadderTriggerEnter(other);
    }
    
    void OnTriggerExit(Collider other)
    {
        playerControl.OnLadderTriggerExit(other);
    }
    
    void OnTriggerStay(Collider other)
    {
        playerControl.OnLadderTriggerStay(other);
    }
}