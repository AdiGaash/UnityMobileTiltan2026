using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/*
 * ═══════════════════════════════════════════════════════════════════════════════════
 * PRACTICAL EXAMPLE: Simple Fade Animation Timeline Playable
 * ═══════════════════════════════════════════════════════════════════════════════════
 * 
 * This is a real-world example: A fade in/out animation for any UI element or object
 * Easy to understand and modify for your own needs
 */

// ═══════════════════════════════════════════════════════════════════════════════════
// STEP 1: The Clip Asset - Configuration/Data Storage
// ═══════════════════════════════════════════════════════════════════════════════════
[System.Serializable]
public class FadeClipAsset : PlayableAsset
{
    // ──────────────────────────────────────────────────────────────────────────
    // PARAMETERS - These appear in the Inspector
    // ──────────────────────────────────────────────────────────────────────────
    
    [SerializeField]
    [Tooltip("Duration of the fade animation in seconds")]
    public float fadeDuration = 1f;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Start alpha value (0 = transparent, 1 = opaque)")]
    public float startAlpha = 0f;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("End alpha value (0 = transparent, 1 = opaque)")]
    public float endAlpha = 1f;

    [SerializeField]
    [Tooltip("Easing curve for smooth fade")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    [Tooltip("Should this fade fade in or out?")]
    public bool isFadeIn = true;

    // ──────────────────────────────────────────────────────────────────────────
    // REQUIRED: Create Playable
    // ──────────────────────────────────────────────────────────────────────────
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Create the behavior wrapper
        var playable = ScriptPlayable<FadePlayableBehavior>.Create(graph);

        // Get the behavior
        var behavior = playable.GetBehaviour();

        // Transfer parameters from this asset to the behavior
        behavior.startAlpha = startAlpha;
        behavior.endAlpha = endAlpha;
        behavior.fadeCurve = fadeCurve;
        behavior.isFadeIn = isFadeIn;

        // Set how long this playable plays
        playable.SetDuration(fadeDuration);

        return playable;
    }

    
}


// ═══════════════════════════════════════════════════════════════════════════════════
// STEP 2: The Playable Behavior - Animation Logic
// ═══════════════════════════════════════════════════════════════════════════════════
public class FadePlayableBehavior : PlayableBehaviour
{
    // ──────────────────────────────────────────────────────────────────────────
    // Data from the Clip Asset
    // ──────────────────────────────────────────────────────────────────────────
    public float startAlpha = 0f;
    public float endAlpha = 1f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool isFadeIn = true;

    // ──────────────────────────────────────────────────────────────────────────
    // Internal References
    // ──────────────────────────────────────────────────────────────────────────
    private CanvasGroup canvasGroup;
    private GameObject bindingTarget;
    private float originalAlpha;

