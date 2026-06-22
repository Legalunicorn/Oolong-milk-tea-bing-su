using UnityEngine;


// Our Room UI buttons are in Canvas Prefab
// So their OnClick cant Directly access the CameraController singleton
// So we use this script as a bridge to connect to Singleton by SCRIPT instead of directly 
// wiring in the inspector 
// hence a "Bridge" to the CamerController

// USAGE: called by UI buttons only
public class RoomButtonsBridge : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoToRoom1()
    {
        CameraController.instance.PanToRoom1();
    }

    public void GoToRoom2()
    {
        CameraController.instance.PanToRoom2();
    }
}
