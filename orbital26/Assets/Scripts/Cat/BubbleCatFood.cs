using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// reference: https://stackoverflow.com/questions/79773098/why-onmousedown-is-still-not-working-after-i-checked-the-script-and-the-collid
// 1. need to add 2d physics ray cast to main camera of the scene 
// 2. UI -> EventSystem add that into the scene as well
public class BubbleCatFood : MonoBehaviour
{
    // Cat.cs script 
    private Cat cat; 
    // how long the user has clicked on the bubble
    [SerializeField] private float holdTime = 0f; 
    [SerializeField] private float targetHoldTime = 1.5f;
    [SerializeField] private Image progressRing;
    private bool isHolding = false;
    private bool isFeedingTriggered = false;

    // Set up, called by "Cat.cs" AFTER bubble is initialized
    public void Setup(Cat catRef)
    {
        cat = catRef;
    }

    void Update()
    {
        // Unfortunately the only solution that worked for fixing the bug (bubbles randomly stop detecting clicks)
        // is to manually detect the mouse position and estimate if the bubble is being clicked
        // tried everything else, this is last resort, or otherwise other last resort is use legacy input system
        if (Mouse.current.leftButton.wasPressedThisFrame) MouseClicked();
        if (Mouse.current.leftButton.wasReleasedThisFrame) MouseLetGo();
        if (isHolding) Mouseholding();
        

    }

    void MouseClicked()
    {
        if (InputUtils.IsMouseInRadius(transform.position)) isHolding = true;
    }

    void MouseLetGo()
    {
        isHolding = false;
        holdTime = 0f;
        progressRing.fillAmount = 0f;
        // MouseHolding() has triggered feeding already
        if (isFeedingTriggered) Destroy(gameObject);
    }

    void Mouseholding()
    {
        holdTime += Time.deltaTime;
        progressRing.fillAmount = holdTime / targetHoldTime;
        if (holdTime >= targetHoldTime && !isFeedingTriggered)
        {
            // not enough money to feed cat
            if (!GameManager.instance.SpendCash(cat.GetFeedCost()))
            {
                Debug.Log("Not enough cash to feed");
                isHolding = false;
                holdTime = 0f;
                return;
            }
            isFeedingTriggered = true;
            cat.FeedCat();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }

    }
}
