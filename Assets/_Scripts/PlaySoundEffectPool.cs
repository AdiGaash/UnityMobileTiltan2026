using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class PlaySoundEffectPool : MonoBehaviour, IPoolableObject
{
    AudioSource audioSource;
    
    private UnityAction returnAction;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found on PlaySoundEffectPool object.");
        }
    }
    
    public void PlaySound(AudioClip clip, Action onComplete = null)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            
            if (onComplete != null)
            {
                StartCoroutine(MonitorAudioCompletion(onComplete));
            }
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is null. Cannot play sound.");
        }
    }
    
    
    private IEnumerator MonitorAudioCompletion(Action onComplete)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        onComplete?.Invoke();
    }


    private void OnEnable()
    {
        // Reset any state when retrieved from pool
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        
    }

    private void OnDisable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
        returnAction = null;
    }
    

    public void TriggerReturn()
    {
        returnAction?.Invoke();
        Debug.Log("trigger return to pool");
    }

    public void SetReturnAction(UnityAction action)
    {
        returnAction = action;
    }
}
