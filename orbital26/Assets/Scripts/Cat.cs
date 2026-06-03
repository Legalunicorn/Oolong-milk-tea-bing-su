using UnityEngine;
using System.Collections.Generic;

/**
 * Script controls CAT behavior in the game 
 * - movement, animation, walking mechanics, hunger  (for now)
 * - might split to further scripts later if neede
 */ 
public class Cat : MonoBehaviour
{

    // Refactorize cat states to ENUM rather than bunch of booleans
    public enum CatState
    {
        Idle,
        Walking,
        Hungry,
        Eating,
        Tired,
        Sleeping
    }
    [SerializeField] private CatState state;
    
    // [ANIMATION] component (handle x-direction, and current state)
    Animator animator;
    SpriteRenderer spriteRenderer;
    private Vector2 lastPosition; // use last pos to face sprite in the right direciton: left or right
    private float flipSpriteCoolDown = 0f;
    private float minFlipSpriteInterval = 0.5f; // limit the number of flips

    //  [MOVEMENT]  Related Mechanics 
    [SerializeField] private float speed = 2f;
    // [SerializeField] private BoxCollider2D area; // boundary object
    [SerializeField] private Vector2 targetPos; // position to move towards
    private const float fixedIdleDuration = 1f; // how long to idle between each walk
    private float idleTimeLeft = 0f; // reset to 1f
    // NOTE: ##### NEW PATH VARIABLES FOR PATH FINDING ##########
    [SerializeField] private List<Node> currPath;
    private int pathIdx;

    // [HUNGER] variables
    [SerializeField] private float timeBetweenMeal = 10f; 
    private float timeLeftToHungry;
    [SerializeField] private GameObject foodBubblePrefab;
    [SerializeField] private float eatDuration = 4f;
    // SerializeField -> draw from cat prefab type
    // Default to 2 here
    [SerializeField] private int feedCost = 1;
    
    // [SLEEP] variables
    [SerializeField] private float timeBetweenSleep = 10f;
    private float timeLeftToSleep;
    [SerializeField] private GameObject sleepBubblePrefab;
    [SerializeField] private float sleepDuration = 4f;

    // [Sleep/Hunger] shared bubble
    private GameObject currentBubble;

    // [Unsatisfied Cat tracking] 
    [SerializeField] private float maxDurationWainting = 10f; // max duration to stay hungry/tired
    private float currentWaitDuration;


    void Start()
    {
        // Initialize variables 
        // find animator component to edit params
        animator = GetComponent<Animator>();
        // find the sprite renderer to flip it 
        spriteRenderer = GetComponent<SpriteRenderer>();
        // start position 
        lastPosition = transform.position;
        // start walking already
        SetToIdle(); 
        // Start timer to become hungry 
        timeLeftToHungry = Random.Range(timeBetweenMeal, 2 * timeBetweenMeal);
        timeLeftToSleep = Random.Range(timeBetweenSleep, 2 * timeBetweenSleep);
        currentWaitDuration = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHunger();
        HandleSleep();
        HandleMovement();
        OrientateCat();
        HandleDissatisfaction();
    }

    void HandleDissatisfaction()
    {
        // one of two waiting states
        if (state == CatState.Hungry || state == CatState.Tired)
        {
            currentWaitDuration += Time.deltaTime;
            if (currentWaitDuration >= maxDurationWainting)
            {
                LevelManager.instance.DecreaseSatisfaction(10);
                Debug.Log(gameObject.name + " is Unsatisfied!");
                currentWaitDuration = 0f;
                // Rest cat to fresh
                timeLeftToHungry = Random.Range(timeBetweenMeal, 2 * timeBetweenMeal);
                timeLeftToSleep = Random.Range(timeBetweenSleep, 2 * timeBetweenSleep);
                if (currentBubble != null) Destroy(currentBubble);

                SetToIdle();
            }
        }
    }

    void HandleSatisfaction(int increaseBy)
    {
        LevelManager.instance.IncreaseSatisfaction(increaseBy);
    }

    // Hunger takes priorty over Sleep
    void HandleSleep()
    {
        // Update sleep only if cat is IDLE or WALKING
        if (state != CatState.Idle && state != Cat.CatState.Walking ) return;

        timeLeftToSleep -= Time.deltaTime;
        if (timeLeftToSleep <= 0f)
        {
            state = CatState.Tired;
            UpdateAnimation();
            SpawnSleepBubble();
        }
    }

    public void SleepCat()
    {
        // remove Prefab Instance of any bubble
        if (currentBubble != null) Destroy(currentBubble);
        currentBubble = null;
        currentWaitDuration = 0f;
        timeLeftToSleep = Random.Range(timeBetweenSleep/2,timeBetweenSleep);
        state = CatState.Sleeping;
        animator.SetBool("isSleeping", true);
        UpdateAnimation();

        // Just incase there was multiple invocations from overclicking
        // just defensive measure, probably not needed 
        CancelInvoke(nameof(FinishSleep));
        // call finish eating
        Invoke(nameof(FinishSleep), sleepDuration);     
    }

