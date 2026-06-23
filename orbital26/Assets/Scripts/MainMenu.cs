using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // SceneManager.LoadSceneAsync(1);
        // TODO: check the differ between async version 
        // if (GameManager.instance.currlevel == 4) SceneManager.LoadScene()
        // SceneManager.LoadScene(GameManager.instance.currLevel);
        int levelLoad = GameManager.instance.currLevel;
        if (GameManager.instance.currLevel > GameManager.totalLevels) levelLoad = 1; // load back first level!
        SceneManager.LoadScene("Level" + levelLoad);
    }

    public void StartNewGame()
    {
        //
        GameManager.instance.StartNewGame();
    }
}
