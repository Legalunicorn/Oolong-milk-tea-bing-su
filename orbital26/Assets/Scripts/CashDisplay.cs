using UnityEngine;
using TMPro;


// 
public class CashDisplay : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textElement;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance == null) return;
        textElement.text  = "$ " + GameManager.instance.cash;
    }
}
