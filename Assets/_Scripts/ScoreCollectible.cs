using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ScoreCollectible : MonoBehaviour, ICollectible, IPoolableObject
{
    public int scoreValue = 10;
    public AudioClip SFX;
    private UnityAction returnAction;
    

    private void Awake()
    {
        // find target UI place to move to
    }

    private void OnEnable()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
       
    }

    public void OnCollected()
    {
        GetComponent<Collider>().enabled = false; // Disable collider to prevent multiple collections
        // Add score
        GameManager.Instance.AddScore(scoreValue);


        AnimateCollectedCoin();
        // Optional: Play sound
        PlaySound();

    }


    void AnimateCollectedCoin()
    {
        // Implement coin animation logic here
        // get sound clip length
        var length = Mathf.Max(SFX.length,5f);
        Vector3 targetPosition = GameObject.FindGameObjectWithTag("ScoreUI").transform.position;
        transform.DOMove(targetPosition, length).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // Return to pool after animation completes
            TriggerReturn();
        });
        
    }
    void PlaySound()
    {
        if (SFX != null)
        {
            GameObject sFXPlayer = TaggedObjectPooler.Instance.GetPooledObjectWithAutoReturn("SoundFX");
            sFXPlayer.transform.parent = transform; // Parent to the coin so it moves with it
            
            PlaySoundEffectPool sfxPool = sFXPlayer.GetComponent<PlaySoundEffectPool>();
            if (sfxPool != null)
            {
                sfxPool.PlaySound(SFX, () =>
                {
                    // This callback will be called when the sound finishes playing
                    sfxPool.TriggerReturn();
                });

            }
        }
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
