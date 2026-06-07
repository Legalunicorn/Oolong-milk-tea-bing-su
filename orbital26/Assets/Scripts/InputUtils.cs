using UnityEngine;
using UnityEngine.InputSystem;

// UTIL methods for inputh
// we initially used the new input system
// but it keep producing a lot of bugs with ray casting 
// like clicks were not randomly registering etc 
// so we switched to a more mannual form of input detection
// which is just measure distance of mouse click from the center
// we reuse the code in alot of files and its annoying
// so i decided to abstract the logic here instead, in this utility class
public class InputUtils : MonoBehaviour
{
    // worldpoint -> pivot position of the object 
    public static bool IsMouseInRadius(Vector2 worldPoint,float radius = 0.5f)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        return Vector2.Distance(pos,worldPoint) < radius;
    }

    public static bool IsLeftClickInRadius(Vector2 worldPoint, float radius = 0.5f)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            return IsMouseInRadius(worldPoint, radius);
        }
        else
        {
            return false;
        }
    }
}
