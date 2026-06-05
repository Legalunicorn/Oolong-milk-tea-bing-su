using UnityEngine;
using System.Collections.Generic;

public class CustomerController : MonoBehaviour
{

    // TODO: refacfactor this code
    // remove all the bools and use an enum to manage the customer 
    
    public enum CustomerState
    {
        WalkingToCounter,
        WaitingAtCounter,
        WalkingToChair,
        Eating,
        RequestCat, // not impl
        WalkingToCat, // not impl
        InteractingWithCat, // not impl
        Exiting
    }

    private CustomerState state;


    [Header("Movement Points")]
    // fakeEntrance -> spawn point 
    public Transform entrance;
    // arbitarary empty game object in cafe marked as the stop position
    public Transform stopPosition;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;

    [Header("References")]
    public Animator animator;
    // BUG: this is directly linked in the inspector, does not worked with spawned prefabs
    // TODO: use this as a Prefab
    public GameObject hungryBubblePrefab;
    public GameObject currentBubble;

    // private bool isMoving = true;
    // private bool hasReachedStopPosition = false;
    // private bool isHungry = false;
    // private bool isExiting = false;
    private List<Node> currPath;
    private int pathIdx;

    // when the customer starts being unhappy 
    [SerializeField] private float hungryTime = 0f;
    [SerializeField] private float maxWaitDuration = 5f;
    // howoften to decrease the satisfaction
    [SerializeField] private float complainInterval = 1f;
    [SerializeField] private float timeSinceLastComplain = 0f;

    private Vector3 currDest;

    // CHAIR 
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
        } else if  (state == CustomerState.Exiting)
        {
            MoveTowards(currDest);
            if (HasReachedDestination(currDest)) Destroy(gameObject);
        }
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
        Vector2 dir = (nxt - (Vector2)transform.position).normalized;
        animator.SetBool("isWalking", true);
        animator.SetFloat("InputX", dir.x);
        animator.SetFloat("InputY", dir.y);
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
        if (isChairRight) render.flipX = false;
        else render.flipX = true;

        animator.SetBool("isEating", true);
        Invoke(nameof(FinishEating), 5f);
    }

     void FinishEating()
    {
        animator.SetBool("isEating", false); // make a transition into Sitting or something
        chair.isOccupied = false;
        chair = null;
        StartExiting();
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
        // TODO: we might need to offset the position to the chair 
        // so the customer aligneds perfectly
        Vector3 chairOffset = new Vector3(0f, 0.5f,  0f); // mannually adjust
        // move slightly higher
        SetPath(chair.transform.position + chairOffset,  CustomerState.WalkingToChair);
    }
}
