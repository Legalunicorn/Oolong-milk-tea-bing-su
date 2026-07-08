using UnityEngine;
using System.Collections.Generic;



// another Singletone again
// manages and tracks the inventory of food 
// i didnt want to flood game manager so made a new file
public class FoodManager : MonoBehaviour
{
    public static FoodManager instance {get; private set;}
    //private Dictionary<FoodType, int> stock = new Dictionary<FoodType, int>();
    private Dictionary<FoodData, int> stock = new Dictionary<FoodData, int>();

    void Awake()
    {
        // code for singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // dont override existing instace
            Destroy(gameObject);
        }
    }

    public int GetStock(FoodData ft)
    {
        // return stock[ft];
        // safer to check key firsts 
        return stock.ContainsKey(ft) ? stock[ft] : 0;
    }

    public bool HasStock(FoodData ft)
    {
        return GetStock(ft) >0;
    }

    public void IncreaseStock(FoodData ft)
    {
        if (!stock.ContainsKey(ft)) stock[ft] = 0;
         AudioManager.instance?.PlaySpendMoneySound();
        stock[ft]++;
        // TODO: update UI of food increment
        // or maybe assign responsibility to who everr cllaed this
    }

    public bool ServeFood(FoodData ft)
    {
        if ( !HasStock(ft)) return false;
        stock[ft]--;
        // TODO: update UI of counter to decrease, or assign responsiblity 
        return true;
    }
}
