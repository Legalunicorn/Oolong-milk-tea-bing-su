using UnityEngine;
using System.Collections.Generic;

public class CustomerController : MonoBehaviour
{

    public enum CustomerState
    {
        WalkingToCounter,
        WaitingAtCounter,
        WalkingToChair,
        Eating,
        RequestCat, 
        WalkingToCat, 
        InteractingWithCat, 
        Exiting
    }

    private CustomerState state;
    private Cat assignedCat;
    // NOTE: these are hardcoded for now 
    // TODO: the customner request from the level manager to get the prefab instead
    [SerializeField] private GameObject catRequestBubblePrefab;
    // [SerializeField] private CatData requestedCatData;

    [Header("Movement Points")]
    public Transform entrance;
    // this is hardcoded, i will switch to finding stop position from level manager 
    public Transform stopPosition;
    private GameObject[] stopPositions;
    public Transform stopPositionLeft;
    public Transform stopPositionRight;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;

    [Header("References")]
    public Animator animator;

    private FoodData requestedFood;
    // NOTE: this prefab has changed to an empty bubbl
    // we have to assign foodData object to it
    public GameObject hungryBubblePrefab;
    public GameObject currentBubble;

    private List<Node> currPath;
    private int pathIdx;

    // when the customer starts being unhappy 
    [SerializeField] private float hungryTime = 0f;
    [SerializeField] private float maxWaitDuration = 20f;
    // howoften to decrease the satisfaction
    [SerializeField] private float complainInterval = 1f; //TODO: remoe
    [SerializeField] private float timeSinceLastComplain = 0f; //TODO: remove
    // timers for waiting for catsd
    [SerializeField] private float catWaitingTime = 0f;
    [SerializeField] private float maxCatWaitTime =  20f;
    [SerializeField] private float catComplainInterval =  1f; //TODO: remove
    [SerializeField] private float timeSinceLastCatComplain =  0f; //TODO: remove

    private Vector3 currDest;
    private ChairSlot chair;

    void Awake()
    {
        entrance = GameObject.FindWithTag("Entrance").transform;
        // un needed already
        stopPosition = GameObject.FindWithTag("Counter").transform;

        stopPositionLeft = GameObject.FindWithTag("stopLeft").transform;
        stopPositionRight = GameObject.FindWithTag("stopRight").transform;

    }

    void Start()
    {
        maxWaitDuration = LevelManager.instance.customerWaitCounterDuration;
        maxCatWaitTime = LevelManager.instance.customerWaitForCatDuration;
        transform.position = entrance.position;
        // actually i dont like this, randomn is not good enough 
        // change to any position between two points
        // find random counter instead
        // Transform randomStop = stopPositions[Random.Range(0, stopPositions.Length)].transform;
        // SetPath(randomStop.position, CustomerState.WalkingToCounter);

        Vector3 randpos = new Vector3(
            Random.Range(stopPositionLeft.position.x, stopPositionRight.position.x),
            Random.Range(stopPositionLeft.position.y, stopPositionRight.position.y),
            stopPositionLeft.position.z // both z same
        );
        SetPath(randpos, CustomerState.WalkingToCounter);
    }

    void Update()
    {
        if (state == CustomerState.WalkingToCounter)
        {
            MoveTowards(currDest);
            if (HasReachedDestination(currDest)) OnReachCounter();
        } else if (state == CustomerState.WaitingAtCounter)
        {
            HandleWaiting();
        } else if (state == CustomerState.WalkingToChair)
        {
            MoveTowards(currDest);
            if (HasReachedDestination(currDest)) OnReachChair();
        } else if (state == CustomerState.Eating)
        {
            // don handle? once chair is reached
            // the animation is cued and finish eating is called
        } else if (state == CustomerState.RequestCat)
        {
            HandleCatWaiting();
        } else if (state == CustomerState.WalkingToCat)
        {
            MoveTowards(currDest);
            if (HasReachedDestination(currDest)) OnReachCat();
        } else if (state == CustomerState.InteractingWithCat)
        {
            // TODO:
            // For now I realised I dont need this state
            // but keep here first incase needed
        } else if  (state == CustomerState.Exiting)
        {
            MoveTowards(currDest);
            if (HasReachedDestination(currDest)) Destroy(gameObject);
        }
    }

