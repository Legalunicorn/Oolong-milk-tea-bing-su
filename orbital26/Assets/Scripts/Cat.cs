using UnityEngine;

/**
 * Script controls CAT behavior in the game 
 * - movement, animation, walking mechanics, hunger  (for now)
 * - might split to further scripts later if neede
 */ 
public class Cat : MonoBehaviour
{
    // States for the cat 
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isHungry;
    public float speed = 2f;

    // Animator component (handle x-direction, and current state)
    Animator animator;

    // Movement Related Mechanics (abstract later if needed)
    public BoxCollider2D area; // boundary object
    private Vector2 targetPos; // position to move towards
    private const float fixedIdleDuration = 1f; // how long to idle between each walk
    private float idleTimeLeft = 0f; // reset to 1f 
    
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        PickRandomTargetPos();
        isWalking = true;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }

    void handleMovement()
    {
        // cat is idle  
        if (!isWalking)
        {
            // minus the duration since the last Frame
            idleTimeLeft -= Time.deltaTime;
            if (idleTimeLeft <= 0f)
            {
                // cat has idled long enough, time to start walking
                isWalking = true;
                PickRandomTargetPos();
            }
        } else
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
                isWalking = false;
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
