using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/*
 * ═══════════════════════════════════════════════════════════════════════════════════
 * TIMELINE PLAYABLE SYSTEM - COMPLETE GUIDE
 * ═══════════════════════════════════════════════════════════════════════════════════
 * 
 * This script demonstrates how to create a custom Timeline playable system that includes:
 * 1. A Clip Asset - Stores data/configuration for the clip
 * 2. A Playable Behavior - Handles the actual execution/animation logic
 * 3. A Track Asset - Manages clips on the timeline track
 * 
 * The complete workflow:
 * Clip Asset (stores data) → Playable Behavior (executes logic) → Output (animation result)
 * ═══════════════════════════════════════════════════════════════════════════════════
 */

/// <summary>
/// STEP 1: Create a Playable Asset (The Data Container)
/// 
/// A ScriptableObject that stores all parameters and configuration for your clip.
/// This asset is what gets created when you drag a clip onto the timeline.
/// Think of it as the "blueprint" or "configuration" of your clip.
/// </summary>
public class MyCustomClipAsset : PlayableAsset
{
    /*
     * ─────────────────────────────────────────────────────────────────────────
     * IMPORTANT CONCEPTS:
     * ─────────────────────────────────────────────────────────────────────────
     * 
     * 1. PUBLIC FIELDS = Timeline Inspector Parameters
     *    - These appear in the Inspector when you select a clip
     *    - Users can modify these values directly in the Timeline UI
     *    - Must be serializable (public or [SerializeField] with basic types)
     * 
     * 2. THESE DATA ARE PASSED TO THE PLAYABLE BEHAVIOR
     *    - When the timeline plays, the data from this asset is sent to the
     *      corresponding behavior through PlayableAsset.CreatePlayable()
     */

    // ──────────────────────────────────────────────────────────────────────────
    // EXAMPLE PARAMETERS - What the user can configure in the Inspector
    // ──────────────────────────────────────────────────────────────────────────

    [SerializeField]
    public float duration = 1f;  // How long this clip plays (in seconds)

    [SerializeField]
    public float intensity = 1f;  // Animation intensity (0-1)

    [SerializeField]
    public Color tintColor = Color.white;  // Color to apply (example parameter)

    [SerializeField]
    public AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    // ^ Allows users to define how animation eases in/out

    [SerializeField]
    public string targetObjectName = "";  // Name of object to affect

    [SerializeField]
    public float rotationAmount = 90f;  // Example: rotation in degrees

    // Optional: Reference to a GameObject or component this clip should affect
    [SerializeField]
    public ExposedReference<GameObject> targetGameObject;


    // ──────────────────────────────────────────────────────────────────────────
    // REQUIRED OVERRIDE: CreatePlayable
    // ──────────────────────────────────────────────────────────────────────────
    /*
     * This is THE MOST IMPORTANT method!
     * 
     * HOW IT WORKS:
     * 1. Timeline calls this method when it needs to create the actual playable
     * 2. You create a Playable (using ScriptPlayable<T>) with your behavior
     * 3. You set the template/data on that behavior
     * 4. The behavior receives this data and uses it during playback
     * 
     * SIGNATURE EXPLANATION:
     * - PlayableGraph graph: The graph that manages all playables this frame
     * - GameObject owner: The GameObject that owns the Timeline
     * - defaultAnimator: If you're animating an Animator component
     * - localTitle: What to display in the Timeline UI
     */
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Step 1: Create a ScriptPlayable wrapper for your behavior
        // The generic type <MyCustomPlayableBehavior> tells it what behavior to use
        var playable = ScriptPlayable<MyCustomPlayableBehavior>.Create(graph);

        // Step 2: Get the behavior instance from the playable
        var behavior = playable.GetBehaviour();

