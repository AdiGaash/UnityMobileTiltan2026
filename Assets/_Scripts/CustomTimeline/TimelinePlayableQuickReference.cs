using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/*
 * ═══════════════════════════════════════════════════════════════════════════════════
 * TIMELINE PLAYABLE SYSTEM - QUICK REFERENCE GUIDE
 * ═══════════════════════════════════════════════════════════════════════════════════
 * 
 * This file is a condensed reference for all important concepts and code patterns.
 * Use this alongside the detailed examples to quickly look up how to do things.
 */


// ═══════════════════════════════════════════════════════════════════════════════════
// 1. BASIC STRUCTURE TEMPLATE
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * EVERY TIMELINE PLAYABLE NEEDS:
 * 
 * 1. Clip Asset (PlayableAsset)
 *    └─ Stores serialized parameters
 *    └─ Implements CreatePlayable()
 *    └─ Passes data to behavior
 * 
 * 2. Behavior (PlayableBehaviour)
 *    └─ Receives data from asset
 *    └─ Implements lifecycle methods
 *    └─ Creates animation/effect each frame
 * 
 * 3. Track Asset (TrackAsset)
 *    └─ Allows adding clip to Timeline UI
 *    └─ Configures track appearance/binding
 *    └─ Let's Timeline know what clip types to accept
 */


// ═══════════════════════════════════════════════════════════════════════════════════
// 2. PARAMETER PASSING PATTERN
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * HOW DATA FLOWS FROM INSPECTOR TO ANIMATION:
 * 
 * [Clip Asset Fields are shown in Inspector]
 *            ↓
 * User modifies values in Timeline Inspector
 *            ↓
 * user presses Play or Timeline scrubs
 *            ↓
 * CreatePlayable() is called
 *            ↓
 * YOUR CODE: behavior.intensity = this.intensity;
 *            ↓
 * Behavior now has the value
 *            ↓
 * ProcessFrame() uses behavior.intensity
 *            ↓
 * Animation happens with that value!
 */

// MINIMAL PARAMETER PASSING EXAMPLE:
public class ParameterPassingExample_ClipAsset : PlayableAsset
{
    [SerializeField] public float myParameter = 1f;  // This appears in Inspector

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ParameterPassingExample_Behavior>.Create(graph);
        var behavior = playable.GetBehaviour();
        
        // ← THIS IS THE KEY LINE ←
        behavior.myParameter = myParameter;  // Copy from asset to behavior
        
        playable.SetDuration(1f);
        return playable;
    }
}

public class ParameterPassingExample_Behavior : PlayableBehaviour
{
    public float myParameter = 1f;  // Receives data from asset

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Now you can use myParameter!
        Debug.Log("Parameter value: " + myParameter);
    }
}


// ═══════════════════════════════════════════════════════════════════════════════════
// 3. PROGRESS CALCULATION PATTERNS
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * PATTERN 1: Linear Progress (0 to 1)
 */
public class Pattern_LinearProgress : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        double currentTime = playable.GetTime();
        double duration = playable.GetDuration();
        float progress = (float)(currentTime / duration);
        progress = Mathf.Clamp01(progress);
        
        // progress is 0 at start, 1 at end
    }
}

/*
 * PATTERN 2: Eased Progress (smooth motion)
 */
public class Pattern_EasedProgress : PlayableBehaviour
{
    public AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        double currentTime = playable.GetTime();
        double duration = playable.GetDuration();
        float progress = Mathf.Clamp01((float)(currentTime / duration));
        
        // Apply easing curve for smooth motion
        float easedProgress = easingCurve.Evaluate(progress);
        
        // Use easedProgress for smooth animation
    }
}

/*
 * PATTERN 3: Delta Time (time since last frame)
 */
public class Pattern_DeltaTime : PlayableBehaviour
{
    private float accumulatedTime = 0f;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // info.deltaTime = time since last frame
        accumulatedTime += (float)info.deltaTime;
        
        // Use accumulatedTime for cumulative animation
    }
}

/*
 * PATTERN 4: Frame Number (for frame-based animation)
 */
public class Pattern_FrameNumber : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // info.frameId = current frame number
        int currentFrame = (int)info.frameId;
        
        // Use frame number for discrete animations
    }
}


// ═══════════════════════════════════════════════════════════════════════════════════
// 4. COMMON ANIMATION PATTERNS
// ═══════════════════════════════════════════════════════════════════════════════════

public class Pattern_Transform : PlayableBehaviour
{
    public float moveAmount = 5f;
    public float rotateAmount = 90f;
    public float scaleAmount = 1.5f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private GameObject target;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        target = playerData as GameObject;
        if (target == null) return;

        float progress = Mathf.Clamp01((float)(playable.GetTime() / playable.GetDuration()));
        float eased = curve.Evaluate(progress);

        // MOVE: Linear movement along axis
        target.transform.position += Vector3.right * moveAmount * eased;

        // ROTATE: Rotation around axis
        target.transform.Rotate(Vector3.up * rotateAmount * eased);

        // SCALE: Size change
        target.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(scaleAmount, scaleAmount, scaleAmount), eased);
    }
}

