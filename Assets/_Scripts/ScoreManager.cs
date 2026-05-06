using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private IntVariable scoreVariable;

    public UnityEvent<int> OnScoreUpdated = new UnityEvent<int>();

    private void Start()
    {
        // Subscribe to score changes from the IntVariable
        if (scoreVariable != null)
        {
            scoreVariable.OnValueChanged += HandleScoreChanged;
        }
        else
        {
            Debug.LogWarning("ScoreManager: scoreVariable is not assigned!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (scoreVariable != null)
        {
            scoreVariable.OnValueChanged -= HandleScoreChanged;
        }
    }

    
    private void HandleScoreChanged(int newScore)
    {
        OnScoreUpdated.Invoke(newScore);
        Debug.Log($"Score updated to: {newScore}");
    }
}