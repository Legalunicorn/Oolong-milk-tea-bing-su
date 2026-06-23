using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Another SINGLETON (like level manager)
public class GameManager : MonoBehaviour
{
    // static instance accessible globbally 
    public static GameManager instance {get; private set;}

    public int cash; 
    [SerializeField] private int startingCash = 50;
    // Hash Map to store Cats 
    // right now its only one cat but this will expand 
    public Dictionary<CatData, int> ownedCats = new Dictionary<CatData,int>();

    public CatData brownCat;
    public int minBrownCat;

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
            // TODO: uncomment when inventory is fully in place
            //InitInventory();
            cash = startingCash;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // TODO: this is never called 
    private void InitInventory()
    {
        // ensure brown cats is enough
        // TODO: in the future: retrieve cat data from file storage
        if (brownCat != null && ownedCats[brownCat] < minBrownCat)
        {
            ownedCats[brownCat] = minBrownCat;
        }
    }



    /**
    * Reset the game from level 1 
    */
    public void StartNewGame()
    {
        currLevel = 1;
        cash = startingCash;
        ownedCats.Clear();
        SceneManager.LoadScene(1);
    }

    public void GoNextLevel()
    {
        currLevel++;
        // if exeed max level (which should not happen)
        // just load the start menu again 
        if (currLevel > totalLevels) SceneManager.LoadScene(0); // 0 -> start menu 
        else SceneManager.LoadScene(currLevel);
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public bool IsAtFinalLevel()
    {
        return currLevel >= totalLevels;
    }
        

    public void AddCash(int amt)
    {
        cash += amt;
    }

    public bool SpendCash(int amt)
    {
        if (cash < amt) return false;
        cash -= amt;
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
        if (Keyboard.current.qKey.wasPressedThisFrame) cash += 15;
        
    }
}
