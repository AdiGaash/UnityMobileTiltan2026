using UnityEngine;
using UnityEngine.Events;

public class GameEvent : ScriptableObject
{
    [SerializeField] protected UnityEvent eventInvoked = new UnityEvent();
    
    public virtual void Raise() => eventInvoked?.Invoke();
    public virtual void Subscribe(UnityAction listener) => eventInvoked.AddListener(listener);
    public virtual void Unsubscribe(UnityAction listener) => eventInvoked.RemoveListener(listener);
}

public class GameEvent<T> : ScriptableObject
{
    [SerializeField] protected UnityEvent<T> eventInvoked = new UnityEvent<T>();
    
    public virtual void Raise(T value) => eventInvoked?.Invoke(value);
    public virtual void Subscribe(UnityAction<T> listener) => eventInvoked.AddListener(listener);
    public virtual void Unsubscribe(UnityAction<T> listener) => eventInvoked.RemoveListener(listener);
}