    // ──────────────────────────────────────────────────────────────────────────
    // Lifecycle: Initialization
    // ──────────────────────────────────────────────────────────────────────────
    public override void OnPlayableCreate(Playable playable)
    {
        Debug.Log("[FadePlayable] Created");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Lifecycle: When clip starts
    // ──────────────────────────────────────────────────────────────────────────
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("[FadePlayable] Play started");
        
        // Store original alpha for restoration
        if (canvasGroup != null)
        {
            originalAlpha = canvasGroup.alpha;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Lifecycle: Process frame (called every frame while playing)
    // ──────────────────────────────────────────────────────────────────────────
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Get the bound GameObject from Timeline
        bindingTarget = playerData as GameObject;

        if (bindingTarget == null)
        {
            Debug.LogWarning("[FadePlayable] No target GameObject bound to this track!");
            return;
        }

        // Find or create CanvasGroup (needed for alpha control)
        if (canvasGroup == null)
        {
            canvasGroup = bindingTarget.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = bindingTarget.AddComponent<CanvasGroup>();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // CALCULATE ANIMATION PROGRESS
        // ──────────────────────────────────────────────────────────────────────
        double currentTime = playable.GetTime();
        double duration = playable.GetDuration();

        // Progress from 0 to 1
        float progress = Mathf.Clamp01((float)(currentTime / duration));

        // ──────────────────────────────────────────────────────────────────────
        // APPLY EASING CURVE
        // ──────────────────────────────────────────────────────────────────────
        float easedProgress = fadeCurve.Evaluate(progress);

        // ──────────────────────────────────────────────────────────────────────
        // CALCULATE AND APPLY ALPHA
        // ──────────────────────────────────────────────────────────────────────
        if (isFadeIn)
        {
            // Fade in: interpolate from startAlpha to endAlpha
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, easedProgress);
        }
        else
        {
            // Fade out: interpolate in reverse
            canvasGroup.alpha = Mathf.Lerp(endAlpha, startAlpha, easedProgress);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Lifecycle: When clip ends or timeline stops
    // ──────────────────────────────────────────────────────────────────────────
    public override void OnPlayableDestroy(Playable playable)
    {
        Debug.Log("[FadePlayable] Destroyed");
        
        // Restore original alpha
        if (canvasGroup != null)
        {
            canvasGroup.alpha = originalAlpha;
        }
    }
}


// ═══════════════════════════════════════════════════════════════════════════════════
// STEP 3: The Track Asset - Allows adding the clip to Timeline
// ═══════════════════════════════════════════════════════════════════════════════════
[TrackColor(0.2f, 0.8f, 0.2f)]  // Green track color
[TrackClipType(typeof(FadeClipAsset))]  // This track uses FadeClipAsset
[TrackBindingType(typeof(GameObject))]  // Binds to GameObjects
public class FadeTrack : TrackAsset
{
    // Timeline will automatically handle clip management
}


/*
 * ═══════════════════════════════════════════════════════════════════════════════════
 * HOW TO USE THIS PRACTICAL EXAMPLE:
 * ═══════════════════════════════════════════════════════════════════════════════════
 * 
 * 1. CREATE A TIMELINE SETUP:
 *    • Create a Timeline asset
 *    • Add it to a GameObject with a Playable Director
 * 
 * 2. ADD THE FADE TRACK:
 *    • In Timeline window, right-click → "Add Track"
 *    • Find and select "Fade Track"
 * 
 * 3. ADD A FADE CLIP:
 *    • Right-click on the Fade Track
 *    • Select "New > Fade Clip Asset"
 *    • Drag to create a clip at desired time
 * 
 * 4. CONFIGURE THE CLIP:
 *    • Select the clip in Timeline
 *    • In Inspector, adjust:
 *      - Fade Duration: How long fade takes
 *      - Start Alpha: Initial transparency (0-1)
 *      - End Alpha: Final transparency (0-1)
 *      - Fade Curve: Easing profile
 *      - Is Fade In: Fade in or out?
 * 
 * 5. BIND TARGET OBJECT:
 *    • In Playable Director, find "Fade Track"
 *    • Drag the UI element or GameObject to fade
 * 
 * 6. PLAY AND TEST!
 * 
 * ═══════════════════════════════════════════════════════════════════════════════════
 * 
 * EXAMPLES OF PARAMETER COMBINATIONS:
 * ═══════════════════════════════════════════════════════════════════════════════════
 * 
 * FADE IN:
 *   - Start Alpha: 0
 *   - End Alpha: 1
 *   - Is Fade In: true
 * 
 * FADE OUT:
 *   - Start Alpha: 1
 *   - End Alpha: 0
 *   - Is Fade In: false
 * 
 * QUICK FADE IN (0.5 seconds):
 *   - Fade Duration: 0.5
 *   - Start Alpha: 0
 *   - End Alpha: 1
 * 
 * SLOW FADE WITH EASE:
 *   - Fade Duration: 3
 *   - Fade Curve: EaseInOut
 *   - Custom curve: Create smooth S-curve
 * 
 * ═══════════════════════════════════════════════════════════════════════════════════
 */

