using UnityEngine;

/// <summary>
/// Demonstrates how to use an IntVariable to trigger GameEvent calls.
/// This script listens for changes to an IntVariable and can raise events when certain conditions are met.
/// </summary>
public class IntVariableGameEventDemo : MonoBehaviour
{
    [Header("IntVariable Reference")]
    [SerializeField] private IntVariable intVariable;
    
    [Header("GameEvents to Trigger")]
    [SerializeField] private GameEvent onValueChangedEvent;
    [SerializeField] private GameEvent<int> onValueChangedWithDataEvent;
    [SerializeField] private GameEvent onThresholdReachedEvent;
    
    [Header("Configuration")]
    [SerializeField] private int thresholdValue = 10;
    [SerializeField] private bool enableDebugLogs = true;
    
    private int previousValue;

    /// <summary>
    /// Subscribe to IntVariable value changes when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (intVariable != null)
        {
            intVariable.OnValueChanged += OnIntVariableChanged;
            previousValue = intVariable.Value;
            
            if (enableDebugLogs)
                Debug.Log($"[IntVariableGameEventDemo] Subscribed to IntVariable. Current value: {intVariable.Value}");
        }
        else
        {
            Debug.LogWarning("[IntVariableGameEventDemo] IntVariable reference is null! Please assign it in the Inspector.");
        }
    }

    /// <summary>
    /// Unsubscribe from IntVariable value changes when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (intVariable != null)
        {
            intVariable.OnValueChanged -= OnIntVariableChanged;
            
            if (enableDebugLogs)
                Debug.Log("[IntVariableGameEventDemo] Unsubscribed from IntVariable.");
        }
    }

    /// <summary>
    /// Called whenever the IntVariable's value changes.
    /// Triggers appropriate GameEvents based on the new value.
    /// </summary>
    /// <param name="newValue">The new value of the IntVariable</param>
    private void OnIntVariableChanged(int newValue)
    {
        if (enableDebugLogs)
            Debug.Log($"[IntVariableGameEventDemo] IntVariable changed from {previousValue} to {newValue}");

        // Trigger basic GameEvent (no data)
        TriggerValueChangedEvent();

        // Trigger GameEvent with int data
        TriggerValueChangedWithDataEvent(newValue);

        // Check if threshold was reached
        CheckThresholdReached(newValue);

        previousValue = newValue;
    }

    /// <summary>
    /// Triggers a simple GameEvent without data when the value changes.
    /// </summary>
    private void TriggerValueChangedEvent()
    {
        if (onValueChangedEvent != null)
        {
            onValueChangedEvent.Raise();
            
            if (enableDebugLogs)
                Debug.Log("[IntVariableGameEventDemo] Triggered onValueChangedEvent");
        }
    }

    /// <summary>
    /// Triggers a GameEvent with the new int value as data.
    /// </summary>
    /// <param name="value">The new value to pass with the event</param>
    private void TriggerValueChangedWithDataEvent(int value)
    {
        if (onValueChangedWithDataEvent != null)
        {
            onValueChangedWithDataEvent.Raise(value);
            
            if (enableDebugLogs)
                Debug.Log($"[IntVariableGameEventDemo] Triggered onValueChangedWithDataEvent with value: {value}");
        }
    }

    /// <summary>
    /// Checks if the new value has reached or exceeded the threshold and triggers an event if so.
    /// </summary>
    /// <param name="newValue">The new value to check</param>
    private void CheckThresholdReached(int newValue)
    {
        // Only trigger if we crossed the threshold (wasn't above before, but is now)
        if (previousValue < thresholdValue && newValue >= thresholdValue)
        {
            if (onThresholdReachedEvent != null)
            {
                onThresholdReachedEvent.Raise();
                
                if (enableDebugLogs)
                    Debug.Log($"[IntVariableGameEventDemo] Threshold {thresholdValue} reached! Triggered onThresholdReachedEvent");
            }
        }
    }

    /// <summary>
    /// Public method to manually increment the IntVariable value.
    /// Useful for testing or triggering from UI buttons.
    /// </summary>
    [ContextMenu("Increment Value")]
    public void IncrementValue()
    {
        if (intVariable != null)
        {
            intVariable.Value++;
        }
    }

    /// <summary>
    /// Public method to manually decrement the IntVariable value.
    /// Useful for testing or triggering from UI buttons.
    /// </summary>
    [ContextMenu("Decrement Value")]
    public void DecrementValue()
    {
        if (intVariable != null)
        {
            intVariable.Value--;
        }
    }

    /// <summary>
    /// Public method to set the IntVariable to a specific value.
    /// Useful for testing or triggering from UI inputs.
    /// </summary>
    /// <param name="newValue">The value to set</param>
    public void SetValue(int newValue)
    {
        if (intVariable != null)
        {
            intVariable.Value = newValue;
        }
    }

    /// <summary>
    /// Public method to reset the IntVariable to its default value.
    /// </summary>
    [ContextMenu("Reset to Default")]
    public void ResetToDefault()
    {
        if (intVariable != null)
        {
            // Note: We can't directly access defaultValue, so we'll set to 0
            // In a real implementation, you might want to expose defaultValue in IntVariable
            intVariable.Value = 0;
        }
    }
}