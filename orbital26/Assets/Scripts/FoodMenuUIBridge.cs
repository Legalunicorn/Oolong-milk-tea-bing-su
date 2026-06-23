using UnityEngine;

public class FoodMenuUIBridge : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Toggle()
    {
        FoodMenuUI.instance.Toggle();
    }

    public void Hide()
    {
        FoodMenuUI.instance.Hide();
    }
 
}
