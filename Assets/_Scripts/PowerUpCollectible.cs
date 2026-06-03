using UnityEngine;

public class PowerUpCollectible : MonoBehaviour, ICollectible
{
    public string powerUpName = "Jump";
    public float duration = 5.0f;

    public void OnCollected()
    {
        // Apply power-up effect to player
        
    }
    
}
