using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// bubble with a cat inside 
// when clicked -> will change the game to selection mode
public class RequestCatBubble : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CustomerController customer;
    [SerializeField] private CatData requestedCatData;
    // Cat sprite icon based on cat data!
    [SerializeField] private SpriteRenderer catIcon;

    [SerializeField] private  CustomerTimer timer;

    [SerializeField] private  TextMeshPro catName;

    public void Setup(CustomerController customerRef, CatData data)
    {
        customer = customerRef;
        requestedCatData = data;
        catIcon.sprite = data.icon;
        catName.text = data.catName;
    }


    void Update()
    {
        if (InputUtils.IsLeftClickInRadius(transform.position))
        {
            AudioManager.instance?.PlayCustomerBubbleClickSound();
            Debug.Log("cat req was clicked");
            //tutorial  callback
            TutorialManager.instance?.OnCustomerCatReqClicked();
        
            LevelManager.instance.EnterCatSelection(requestedCatData, customer);
        }

        float left = customer.GetTimerLeft();
        if (left >=0f && left <= 1.0f)
        {
            timer.SetProgress(left);
        }



        // if (Mouse.current.leftButton.wasPressedThisFrame)
        // {
        //     Debug.Log("CAT REQ BUBBLE CLICKED");
        //     OnMouseClicked();
        // }
    }

}
