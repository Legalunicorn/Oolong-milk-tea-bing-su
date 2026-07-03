using UnityEngine;


[CreateAssetMenu(fileName = "NewFoodData", menuName = "Food/FoodData")]
public class FoodData : ScriptableObject
{
    public string foodName;
    // enumn is redundant actly
    //public FoodType foodType; // ENUM
    public Sprite icon; // display picture
    // maybe add this, for now cost = 0;
    public int cookCost;
    public float cookTime;
    public int price; // revenue
}
