// restart level -> call same scene
using UnityEngine.SceneManagement;
using UnityEngine;


// SINGLETON: 
// https://www.youtube.com/watch?v=yhlyoQ2F-NM
public class LevelManager : MonoBehaviour
{

    // Singletone
    public static LevelManager instance {get; private set;}

    // #### LEVEL STATES  ### 
    // temporarily hardcoded for one level
    public int satisfaction = 100;
    public int maxSatisfaction = 100;
    // Might implement in the future, 
    // Satisfaction automatically decrease on harder levels?
    public int satisfactionDecay = 0;

    // NOTE: final cash should persist after level ends, but keep it simple now
    public int cash = 0;
    
    // NOTE: hardcode this for now since one level only 
    // with multiple levels -> read these from file instead
    public float timeLimit = 60f;
    public float timeLeft = 60f;

    public bool isGameOver = false;


    // #### UI #### 
    public SatisfactionBar satisfactionBar;

    // #### Customer Spawn Logic #### 
    // #### plan first because Customer Prefab not completed
    // public GameObject CustomerPrefab
    public Transform spawnPoint;
    public float spawnInterval = 10f;
    public float spawnTimer = 0f;
 


    // Ensure one SINGLETON instance
    // Awake: called immediately when object/script is loaded
    // Start: called before first frame update 
    // ALL Awake() happens before any start
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this; 
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeLeft = timeLimit;
        satisfactionBar.SetMaxSatisfaction(maxSatisfaction);
    }

    // // Update is called once per frame
    void Update()
    {
        // 1. check and update gameoverstatus 
        if (isGameOver) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0;
            TriggerEndGame("YOU WIN!");
            // TODO: trigger game over mechanic
        }
        if (satisfaction <= 0)
        {
            TriggerEndGame("YOU LOSE");
        }

        

        // 2. manage spawn of customers 
        // TODO: make spawn interval vary by randomness
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            // TODO: spawn customer prefab
        }


        // [potential] 3. add decay to satisfactoin
    }



    // ############ PUBLIC Methods ############### 
    // Mainly interaction with satisfaction bar 
    // For the sake of separation of layers, we
    // decrease this method here, as the "middle" man 
    // between our NPC and UI 
    public void DecreaseSatisfaction(int val)
    {
        satisfaction -= val;
        satisfactionBar.DecreaseSatisfaction(val);
    }

    public void IncreaseSatisfaction(int val)
    {
        satisfaction += val;
        satisfactionBar.IncreaseSatisfaction(val);
    }

    // Spend money
    // Return success status
    // val: positive number representing cost
    public bool SpendCash(int val)
    {
        if (cash >= val)
        {
            cash -= val;
            return true;
        }
        return false;
    }

    // val: positive number representing revenue
    public bool AddCash(int val)
    {
        cash += val;
        return true;
    }

    // Restart level 
    // TODO: add button/UI to restart rather that automatically 
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // ############## PRIVATE METHODS ############## 
    void TriggerEndGame(string msg)
    {
        isGameOver = true;
        Debug.Log("Game over: " + msg);
        RestartLevel();
    }
}