        // Step 3: TRANSFER DATA from this Asset to the Behavior
        // This is how parameters are passed!
        // (See MyCustomPlayableBehavior for how the behavior uses this data)
        behavior.intensity = intensity;
        behavior.tintColor = tintColor;
        behavior.easeInOutCurve = easeInOutCurve;
        behavior.targetObjectName = targetObjectName;
        behavior.rotationAmount = rotationAmount;
        behavior.targetGameObject = targetGameObject.Resolve(graph.GetResolver());

        // Step 4: Set the duration for how long this clip plays
        // This affects how many frames the behavior's methods are called
        playable.SetDuration(duration);

        // Step 5: Return the playable
        // Timeline will use this to integrate with the animation graph
        return playable;
    }

    
}


/// <summary>
/// STEP 2: Create a Playable Behavior (The Execution Engine)
/// 
/// This is where the actual logic happens during timeline playback.
/// These methods are called repeatedly as the timeline plays.
/// </summary>
public class MyCustomPlayableBehavior : PlayableBehaviour
{
    /*
     * ─────────────────────────────────────────────────────────────────────────
     * IMPORTANT: These fields receive data from MyCustomClipAsset.CreatePlayable()
     * 
     * The data flow is:
     * ClipAsset (Inspector) → CreatePlayable() → Behavior fields → Used in callbacks
     * ─────────────────────────────────────────────────────────────────────────
     */

    // Public fields to receive data from the clip asset
    public float intensity = 1f;
    public Color tintColor = Color.white;
    public AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public string targetObjectName = "";
    public float rotationAmount = 90f;
    public GameObject targetGameObject;

    // Internal state variables
    private bool hasInitialized = false;
    private GameObject cachedTarget;


    // ──────────────────────────────────────────────────────────────────────────
    // LIFECYCLE METHOD 1: OnPlayableCreate
    // ──────────────────────────────────────────────────────────────────────────
    /*
     * WHEN: Called once when the playable is first created
     * USE CASES:
     * - Finding target objects
     * - Caching components
     * - Storing initial values (for lerping/animations)
     * - Setting up references
     * 
     * Called BEFORE the first frame of playback
     */
    public override void OnPlayableCreate(Playable playable)
    {
        Debug.Log("Playable Created! This is called once.");

        hasInitialized = true;

        // Example: Find the target object if it wasn't assigned directly
        if (targetGameObject != null)
        {
            cachedTarget = targetGameObject;
        }
        else if (!string.IsNullOrEmpty(targetObjectName))
        {
            cachedTarget = GameObject.Find(targetObjectName);
            if (cachedTarget == null)
            {
                Debug.LogWarning($"Could not find target object: {targetObjectName}");
            }
        }
    }


    // ──────────────────────────────────────────────────────────────────────────
    // LIFECYCLE METHOD 2: OnBehaviourPlay
    // ──────────────────────────────────────────────────────────────────────────
    /*
     * WHEN: Called when the clip starts playing (when timeline play begins)
     * USE CASES:
     * - Start sounds/effects
     * - Initialize animation state
     * - Reset positions for animation
     * - Trigger events at clip start
     * 
     * TIP: Only called when playback actually starts (not every frame)
     */
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("Clip started playing!");

