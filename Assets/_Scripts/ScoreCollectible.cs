using UnityEngine;

public class ScoreCollectible : MonoBehaviour, ICollectible
{
    public int scoreValue = 10;
    public AudioClip SFX;

    public void OnCollected()
    {
        // Add score
        
        
        // Optional: Spawn effect
        
        // Optional: Play sound
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
