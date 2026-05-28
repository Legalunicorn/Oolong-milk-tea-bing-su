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
    // TODO: use 
    public GameObject hungryBubble;

    private bool isMoving = true;
    private bool hasReachedStopPosition = false;


    void Start()
    {
        transform.position = fakeEntrance.position;

        if (hungryBubble != null)
        {
            hungryBubble.SetActive(false);
        }
    }

    void Update()
    {
        // keep moving to stop position
        if (isMoving && !hasReachedStopPosition)
        {
            MoveToStopPosition();
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
        // TODO: refactor this: instantiate bubble instead of having relation to gameobject exist before hand
        // TODO: start hungry timer 
        if (hungryBubble != null)
        {
            hungryBubble.SetActive(true);
        }
    }

    public void Unhungry()
    {
        // TODO: end hungry timer
        if (hungryBubble != null)
        {
            hungryBubble.SetActive(false);
        }

        animator.SetBool("isWalking", false);
    }
}
