using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Weapon Properties")]
    public string weaponName = "Basic Weapon";
    public GameObject projectilePrefab;
    
    [Header("Damage Settings")]
    public float damage = 10f;
    
    [Header("Fire Rate Settings")]
    [Range(0.1f, 5f)]
    public float fireRate = 1f; // Shots per second
    
    [Header("Fire Action")]
    public UnityEvent onFire;
    
    /// <summary>
    /// Get the delay between shots based on fire rate
    /// </summary>
    public float GetFireDelay()
    {
        return 1f / fireRate;
    }
    
    /// <summary>
    /// Execute the fire action
    /// </summary>
    public virtual void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        // Instantiate projectile if prefab exists
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePosition, Quaternion.LookRotation(fireDirection));
            
            // Set damage if projectile has a damage component
            
        }
        
        // Invoke fire event
        onFire?.Invoke();
    }
}
