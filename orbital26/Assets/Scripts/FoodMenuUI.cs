using UnityEngine;

public class FoodMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private CanvasGroup canvasGroup;


    // Bug fix to not pause timer
    // instead of set active false, we just make it invisible, and not interactable

    void Awake()
    {
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
        if (canvasGroup.alpha == 0) Show();
        else Hide();
        // panel.SetActive(!panel.activeSelf);
    }

    void Show()
    {
        // change from toggling SetActive
        // to let the timers keep running
        canvasGroup.alpha = 1f; 
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void Hide()
    {
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
