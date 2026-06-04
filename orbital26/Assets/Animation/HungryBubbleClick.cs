using UnityEngine;

public class HungryBubbleClick : MonoBehaviour
{
    // BUG: this cannot work for prefabs, we cant manually connect spawned bubble to customer controller 
    // FIX: customer should instantiate HungryBubbleClick prefab and parent it etc
    // Current the connection is physically dragged via the in
    public CustomerController customerController;
 
    // once this object has been instantiated by customer
    // it will call the set up to link tofether
    public void Setup(CustomerController customerRef)
    {
        customerController = customerRef;
    }


    private void Update()
    {
        // GetMouseButtonDown(0) -> left click
        if (Input.GetMouseButtonDown(0))
        {
            // convert mouse coordinates to world cooordinates, check if within bubble collision
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
 
            if (hit != null && hit.gameObject == gameObject && customerController != null)
            {
                customerController.ServeFood();
            }
           
        }
    }
}
