using UnityEngine;
using UnityEngine.InputSystem;


// Referncing a YT tutorial
// https://www.youtube.com/watch?v=BCYJ4UNClhk
// this method is webgl friendly which is better for our project 
// the alternative to just set the image directly in proejct settings might not work for us
public class CursorUI : MonoBehaviour
{

    // Singleton as well -> used by level manager to swtich cursor styles
    public static CursorUI instance {get; private set;}

    [SerializeField] private InputActionReference pointerPosAction;
    private RectTransform cursorTransform;
    private RectTransform canvasRectTransform;
    private Canvas parentCanvas;
    private Camera canvasCam;

    [SerializeField] private GameObject defaultCursor;
    [SerializeField] private GameObject selectionCursor;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        cursorTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            canvasRectTransform = parentCanvas.GetComponent<RectTransform>();
            canvasCam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay? null: parentCanvas.worldCamera;
        }
        ShowDefaultCursor();
    }
    private void OnEnable()
    {
        Cursor.visible = false;
        pointerPosAction.action.performed += OnPointerPositionChanged;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        pointerPosAction.action.performed -= OnPointerPositionChanged;
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext cb)
    {
        if (cursorTransform == null || canvasRectTransform == null ) return;
        var mousePosition = cb.ReadValue<Vector2>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, mousePosition, canvasCam, out var localPoint
        ))
        {
            cursorTransform.anchoredPosition = localPoint;
        }
    }

    // the more extensible method, incase we add more cursor desigsn
    // then we can just edit thisjh
    private void SetEveryCursorInactive()
    {
        defaultCursor.SetActive(false);
        selectionCursor.SetActive(false);
    }

    public void ShowDefaultCursor()
    {
        SetEveryCursorInactive();
        defaultCursor.SetActive(true);
    }

    public void ShowSelectionCursor()
    {
        SetEveryCursorInactive();
        selectionCursor.SetActive(true);
    }
}
