using UnityEngine;
using System.Collections;

public class StartLevelCanvas : MonoBehaviour
{
    [SerializeField] private GameObject group;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // show the items
        // this is not working, likely because level manager force calls resume when the level starts
        // we could reorder the start sequence but can also just be lazy and pause the next frame
        // LevelManager.instance.PauseLevel();


        StartCoroutine(PauseNextFrame());
    }

    IEnumerator PauseNextFrame(){
        yield return null;
        LevelManager.instance.PauseLevel();
    }


    public void ClosePrompt(){
        group.SetActive(false);
        LevelManager.instance.ResumeLevel();
    }

}