        if (cachedTarget != null)
        {
            Renderer renderer = cachedTarget.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Store original color in the target for restoration later
                cachedTarget.GetComponent<ColorAnimator>()?.OnClipStart();
            }
        }
    }


    // ──────────────────────────────────────────────────────────────────────────
    // LIFECYCLE METHOD 3: ProcessFrame (THE MAIN ANIMATION LOOP)
    // ──────────────────────────────────────────────────────────────────────────
    /*
     * WHEN: Called once per frame while the clip is playing
     * CALLS: This is your main animation/logic loop
     * FREQUENCY: If your clip is 2 seconds at 60fps, this called ~120 times
     * 
     * HOW TO GET CURRENT TIME:
     * - playable.GetTime() - Current time in the clip (0 to duration)
     * - info.deltaTime - How much time passed since last frame
     * - info.frameId - Current frame number
     * 
     * THIS IS WHERE YOU:
     * 1. Calculate animation progress (0 to 1)
     * 2. Apply transformations (rotation, scale, position, etc.)
     * 3. Update material properties
     * 4. Use easing curves for smooth motion
     */
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Only process if we have a valid target
        if (cachedTarget == null)
            return;

        /*
         * ─────────────────────────────────────────────────────────────────────
         * STEP 1: Calculate Animation Progress (0.0 to 1.0)
         * ─────────────────────────────────────────────────────────────────────
         */
        double currentTime = playable.GetTime();
        double duration = playable.GetDuration();

        // Safety check
        if (duration <= 0)
            return;

        // Progress from 0 to 1 (0 = start of clip, 1 = end of clip)
        float progress = (float)(currentTime / duration);

        // Clamp to 0-1 range to prevent overflow
        progress = Mathf.Clamp01(progress);

        /*
         * ─────────────────────────────────────────────────────────────────────
         * STEP 2: Apply Easing Curve (Optional but Recommended)
         * ─────────────────────────────────────────────────────────────────────
         * An easing curve makes motion feel more natural by:
         * - Easing in: slow start, fast end
         * - Easing out: fast start, slow end
         * - Custom curves: smooth, bouncy, or any custom motion
         */
        float easedProgress = easeInOutCurve.Evaluate(progress);

        /*
         * ─────────────────────────────────────────────────────────────────────
         * STEP 3: Apply Visual Effects/Modifications
         * ─────────────────────────────────────────────────────────────────────
         */

        // Example 1: Rotate the object
        float currentRotation = rotationAmount * easedProgress * intensity;
        cachedTarget.transform.Rotate(0, currentRotation, 0, Space.Self);

        // Example 2: Tint the object's material
        Renderer renderer = cachedTarget.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            // Blend between original color and tint color
            Color currentColor = Color.Lerp(Color.white, tintColor, easedProgress * intensity);
            renderer.material.color = currentColor;
        }

        // Example 3: Scale animation
        float scale = 1f + (0.2f * easedProgress * intensity);
        cachedTarget.transform.localScale = new Vector3(scale, scale, scale);

        Debug.Log($"Frame: Progress={progress:F2}, Eased={easedProgress:F2}, Time={currentTime:F2}s");
    }


    // ──────────────────────────────────────────────────────────────────────────
    // LIFECYCLE METHOD 4: OnBehaviourPause
    // ──────────────────────────────────────────────────────────────────────────
    /*
     * WHEN: Called when the clip is paused (timeline paused)
     * USE CASES:
     * - Stop sounds
     * - Pause animations
     * - Save animation state
     */
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log("Clip paused!");
    }


    // ──────────────────────────────────────────────────────────────────────────
    // LIFECYCLE METHOD 5: OnPlayableDestroy
    // ──────────────────────────────────────────────────────────────────────────
    /*
     * WHEN: Called when the playable is destroyed (clip ends or timeline stops)
     * USE CASES:
     * - Clean up resources
     * - Restore original states
     * - Reset objects to pre-animation state
     */
    public override void OnPlayableDestroy(Playable playable)
    {
        Debug.Log("Playable destroyed!");

        if (cachedTarget != null)
        {
            // Restore original state
            cachedTarget.transform.localScale = Vector3.one;
            Renderer renderer = cachedTarget.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = Color.white;
            }
        }

        cachedTarget = null;
        hasInitialized = false;
    }
}





/// <summary>
/// HELPER CLASS: ColorAnimator
/// 
/// A helper component that can be attached to GameObjects to support color animation.
/// </summary>
public class ColorAnimator : MonoBehaviour
{
    private Color originalColor;
    private Renderer targetRenderer;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null && targetRenderer.material != null)
        {
            originalColor = targetRenderer.material.color;
        }
    }

    public void OnClipStart()
    {
        // Store the current color when clip starts
        if (targetRenderer != null && targetRenderer.material != null)
        {
            originalColor = targetRenderer.material.color;
        }
    }

    public void ResetColor()
    {
        if (targetRenderer != null && targetRenderer.material != null)
        {
            targetRenderer.material.color = originalColor;
        }
    }
}


