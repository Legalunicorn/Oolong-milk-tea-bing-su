using UnityEngine;

public class HungryBubbleClick : MonoBehaviour
{
    public CustomerController customerController;

    private void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

            if (hit != null)
            {
                

                if (hit.gameObject == gameObject)
                {
                   

                    if (customerController != null)
                    {
                        customerController.Unhungry();
                    }
                    
                }
            }
           
        }
    }
}
