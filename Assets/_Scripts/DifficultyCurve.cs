
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty Curve", menuName = "ScriptableObjects/DifficultyCurve", order = 1)]
public class DifficultyCurve : ScriptableObject
{
    public AnimationCurve difficultyOverTime;
    public float initialDifficulty = 1f;
    public float timeMultiplier = 1.0f;

    private float currentTime = 0f;

    public void ResetTime()
    {
        currentTime = 0f;
    }

    private void OnEnable()
    {
        difficultyOverTime.postWrapMode = WrapMode.ClampForever;
    }

    public float GetDifficulty(float deltaTime)
    {
        currentTime += deltaTime * timeMultiplier;
        currentTime = Mathf.Clamp(currentTime, 0f, initialDifficulty);
        return difficultyOverTime.Evaluate(currentTime);
    }
}