using UnityEngine;

// Created this scriptabl
[CreateAssetMenu(fileName = "NewCatData", menuName = "Cats/CatData")]
public class CatData : ScriptableObject
{
    public string catName;
    public GameObject prefab;

    // shop 
    public Sprite icon;
    public int price;

    public int satisfactionGain;
    public int customerTips;
    
    public float catSpeed;
    //[TextArea] public string description
}
