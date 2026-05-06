
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic ScriptableObject that implements the Observer pattern for events with data.
/// This version passes data of type T to all listeners when raised.
/// Uses a List<T> to manually manage listeners instead of UnityEvent for more control.
/// </summary>
/// <typeparam name="T">The type of data passed to listeners when the event is raised</typeparam>
public class GameEvent<T> : ScriptableObject
{
    /// <summary>
    /// List of all listeners subscribed to this event.
    /// Uses a List instead of UnityEvent to allow custom iteration and removal logic.
    /// </summary>
    readonly List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();

    /// <summary>
    /// Triggers the event and invokes all registered listeners with the provided value.
    /// Iterates backwards through the list to safely remove listeners during iteration if needed.
    /// </summary>
    /// <param name="value">The data to pass to all listeners</param>
    public virtual void Raise(T value)
    {
        // Iterate backwards to safely allow listeners to unsubscribe during the raise
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    /// <summary>
    /// Adds a listener to this event if it's not already subscribed.
    /// Prevents duplicate subscriptions of the same listener.
    /// </summary>
    /// <param name="listener">The listener implementing IGameEventListener<T> to subscribe</param>
    public virtual void Subscribe(IGameEventListener<T> listener)
    {
        // Only add if this listener is not already in the list (prevent duplicates)
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    /// <summary>
    /// Removes a listener from this event.
    /// The listener will no longer be called when Raise(T value) is invoked.
    /// </summary>
    /// <param name="listener">The listener implementing IGameEventListener<T> to unsubscribe</param>
    public virtual void Unsubscribe(IGameEventListener<T> listener) => listeners.Remove(listener);
}

/// <summary>
/// Non-generic version of GameEvent that inherits from GameEvent<Unit>.
/// Allows events to be raised without needing to pass data, using a dummy Unit struct.
/// This eliminates the need for a separate non-generic implementation.
/// Usage: Create a GameEvent (not GameEvent<T>) for simple notifications without data.
/// </summary>
[CreateAssetMenu (menuName = "Game Event (No Data)")]
public class GameEvent : GameEvent<Unit>
{
    /// <summary>
    /// Raises the event without requiring any data parameter.
    /// Internally passes Unit.Default to satisfy the generic base class.
    /// </summary>
    public void Raise() => Raise(Unit.Default);
}

/// <summary>
/// A dummy struct used as a placeholder type for GameEvent when no data needs to be passed.
/// This allows the non-generic GameEvent to inherit from the generic GameEvent<T> cleanly.
/// Eliminates the need for two separate implementations.
/// </summary>
public struct Unit
{
    /// <summary>
    /// Returns the default value of the Unit struct.
    /// Used when raising GameEvent without data.
    /// </summary>
    public static Unit Default => default;
}

/// <summary>
/// Interface for objects that want to listen to generic game events.
/// Any class implementing this interface can subscribe to GameEvent<T>.
/// Defines the callback method that gets invoked when an event is raised.
/// </summary>
/// <typeparam name="T">The type of data the listener expects to receive</typeparam>
public interface IGameEventListener<T>
{
    /// <summary>
    /// Called when the event is raised with data.
    /// Implement this method to respond to the event.
    /// </summary>
    /// <param name="data">The data passed from the event</param>
    void OnEventRaised(T data);
}

/// <summary>
/// A MonoBehaviour component that listens to GameEvent<T> and responds with a UnityEvent.
/// This acts as a bridge between a GameEvent<T> and a UnityEvent<T> response.
/// Automatically subscribes/unsubscribes on enable/disable for proper cleanup.
/// </summary>
/// <typeparam name="T">The type of data this listener expects to receive</typeparam>
public class GameEventListener<T> : MonoBehaviour, IGameEventListener<T>
{
    /// <summary>
    /// Reference to the GameEvent<T> this listener subscribes to.
    /// Must be assigned in the Unity Inspector.
    /// </summary>
    [SerializeField] private GameEvent<T> gameEvent;
    
    /// <summary>
    /// The response UnityEvent that gets invoked when the event is raised.
    /// Can be configured with callbacks in the Unity Inspector.
    /// </summary>
    [SerializeField] private UnityEvent<T> response;

    /// <summary>
    /// Called when the GameObject/component is enabled.
    /// Subscribes this listener to the GameEvent<T>.
    /// Includes null check to prevent errors if gameEvent is not assigned in the Inspector.
    /// </summary>
    private void OnEnable()
    {
        if (gameEvent != null)
            gameEvent.Subscribe(this);
    }

    /// <summary>
    /// Called when the GameObject/component is disabled.
    /// Unsubscribes this listener from the GameEvent<T> to prevent memory leaks and stale references.
    /// Includes null check to prevent errors if gameEvent is not assigned in the Inspector.
    /// </summary>
    private void OnDisable()
    {
        if (gameEvent != null)
            gameEvent.Unsubscribe(this);
    }

    /// <summary>
    /// Called when the GameEvent<T> is raised.
    /// Invokes the response UnityEvent with the received data.
    /// Uses null-safe operator (?.) to handle cases where response might not be configured.
    /// </summary>
    /// <param name="data">The data passed from the GameEvent</param>
    public void OnEventRaised(T data)
    {
        response?.Invoke(data);
    }
}