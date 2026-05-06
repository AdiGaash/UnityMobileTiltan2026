using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private IntVariable scoreVariable;

    

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
        
        Debug.Log($"Score updated to: {newScore}");
    }
}