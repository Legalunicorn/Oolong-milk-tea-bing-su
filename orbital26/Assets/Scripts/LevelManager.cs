// restart level -> call same scene
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

// SINGLETON: 
// https://www.youtube.com/watch?v=yhlyoQ2F-NM
public class LevelManager : MonoBehaviour
{
    // CAT SELECTION INTERACTION
    // we need new variavles to handle from the level
    public enum LevelState { Normal, CatSelection}
    private LevelState state;
    private CatData requestedCatData;
    private CustomerController pendingCustomer;
    // when go into selection, hide these UI buttons
    [SerializeField] private List<GameObject> hideOnCatSelection;
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
    // TODO: this might be redudant now, remove if not used
    public int cash = 0;
    
    // NOTE: hardcode this for now since one level only 
    // with multiple levels -> read these from file instead
    public float timeLimit = 30f;
    public float timeLeft;

    public bool isGameOver = false;

    // Singlular spawn point in a level for now
    public Transform catSpawnPoint;
    // TODO: create MULTIPLE cat spawn points
    [SerializeField] private List<Transform> catSpawnPoints;



    // #### UI #### 
    public SatisfactionBar satisfactionBar;

    // #### Customer Spawn Logic #### 
    public GameObject[] CustomerPrefabs;
    public Transform CustomerspawnPoint;
    [SerializeField] private float CustomerspawnInterval = 6f;
    public float CustomerspawnTimer = 0f;
 
    // Manage chairs 
    private ChairSlot[] chairSlots;

    // Cats customer can request in this level
    [SerializeField] private List<CatData> requestableCats;
    [SerializeField] private int allowedCatCount = 1; // higher level = more cat variants 

    // Food customers can request in this level
    // [Cake, Matcha, Main] 
    [SerializeField] private List<FoodData> requestableFoods;
    [SerializeField] private int allowedFoodCount = 1;


    // FEATURES FOR ROOM 2 
    // folder with all the waypoints, chairs, nodes etc to disable for room 2
    [SerializeField] private GameObject room2Objects;
    // NOTE: this HAS to be manually wired from the canvas prefab
    // we only have to do it once for each level which is okay for now 
    // i think its unnecessary complicated to spam tags for now
    [SerializeField] private GameObject switchRoomUI;
    [SerializeField] private bool hasSecondRoom = false;


    [SerializeField] private GameObject catSelectionUI;

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
        isGameOver = false;
        cash = GameManager.instance.cash;
        timeLeft = timeLimit;
        satisfactionBar.SetMaxSatisfaction(maxSatisfaction);

        if (room2Objects != null) room2Objects.SetActive(hasSecondRoom);
        //https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Object.FindObjectsByType.html
        chairSlots = FindObjectsByType<ChairSlot>(FindObjectsSortMode.None);

