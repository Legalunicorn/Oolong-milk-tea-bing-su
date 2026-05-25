using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// ** very similar to BubbleCatFood
// NOTE: consider abstracting to an interface
public class BubbleCatSleep : MonoBehaviour
{

    private Cat cat;
    [SerializeField] private float holdTime = 0f;
    [SerializeField] private float targetHoldTime = 1.5f;
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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        float distance = Vector2.Distance(mousePos, (Vector2) transform.position);
        // estimate radius of bubble ~ 0.5f
        if (distance < 0.5f) isHolding = true;
    }

    void MouseLetGo()
    {
        isHolding = false;
        holdTime = 0f;
        if (isSleepingTriggered) Destroy(gameObject);
    }

    void MouseHolding()
    {
        holdTime += Time.deltaTime;
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

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
