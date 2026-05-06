using UnityEngine;

public class ClimbingMotor : MonoBehaviour
{
    [Header("Settings")]
    public float climbSpeed = 5f;
    public float detectionRadius = 0.4f;
    public LayerMask ladderLayer;

    /// <summary>
    /// Checks for ladder presence at the character's current position.
    /// Cross-referenced: Unity Physics.CheckSphere for non-allocating spatial checks.
    /// </summary>
    public bool IsLadderDetected()
    {
        return Physics.CheckSphere(transform.position, detectionRadius, ladderLayer);
    }

    /// <summary>
    /// Executes vertical translation. 
    /// Should be called by a Controller script passing input values.
    /// </summary>
    public void RequestVerticalMovement(float inputDirection)
    {
        if (IsLadderDetected())
        {
            // Direct transform manipulation for non-physics movement.
            // Source: Unity Scripting API - Transform.Translate
            transform.Translate(Vector3.up * inputDirection * climbSpeed * Time.deltaTime);
        }
    }
}