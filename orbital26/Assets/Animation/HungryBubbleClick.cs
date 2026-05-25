using UnityEngine;

public class HungryBubbleClick : MonoBehaviour
{
    public CustomerController customerController;

    private void Start()
    {
        Debug.Log("HungryBubbleClick script is running");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

            if (hit != null)
            {
                Debug.Log("Clicked object: " + hit.gameObject.name);

                if (hit.gameObject == gameObject)
                {
                    Debug.Log("Bubble detected!");

                    if (customerController != null)
                    {
                        customerController.Unhungry();
                    }
                    else
                    {
                        Debug.Log("CustomerController not assigned!");
                    }
                }
            }
            else
            {
                Debug.Log("Clicked nothing");
            }
        }
    }
}
