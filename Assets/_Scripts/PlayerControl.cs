using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float verticalSpeed = 4f;
    
    [Header("Layers")]
    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private LayerMask platformLayer;

    
    public Vector2 MovementInput { get; set; } = Vector2.zero;
    
    // Current state
    private MovementMode movementMode = MovementMode.Platform;
    private bool isOnPlatform = false;
    private HashSet<Collider> activeLadders = new HashSet<Collider>();
    private Vector3 ladderCenter;

    // Optional: For player input only
    private InputSystemActions inputActions;
    
    public enum MovementMode { Platform, Ladder }
    
    

    private void Awake()
    {
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
        inputActions.Enable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        ApplyMovement();
    }
    
    // Public API for external control (AI, etc.)
    public void SetMovementInput(Vector2 input)
    {
        MovementInput = input;
    }

    public void SetHorizontalInput(float input)
    {
        MovementInput = new Vector2(input, MovementInput.y);
    }

    public void SetVerticalInput(float input)
    {
        MovementInput = new Vector2(MovementInput.x, input);
    }

    private void ApplyMovement()
    {
        Vector3 move = Vector3.zero;
        float horizontalInput = Mathf.Clamp(MovementInput.x, -1f, 1f);
        float verticalInput = Mathf.Clamp(MovementInput.y, -1f, 1f);

        if (movementMode == MovementMode.Platform)
        {
            move.x = horizontalInput * horizontalSpeed;

            if (IsOnLadder() && Mathf.Abs(verticalInput) > 0.01f)
            {
                EnterLadderMode();
            }
        }
        else if (movementMode == MovementMode.Ladder)
        {
            if (!IsOnLadder())
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

        transform.position += move * Time.deltaTime;
    }
    
    
    

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
            
        }
    }

    public void OnPlatformTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, platformLayer))
        {
            isOnPlatform = true;
           
        }
    }

    public void OnPlatformTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, platformLayer))
        {
            isOnPlatform = false;
           
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