using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Globally accessible AudioManager
    public static AudioManager instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bubbleClickSound;
    [SerializeField] private AudioClip uiClickSound;

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;

            // Keep AudioManager when changing scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Prevent duplicate AudioManagers
            Destroy(gameObject);
        }
    }

    // For cat and customer bubbles
    public void PlayBubbleClickSound()
    {
        sfxSource.PlayOneShot(bubbleClickSound);
    }

    // For shop and menu buttons
    public void PlayUIClickSound()
    {
        sfxSource.PlayOneShot(uiClickSound);
    }
}
