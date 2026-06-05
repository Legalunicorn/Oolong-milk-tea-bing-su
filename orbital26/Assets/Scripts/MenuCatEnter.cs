using UnityEngine;

public class MenuCatWalkAlongPavement : MonoBehaviour
{
    public float targetX = 100f;     // where the cat stops
    public float speed = 180f;

    private RectTransform rectTransform;
    private Animator animator;
    private bool hasArrived = false;
    private float fixedY;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();

        // Keep the cat on the same pavement line
        fixedY = rectTransform.anchoredPosition.y;

        if (animator != null)
        {
            animator.Play("WhiteSpot_Walking");
        }
    }

    void Update()
    {
        if (hasArrived) return;

        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(targetX, fixedY);

        rectTransform.anchoredPosition = Vector2.MoveTowards(
            currentPos,
            targetPos,
            speed * Time.deltaTime
        );

        if (Mathf.Abs(rectTransform.anchoredPosition.x - targetX) < 0.1f)
        {
            rectTransform.anchoredPosition = targetPos;
            hasArrived = true;

            if (animator != null)
            {
                animator.Play("WhiteSpot_Idle");
            }
        }
    }
}
