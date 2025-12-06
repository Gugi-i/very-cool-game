using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Footstep Sounds")]
    public AudioClip[] footstepClips;

    [Header("Other SFX")]
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip shootClip;
    public AudioClip hurtClip;
    public AudioClip swingClip;

    // Called from animation event
    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        AudioManager.Instance.PlaySFX(clip);
    }

    public void PlayJump()
    {
        AudioManager.Instance.PlaySFX(jumpClip);
    }

    public void PlayLand()
    {
        AudioManager.Instance.PlaySFX(landClip);
    }

    public void PlayShoot()
    {
        AudioManager.Instance.PlaySFX(shootClip);
    }

    public void PlaySwing()
    {
        AudioManager.Instance.PlaySFX(swingClip);
    }

    public void PlayHurt()
    {
        AudioManager.Instance.PlaySFX(hurtClip);
    }
}
