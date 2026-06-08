using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// GameManager -> manage game scenes, states that persist beyond a level session 
// Unit tests -> cash functions work as expected
public class GameManagerUnitTest
{

    private GameManager gm;
    private int baseCash = 100; 

    // //set up our mocks instance with mock variables
    [SetUp] 
    public void Setup()
    {
        GameObject obj = new UnityEngine.GameObject();
        gm = obj.AddComponent<GameManager>();
        gm.cash = baseCash;
    }
    
    [Test]
    public void SpendWithSufficientCash()
    {
        // 40 < 100, should be able to spend no issue
        int cost = 40;
        int balance = baseCash - cost;
        bool res = gm.SpendCash(cost);
        Assert.IsTrue(res);
        Assert.AreEqual(balance, gm.cash);
    }

    [Test]
    public void SpendWithInSufficientCash()
    {
        int cost = 2 * baseCash;
        bool res = gm.SpendCash(cost);
        // did not deduct points
        Assert.IsFalse(res);
        // user cash remained the same
        Assert.AreEqual(gm.cash, baseCash);
    }






    // // A Test behaves as an ordinary method
    // [Test]
    // public void GameManagerUnitTestSimplePasses()
    // {
    //     // Use the Assert class to test conditions
    // }

    // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator GameManagerUnitTestWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }
}