    private void HandleCatWaiting()
    {
        catWaitingTime += Time.deltaTime;
        if (catWaitingTime <= maxCatWaitTime) return;
        if (LevelManager.instance != null) LevelManager.instance.DecreaseSatisfaction(10);
        if(FloatingTextManager.instance != null) FloatingTextManager.instance.SpawnHeartText("-10", transform.position+Vector3.up);
        if (currentBubble != null) Destroy(currentBubble);
        StartExiting();
    }

    public void RequestCat()
    {
        if (currentBubble != null) Destroy(currentBubble);
        state = CustomerState.RequestCat;

        // tutotial call back
        TutorialManager.instance?.OnCustomerStartedCatReq();

        // use level manager to request rand cat 
        CatData requestedCatData = LevelManager.instance.GetRandomRequestableCatData();

        // position and spawn bubble 
        currentBubble = Instantiate(
            catRequestBubblePrefab,
            transform.position,
            Quaternion.identity
        );
        currentBubble.transform.SetParent(transform);
        // offset the bubble
        currentBubble.transform.localPosition = new Vector3(-1f,1f,0);
        // get the script
        RequestCatBubble bubble = currentBubble.GetComponent<RequestCatBubble>();
        bubble.Setup(this, requestedCatData);

    }

    public void WalkToCat(Cat cat)
    {
        assignedCat = cat;
        // adjust is so that the customer is on the cat 
        Vector3 offset = new Vector3(0f,0.5f,0f);
        SetPath(cat.transform.position+offset, CustomerState.WalkingToCat);
    }

    public void OnReachCat()
    {
        state = CustomerState.InteractingWithCat;
        StopWalking(); 
        animator.SetBool("isPet", true);
        //TODO: for now hardcode the interaction time 
        // but later make it store in some manager globally 
        // the cat -> que animation for duration, then auto relase
        assignedCat.InteractWithCustomer();
        Invoke(nameof(FinishCatInteraction), 4f);
    }

    private void FinishCatInteraction()
    {
        // cat will reset on its own to dle
        animator.SetBool("isPet", false);
        assignedCat = null;
        StartExiting();
    }

    // Navigation functions
    void SetPath(Vector2 position, CustomerState updatedState)
    {
        currPath = AStar.instance.GeneratePath(transform.position, position);
        pathIdx = 0;
        state = updatedState;
        currDest = position;
    }

    void MoveTowards(Vector2 end)
    {
        if (currPath == null) return;
        Vector2 nxt;
        // move along path given
        if (pathIdx < currPath.Count)
        {
            nxt = currPath[pathIdx].transform.position;
            transform.position = Vector2.MoveTowards(
                transform.position,
                nxt,
                moveSpeed * Time.deltaTime
            );
            if (Vector2.Distance(transform.position,nxt)<=0.01f) pathIdx++;
        }
        else
        {
            nxt = end;
            transform.position = Vector2.MoveTowards(
                transform.position,
                end,
                moveSpeed * Time.deltaTime
            );
        }
        Vector2 dir = ((Vector2)nxt - (Vector2)transform.position).normalized;
        animator.SetBool("isWalking", true);
        if (dir.magnitude > 0.01f)
        {
            animator.SetFloat("InputX", dir.x);
            animator.SetFloat("InputY", dir.y);
        }
        animator.SetFloat("LastInputX", dir.x);
        animator.SetFloat("LastInputY", dir.y);
    }

    bool HasReachedDestination(Vector2 end)
    {
        return currPath != null && pathIdx >= currPath.Count && 
            Vector2.Distance(transform.position, end) < 0.01f;
    }
    
    void StopWalking()
    {
        animator.SetBool("isWalking", false);
        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", 0);
    }

    void OnReachCounter()
    {
        TutorialManager.instance?.OnReachCounter();
        // start to request for food
        state = CustomerState.WaitingAtCounter;
        StopWalking();
        Invoke(nameof(BecomeHungry),1.5f);
    } 

