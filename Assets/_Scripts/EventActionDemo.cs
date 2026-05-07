using UnityEngine;
using System;

public class EventActionDemo : MonoBehaviour
{
    // C# event declaration
    public event Action OnPlayerDied;

    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log("Player took damage");

        // Check if player died
        if (health <= 0)
        {
            Debug.Log("Player died");

            // Raise the event safely
            OnPlayerDied?.Invoke();
        }
    }
}


public class EventActionListenerDemo : MonoBehaviour
{
    // Reference to the broadcaster
    [SerializeField]
    private EventActionDemo eventActionDemo;

    // Subscribe when object becomes active
    private void OnEnable()
    {
        eventActionDemo.OnPlayerDied += ShowGameOverScreen;
    }

    // Unsubscribe when object becomes inactive
    private void OnDisable()
    {
        eventActionDemo.OnPlayerDied -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        Debug.Log("Game Over Screen Opened");

        // Example:
        // gameOverPanel.SetActive(true);
    }
}
