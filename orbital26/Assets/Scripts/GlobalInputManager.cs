using UnityEngine;

public class GlobalInputManager : MonoBehaviour
{

    // some short keys
    // F -> toggle fridghe
    // A -> toggle rm2
    // D -> room1 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && FoodMenuUI.instance != null)
        {
            FoodMenuUI.instance.Toggle();
        }
        if (LevelManager.instance != null && LevelManager.instance.HasSecondRoom())
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (CameraController.instance != null) CameraController.instance.PanToRoom2();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (CameraController.instance != null) CameraController.instance.PanToRoom1();
            }
        }
       
    }
}
