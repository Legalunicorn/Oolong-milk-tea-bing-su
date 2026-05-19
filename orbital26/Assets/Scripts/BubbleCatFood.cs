using UnityEngine;
using UnityEngine.EventSystems;

// reference: https://stackoverflow.com/questions/79773098/why-onmousedown-is-still-not-working-after-i-checked-the-script-and-the-collid
// 1. need to add 2d physics ray cast to main camera of the scene 
// 2. UI -> EventSystem add that into the scene as well
public class BubbleCatFood : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Cat.cs script 
    private Cat cat; 
    // how long the user has clicked on the bubble
    private float holdTime = 0f; 
    [SerializeField] private float targetHoldTime = 3f;
    private bool isHolding = false;

    // Set up, called by "Cat.cs" AFTER bubble is initialized
    public void Setup(Cat catRef)
    {
        cat = catRef;
    }


    // Update is called once per frame
    void Update()
    {       
        Debug.Log("Hold durat: " + holdTime);
        if (!isHolding) return;
        holdTime += Time.deltaTime;
        if (holdTime >= targetHoldTime)
        {
            // will automatically destroy this bubble as well
            cat.FeedCat();
        }
    }

    // MOUSE INPUTS 
    // worked with collider 2d 
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Bubble clicked!");
        isHolding = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdTime = 0f;
    } 
}
