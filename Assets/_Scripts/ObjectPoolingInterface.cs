using System;

public interface IPoolableObject
{
    
    /// <summary>
    /// Sets up an action to be called when the object should be returned to pool
    /// </summary>
    /// <param name="returnAction">Action to execute when return is triggered</param>
    void SetReturnAction(Action returnAction);
    
    /// <summary>
    /// Triggers the return action manually
    /// </summary>
    void TriggerReturn();
    
}
