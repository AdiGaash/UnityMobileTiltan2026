using UnityEngine;

/// <summary>
/// GameManager class that uses the Singleton pattern to manage game logic.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // Add any public or private fields for managing game state here

    public IntVariable score;
    public void AddScore(int scoreToAdd)
    {
        score.Value += scoreToAdd;
    }

    protected override void Awake()
    {
        base.Awake();
        InitGame();
    }

    public void InitGame()
    {
        score.Value = 0;
    }
    
    

    // Add any additional methods for managing game state here
}