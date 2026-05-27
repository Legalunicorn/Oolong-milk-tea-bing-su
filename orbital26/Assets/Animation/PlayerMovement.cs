using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of the player movement

    public Transform fakeEntrance;
    public Transform stopPoint;

    private Animator animator;
    private bool hasReachedStopPoint = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        transform.position = fakeEntrance.position; // Start at the fake entrance
        animator.SetBool("isWalking", true); // Start walking animation
    }

    // Update is called once per frame
    void Update()
    {
       if(!hasReachedStopPoint)
        {
            WalkToStopPoint();
        }
        else
        {
            Idle();
        }
    }

     void WalkToStopPoint()
    {
        Vector3 direction = stopPoint.position - transform.position;
        Vector3 moveDirection = direction.normalized;

        transform.position = Vector3.MoveTowards(
            transform.position,
            stopPoint.position,
            moveSpeed * Time.deltaTime
        );

        animator.SetFloat("InputX", moveDirection.x);
        animator.SetFloat("InputY", moveDirection.y);

        animator.SetFloat("LastInputX", moveDirection.x);
        animator.SetFloat("LastInputY", moveDirection.y);

        animator.SetBool("isWalking", true);

        if (Vector3.Distance(transform.position, stopPoint.position) < 0.05f)
        {
            hasReachedStopPoint = true;
        }
    }

     void Idle()
    {
        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", 0);

        animator.SetBool("isWalking", false);
    }

    }
