using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CustomPlayableAsset : PlayableAsset
{
    public string sampleMessage = "Hello World";

    // Factory method to instantiate the behavior piece of the playable tree
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Create a script playable with the behavior logic
        var playable = ScriptPlayable<CustomPlayableBehavior>.Create(graph);
        
        // Access the behavior instance to inject the data fields
        CustomPlayableBehavior behavior = playable.GetBehaviour();
        behavior.sampleMessage = sampleMessage;

        return playable;
    }
}
