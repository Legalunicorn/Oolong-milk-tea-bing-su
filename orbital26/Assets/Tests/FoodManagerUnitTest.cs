using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// Food manager manages the stock 
// 
public class NewTestScript
{

    private FoodManager foodManager;


    [SetUp]
    public void Setup()
    {
        GameObject obj = new GameObject();
        foodManager = obj.AddComponent<FoodManager>();
    }

    // TODO: tear down obj 

    [Test]
    public void IncreaseStockFromZero_HasOne()
    {
        FoodData food = ScriptableObject.CreateInstance<FoodData>();
        foodManager.IncreaseStock(food);
        Assert.AreEqual(1, foodManager.GetStock(food));
    }
        
    [Test]
    public void ServeWithNoStock_ReturnsFalse()
    {
        FoodData mock = ScriptableObject.CreateInstance<FoodData>();
        Assert.IsFalse(foodManager.ServeFood(mock));
    }

    [Test]
    public void HasStockWithNoStock_ReturnsFalse()
    {
        FoodData mock = ScriptableObject.CreateInstance<FoodData>();
        Assert.IsFalse(foodManager.HasStock(mock));
    }

    [Test]
    public void ServeWithoutStock_DoesNotMakeNegativeStock()
    {
        FoodData mock = ScriptableObject.CreateInstance<FoodData>();
        foodManager.ServeFood(mock);
        Assert.AreEqual(0, foodManager.GetStock(mock));
    }

}
