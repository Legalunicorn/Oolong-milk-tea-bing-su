using UnityEngine;

public class FoodMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panel.SetActive(false);
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }

    // bottom two methods just to be explicit
    public void Close()
    {
        panel.SetActive(false);
    }

    public void Open()
    {
        panel.SetActive(true);
    }

}