        // hide room2 ELEMENTS if needed
        if (switchRoomUI != null) switchRoomUI.SetActive(hasSecondRoom);
        if (catSelectionUI != null) catSelectionUI.SetActive(false);
        // find all nodes again, and ignore room2 nodes
        // there was an issue where it found the nodes before level manager disabled them
        AStar.instance.RefreshNodes();
        SpawnCatsFromInventory();
    }

    void SpawnCatsFromInventory()
    {
        Debug.Log("spawning cats");
        // iterate map of cat counts
        foreach(var entry in GameManager.instance.ownedCats)
        {
            CatData catData = entry.Key;
            for (int i = 0; i < entry.Value; i++)
            {
                Transform point = GetRandomCatSpawnPoint();
                // fall back to old verion -> single spawn points
                if (point == null)
                {
                    Instantiate(
                        catData.prefab,
                        catSpawnPoint.position,
                        Quaternion.identity
                    );
                }
                else
                {
                    Instantiate(
                        catData.prefab,
                        point.position,
                        Quaternion.identity
                    );
                }
            }
        }
    }

    // // Update is called once per frame
    void Update()
    {

        // 1. check and update gameoverstatus 
        if (isGameOver)
        {
            Debug.Log("GAME IS OVER");
            return;
        }
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0;
            // TriggerEndGame("YOU WIN!");
            TriggerEndGame(true);
            return;
        }
        if (satisfaction <= 0)
        {
            TriggerEndGame(false);
            return;
            // TriggerEndGame("YOU LOSE");
        }

        // 2. manage spawn of customers 
        // TODO: make spawn interval vary by randomness
        CustomerspawnTimer += Time.deltaTime;
        if (CustomerspawnTimer >= CustomerspawnInterval)
        {
            CustomerspawnTimer = 0f;
            SpawnCustomer();
        }
        
        // escape out of selection mode 
        if (Input.GetKeyDown(KeyCode.Escape) && state == LevelState.CatSelection)
        {
            CancelCatSelection();
        }
    }

   void SpawnCustomer()
    {
        if (CustomerPrefabs == null || CustomerPrefabs.Length == 0 || CustomerspawnPoint == null)
        {
            Debug.Log(" no customer spawn point, or prefab is NULL!");
            return;
        }

        int randomIndex = Random.Range(0, CustomerPrefabs.Length);
        GameObject selectedCustomerPrefab = CustomerPrefabs[randomIndex];

        GameObject customer = Instantiate(
            selectedCustomerPrefab,
            CustomerspawnPoint.position,
            Quaternion.identity
        );
    }


    private Transform GetRandomCatSpawnPoint()
    {
        if (catSpawnPoints == null || catSpawnPoints.Count == 0) return null;
        // choose a random one       
        // TODO: for customer counter wait points as well
        return catSpawnPoints[Random.Range(0, catSpawnPoints.Count)];
    }

    // ############ PUBLIC Methods ############### 

    public bool HasSecondRoom()
    {
        return hasSecondRoom;
    }

    // return random food data that a customer can requests
    // for this given level
    // TODO: run unit tests
    public FoodData GetRandomRequestableFoodData()
    {
        int idx = Random.Range(0, allowedFoodCount);
        idx = Mathf.Min(idx, requestableFoods.Count - 1); // shouldnt happen but just in case 
        return requestableFoods[idx];
    }

    // TODO: add unit tests
    public CatData GetRandomRequestableCatData()
    {
        // c# random is exclusive on upper bound
        int idx = Random.Range(0, allowedCatCount);
        idx = Mathf.Min(idx, requestableCats.Count - 1); // just preventive measure to avoid RTE 
        return requestableCats[idx];
    }


    public ChairSlot GetAvailableChair()
    {
        foreach(ChairSlot chair in chairSlots)
        {
            if (!chair.isOccupied) return chair;
        }
        Debug.Log("Chair not found.");
        return null;
    }

    // Mainly interaction with satisfaction bar 
    // For the sake of separation of layers, we
    // decrease this method here, as the "middle" man 
    // between our NPC and UI 
    public void DecreaseSatisfaction(int val)
    {
        // NOTE: dont decrease is game over 
        // otherwise, the player can still "lose" after winning 
        if (isGameOver) return;

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
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // ############## PRIVATE METHODS ############## 
    void TriggerEndGame(bool res)
    {
        isGameOver = true;
        if (LevelResultUI.instance == null)
        {
            Debug.Log("LevelUI NULL");
        }
        if (res) LevelResultUI.instance.ShowWin();
        else LevelResultUI.instance.ShowLose();
        // Debug.Log("Game over: " + msg);
        // RestartLevel();
    }

    public void SpawnCat(GameObject catPrefab)
    {
        Transform randPoint = GetRandomCatSpawnPoint();
        if (randPoint == null)
        {
            // fall back to single spawn point 
            Instantiate(catPrefab, catSpawnPoint.position, Quaternion.identity);
        }
        else
        {

            Instantiate(catPrefab, randPoint.position, Quaternion.identity);
        }
    }

    // # CAT SELECTION FUNCTIONS, STATE MANAGEMENT
    public void EnterCatSelection(CatData catData, CustomerController customer)
    {
        state = LevelState.CatSelection;
        requestedCatData = catData;
        pendingCustomer = customer;

        // TODO: wired manually, consider switching to TAGS
        foreach(GameObject ui in hideOnCatSelection) ui.SetActive(false);

        // enable UI during select 
        if (catSelectionUI != null) catSelectionUI.SetActive(true);
    }

    // Match the cat to the customer = )
    // then cancel out of selection mode
    // method only called when a valid cat was selected
    public void ConfirmCatSelection(Cat cat)
    {
        if (state!=LevelState.CatSelection) return;
        cat.LockForCustomer();
        pendingCustomer.WalkToCat(cat);
        // remove the bubble as well 
        pendingCustomer.ClearBubble();
        CancelCatSelection();
    }

    // Exit out of selection mode 
    public void CancelCatSelection()
    {
        state = LevelState.Normal;
        requestedCatData = null;
        pendingCustomer = null;  
        
        //TODO: wired manually consider switched tags
        foreach(GameObject ui in hideOnCatSelection) ui.SetActive(true);

        if (catSelectionUI != null) catSelectionUI.SetActive(false);
    }

    public bool IsInCatSelection()
    {
        return state == LevelState.CatSelection;
    }

    public CatData GetRequestedCatData()
    {
        return requestedCatData;
    }
}
