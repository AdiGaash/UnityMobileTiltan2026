using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/Item Collected Event")]
public class ItemCollectedEventSO : ScriptableObject
{
    // Event with no parameters
    public event Action OnRaised;

    // Method to trigger event
    public void Raise()
    {
        OnRaised?.Invoke();
    }
}