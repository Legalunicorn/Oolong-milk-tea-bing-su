using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        LevelManager.instance.RestartLevel();
    }

    public void QuitApplication()
    {
        // we will host on web games, so there is no quit 
        /// it will go back to start menu 
        GameManager.instance.GoToStartMenu();

        // Debug.Log("Quit was called");
        // //https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Application.Quit.html
        // Application.Quit();
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
    }
}
