using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class GameEvent<T> : ScriptableObject
{
    readonly List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();
    
    public virtual void Raise(T value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }
    
    public virtual void Subscribe(IGameEventListener<T> listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }
   
    public virtual void Unsubscribe(IGameEventListener<T> listener) => listeners.Remove(listener);
}

// so we can use the gameevent without a parameter, and avoid the need for a dummy struct when we don't care about the data.
public class GameEvent : GameEvent<Unit>
{
    public void Raise() => Raise(Unit.Default);
}

public struct Unit
{
    public static Unit Default => default;
}



public interface IGameEventListener<T>
{
    void OnEventRaised(T data);
}

public class GameEventListener<T> : MonoBehaviour, IGameEventListener<T>
{
    [SerializeField] private GameEvent<T> gameEvent;
    [SerializeField] private UnityEvent<T> response;

    private void OnEnable()
    {
        if (gameEvent != null)
            gameEvent.Subscribe(this);
    }

    private void OnDisable()
    {
        if (gameEvent != null)
            gameEvent.Unsubscribe(this);
    }

    public void OnEventRaised(T data)
    {
        response?.Invoke(data);
    }
}