using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform fakeEntrance;
    public Transform stopPosition;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;

    [Header("References")]
    public Animator animator;
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
        if (isMoving && !hasReachedStopPosition)
        {
            MoveToStopPosition();
        }
    }

    private void MoveToStopPosition()
    {
        Vector3 direction = stopPosition.position - transform.position;

        if (direction.magnitude <= 0.05f)
        {
            transform.position = stopPosition.position;
            StopMoving();

            Invoke(nameof(BecomeHungry), 1.5f);
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
        if (hungryBubble != null)
        {
            hungryBubble.SetActive(true);
        }
    }

    public void Unhungry()
    {
        if (hungryBubble != null)
        {
            hungryBubble.SetActive(false);
        }

        animator.SetBool("isWalking", false);
    }
}
