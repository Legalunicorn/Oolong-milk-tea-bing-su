using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonClickSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    public string sceneName;

    public void PlaySoundAndLoadScene()
    {
        StartCoroutine(PlaySoundThenLoad());
    }

    IEnumerator PlaySoundThenLoad()
    {
        audioSource.PlayOneShot(clickSound);

        yield return new WaitForSeconds(clickSound.length);

        SceneManager.LoadScene(sceneName);
    }
}