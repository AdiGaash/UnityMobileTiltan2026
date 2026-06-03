using UnityEngine;

public class ScoreCollectible : MonoBehaviour, ICollectible
{
    public int scoreValue = 10;
    public AudioClip SFX;

    public void OnCollected()
    {
        // Add score
        GameManager.Instance.AddScore(scoreValue);


        AnimateCollectedCoin();
        // Optional: Play sound
        PlaySound();

    }


    void AnimateCollectedCoin()
    {
        // Implement coin animation logic here
    }
    void PlaySound()
    {
        if (SFX != null)
        {
            GameObject sFXPlayer = TaggedObjectPooler.Instance.GetPooledObjectWithAutoReturn("SoundFX");
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
  
}
