using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Events/Item Collected Unity Event")]
public class ItemCollectedUnityEventSO : ScriptableObject
{
    // UnityEvent = visible in Inspector + designer-friendly
    public UnityEvent OnRaised;

    // Method used by gameplay code to trigger the event
    public void Raise()
    {
        Debug.Log("ItemCollectedEventSO Raised");

        // Safely invoke UnityEvent (if anything is connected)
        OnRaised?.Invoke();
    }
}