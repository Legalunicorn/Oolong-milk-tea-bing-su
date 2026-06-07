using UnityEngine;
using UnityEngine.InputSystem;

public class HungryBubbleClick : MonoBehaviour
{
    public CustomerController customerController;
 
    // once this object has been instantiated by customer
    // it will call the set up to link tofether
    public void Setup(CustomerController customerRef)
    {
        customerController = customerRef;
    }

    private void Update()
    {
        if (InputUtils.IsLeftClickInRadius(transform.position))
        {
            customerController.ServeFood();
        }
    }
}
