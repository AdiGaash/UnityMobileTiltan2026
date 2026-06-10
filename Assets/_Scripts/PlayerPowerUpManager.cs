
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all active power-ups on the player
/// </summary>
public class PlayerPowerUpManager : MonoBehaviour
{
    private Dictionary<string, (IPowerUpEffect effect, float endTime, GameObject effectGameObject)> activePowerUps = 
        new Dictionary<string, (IPowerUpEffect, float, GameObject)>();

    private PlayerControl playerControl;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
    }

    private void Update()
    {
        // Check for expired power-ups
        List<string> expiredPowerUps = new List<string>();

        foreach (var kvp in activePowerUps)
        {
            if (Time.time >= kvp.Value.endTime)
            {
                expiredPowerUps.Add(kvp.Key);
            }
        }

        // Remove expired power-ups
        foreach (var powerUpName in expiredPowerUps)
        {
            RemovePowerUp(powerUpName);
        }
    }

    /// <summary>
    /// Apply a power-up to the player
    /// </summary>
    public void ApplyPowerUp(IPowerUpEffect powerUpEffect, float duration)
    {
        string powerUpName = powerUpEffect.GetPowerUpName();

        // If same power-up already exists, remove it first
        if (activePowerUps.ContainsKey(powerUpName))
        {
            RemovePowerUp(powerUpName);
        }

        // Apply the power-up
        powerUpEffect.Apply(playerControl);
        GameObject effectGameObject = (powerUpEffect as MonoBehaviour)?.gameObject;
        activePowerUps[powerUpName] = (powerUpEffect, Time.time + duration, effectGameObject);

        Debug.Log($"Power-up '{powerUpName}' applied for {duration} seconds");
    }

    /// <summary>
    /// Remove a specific power-up
    /// </summary>
    /// <summary>
    /// Remove a specific power-up
    /// </summary>
    public void RemovePowerUp(string powerUpName)
    {
        if (activePowerUps.TryGetValue(powerUpName, out var powerUpData))
        {
            powerUpData.effect.Remove(playerControl);
        
            // Destroy the effect GameObject
            if (powerUpData.effectGameObject != null)
            {
                Destroy(powerUpData.effectGameObject);
            }
        
            activePowerUps.Remove(powerUpName);
            Debug.Log($"Power-up '{powerUpName}' removed");
        }
    }

    /// <summary>
    /// Get remaining time for a power-up (returns -1 if not active)
    /// </summary>
    public float GetPowerUpRemainingTime(string powerUpName)
    {
        if (activePowerUps.TryGetValue(powerUpName, out var powerUpData))
        {
            return Mathf.Max(0, powerUpData.endTime - Time.time);
        }
        return -1f;
    }
}
