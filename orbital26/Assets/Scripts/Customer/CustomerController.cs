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
    [SerializeField] private GameObject catRequestBubblePrefab;
    [SerializeField] private CatData requestedCatData;

    [Header("Movement Points")]
    public Transform entrance;
    public Transform stopPosition;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;

    [Header("References")]
    public Animator animator;

    public GameObject hungryBubblePrefab;
    public GameObject currentBubble;

    private List<Node> currPath;
    private int pathIdx;

    // when the customer starts being unhappy 
    [SerializeField] private float hungryTime = 0f;
    [SerializeField] private float maxWaitDuration = 5f;
    // howoften to decrease the satisfaction
    [SerializeField] private float complainInterval = 1f;
    [SerializeField] private float timeSinceLastComplain = 0f;
    // timers for waiting for catsd
    [SerializeField] private float catWaitingTIme = 0f;
    [SerializeField] private float maxCatWaitTime =  9f;
    [SerializeField] private float catComplainInterval =  1f;
    [SerializeField] private float timeSinceLastCatComplain =  0f;

    private Vector3 currDest;
    private ChairSlot chair;

    void Awake()
    {
        entrance = GameObject.FindWithTag("Entrance").transform;
        stopPosition = GameObject.FindWithTag("Counter").transform;
    }

    void Start()
    {
        transform.position = entrance.position;
        SetPath(stopPosition.position, CustomerState.WalkingToCounter);
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
            // I USE THIS LOGIC ALOT, maybe abstrqact it as well 
            // catWaitingTIme += Time.deltaTime;
            // if (catWaitingTIme >= maxCatWaitTime)
            // {
            //     timeSinceLastCatComplain += Time.deltaTime;
            //     if (timeSinceLastCatComplain >= catComplainInterval)
            //     {
            //         LevelManager.instance.DecreaseSatisfaction(1);
            //         timeSinceLastCatComplain = 0f;
            //     }
            // }
            // TODO : 
        } else if (state == CustomerState.WalkingToCat)
        {
            // TODO: 
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

    public void RequestCat()
    {
        if (currentBubble != null) Destroy(currentBubble);
        state = CustomerState.RequestCat;
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
        Debug.Log(dir);
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
        Invoke(nameof(FinishEating), 5f);
    }

    // TODO: probalby bug wioth how the animation shifts 
    // will figure that out later
     void FinishEating()
    {
        // if we flipped the customer when sitting
        // we need to flip back or else he would walk backwards
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (render.flipX) render.flipX = false;

        animator.SetBool("isEating", false); // make a transition into Sitting or something
        chair.isOccupied = false;
        chair = null;
        Invoke(nameof(RequestCat), 2f); 
    }

    void StartExiting()
    {
        SetPath(entrance.position, CustomerState.Exiting);
    }

    // INTERACTION FUNCTIONS - HUNGRY etc
    void BecomeHungry()
    {
        if (state != CustomerState.WaitingAtCounter) return;
        if (currentBubble != null) Destroy(currentBubble);

        // right now we only have one food, we extend in the future 
        // to have more food 
        currentBubble = Instantiate(hungryBubblePrefab, transform.position, Quaternion.identity);
        currentBubble.transform.SetParent(transform);
        currentBubble.transform.localPosition = new Vector3(-1f,1f,0);
        HungryBubbleClick bubble = currentBubble.GetComponent<HungryBubbleClick>();
        if (bubble != null) bubble.Setup(this);
    }

    void HandleWaiting()
    {
        hungryTime +=Time.deltaTime;
        if (hungryTime < maxWaitDuration) return;
        timeSinceLastComplain += Time.deltaTime;
        if (timeSinceLastComplain >= complainInterval && LevelManager.instance != null)
        {
            LevelManager.instance.DecreaseSatisfaction(1);
            timeSinceLastComplain = 0f; // reset complain timer
        }
    }

    // public method called by bubble script 
    public void ServeFood()
    {
        if (currentBubble != null) Destroy(currentBubble);
        if (GameManager.instance != null) GameManager.instance.AddCash(10);
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
        // move slightly higher
        Vector3 chairOffset = new Vector3(0f, 0.5f,  0f); // mannually adjust
        SetPath(chair.transform.position + chairOffset,  CustomerState.WalkingToChair);
    }

    public void ClearBubble()
    {
        if (currentBubble != null) Destroy(currentBubble);
        currentBubble = null;
    }
}
