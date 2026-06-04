using UnityEngine;

public class ShotgunWeaponData : WeaponData
{
    [Header("Shotgun Settings")]
    public int pelletsPerShot = 5;
    public float spreadAngle = 15f;

    public override void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        // Custom shotgun firing logic
        for (int i = 0; i < pelletsPerShot; i++)
        {
            //Vector3 spreadDirection = ApplySpread(fireDirection, spreadAngle);
            // Instantiate pellet with spread
        }
        
        onFire?.Invoke();
    }
}
