using UnityEngine;

public class PowerUpCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private string powerUpName = "Jump";
    [SerializeField] private float duration = 5.0f;
    [SerializeField] private GameObject powerUpEffectPrefab; // The prefab with IPowerUpEffect component

    public void OnCollected()
    {
        // Find the player
        PlayerControl player = FindObjectOfType<PlayerControl>();
        if (player == null)
        {
            Debug.LogError("PlayerControl not found in scene");
            return;
        }

        // Get the PowerUpManager from the player
        PlayerPowerUpManager powerUpManager = player.GetComponent<PlayerPowerUpManager>();
        if (powerUpManager == null)
        {
            Debug.LogError("PlayerPowerUpManager not found on player");
            return;
        }

        // Instantiate the power-up effect and apply it
        if (powerUpEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(powerUpEffectPrefab, player.transform);
            IPowerUpEffect powerUpEffect = effectInstance.GetComponent<IPowerUpEffect>();

            if (powerUpEffect != null)
            {
                powerUpManager.ApplyPowerUp(powerUpEffect, duration);
            }
            else
            {
                Debug.LogError($"PowerUpEffectPrefab does not contain a component implementing IPowerUpEffect");
                Destroy(effectInstance);
            }
        }
        else
        {
            Debug.LogError("PowerUpEffectPrefab not assigned");
        }

        // Disable this collectible
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
       
    }
}
