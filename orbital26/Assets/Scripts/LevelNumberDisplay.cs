using UnityEngine;
using TMPro;

public class LevelNumberDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LevelManager.instance.IsTutorialLevel()) levelText.text = "Tutorial";
        else levelText.text = "Level " + GameManager.instance.currLevel;
    }
}
