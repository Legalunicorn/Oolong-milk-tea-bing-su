using UnityEngine;
using UnityEngine.UI;
using TMPro;


// UI mangaer for the panel that appear at the END of a level 
// Expected behavioir 
// WIN 
// -> start menu or next level (if last level then no next level)
// LOSE 
// -> start menu or restartlevel

// I intend to put this inside canvas prefab, should work fine:
public class LevelResultUI : MonoBehaviour
{
    // also singleton pattern 
    public static LevelResultUI instance;

    // entire panel 
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button startMenuButton;

    void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }

    public void ShowWin()
    {
        panel.SetActive(true);
        // m
        if (GameManager.instance.IsAtFinalLevel())
        {
            headerText.text = "GAME COMPLETED!";
            nextLevelButton.gameObject.SetActive(false);
        }
        else
        {
            headerText.text = "Level Complete!";
            nextLevelButton.gameObject.SetActive(true);
        }
        restartLevelButton.gameObject.SetActive(false);
        // start menu button should be active by default
       
    }

    // display panel 
    public void ShowLose()
    {
        panel.SetActive(true);
        headerText.text = "Mission Failed : (";
        nextLevelButton.gameObject.SetActive(false);
        restartLevelButton.gameObject.SetActive(true);
        // startmenu default active
    }

    // on button clicks -> trigger follow up events
    public void OnNextLevel()
    {
        panel.SetActive(false);
        GameManager.instance.GoNextLevel();
    }

    public void OnRestart()
    {
        panel.SetActive(false);
        LevelManager.instance.RestartLevel();
    }

    public void OnMainMenu()
    {
        panel.SetActive(false);
        GameManager.instance.GoToStartMenu();
    }
       
}