    void OnReachChair()
    {
        state = CustomerState.Eating;
        StopWalking();
        bool isChairRight = chair.isFaceRight;
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (isChairRight) {
            render.flipX = false;
        }
        else {
            render.flipX = true;
        }

        animator.SetBool("isEating", true);
        Invoke(nameof(FinishEating), 3.5f);
    }

     void FinishEating()
    {
        // if we flipped the customer when sitting
        // we need to flip back or else he would walk backwards

        TutorialManager.instance?.OnFinishEating();

        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (render.flipX) render.flipX = false;

        animator.SetBool("isEating", false); // make a transition into Sitting or something
        chair.isOccupied = false;
        chair = null;
        Invoke(nameof(RequestCat), 2f); 
    }

    void StartExiting()
    {
        CancelInvoke(nameof(BecomeHungry));
        CancelInvoke(nameof(RequestCat));
        SetPath(entrance.position, CustomerState.Exiting);
    }

    // INTERACTION FUNCTIONS - HUNGRY etc
    void BecomeHungry()
    {
        if (state != CustomerState.WaitingAtCounter) return;
        if (currentBubble != null) Destroy(currentBubble);

        requestedFood = LevelManager.instance.GetRandomRequestableFoodData();
        hungryTime = 0f;
        currentBubble = Instantiate(hungryBubblePrefab, transform.position, Quaternion.identity);
        currentBubble.transform.SetParent(transform);
        currentBubble.transform.localPosition = new Vector3(-1f,1f,0);
        HungryBubbleClick bubble = currentBubble.GetComponent<HungryBubbleClick>();
        if (bubble != null) bubble.Setup(this, requestedFood);
    }

    void HandleWaiting()
    {
        hungryTime +=Time.deltaTime;
        if (hungryTime < maxWaitDuration) return;
        // instead of constantly compaining the cusomter just leaves immediatel

        // TODO:   
        // Right now im hardcoding the decrease amount
        // Later ti should refererence from the level manager
        // so for now all 3 levels has same peanlioty 
        // i think its fine bc we already increase customer frequency  
        if (LevelManager.instance != null)
        {
            LevelManager.instance.DecreaseSatisfaction(8);
        }
        if (FloatingTextManager.instance != null)
        {
            FloatingTextManager.instance.SpawnHeartText("-8", transform.position + Vector3.up);
        }
        if (currentBubble != null) Destroy(currentBubble);
        StartExiting(); // transition state to exit -> and walk out
    }

    // Serve food to customer
    // triggered when interaction by player is successful
    public void ServeFood(int price)
    {
        // tutorial call back
        TutorialManager.instance?.OnFoodServed();

        if (currentBubble != null) Destroy(currentBubble);
        if (GameManager.instance != null) GameManager.instance.AddCash(price);
        // spawn visual UIk
        if (FloatingTextManager.instance != null)
        {
            string msg = "+" + price;
            FloatingTextManager.instance.SpawnCoinText(msg, transform.position + Vector3.up);
        }
        hungryTime = 0f;
        timeSinceLastComplain = 0f;
        chair = LevelManager.instance.GetAvailableChair();
        if (chair == null)
        {
            // this should not happen because i put alot of chairs 
            // but just incase this happens we make the customer exit 
            // maybe in the future we can do sth else 
            StartExiting();
            return;
        }
        chair.isOccupied = true;
        Vector3 chairOffset = new Vector3(0f, 0.5f,  0f); // mannually adjust
        SetPath(chair.transform.position + chairOffset,  CustomerState.WalkingToChair);

        // Experimental: close the food menu automatically 
        FoodMenuUI.instance.Hide();
    }

    // checker before actually serviing food
    // can combine with function above but its easier to just extend like this
    public bool TryServeFood(FoodData food)
    {
        if (state != CustomerState.WaitingAtCounter) return false;
        if (food != requestedFood) return false;
        ServeFood(food.price);
        return true;
    }

    public void ClearBubble()
    {
        if (currentBubble != null) Destroy(currentBubble);
        currentBubble = null;
    }
}
