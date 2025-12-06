using UnityEngine;

public class PlayerAnimationRelay : MonoBehaviour
{
    private PlayerMovement parentMovement;

    void Awake()
    {
        // Automatically find the script on the parent object
        parentMovement = GetComponentInParent<PlayerMovement>();
    }

    // Connect this to your Animation Event
    public void ResetBusyState()
    {
        if (parentMovement != null)
        {
            parentMovement.ResetBusyState();
        }
    }

    // Optional: Add other events here if needed (e.g. footstep sounds)
    public void TriggerHit()
    {
        // Example if you need hit logic at a specific frame
    }

    public void TriggerAttack()
    {
        if (parentMovement != null)
        {
            parentMovement.AttackHit();
        }
    }
}