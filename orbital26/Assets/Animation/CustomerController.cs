using UnityEngine;


public class CustomerController : MonoBehaviour
{
    [Header("Movement Points")]
    // fakeEntrance -> spawn point 
    public Transform fakeEntrance;
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

    private bool isMoving = true;
    private bool hasReachedStopPosition = false;
    private bool isHungry = false;
    private bool isExiting = false;

    // when the customer starts being unhappy 
    [SerializeField] private float hungryTime = 0f;
    [SerializeField] private float maxWaitDuration = 5f;
    // howoften to decrease the satisfaction
    [SerializeField] private float complainInterval = 1f;
    [SerializeField] private float timeSinceLastComplain = 0f;

    void Awake()
    {
        fakeEntrance = GameObject.FindWithTag("fakeEntrance").transform;
        stopPosition = GameObject.FindWithTag("stopPosition").transform;
    }
    void Start()
    {
        transform.position = fakeEntrance.position;
    }

    void Update()
    {
        // move to entrance when need to exit
        if(isExiting)
        {
            MoveToEntrance();
            return;
        }

        // keep moving to stop position
        if (isMoving && !hasReachedStopPosition)
        {
            MoveToStopPosition();
        }
        HandleIsHungry();
    }

    void HandleIsHungry()
    {
        if (isHungry)
        {
            hungryTime += Time.deltaTime;
            if (hungryTime >= maxWaitDuration)
            {
                timeSinceLastComplain += Time.deltaTime;
                if (timeSinceLastComplain >= complainInterval)
                {
                    Debug.Log("satisfaction DOWN "); 
                    if (LevelManager.instance != null)
                    {
                        LevelManager.instance.DecreaseSatisfaction(1);
                    }
                    timeSinceLastComplain = 0f; //reset timer                
                }

            }
        }
    }

    private void MoveToStopPosition()
    {
        Vector3 direction = stopPosition.position - transform.position;
        
        // close enough to target position, stop and be hungry
        if (direction.magnitude <= 0.05f)
        {
            transform.position = stopPosition.position;
            StopMoving();
            Invoke(nameof(BecomeHungry), 1.5f);
            return;
        }

        // Keep on moving 

        Vector3 moveDirection = direction.normalized;
        // speed = distance/time  -> distance = speed * time
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        animator.SetBool("isWalking", true);

        animator.SetFloat("InputX", moveDirection.x);
        animator.SetFloat("InputY", moveDirection.y);

        animator.SetFloat("LastInputX", moveDirection.x);
        animator.SetFloat("LastInputY", moveDirection.y);
    }

    private void MoveToEntrance()
    {
        Vector3 direction = fakeEntrance.position - transform.position;

        // If close enough to entrance, remove customer
        if (direction.magnitude <= 0.05f)
        {
            transform.position = fakeEntrance.position;
            StopMoving();

             Destroy(gameObject); 
            return;
        }

        Vector3 moveDirection = direction.normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        animator.SetBool("isWalking", true);

        animator.SetFloat("InputX", moveDirection.x);
        animator.SetFloat("InputY", moveDirection.y);

        animator.SetFloat("LastInputX", moveDirection.x);
        animator.SetFloat("LastInputY", moveDirection.y);
    }

    private void StopMoving()
    {
        isMoving = false;
        hasReachedStopPosition = true;

        animator.SetBool("isWalking", false);
        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", 0);
    }

    private void BecomeHungry()
    {
        if (isHungry) return;
        isHungry = true;
        // TODO: refactor this: instantiate bubble instead of having relation to gameobject exist before hand
        // TODO: start hungry timer 
        // if (hungryBubble != null)
        // {
        //     hungryBubble.SetActive(true);
        // }

        // remove currentBubble incase it exists, also planning for future bubbles (cat bubble)
        if (currentBubble != null) Destroy(currentBubble);
        currentBubble = Instantiate(
            hungryBubblePrefab,
            transform.position,
            Quaternion.identity
        );
        currentBubble.transform.SetParent(transform);
        currentBubble.transform.localPosition = new Vector3(-1f, 1f, 0);
        HungryBubbleClick bubbleScript = currentBubble.GetComponent<HungryBubbleClick>();
        if (bubbleScript != null) bubbleScript.Setup(this);
    }

    public void Unhungry()
    {
        // TODO: end hungry timer
        if (currentBubble != null)
        {
            Destroy(currentBubble);
           // hungryBubble.SetActive(false);
        }
        // Add cash once feed 
        if (GameManager.instance != null)
        {
            GameManager.instance.AddCash(5); 
        }
        isHungry = false;
        hungryTime = 0f;
        timeSinceLastComplain = 0f;

        // After feeding, the customer exits the scene 
        StartExiting();
    }

    private void StartExiting()
    {
        isHungry = false;
        isMoving = false;
        hasReachedStopPosition = false;
        isExiting = true;

        animator.SetBool("isWalking", true);
    }
}
