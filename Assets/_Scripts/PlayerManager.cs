using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public FloatVariable playerHealth;

     void Awake()
    {
        playerHealth.Value = 100f;
    }
}
