using UnityEngine;
using UnityEngine.InputSystem;


// NOTE: this script is attached the the FRIDGE 
// object 
 
// then it will control the menu UI later

// the click util uses circle 
// but our fridge is a rectange 2 x 1 ratio 
// TODO: InputUtils to create a new method to check if clikc in rectangle? 
// we can delay that for later now just write the logic here unless its also needed else where
public class FridgeClickable : MonoBehaviour
{

    [SerializeField] private FoodMenuUI foodMenuUI;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (boxCollider.OverlapPoint(pos))
            {
                foodMenuUI.Toggle();
            }
        }
    }
}
