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
    [SerializeField] private AudioClip doorBellSound; // when customer enters
    [SerializeField] private AudioClip winLevelSound;
    [SerializeField] private AudioClip loseLevelSound; 
    [SerializeField] private AudioClip gibberishSound; // when customer order haha sounds goofy
    [SerializeField] private AudioClip meowOnce; 
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
        sfxSource.PlayOneShot(pettingSound, 0.6f); // softer, its a bit startling
    }

    public void PlayDoorBellSound()
    {
        sfxSource.PlayOneShot(doorBellSound);
    }

    public void PlayWinLevelSound()
    {
        sfxSource.PlayOneShot(winLevelSound);
    }
    public void PlayLoseLevelSound()
    {
        sfxSource.PlayOneShot(loseLevelSound);
    }
    public void PlayGibberishSound()
    {
        sfxSource.PlayOneShot(gibberishSound, 1.5f); // 150% volume
    }
    public void PlayMeowOnceSound()
    {
        sfxSource.PlayOneShot(meowOnce);
    }
    
}
