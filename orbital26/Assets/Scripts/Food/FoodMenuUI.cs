using UnityEngine;

public class FoodMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private CanvasGroup canvasGroup;


    // Bug fix to not pause timer
    // instead of set active false, we just make it invisible, and not interactable

    public static FoodMenuUI instance;
    void Awake()
    {
        // Edit: ill make this a singleton as well, then canvas elements can be preabs with no refernece
        instance = this;
        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // panel.SetActive(false);
        Hide();
    }

    public void Toggle()
    {
        if (canvasGroup.alpha == 0f) Show();
        else Hide();
        // panel.SetActive(!panel.activeSelf);
    }

    public void Show()
    {
        // change from toggling SetActive
        // to let the timers keep running
        TutorialManager.instance?.OnFoodMenuOpened();
        canvasGroup.alpha = 1f; 
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        Debug.Log("Hide food triggered");
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // bottom two methods just to be explicit
    // public void Close()
    // {
    //     panel.SetActive(false);
    // }

    // public void Open()
    // {
    //     panel.SetActive(true);
    // }

}
