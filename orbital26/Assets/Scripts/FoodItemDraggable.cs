using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// drag and drop mecahnic for food 
// the drag icon will match the food icon
// we can create a tempporary drag item / image
public class FoodItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private FoodData foodData;
    private Canvas canvas;
    // drag item, temporarily clone of image
    private GameObject temp;
    private Image image;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        // food icon
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // cannot drag if no inventory
        if (!FoodManager.instance.HasStock(foodData))
        {
            eventData.pointerDrag = null;
            return;
        }
        temp = new GameObject("TempFood");
        temp.transform.SetParent(canvas.transform, false);
        // TODO: add docsw
        temp.transform.SetAsLastSibling();
        Image tempImage = temp.AddComponent<Image>();
        tempImage.sprite = image.sprite;
        // disable the raycastr so it can pass through under the layer
        tempImage.raycastTarget = false;
        CanvasGroup cg = temp.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        temp.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64); //drag size
        // lower opacity
        image.color = new Color(1,1,1,0.4f);
        FollowMouse(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (temp == null) return ;
        else FollowMouse(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // reset back image opacity to 1f
        image.color = new Color(1,1,1,1f);
        if (temp != null)
        {
            Destroy(temp);
            temp = null;
        }

        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        // search for the customer pos
        Collider2D collider = Physics2D.OverlapPoint(pos);
        if (collider == null) return;
        
        CustomerController customer = collider.GetComponent<CustomerController>();
        if (customer == null) return;

        // found customer, try to search food now 
        if (customer.TryServeFood(foodData))
        {
            // check if this update the UI ? 
            FoodManager.instance.ServeFood(foodData);
        }
    }

    void FollowMouse(Vector2 pos)
    {
        // temp -> ui element in the canvas 
        // mouse pos -> in screen pixels 
        // temp.pos != mouse pos 
        // we need to convert screen pixel coord to canvas coor 
        // input: canvas rect transform, screen pos of mouse, camera, result
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            pos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, 
            out Vector2 localPoint
        );
        temp.GetComponent<RectTransform>().localPosition = localPoint;
    }
}
