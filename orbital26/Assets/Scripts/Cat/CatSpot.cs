using UnityEngine;

public enum CatSpotType
{
    Eat,
    Sleep
}

public class CatSpot : MonoBehaviour
{
    public CatSpotType spotType;
    public bool isOccupied = false;
}