public class Pattern_Color : PlayableBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Renderer renderer;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject target = playerData as GameObject;
        if (target == null) return;

        if (renderer == null)
            renderer = target.GetComponent<Renderer>();

        if (renderer == null || renderer.material == null) return;

        float progress = Mathf.Clamp01((float)(playable.GetTime() / playable.GetDuration()));
        float eased = curve.Evaluate(progress);

        // Linear interpolation between colors
        renderer.material.color = Color.Lerp(startColor, endColor, eased);
    }
}

public class Pattern_Visibility : PlayableBehaviour
{
    public float fadeDistance = 2f;
    private Renderer renderer;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject target = playerData as GameObject;
        if (target == null) return;

        if (renderer == null)
            renderer = target.GetComponent<Renderer>();

        if (renderer == null) return;

        float progress = Mathf.Clamp01((float)(playable.GetTime() / playable.GetDuration()));

        // Fade out by moving invisibly far away (one way to do it)
        // Or use material alpha if using a transparent shader
        renderer.enabled = progress < 0.9f;  // Disappear at 90% done
    }
}


// ═══════════════════════════════════════════════════════════════════════════════════
// 5. BINDING PATTERNS (How to access the animated object)
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * PATTERN 1: Using playerData (Parameter passed to ProcessFrame)
 * Most common and recommended
 */
public class Pattern_Binding_PlayerData : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // playerData is the bound GameObject passed from Timeline
        GameObject target = playerData as GameObject;
        
        if (target != null)
        {
            // Animate target
        }
    }
}

/*
 * PATTERN 2: Direct reference in Clip Asset
 * Use ExposedReference for binding in Inspector
 */
public class Pattern_Binding_ExposedReference : PlayableAsset
{
    [SerializeField]
    public ExposedReference<GameObject> targetObject;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<Pattern_Binding_ExposedReference_Behavior>.Create(graph);
        var behavior = playable.GetBehaviour();
        
        // Resolve the reference using the graph's resolver
        behavior.targetObject = targetObject.Resolve(graph.GetResolver());
        
        playable.SetDuration(1f);
        return playable;
    }
}

public class Pattern_Binding_ExposedReference_Behavior : PlayableBehaviour
{
    public GameObject targetObject;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetObject != null)
        {
            // Use targetObject directly
        }
    }
}


// ═══════════════════════════════════════════════════════════════════════════════════
// 6. LIFECYCLE CALLBACK CHEAT SHEET
// ═══════════════════════════════════════════════════════════════════════════════════

public class Pattern_LifecycleAll : PlayableBehaviour
{
    private float startTime;
    private Vector3 originalPosition;

    public override void OnPlayableCreate(Playable playable)
    {
        /*
         * WHEN: Called once when playable created
         * 
         * TYPICAL USES:
         * - Caching GetComponent() calls
         * - Finding target objects
         * - Setting up references
         * 
         * BEFORE: Everything else
         */
        Debug.Log("Playable Created");
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        /*
         * WHEN: Called when playback starts (timeline play pressed)
         * 
         * TYPICAL USES:
         * - Store initial values (position, rotation, color)
         * - Start audio/effects
         * - Setup animation state
         * 
         * CALLED: Once per play (not every frame)
         */
        Debug.Log("Play Started");
        startTime = Time.time;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        /*
         * WHEN: Called every frame while clip plays
         * 
         * TYPICAL USES:
         * - Calculate animation progress
         * - Update transforms, colors, materials
         * - Apply easing curves
         * 
         * FREQUENCY: Every frame (60+ times per second typically)
         */
        Debug.Log("Processing Frame");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        /*
         * WHEN: Timeline paused (stop pressed mid-clip)
         * 
         * TYPICAL USES:
         * - Pause audio
         * - Save current animation state
         * - Freeze visual effects
         * 
         * CALLED: Once when paused
         */
        Debug.Log("Playback Paused");
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        /*
         * WHEN: Clip ends or timeline stops completely
         * 
         * TYPICAL USES:
         * - Restore original state (position, color, etc)
         * - Clean up resources
         * - Stop audio
         * 
         * CALLED: Once at end
         */
        Debug.Log("Playable Destroyed");
    }
}


// ═══════════════════════════════════════════════════════════════════════════════════
// 7. TRACK AND CLIP CONFIGURATION ATTRIBUTES
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * ATTRIBUTE: [TrackColor]
 * Sets the color of your track in the Timeline UI
 * Syntax: [TrackColor(red, green, blue)]
 * Values: 0 to 1 for each channel
 */
[TrackColor(1f, 0.5f, 0.2f)]  // Orange
public class Example_TrackColorOrange : TrackAsset { }

[TrackColor(0.2f, 0.8f, 0.2f)]  // Green
public class Example_TrackColorGreen : TrackAsset { }

/*
 * ATTRIBUTE: [TrackClipType]
 * Specifies what clip asset types this track accepts
 * Syntax: [TrackClipType(typeof(YourClipAsset))]
 */
