using UnityEngine;
using UnityEngine.InputSystem;

// bubble with a cat inside 
// when clicked -> will change the game to selection mode
public class RequestCatBubble : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CustomerController customer;
    [SerializeField] private CatData requestedCatData;

    public void Setup(CustomerController customerRef, CatData data)
    {
        customer = customerRef;
        requestedCatData = data;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("CAT REQ BUBBLE CLICKED");
            OnMouseClicked();
        }
    }

    void OnMouseClicked()
    {
        // same logic again 
        // im thinking of abstracting this to some utility class function
        // then refactor the code 
        Vector2 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float dist = Vector2.Distance(pos, (Vector2)transform.position);
        if (dist > 0.5f) return;
        LevelManager.instance.EnterCatSelection(requestedCatData, customer);
        // dont destroy the bubble yet 
        // when the match is success ful destroy the bubblre 
    }
}