    void FinishSleep()
    {
        animator.SetBool("isSleeping", false);
        // TODO: this is currently hardcoded (!!) 
        // Different cats might have different benefits when happen
        HandleSatisfaction(5); 
        SetToIdle();      
    }

    void SpawnSleepBubble()
    {
        if (currentBubble != null) Destroy(currentBubble);
        // assign current bubble to FOOD BUBBLE 
        currentBubble = Instantiate(
            sleepBubblePrefab,
            transform.position,
            Quaternion.identity
        );
        // parent bubble position to cat pos
        currentBubble.transform.SetParent(transform);
        // offset bubble above cat
        currentBubble.transform.localPosition = new Vector3(0, 1f, 0);

        BubbleCatSleep bubbleScript = currentBubble.GetComponent<BubbleCatSleep>();
        bubbleScript.Setup(this);
     
    }

    void HandleHunger()
    {
        // Update hunger only if cat is IDLE or WALKING
        if (state != CatState.Idle && state != Cat.CatState.Walking ) return;        
        // reduce time left to hungry by duration since last frame
        timeLeftToHungry -= Time.deltaTime; 
        if (timeLeftToHungry <= 0f)
        {
            state = CatState.Hungry;
            UpdateAnimation();
            SpawnHungerBubble(); 
        }
    }
    // Feed the cat 
    public void FeedCat()
    {
        // remove Prefab Instance of any bubble
        if (currentBubble != null) Destroy(currentBubble); 
        currentBubble = null;
        currentWaitDuration = 0f;
        timeLeftToHungry = Random.Range(timeBetweenMeal/2,timeBetweenMeal);
        state = CatState.Eating;
        animator.SetBool("isEating", true);
        UpdateAnimation();

        

        // Just incase there was multiple invocations from overclicking
        // just defensive measure, probably not needed
        CancelInvoke(nameof(FinishEating));
        // call finish eating
        Invoke(nameof(FinishEating), eatDuration);
    }

    void FinishEating()
    {
        animator.SetBool("isEating", false);
        //TODO: this is current hard coded, leave it for now
        // when we expand to more cats with personalities change
        HandleSatisfaction(8); 
        SetToIdle();
    }

    void SpawnHungerBubble()
    {
        if (currentBubble != null) Destroy(currentBubble);
        // assign current bubble to FOOD BUBBLE 
        currentBubble = Instantiate(
            foodBubblePrefab,
            transform.position,
            Quaternion.identity
        );
        // parent bubble position to cat pos
        currentBubble.transform.SetParent(transform);
        // offset bubble above cat
        currentBubble.transform.localPosition = new Vector3(0, 1f, 0);

        BubbleCatFood bubbleScript = currentBubble.GetComponent<BubbleCatFood>();
        bubbleScript.Setup(this); // pass reference of cat to bubble
    }

    // make cat face left/right depending on delta x
    void OrientateCat()
    {
        // only flip direction when walking
        if (state != CatState.Walking) return; 

        flipSpriteCoolDown -= Time.deltaTime;

        Vector2 movementDelta = (Vector2) transform.position - lastPosition;
        if (flipSpriteCoolDown <= 0f)
        {      
            if (movementDelta.x > 0.01f)
            {
                spriteRenderer.flipX = false;
                flipSpriteCoolDown = minFlipSpriteInterval;
            }
            else if (movementDelta.x < -0.01f)
            {
                spriteRenderer.flipX = true;
                flipSpriteCoolDown = minFlipSpriteInterval;
            }
        }
    
        lastPosition = transform.position; 
    }

    void HandleMovement()
    {
        if (state == CatState.Idle)
        {
            idleTimeLeft -= Time.deltaTime;
            if (idleTimeLeft <= 0f)
            {
                state = CatState.Walking;
                UpdateAnimation();
                PickRandomTargetPos();
            }
        }
        else if (state == CatState.Walking)
        {
            if (currPath == null || pathIdx >= currPath.Count)
            {
                // done walking 
                SetToIdle();
                return;
            }
            Vector2 next = currPath[pathIdx].transform.position;
            transform.position = Vector2.MoveTowards(
                transform.position,
                next,
                speed * Time.deltaTime
            );
            if (Vector2.Distance(transform.position, next) <= 0.1f)
            {
                pathIdx++;
            }
        }
    }

    void UpdateAnimation()
    {
        // just to be safe update everything
        animator.SetBool("isWalking", state == CatState.Walking);      
        animator.SetBool("isEating", state == CatState.Eating);      
        animator.SetBool("isSleeping", state == CatState.Sleeping);      
    }

    // common action abstracted, reset cat to Idle and update needed variables
    void SetToIdle()
    {
        state = CatState.Idle;
        UpdateAnimation();
        idleTimeLeft = fixedIdleDuration;        
    }

    void PickRandomTargetPos()
    {
        Node target = AStar.instance.GetRandomNode();
        currPath = AStar.instance.GeneratePath(
            transform.position, // current cat position
            target.transform.position // selected target pos
        );
        pathIdx = 0;
    }
    
    public int GetFeedCost()
    {
        return feedCost;
    }
}