/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════════════
/// COMPLETE SETUP INSTRUCTIONS
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// HOW TO USE THIS IN UNITY:
/// 
/// 1. SETUP:
///    • Create a Timeline (Create > Timeline > Timeline Asset)
///    • Create a GameObject that will own the Timeline (create an empty GameObject)
///    • Add a Playable Director component to that GameObject
///    • Assign your Timeline to the Playable Director
/// 
/// 2. ADD YOUR CUSTOM TRACK:
///    • In the Timeline window, right-click and select "Add Track" 
///    • Choose your "MyCustomTrack" from the list
/// 
/// 3. ADD YOUR CUSTOM CLIP:
///    • Right-click on your MyCustomTrack
///    • Select your "MyCustomClipAsset"
///    • Drag it to create a clip
/// 
/// 4. CONFIGURE THE CLIP:
///    • Select the clip in the Timeline
///    • In the Inspector, you'll see all public fields from MyCustomClipAsset:
///      - Duration: How long the clip plays
///      - Intensity: How strong the effect is
///      - Tint Color: What color to tint with
///      - Ease In Out Curve: Animation easing profile
///      - Target Object Name: What object to animate
///      - Rotation Amount: How much to rotate
///      - Target Game Object: Direct reference to object (alternative to name)
/// 
/// 5. BIND A GAME OBJECT:
///    • In the Playable Director Inspector, find "MyCustomTrack"
///    • Drag any GameObject to bind it
///    • This object will be animated by your clip
/// 
/// 6. PLAY AND TEST:
///    • Press Play in the Editor
///    • Your animation will play according to the parameters!
/// 
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// DATA FLOW DIAGRAM:
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// Timeline UI Inspector
///        ↓ (User sets values)
/// MyCustomClipAsset Fields (public serialized data)
///        ↓ (Called by Timeline)
/// CreatePlayable() Method
///        ↓ (Transfers data)
/// MyCustomPlayableBehavior (receives data in fields)
///        ↓ (Called per frame)
/// ProcessFrame() Method
///        ↓ (Uses progress and easing curve)
/// Visual Result on GameObject
/// 
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// KEY TIMELINE CALLBACKS REFERENCE:
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// 1. OnPlayableCreate()
///    When: Playable created
///    Use: Initialize, cache objects, set initial states
/// 
/// 2. OnBehaviourPlay()
///    When: Starts playing (once per clip)
///    Use: Start effects, store initial values
/// 
/// 3. ProcessFrame()
///    When: Every frame while playing
///    Use: Update animation, modify transforms, update materials
///    Progress: Calculate via GetTime()/GetDuration()
/// 
/// 4. OnBehaviourPause()
///    When: Timeline paused
///    Use: Pause effects, save state
/// 
/// 5. OnPlayableDestroy()
///    When: Playable destroyed (clip ended)
///    Use: Clean up, restore original state
/// 
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// ADVANCED TIPS:
/// ═══════════════════════════════════════════════════════════════════════════════════
/// 
/// • MULTIPLE CLIPS: You can stack multiple instances of your clip on the timeline
///   Each will have independent parameters set in the Inspector
/// 
/// • ANIMATION CURVES: Use AnimationCurve fields to let users define custom easing
///   This is more powerful than simple easings!
/// 
/// • PARAMETER BLENDING: If you have overlapping clips, Timeline automatically
///   blends them based on their track mixer
/// 
/// • BINDING: Use ExposedReference<T> to allow binding in the Inspector
///   The binding is resolved in CreatePlayable() via graph.GetResolver()
/// 
/// • PERFORMANCE: Cache your component lookups in OnPlayableCreate() to avoid
///   repeated GetComponent() calls every frame
/// 
/// • DEBUG: Add Debug.Log() calls to understand the lifecycle and data flow
/// 
/// ═══════════════════════════════════════════════════════════════════════════════════
/// </summary>
public class TimelinePlayableDocumentation { }

