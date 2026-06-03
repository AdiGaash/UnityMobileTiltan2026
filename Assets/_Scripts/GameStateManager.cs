using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Enumeration of possible game states
/// </summary>
public enum GameStateType
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Loading,
    Victory
}

/// <summary>
/// ScriptableObject for managing game state with events
/// </summary>
[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState")]
public class GameState : ScriptableObject
{
    [SerializeField] private GameStateType currentState;
    [SerializeField] private GameStateType defaultState = GameStateType.Menu;

    public event UnityAction<GameStateType, GameStateType> OnStateChanged = delegate { };
    public event UnityAction<GameStateType> OnStateEntered = delegate { };
    public event UnityAction<GameStateType> OnStateExited = delegate { };

    /// <summary>
    /// Current game state
    /// </summary>
    public GameStateType CurrentState
    {
        get => currentState;
        set => ChangeState(value);
    }

    /// <summary>
    /// Changes the game state and invokes appropriate events
    /// </summary>
    /// <param name="newState">The new state to transition to</param>
    public void ChangeState(GameStateType newState)
    {
        if (currentState == newState) return;

        GameStateType previousState = currentState;
        
        // Exit previous state
        OnStateExited.Invoke(previousState);
        
        // Update current state
        currentState = newState;
        
        // Enter new state
        OnStateEntered.Invoke(currentState);
        
        // Notify of state change
        OnStateChanged.Invoke(previousState, currentState);
    }

    /// <summary>
    /// Checks if the current state matches the specified state
    /// </summary>
    /// <param name="state">State to check against</param>
    /// <returns>True if current state matches</returns>
    public bool IsState(GameStateType state)
    {
        return currentState == state;
    }

    /// <summary>
    /// Checks if the current state is one of the specified states
    /// </summary>
    /// <param name="states">Array of states to check against</param>
    /// <returns>True if current state matches any of the provided states</returns>
    public bool IsAnyState(params GameStateType[] states)
    {
        foreach (var state in states)
        {
            if (currentState == state) return true;
        }
        return false;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            currentState = defaultState;
            OnStateChanged.Invoke(currentState, currentState);
        }
    }
#endif
}
