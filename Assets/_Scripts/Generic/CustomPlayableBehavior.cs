using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CustomPlayableBehavior : PlayableBehaviour
{
    public string sampleMessage;

    // Called when the graph starts playing - also when the behaviour was created on the timeline in UnityEditor
    // (edit and play mode)
    public override void OnGraphStart(Playable playable)
    {
        
    }

    // Called when the graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log($"Started playing: {sampleMessage}");
        
        // how to get to the instance of the object that was binding to the track:
        // 1. Extract the PlayableGraph from the instance
        PlayableGraph graph = playable.GetGraph();
        
        // 2. Fetch the resolver and cast it to the PlayableDirector executing the graph
        PlayableDirector director = graph.GetResolver() as PlayableDirector;
        
        if (director != null)
        {
            // 3. Iterate through the graph's high-level outputs to locate the matching track metadata
            int outputCount = graph.GetOutputCount();
            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output = graph.GetOutput(i);
                // GetReferenceObject retrieves the source TrackAsset reference serving as the binding key
                Object sourceAsset = output.GetReferenceObject();

                if (sourceAsset != null)
                {
                    // 4. Query the Director to get the actual bound runtime scene object instance
                    GameObject boundObject = director.GetGenericBinding(sourceAsset) as GameObject;
                
                    if (boundObject != null)
                    {
                        Debug.Log($"Successfully grabbed reference: {boundObject.name}");
                        // Execute initialization logic on the reference here
                        break; 
                    }
                }
            }
        }
    }

    // Called when the owning play-state changes to Pause
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log($"Paused or stopped: {sampleMessage}");
    }

    // Called each frame during the playback process
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Use playerData if a binding is assigned via a PlayableTrack
        GameObject boundObject = playerData as GameObject;
        if (boundObject != null)
        {
            // Execute runtime logic on the instance reference
            boundObject.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
    }
}