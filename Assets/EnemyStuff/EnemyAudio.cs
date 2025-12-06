using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public AudioClip attackClip;
    public AudioClip hitClip;
    public AudioClip dieClip;

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        AudioManager.Instance.PlaySFX(clip);
    }

    public void PlayAttack()
    {
        AudioManager.Instance.PlaySFX(attackClip);
    }

    public void PlayHit()
    {
        AudioManager.Instance.PlaySFX(hitClip);
    }

    public void PlayDie()
    {
        AudioManager.Instance.PlaySFX(dieClip);
    }
}
