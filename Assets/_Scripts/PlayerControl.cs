using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float verticalSpeed = 4f;

    
    [Header("Boundary Checking")] [SerializeField]
    private float boundaryBuffer = 0.1f; // Small buffer to prevent edge clipping

    public Vector2 MovementInput { get; set; } = Vector2.zero;

    // Current state
    private MovementMode movementMode = MovementMode.Platform;

    private bool isOnPlatform = true;
    
    List <Collider> activeLadders = new List<Collider>();

    private bool isOnLadder => activeLadders.Count > 0;

    
    private Vector3 ladderCenter;

    // Platform boundary tracking
    private Collider currentPlatform;
    private Bounds platformBounds;

    
    private InputSystemActions inputActions;
    private bool jumpInputPressed = false;
    private Coroutine jumpCoroutine;
    

    public enum MovementMode
    {
        Platform,
        Ladder
    }

    private Collider playerCollider;


    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        movementMode = MovementMode.Platform;
        InitializePlayerInput();

    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void InitializePlayerInput()
    {
        inputActions = new InputSystemActions();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput; // Add jump input handling
        inputActions.Enable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        jumpInputPressed = context.performed;
    }

    /// <summary>
    /// Check if jump input was pressed (called by PowerUpEffect)
    /// </summary>
    public bool TryGetJumpInput()
    {
        if (jumpInputPressed)
        {
            jumpInputPressed = false; // Consume the input
            return true;
        }
        return false;
    }

    /// <summary>
    /// Perform a jump to a target position
    /// </summary>
    public void JumpToPosition(Vector3 targetPosition, float jumpDuration)
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
        }
        jumpCoroutine = StartCoroutine(JumpCoroutine(targetPosition, jumpDuration));
    }

    private System.Collections.IEnumerator JumpCoroutine(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        // Clamp target position to platform bounds if on platform
        if (currentPlatform != null && movementMode == MovementMode.Platform)
        {
            targetPosition = ClampToPlatformBounds(targetPosition);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Use a parabolic curve for jump arc
            float height = Mathf.Sin(progress * Mathf.PI) * 2f; // Adjust height multiplier as needed

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y += height;

            transform.position = currentPosition;
            yield return null;
        }

        // Ensure final position is set correctly
        transform.position = targetPosition;
    }

    public bool IsOnLadder => activeLadders.Count > 0;
    
    
    
     void LateUpdate()
    {
        ApplyMovement();
    }




    private void ApplyMovement()
    {
        Vector3 move = Vector3.zero;
        float horizontalInput = Mathf.Clamp(MovementInput.x, -1f, 1f);
        float verticalInput = Mathf.Clamp(MovementInput.y, -1f, 1f);

        if (movementMode == MovementMode.Platform)
        {

            move.x = horizontalInput * horizontalSpeed;

            if (isOnLadder && Mathf.Abs(verticalInput) > 0.01f)
            {
                EnterLadderMode();
            }
        }
        else if (movementMode == MovementMode.Ladder)
        {
            if (!isOnLadder)
            {
                ExitLadderMode();
                return;
            }

            if (Mathf.Abs(horizontalInput) > 0.01f && isOnPlatform)
            {
                ExitLadderMode();
                move.x = horizontalInput * horizontalSpeed;
            }
            else
            {
                Vector3 position = transform.position;
                position.x = ladderCenter.x;
                transform.position = position;
                move.y = verticalInput * verticalSpeed;
            }
        }

        // Apply movement with boundary checking
        Vector3 newPosition = transform.position + move * Time.deltaTime;

        // Clamp to platform boundaries if on platform
        if (currentPlatform != null && movementMode == MovementMode.Platform)
        {
            newPosition = ClampToPlatformBounds(newPosition);
        }

        transform.position = newPosition;
    }


    private Vector3 ClampToPlatformBounds(Vector3 targetPosition)
    {
        if (playerCollider == null) return targetPosition;

        // Calculate the effective boundaries considering player size
        float playerHalfWidth = playerCollider.bounds.size.x * 0.5f;
        float playerHalfHeight = playerCollider.bounds.size.y * 0.5f;
        
        float leftBound = platformBounds.min.x + playerHalfWidth + boundaryBuffer;
        float rightBound = platformBounds.max.x - playerHalfWidth - boundaryBuffer;

        // Clamp horizontal position
        targetPosition.x = Mathf.Clamp(targetPosition.x, leftBound, rightBound);
        
        // Ensure player stays on top of platform (bottom of player collider should be at platform top)
        targetPosition.y = platformBounds.max.y + playerHalfHeight;

        return targetPosition;
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
        
        // Snap immediately to platform and ensure within bounds
        Vector3 position = transform.position;
        if (currentPlatform != null)
        {
            position = ClampToPlatformBounds(position);
        }
        transform.position = position;
    }

    // =========================================================
    // TRIGGER DETECTOR CALLBACKS
    // =========================================================

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            activeLadders.Add(other);
            ladderCenter = other.bounds.center;
            
            EnterLadderMode();
            return;
        }
        if (other.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
            currentPlatform = other;
            platformBounds = other.bounds;
            return;
        }
         
        other.TryGetComponent(out ICollectible collectible);
        if (collectible != null)
        {
            collectible.OnCollected();
        }
        
        

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            activeLadders.Remove(other);
            
            if (isOnLadder)
            {
                // Update ladder center to the first remaining ladder
                if (activeLadders.Count > 0)
                {
                    ladderCenter = activeLadders[0].bounds.center;
                }
            }
            else
            {
                ExitLadderMode();
            }
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
            if (currentPlatform == other)
            {
                currentPlatform = null;
            }
        }
    }
}










