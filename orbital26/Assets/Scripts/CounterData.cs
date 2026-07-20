using UnityEngine;


[CreateAssetMenu(fileName ="NewwCounterData", menuName="Counter/CounterData")]
public class CounterData : ScriptableObject
{
    public string name;
    public int price;
    // may not be needed
    public Sprite preview;
    public Sprite sprite; // actuall
}
