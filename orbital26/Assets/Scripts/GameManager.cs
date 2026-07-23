using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Another SINGLETON (like level manager)
public class GameManager : MonoBehaviour
{

    private static readonly string[] levelScenes = {"Level1", "Level2","Level3"};


    // static instance accessible globbally 
    public static GameManager instance {get; private set;}

    public int cash = 65;
    public int tempCash  ; // store here during tutorial
    public int tutorialCash;
    [SerializeField] private int startingCash = 65;
    // Hash Map to store Cats 
    // right now its only one cat but this will expand 
    public Dictionary<CatData, int> ownedCats = new Dictionary<CatData,int>();
    public List<int> ownedCounterIds = new List<int>{0};
    public List<int> ownedGroundIds = new List<int>{0};
    public int currCounterId = 0;
    public int currGroundId = 0;
    private bool finishAllLevels = false;

    public CatData brownCat;
    public CatData cookieCat;
    // public int minBrownCat;

    //TODO: Test level managers 
    public int currLevel = 1;
    public const int totalLevels = 3;

    // SINGLETON logic handled in awake
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // dont destroy, must persist betwene levels
            DontDestroyOnLoad(gameObject);
            InitInventory();
            // TODO: uncomment when inventory is fully in place
            //InitInventory();
            Debug.Log("Game manager restarted new");
            cash = startingCash;
            tempCash = startingCash;
            
        }
        else
        {
            Debug.Log("Game manager dup destroyed, cash is: " + instance.cash);
            Destroy(gameObject);
        }
    }

    // TODO: this is never called 
    private void InitInventory()
    {
        // For now, lets default 2 brown cat and 1 cookie cat 
        if (brownCat != null) ownedCats[brownCat] = 2;
        if (cookieCat != null) ownedCats[cookieCat] = 1;

        // // ensure brown cats is enough
        // // TODO: in the future: retrieve cat data from file storage
        // if (brownCat != null && ownedCats[brownCat] < minBrownCat)
        // {
        //     ownedCats[brownCat] = minBrownCat;
        // }
    }


    public void CompleteLevel(){
        if (currLevel <  totalLevels) currLevel++;
        else finishAllLevels = true;
    }


    /**
    * Reset the game from level 1 
    */
    public void StartNewGame()
    {
        currLevel = 1;
        cash = startingCash;
        ownedCats.Clear();
        ownedCounterIds = new List<int>{0};
        ownedGroundIds = new List<int>{0};
        InitInventory();
        SceneManager.LoadScene("Level1");
        // Reset wallpaper and purchased wallpaper ownership.
        CafeCustomisationManager.ResetCustomisationData();
    }

    public void StartTutorial()
    {
        // cache the actual cash for now 
        // later reset back when tutorial ends
        tempCash = cash;
        Debug.Log("temp cash: "+ tempCash);
        SceneManager.LoadScene("tutorial");
    }

    public void GoNextLevel()
    {
        // should not be possible to be called but just in case
        if (finishAllLevels){
            SceneManager.LoadScene("StartMenu");
            return;
        }
        // CompleteLevel();
        SceneManager.LoadScene(levelScenes[currLevel-1]);
        
        // currLevel++;
        // // if exeed max level (which should not happen)
        // // just load the start menu again 
        // if (currLevel > totalLevels) {
        //     currLevel = totalLevels;
        //     SceneManager.LoadScene("StartMenu"); // 0 -> start menu 
        // }
        // else SceneManager.LoadScene(levelScenes[currLevel - 1]);
    }

    public void GoToStartMenu()
    {
        if (LevelManager.instance.IsTutorialLevel())
        {
            // reset the cash back to the proper level
            cash = tempCash;
        }
        SceneManager.LoadScene(0);
    }
    
    public bool IsAtFinalLevel()
    {
        // return currLevel >= totalLevels;
        return finishAllLevels;
    }
        

    public void AddCash(int amt)
    {
        LevelManager.instance?.AddEarned(amt);
        cash += amt;
    }

    public bool SpendCash(int amt)
    {
        if (cash < amt) return false;
        cash -= amt;
        LevelManager.instance?.AddSpent(amt);
        return true;
    }

    public void AddCat(CatData dat)
    {
        if (ownedCats.ContainsKey(dat)) ownedCats[dat]++;
        else ownedCats[dat] = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: temporary measure to test 
        // if adding cash works, since customer prefab not read 
        // if (Keyboard.current.qKey.wasPressedThisFrame) cash += 15;
        
    }
}
