using UnityEngine;
using UnityEngine.InputSystem;


// NOTE. bubble is not longer click
// bubble is just a visual UI
// but now it needs a child food data for the icon

public class HungryBubbleClick : MonoBehaviour
{
    public CustomerController customerController;
    [SerializeField] private FoodData requestedFood;

    // the child
    [SerializeField] private SpriteRenderer foodIcon;
 
    // once this object has been instantiated by customer
    // it will call the set up to link tofether
    public void Setup(CustomerController customerRef, FoodData food)
    {
        customerController = customerRef;
        requestedFood = food;
        if (foodIcon != null) foodIcon.sprite = food.icon;
    }

    private void Update()
    {
        // OLD method is remove, no longer click to serve,
        // drag food directly to customer
        // if (InputUtils.IsLeftClickInRadius(transform.position))
        // {
        //     customerController.ServeFood();
        // }
    }

    // just extra incase we need 
    // i think customer controller should also store so 
    // if this isnt used then delete 
    public FoodData GetRequestedFood()
    {
        return requestedFood;
    }
}
