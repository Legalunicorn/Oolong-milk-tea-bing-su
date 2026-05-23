using UnityEngine;

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


    //  [MOVEMENT]  Related Mechanics 
    [SerializeField] private float speed = 2f;
    public BoxCollider2D area; // boundary object
    [SerializeField] private Vector2 targetPos; // position to move towards
    private const float fixedIdleDuration = 1f; // how long to idle between each walk
    private float idleTimeLeft = 0f; // reset to 1f

    // [HUNGER] variables
    [SerializeField] private float timeBetweenMeal = 10f; 
    private float timeLeftToHungry;
    // private GameObject currentBubble;
    [SerializeField] private GameObject foodBubblePrefab;
    [SerializeField] private float eatDuration = 4f;
    
    // [SLEEP] variables
    [SerializeField] private float timeBetweenSleep = 10f;
    private float timeLeftToSleep;
    [SerializeField] private GameObject sleepBubblePrefab;
    [SerializeField] private float sleepDuration = 4f;

    // [Sleep/Hunger] shared bubble
    private GameObject currentBubble;






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
        PickRandomTargetPos();
        state = CatState.Walking;
        UpdateAnimation();
        // Start timer to become hungry 
        timeLeftToHungry = Random.Range(timeBetweenMeal, 2 * timeBetweenMeal);
        timeLeftToSleep = Random.Range(timeBetweenSleep, 2 * timeBetweenSleep);
    }

    // Update is called once per frame
    void Update()
    {
        HandleHunger();
        HandleSleep();
        HandleMovement();
        OrientateCat();

    }

    // Hunger takes priorty over Sleep
    void HandleSleep()
    {
        // hunger takes priority 
        if (state == CatState.Hungry || state == CatState.Eating) return;
        // already handled sleepiness
        if (state == CatState.Tired || state == CatState.Sleeping) return; 

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
        currentBubble = null;
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
        state = CatState.Idle;
        idleTimeLeft = fixedIdleDuration;
        UpdateAnimation();        
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
        // already hungry ignore
        // hungry take priority over sleep 
        if (state == CatState.Tired || state == CatState.Sleeping) return;
        if (state == CatState.Hungry || state == CatState.Eating) return;
        

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
        //if (currentBubble != null) Destroy(currentBubble); 
        currentBubble = null;
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
        state = CatState.Idle;
        idleTimeLeft = fixedIdleDuration;
        UpdateAnimation();
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
        Vector2 movementDelta = (Vector2) transform.position - lastPosition;
        if (movementDelta.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        } else if (movementDelta.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        lastPosition = transform.position; 
    }



    void UpdateAnimation()
    {
        // just to be safe update everything
        animator.SetBool("isWalking", state == CatState.Walking);      
        animator.SetBool("isEating", state == CatState.Eating);      
        animator.SetBool("isSleeping", state == CatState.Sleeping);      
    }

    void HandleMovement()
    {
        // no movement when hungry or eating ,so that user can click and hold  on hungry bubble 
        if (state == CatState.Hungry || state == CatState.Eating) return; 
        if (state == CatState.Sleeping || state == CatState.Tired) return;
        if (state == CatState.Idle)
        {
            // minus the duration since the last Frame
            idleTimeLeft -= Time.deltaTime;
            if (idleTimeLeft <= 0f)
            {
                // cat has idled long enough, time to start walking
                state = CatState.Walking;
                UpdateAnimation();
                PickRandomTargetPos();
            }
        } 
        else if (state == CatState.Walking)
        {
            // move towards the target position 
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            // reached the target position 
            if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            {
                state = CatState.Idle;
                UpdateAnimation();
                idleTimeLeft = fixedIdleDuration;
            }
        }
    }

    void PickRandomTargetPos()
    {
        Bounds bound = area.bounds;
        // select random point within bound
        float x = Random.Range(bound.min.x, bound.max.x);
        float y = Random.Range(bound.min.y, bound.max.y);
        targetPos = new Vector2(x, y);
    }
}
