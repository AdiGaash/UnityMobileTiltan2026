using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float climbSpeed = 5f;
    public LayerMask ladderLayer;
    public LayerMask groundLayer;

    private int ladderCount = 0;
    private int groundCount = 0;
    private int pendingLadderExits = 0; // Track exits pending for this frame
    private int pendingGroundExits = 0; // Track exits pending for this frame
    
    private bool isOnLadder => ladderCount > 0;
    private bool isGrounded => groundCount > 0;

    /// <summary>
    /// Executes horizontal movement using Transform.Translate.
    /// </summary>
    public void RequestHorizontalMovement(float inputDirection)
    {
        if (isGrounded || isOnLadder)
        {
            Vector3 movement = Vector3.right * inputDirection * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
    }

    /// <summary>
    /// Executes vertical movement when on ladder using Transform.Translate.
    /// </summary>
    public void RequestVerticalMovement(float inputDirection)
    {
        if (isOnLadder)
        {
            Vector3 movement = Vector3.up * inputDirection * climbSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            ladderCount++;
            Debug.Log($"Entered ladder '{other.name}'. Count: {ladderCount}");
        }
        
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            groundCount++;
            Debug.Log($"Entered ground '{other.name}'. Count: {groundCount}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            pendingLadderExits++;
            Debug.Log($"Exited ladder '{other.name}'. Pending exits: {pendingLadderExits}");
        }
        
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            pendingGroundExits++;
            Debug.Log($"Exited ground '{other.name}'. Pending exits: {pendingGroundExits}");
        }
    }

    private void LateUpdate()
    {
        // Process all pending exits at the end of the frame
        if (pendingLadderExits > 0)
        {
            ladderCount -= pendingLadderExits;
            ladderCount = Mathf.Max(0, ladderCount);
            Debug.Log($"Processed {pendingLadderExits} ladder exits. New count: {ladderCount}");
            pendingLadderExits = 0;
        }

        if (pendingGroundExits > 0)
        {
            groundCount -= pendingGroundExits;
            groundCount = Mathf.Max(0, groundCount);
            Debug.Log($"Processed {pendingGroundExits} ground exits. New count: {groundCount}");
            pendingGroundExits = 0;
        }
    }

}