[TrackClipType(typeof(FadeClipAsset))]  // This track ONLY accepts FadeClipAsset
public class Example_TrackClipType : TrackAsset { }

/*
 * ATTRIBUTE: [TrackBindingType]
 * Specifies what object types can be bound to this track
 * Syntax: [TrackBindingType(typeof(SomeClass))]
 */
[TrackBindingType(typeof(GameObject))]  // Binds to GameObjects
public class Example_TrackBinding_GameObject : TrackAsset { }

[TrackBindingType(typeof(Animator))]  // Binds to Animators specifically
public class Example_TrackBinding_Animator : TrackAsset { }

[TrackBindingType(typeof(AudioSource))]  // Binds to AudioSources
public class Example_TrackBinding_Audio : TrackAsset { }


// ═══════════════════════════════════════════════════════════════════════════════════
// 8. TROUBLESHOOTING QUICK REFERENCE
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * PROBLEM: "ProcessFrame is not being called"
 * SOLUTIONS:
 * - Make sure you added the clip to the timeline
 * - Check that Timeline is actually playing (press Play)
 * - Verify the clip duration is > 0
 * - Check if the playable is connected in the graph
 */

/*
 * PROBLEM: "Parameters aren't appearing in the Inspector"
 * SOLUTIONS:
 * - Make fields PUBLIC (not private)
 * - Use [SerializeField] if you want them private
 * - Make sure you're selecting the CLIP, not the track
 * - Verify the asset is actually a PlayableAsset subclass
 */

/*
 * PROBLEM: "Target GameObject is null when ProcessFrame runs"
 * SOLUTIONS:
 * - In Timeline Playable Director, bind a GameObject to the track
 * - Check that binding is not null in the Inspector
 * - Use Debug.Log() to verify playerData isn't null
 * - Remember to cast: GameObject target = playerData as GameObject;
 */

/*
 * PROBLEM: "Animation only plays once then stops"
 * SOLUTIONS:
 * - Make sure you're updating the animation every frame in ProcessFrame
 * - Check that duration is set correctly
 * - Verify ProcessFrame is actually being called (add Debug.Log)
 * - Don't early return without good reason
 */

/*
 * PROBLEM: "Changes to clip parameters don't take effect"
 * SOLUTIONS:
 * - Stop playing, then play again
 * - Remember to call behavior.field = asset.field in CreatePlayable()
 * - Refresh the timeline (sometimes needed)
 * - Make sure fields are being serialized [SerializeField]
 */


// ═══════════════════════════════════════════════════════════════════════════════════
// 9. PERFORMANCE TIPS
// ═══════════════════════════════════════════════════════════════════════════════════

/*
 * DO:
 * ✓ Cache GetComponent() results in OnPlayableCreate()
 * ✓ Use fields to pass data (not recalculate each frame)
 * ✓ Clamp values to prevent overflow errors
 * ✓ Use Mathf.Clamp01() for 0-1 ranges
 * ✓ Use AnimationCurve for easing (very efficient)
 * 
 * DON'T:
 * ✗ Call GetComponent() every frame (very slow)
 * ✗ Use GameObject.Find() in ProcessFrame (very slow)
 * ✗ Create new objects every frame (memory leak)
 * ✗ Do complex calculations without caching
 * ✗ Forget to clean up resources in OnPlayableDestroy()
 */

public class Performance_GoodExample : PlayableBehaviour
{
    private Renderer renderer;  // Cached, not looked up every frame
    private float cachedValue;
    private bool isInitialized = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Initialize on first frame instead of OnPlayableCreate
        if (!isInitialized && playerData != null)
        {
            GameObject target = playerData as GameObject;
            if (target != null)
            {
                renderer = target.GetComponent<Renderer>();
                isInitialized = true;
            }
        }

        // DO use cached renderer
        if (renderer != null)
            renderer.enabled = true;

        // DON'T call GetComponent here every frame (would be called 60+ times per second)
    }
}
// ═══════════════════════════════════════════════════════════════════════════════════
// 10. DEBUG HELPERS
// ═══════════════════════════════════════════════════════════════════════════════════

public class DebugHelper_Timeline : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        double currentTime = playable.GetTime();
        double duration = playable.GetDuration();
        float progress = (float)(currentTime / duration);

        // Print debug info
        Debug.Log($"[Timeline Debug] " +
            $"Time: {currentTime:F2}s / {duration:F2}s | " +
            $"Progress: {progress:P1} | " +
            $"Frame: {info.frameId} | " +
            $"DeltaTime: {info.deltaTime:F4}s");

        /*
         * Output example:
         * [Timeline Debug] Time: 0.50s / 2.00s | Progress: 25.0% | Frame: 30 | DeltaTime: 0.0167s
         */
    }
}

/*
 * ═══════════════════════════════════════════════════════════════════════════════════
 * END OF QUICK REFERENCE GUIDE
 * 
 * For detailed explanations, see:
 * - TimelinePlayableExample.cs (Complete Guide with everything)
 * - PracticalFadeTimelineExample.cs (Real-world working example)
 * 
 * ═══════════════════════════════════════════════════════════════════════════════════
 */

