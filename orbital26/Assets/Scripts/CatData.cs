using UnityEngine;

// Created this scriptabl
[CreateAssetMenu(fileName = "NewCatData", menuName = "Cats/CatData")]
public class CatData : ScriptableObject
{
    public string name;
    public GameObject prefab;

    // shop 
    public Sprite icon;
    public int price;
    //[TextArea] public string description
}
