using UnityEngine;



// Another bridge script, for buttom to call singleton methods
public class LevelManagerBridge : MonoBehaviour
{
    
    public void CancelCatSelectionBridge()
    {
        LevelManager.instance.CancelCatSelection();
    }
}
