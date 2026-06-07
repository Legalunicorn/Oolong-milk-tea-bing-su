using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatShopItem : MonoBehaviour
{
    public Image catIcon;
    public TextMeshProUGUI catNameText;
    public TextMeshProUGUI catPriceText;
    public Button buyButton;

    private CatData catData;

    // kind of like constructor
    public void Setup(CatData data, ShopManager shopManager)
    {
        catData = data;
        catIcon.sprite = data.icon;
        catNameText.text = data.catName;
        catPriceText.text = ""+data.price;

        // wire the button to a call back function 
        // https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Button-onClick.html
        buyButton.onClick.AddListener(
            () => shopManager.BuyCat(catData) 
        );
    }
}
