using System.Collections.Generic;
using UnityEngine;

public class CatSpotManager : MonoBehaviour
{
    public static CatSpotManager Instance;

    public List<CatSpot> allSpots = new List<CatSpot>();

    private void Awake()
    {
        Instance = this;
    }

    public CatSpot GetAvailableSpot(CatSpotType type, Vector3 catPosition)
    {
        Debug.Log("Searching for spot: ");
        CatSpot closestSpot = null;
        float shortestDistance = Mathf.Infinity;

        foreach (CatSpot spot in allSpots)
        {
            if (spot.spotType == type && !spot.isOccupied)
            {
                float distance = Vector3.Distance(catPosition, spot.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestSpot = spot;
                }
            }
        }
        if (closestSpot != null) Debug.Log(closestSpot.spotType + " " +  closestSpot.isOccupied);

        if (closestSpot != null)
        {
            closestSpot.isOccupied = true;
        }
        else
        {
            Debug.Log("No spot FOUND");
            foreach (CatSpot spot in allSpots)
            {
                Debug.Log(spot.spotType + " " + spot.isOccupied);
            }
        }

        return closestSpot;
    }

    public void ReleaseSpot(CatSpot spot)
    {
        if (spot != null)
        {
            spot.isOccupied = false;
        }
    }
}
