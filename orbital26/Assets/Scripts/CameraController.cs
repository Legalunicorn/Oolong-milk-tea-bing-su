using UnityEngine;


// SINGLETON 
// NOTE: Anchor points wired manually, should not be an issue when we duplicate levels
// Controllers moving the camera between anchor points
// for now we point have two rooms and two points
public class CameraController : MonoBehaviour
{


    public static CameraController instance;
    [SerializeField] private Transform room1Anchor;
    [SerializeField] private Transform room2Anchor;
    [SerializeField] private float panSpeed = 10f;

    private Vector3 targetPos;

    void Awake()
    {
        instance = this; // singleton 
        targetPos = transform.position; // or room 1 pos should work also
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            panSpeed * Time.deltaTime
        );
    }

    // just a small abstraction, maybe we might add third room
    private void PanToRoom(Transform room)
    {
        AudioManager.instance?.PlayUIClickSound();
        targetPos = new Vector3(
            room.position.x,
            room.position.y,
            transform.position.z // z remain same
        );
    }

    public void PanToRoom1()
    {
        PanToRoom(room1Anchor);
    }

    public void PanToRoom2()
    {
        PanToRoom(room2Anchor);
    }
}
