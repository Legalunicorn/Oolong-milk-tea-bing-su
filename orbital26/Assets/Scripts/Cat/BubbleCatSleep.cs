using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ** very similar to BubbleCatFood
// NOTE: consider abstracting to an interface
public class BubbleCatSleep : MonoBehaviour
{
    private Cat cat;
    [SerializeField] private float holdTime = 0f;
    [SerializeField] private float targetHoldTime = 1.5f;
    [SerializeField] private Image progressRing;
    private bool isHolding = false;
    private bool isSleepingTriggered = false;

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
            isHolding = true;
        }
    }

    void MouseLetGo()
    {
        isHolding = false;
        holdTime = 0f;
        if (progressRing != null) progressRing.fillAmount = 0f;
        if (isSleepingTriggered) Destroy(gameObject);
    }

    void MouseHolding() 
    {
        holdTime += Time.deltaTime;
        if (progressRing != null) progressRing.fillAmount = holdTime / targetHoldTime;
        if (holdTime >= targetHoldTime && !isSleepingTriggered)
        {
            // TODO: remove this once settings
            // BUG: when feedingCat, null pointer referencing cat suddenly appears. why??
            if (cat == null)
            {
                Debug.LogWarning("BubbleCatSleep: cat reference is null, destroying bubble");
                Destroy(gameObject);
                return;
            }            
            isSleepingTriggered = true;
            cat.SleepCat();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
