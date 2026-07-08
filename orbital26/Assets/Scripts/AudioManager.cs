using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip catBubbleClickSound;
    [SerializeField] private AudioClip customerBubbleClickSound;
    [SerializeField] private AudioClip uiClickSound;
    [SerializeField] private AudioClip earnMoneySound;
    [SerializeField] private AudioClip spendMoneySound;
    [SerializeField] private AudioClip pettingSound;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Cat bubble
    public void PlayCatBubbleClickSound()
    {
        sfxSource.PlayOneShot(catBubbleClickSound);
    }

    // Customer bubble
    public void PlayCustomerBubbleClickSound()
    {
        sfxSource.PlayOneShot(customerBubbleClickSound);
    }

    // Shop and menu buttons
    public void PlayUIClickSound()
    {
        sfxSource.PlayOneShot(uiClickSound);
    }

     public void PlayEarnMoneySound()
    {
        sfxSource.PlayOneShot(earnMoneySound);
    }

 public void PlaySpendMoneySound()
    {
        sfxSource.PlayOneShot(spendMoneySound);
    }

    public void PlayPettingSound()
    {
        sfxSource.PlayOneShot(pettingSound);
    }
}
