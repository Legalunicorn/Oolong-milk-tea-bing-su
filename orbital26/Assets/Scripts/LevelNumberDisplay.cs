using UnityEngine;
using TMPro;

public class LevelNumberDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelText.text = "Level " + GameManager.instance.currLevel;
    }

}
