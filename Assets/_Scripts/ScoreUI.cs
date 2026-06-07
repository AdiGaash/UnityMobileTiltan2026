using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Score Settings")]
    public IntVariable score;
    [SerializeField] private float scoreUpdateSpeed = 100f; // Points per second
    
    private TextMeshProUGUI scoreText;
    private float currentDisplayScore;
    private int targetScore;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
       
        currentDisplayScore = 0;
        targetScore = 0;
        UpdateScoreDisplay();
        score.OnValueChanged += OnScoreChanged;
    }

    

    private void Update()
    {
        // Update score coating every frame
        UpdateScoreCoating();
    }

    private void UpdateScoreCoating()
    {
        if (currentDisplayScore < targetScore)
        {
            // Smoothly increase the displayed score towards the target
            currentDisplayScore += scoreUpdateSpeed * Time.deltaTime;
            
            // Clamp to not exceed the target score
            if (currentDisplayScore > targetScore)
            {
                currentDisplayScore = targetScore;
            }
            UpdateScoreDisplay();
        }
    }

    private void OnScoreChanged(int newScore)
    {
        targetScore = newScore;
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = Mathf.FloorToInt(currentDisplayScore).ToString();
    }

    private void OnDestroy()
    {
        if (score != null)
        {
            score.OnValueChanged -= OnScoreChanged;
        }
    }
}