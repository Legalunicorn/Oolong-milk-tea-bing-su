using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


// This abstract the logic for bubbles belonging to cats since they share 
// very similar properties 
// 
public abstract class BubbleCatInteraction : MonoBehaviour
{

    protected Cat cat;
    [SerializeField] private float holdTIme = 0f;
    [SerializeField] private  float targetHoldTime = 0.75f;
    [SerializeField] private Image progressRing;
    private bool isHolding = false;
    // action -> sleep or eat
    private bool isActionTriggered = false;

    public void Setup(Cat catRef)
    {
        cat = catRef;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) MouseClicked();
        if (Mouse.current.leftButton.wasReleasedThisFrame) MouseLetGo();
        if (isHolding) MouseHolding();
    }

    void MouseClicked()
    {
        if (InputUtils.IsMouseInRadius(transform.position))
        {
            AudioManager.instance?.PlayCatBubbleClickSound();
            isHolding = true;
        }
    }

    void MouseLetGo()
    {
        isHolding = false;
        holdTIme = 0f;
        if (progressRing != null) progressRing.fillAmount = 0f;
        if (isActionTriggered) Destroy(gameObject);
    }

    void MouseHolding()
    {
        holdTIme += Time.deltaTime;
        if (progressRing != null) progressRing.fillAmount = holdTIme / targetHoldTime;
        if (holdTIme >= targetHoldTime && !isActionTriggered)
        {
            isActionTriggered = true;
            TriggerBubbleAction();
            // GetComponent<SpriteRenderer>().enabled = false;
            // GetComponent<Collider2D>().enabled = false;
        }
    }

    // implementing class to override this 
    protected abstract void TriggerBubbleAction();

}
