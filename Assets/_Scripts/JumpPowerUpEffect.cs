using UnityEngine;

/// <summary>
/// Implements the jump power-up effect that allows the player to jump to the next platform
/// </summary>
public class JumpPowerUpEffect : MonoBehaviour, IPowerUpEffect
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpDuration = 0.3f; // Time to reach peak of jump

    private PlayerControl playerControl;
    private float jumpCooldown = 0f;

    public void Apply(PlayerControl player)
    {
        playerControl = player;
        // Enable the jump component/ability
        enabled = true;
    }

    public void Remove(PlayerControl player)
    {
        playerControl = null;
        // Disable jump ability
        enabled = false;
    }

    public string GetPowerUpName()
    {
        return "Jump";
    }

    private void Update()
    {
        if (playerControl == null || !enabled)
            return;

        jumpCooldown -= Time.deltaTime;

        // Check for jump input (up arrow or app action)
        if (playerControl.TryGetJumpInput() && jumpCooldown <= 0)
        {
            PerformJump();
            jumpCooldown = jumpDuration + 0.2f; // Small cooldown between jumps
        }
    }

    private void PerformJump()
    {
        // Only allow jump if not on a ladder
        if (playerControl.IsOnLadder)
            return;

        // Perform the jump - move player up by one platform height
        Vector3 jumpTarget = playerControl.transform.position + Vector3.up * jumpForce;
        playerControl.JumpToPosition(jumpTarget, jumpDuration);
    }
}
