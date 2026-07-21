using UnityEngine;

[CreateAssetMenu(fileName = "GroundData", menuName = "Scriptable Objects/GroundData")]
public class GroundData : ScriptableObject
{
    public string name;
    public int price;
    // may not be needed
    // maybe we can add the preview later 
    // for now just use the sprite itself
    public Sprite preview;
    public Sprite sprite; // actuall
}
