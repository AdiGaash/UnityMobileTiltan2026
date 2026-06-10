using UnityEngine;

/// <summary>
/// Interface for all power-up effects. Implement this to create new power-up types.
/// </summary>
public interface IPowerUpEffect
{
    /// <summary>
    /// Called when the power-up is collected
    /// </summary>
    void Apply(PlayerControl player);

    /// <summary>
    /// Called when the power-up expires or is removed
    /// </summary>
    void Remove(PlayerControl player);

    /// <summary>
    /// Get the name/type of this power-up
    /// </summary>
    string GetPowerUpName();
}
