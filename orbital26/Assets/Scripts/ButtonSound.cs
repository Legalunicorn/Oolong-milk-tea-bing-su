using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    // I realised an earlier implementation made was to load scene by name 
    // TODO: refactor game manager to match this 
    public string sceneName;

    public void PlaySoundANdLoadTutorial()
    {
        StartCoroutine(LoadTutorial());
    } 

    public void PlaySoundAndLoadScene()
    {
        // StartCoroutine(PlaySoundThenLoad());

        // this button is tied to "Start new game button" 
        // for now ill make this laod a new game only 
        // but in the future we can create alternative methods to just load levels 
        StartCoroutine(NewGameThenLoad());
    }

    IEnumerator NewGameThenLoad()
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSeconds(clickSound.length/2);
        GameManager.instance.StartNewGame();
    }

    IEnumerator LoadTutorial()
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSeconds(clickSound.length/2);
        GameManager.instance.StartTutorial();
    }

    // IEnumerator PlaySoundThenLoad()
    // {
    //     audioSource.PlayOneShot(clickSound);
    //     yield return new WaitForSeconds(clickSound.length);
    //     SceneManager.LoadScene(sceneName);
    // }
}
