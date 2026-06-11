using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// AsyncOperationsDemo demonstrates various async/await patterns in Unity.
/// 
/// Key Concepts:
/// - Async/Await: Modern C# pattern for writing asynchronous code that looks like synchronous code
/// - Tasks: C# threading primitives that represent ongoing operations
/// - Threading: Running code on background threads (use async/await instead for most cases)
/// 
/// When to use what:
/// - async/await: For waiting on network calls, file I/O, or other time-consuming operations without blocking the main thread
/// - Tasks: For threading and parallel operations
/// </summary>
public class AsyncOperationsDemo : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== Async Operations Demo ===");
        
        // Example 1: Async/await method (fire and forget)
        _ = AsyncMethodExample();
        
        // Example 2: Task with delay
        _ = TaskDelayExample();
    }
    
    /// <summary>
    /// Example 1: Async/Await Method
    /// Modern C# pattern for writing asynchronous code.
    /// Good for network requests, file operations, or other non-frame-based delays.
    /// </summary>
    private async Task AsyncMethodExample()
    {
        Debug.Log("[Async] Starting async method example");
        
        // Simulate an operation taking 1 second
        await Task.Delay(1000);
        Debug.Log("[Async] 1 second delay completed using Task.Delay");
        
        // Simulate another operation
        await Task.Delay(1500);
        Debug.Log("[Async] Another 1.5 second delay completed");
    }

    /// <summary>
    /// Example 2: Task.Delay vs WaitForSeconds
    /// Shows the difference between frame-based (coroutine) and real-time (async) delays.
    /// </summary>
    private async Task TaskDelayExample()
    {
        Debug.Log("[TaskDelay] Starting task delay example");
        
        // Task.Delay waits in real-time (not affected by frame rate or Time.timeScale)
        await Task.Delay(500);
        Debug.Log("[TaskDelay] 500ms real-time delay completed");
    }

    /// <summary>
    /// Example 3: Chaining Multiple Async Operations
    /// Shows how async/await makes sequential operations easier to read.
    /// </summary>
    public async Task ChainedAsyncOperations()
    {
        Debug.Log("[Chained] Operation 1 starting");
        await Task.Delay(1000);
        Debug.Log("[Chained] Operation 1 complete, starting Operation 2");
        
        await Task.Delay(1500);
        Debug.Log("[Chained] Operation 2 complete, starting Operation 3");
        
        await Task.Delay(500);
        Debug.Log("[Chained] Operation 3 complete - all operations finished!");
    }

    /// <summary>
    /// Example 4: Parallel Async Operations
    /// Shows how to run multiple async operations concurrently.
    /// </summary>
    public async Task ParallelAsyncOperations()
    {
        Debug.Log("[Parallel] Starting 3 operations in parallel");
        
        // These tasks run concurrently, not sequentially
        Task task1 = Task.Delay(1000)
            .ContinueWith(_ => Debug.Log("[Parallel] Task 1 complete"));
        
        Task task2 = Task.Delay(1500)
            .ContinueWith(_ => Debug.Log("[Parallel] Task 2 complete"));
        
        Task task3 = Task.Delay(800)
            .ContinueWith(_ => Debug.Log("[Parallel] Task 3 complete"));
        
        // Wait for all tasks to complete
        await Task.WhenAll(task1, task2, task3);
        Debug.Log("[Parallel] All tasks completed!");
    }

    /// <summary>
    /// Example 5: Wait for First Completion
    /// Shows how to handle the first completed task among multiple async operations.
    /// </summary>
    public async Task WaitForFirstCompletion()
    {
        Debug.Log("[FirstCompletion] Starting 3 operations, waiting for first");
        
        Task task1 = Task.Delay(2000)
            .ContinueWith(_ => Debug.Log("[FirstCompletion] Task 1 complete"));
        
        Task task2 = Task.Delay(500)
            .ContinueWith(_ => Debug.Log("[FirstCompletion] Task 2 complete"));
        
        Task task3 = Task.Delay(1500)
            .ContinueWith(_ => Debug.Log("[FirstCompletion] Task 3 complete"));
        
        // Wait for the first one to complete
        await Task.WhenAny(task1, task2, task3);
        Debug.Log("[FirstCompletion] First task completed! (Others still running)");
    }

    /// <summary>
    /// Example 6: Async Method with Return Value
    /// Shows how to return data from async operations.
    /// </summary>
    public async Task<int> AsyncMethodWithReturnValue()
    {
        Debug.Log("[ReturnValue] Operation starting");
        await Task.Delay(1000);
        Debug.Log("[ReturnValue] Operation complete, returning value");
        return 42;
    }

    /// <summary>
    /// Example 7: Using Async Method with Return Value
    /// Demonstrates how to use and get the result from async operations.
    /// </summary>
    public async Task UseAsyncWithReturnValue()
    {
        int result = await AsyncMethodWithReturnValue();
        Debug.Log($"[UseReturnValue] Got result: {result}");
    }

   

    /// <summary>
    /// Example 8: Exception Handling in Async
    /// Shows how to handle errors in async operations.
    /// </summary>
    public async Task AsyncWithExceptionHandling()
    {
        try
        {
            Debug.Log("[ExceptionHandling] Starting operation that might fail");
            await Task.Delay(500);
            
            // Simulate an error condition
            if (Random.value > 0.5f)
            {
                throw new System.Exception("Something went wrong!");
            }
            
            Debug.Log("[ExceptionHandling] Operation completed successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ExceptionHandling] Caught exception: {ex.Message}");
        }
        finally
        {
            Debug.Log("[ExceptionHandling] Cleanup code here if needed");
        }
    }

    /// <summary>
    /// Example 9: Async Loop (Repeating Operation)
    /// Shows how to create a loop that waits asynchronously between iterations.
    /// </summary>
    public async Task AsyncLoop(int iterations, int delayMs)
    {
        Debug.Log($"[AsyncLoop] Starting loop with {iterations} iterations");
        
        for (int i = 0; i < iterations; i++)
        {
            Debug.Log($"[AsyncLoop] Iteration {i + 1}/{iterations}");
            await Task.Delay(delayMs);
        }
        
        Debug.Log("[AsyncLoop] Loop completed");
    }

    /// <summary>
    /// Example 12: Timeout Pattern
    /// Shows how to implement a timeout for async operations.
    /// </summary>
    public async Task AsyncWithTimeout()
    {
        Debug.Log("[Timeout] Starting operation with timeout");
        
        var delayTask = Task.Delay(5000); // 5 second operation
        var timeoutTask = Task.Delay(2000); // 2 second timeout
        
        var completedTask = await Task.WhenAny(delayTask, timeoutTask);
        
        if (completedTask == timeoutTask)
        {
            Debug.LogWarning("[Timeout] Operation timed out!");
        }
        else
        {
            Debug.Log("[Timeout] Operation completed before timeout");
        }
    }

    /// <summary>
    /// Demo method to run all examples (call from another script or button)
    /// </summary>
    public async void RunAllDemos()
    {
        Debug.Log("\n=== Running All Async Demos ===\n");
        
        await ChainedAsyncOperations();
        Debug.Log("");
        
        await ParallelAsyncOperations();
        Debug.Log("");
        
        await WaitForFirstCompletion();
        Debug.Log("");
        
        await UseAsyncWithReturnValue();
        Debug.Log("");
        
        await AsyncWithExceptionHandling();
        Debug.Log("");
        
        await AsyncLoop(3, 1000);
        Debug.Log("");
        
        await AsyncWithTimeout();
        
        Debug.Log("\n=== All Demos Completed ===\n");
    }
}

