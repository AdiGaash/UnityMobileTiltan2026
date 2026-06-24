using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class SimplePlayableAsset : PlayableAsset
{
    public string clipMessage = "No track binding required";

    // Factory method to construct the runtime playable node
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SimplePlayableBehavior>.Create(graph);
        
        SimplePlayableBehavior behavior = playable.GetBehaviour();
        behavior.message = clipMessage;

        return playable;
    }
}



public class SimplePlayableBehavior : PlayableBehaviour
{
    public string message;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log($"Clip started: {message}");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log($"Clip paused/stopped: {message}");
    }
}