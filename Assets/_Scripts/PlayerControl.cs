using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum MovementMode
    {
        Platform,
        Ladder
    }

    [Header("Movement")]
    [SerializeField]
    private float horizontalSpeed = 5f;

    [SerializeField]
    private float verticalSpeed = 4f;

    [Header("Layers")]
    [SerializeField]
    private LayerMask ladderLayer;
    
    [SerializeField]
    private LayerMask platformLayer;
    

    // Current movement state
    private MovementMode movementMode;

    // Input values requested externally
    private float horizontalInput;
    private float verticalInput;

    // =========================================================
    // LADDER SYSTEM
    // =========================================================

    // All ladders currently touching the player
    private HashSet<Collider> activeLadders =
        new HashSet<Collider>();

    
    // Is player currently on a platform
    private bool isOnPlatform = false;
    
    // Current ladder center used for snapping
    private Vector3 ladderCenter;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        movementMode = MovementMode.Platform;
    }

    private void Update()
    {
        ApplyMovement();
    }

    // =========================================================
    // EXTERNAL INPUT API
    // =========================================================

    public void RequestHorizontalMovement(float direction)
    {
        horizontalInput =
            Mathf.Clamp(direction, -1f, 1f);
    }

    public void RequestVerticalMovement(float direction)
    {
        verticalInput =
            Mathf.Clamp(direction, -1f, 1f);
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void ApplyMovement()
    {
        Vector3 move = Vector3.zero;

        // -----------------------------------------------------
        // PLATFORM MODE
        // -----------------------------------------------------

        if (movementMode == MovementMode.Platform)
        {
            // Allow ONLY horizontal movement
            move.x = horizontalInput * horizontalSpeed;

            // Enter ladder mode
            if (IsOnLadder() &&
                Mathf.Abs(verticalInput) > 0.01f)
            {
                EnterLadderMode();
            }
        }

        // -----------------------------------------------------
        // LADDER MODE
        // -----------------------------------------------------

        else if (movementMode == MovementMode.Ladder)
        {
            // Safety check
            if (!IsOnLadder())
            {
                ExitLadderMode();
                return;
            }

            // Check for horizontal input and platform availability
            if (Mathf.Abs(horizontalInput) > 0.01f && isOnPlatform)
            {
                ExitLadderMode();
                // Apply horizontal movement immediately
                move.x = horizontalInput * horizontalSpeed;
            }
            else
            {
                // Lock player to ladder center
                Vector3 position = transform.position;
                position.x = ladderCenter.x;
                transform.position = position;

                // Allow ONLY vertical movement
                move.y = verticalInput * verticalSpeed;
            }
        }

        // Move character using transform
        transform.position += move * Time.deltaTime;

        // Optional:
        // reset inputs after processing
        horizontalInput = 0f;
        verticalInput = 0f;
    }

    // =========================================================
    // LADDER HELPERS
    // =========================================================

    private bool IsOnLadder()
    {
    
        return activeLadders.Count > 0;
    }

    private void EnterLadderMode()
    {
        movementMode = MovementMode.Ladder;

        // Snap immediately to ladder
        Vector3 position = transform.position;

        position.x = ladderCenter.x;
    

        transform.position = position;
        Debug.Log("Entered ladder mode");
    }

    private void ExitLadderMode()
    {
        Debug.Log("exit ladder mode");
        movementMode = MovementMode.Platform;
    }

  // =========================================================
        // TRIGGER DETECTOR CALLBACKS
        // =========================================================

        public void OnLadderTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject.layer, ladderLayer))
            {
                activeLadders.Add(other);
                ladderCenter = other.bounds.center;
                Debug.Log("Ladder collider detected ladder!");
            }
        }

        public void OnLadderTriggerStay(Collider other)
        {
            if (IsInLayerMask(other.gameObject.layer, ladderLayer))
            {
                // Update ladder center
                ladderCenter = other.bounds.center;
            }
        }
    
        public void OnLadderTriggerExit(Collider other)
        {
            if (IsInLayerMask(other.gameObject.layer, ladderLayer))
            {
                activeLadders.Remove(other);
                
                // Still touching another ladder
                if (IsOnLadder())
                {
                    foreach (Collider ladder in activeLadders)
                    {
                        ladderCenter = ladder.bounds.center;
                        break;
                    }
                }
                else
                {
                    ExitLadderMode();
                }
                Debug.Log("Ladder collider exited ladder!");
            }
        }
    
        public void OnPlatformTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject.layer, platformLayer))
            {
                isOnPlatform = true;
                Debug.Log("Platform collider detected platform!");
            }
        }
    
        public void OnPlatformTriggerExit(Collider other)
        {
            if (IsInLayerMask(other.gameObject.layer, platformLayer))
            {
                isOnPlatform = false;
                Debug.Log("Platform collider exited platform!");
            }
        }

        // =========================================================
        // LAYER CHECK
        // =========================================================

        private bool IsInLayerMask(int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) != 0;
        }
}