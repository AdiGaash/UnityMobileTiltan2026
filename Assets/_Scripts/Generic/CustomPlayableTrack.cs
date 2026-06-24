using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

// Defines the track name in the Timeline UI, the bound object type, and the asset type it accepts
[TrackColor(0.8f, 0.2f, 0.2f)]
[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(CustomPlayableAsset))]
 
public class CustomPlayableTrack : TrackAsset
{
    
   
    // Evaluates the track mixer to handle blending between overlapping clips
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        
        // Creates a ScriptPlayable using the same custom behavior to act as the mixer logic
        var playable = ScriptPlayable<CustomPlayableBehavior>.Create(graph, inputCount);
        return playable;
    }
    
   
  
}


