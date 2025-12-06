using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    public AudioClip backgroundMusic;

    void Start()
    {
        // This calls the method inside your AudioManager
        AudioManager.Instance.PlayMusic(backgroundMusic);
    }
}