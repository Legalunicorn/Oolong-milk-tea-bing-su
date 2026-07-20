using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterShopItem : MonoBehaviour
{

    // very simlar to wallpaper shop item but whatever, no need to abstract yet
    [SerializeField] private Button button;
    [SerializeField] private Image preview;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI price;

    private int counterId;
    private int priceCache;

    public void Setup(CounterData data, int id)

    {
        // id -> ref back to game manager
        counterId = id;
        preview.sprite = data.sprite;
        name.text = data.name;
        priceCache = data.price;
        SetPriceMessage();
        // listeener to buy button
        button.onClick.AddListener(OnClick);
    }

    public void SetPriceMessage()
    {
        if (GameManager.instance.ownedCounterIds.Contains(counterId))
        {
            price.text = "Owned";
        }
        else
        {
            price.text = "$" + priceCache;
        }
    }

    private void OnClick()
    {
        CafeCustomisationManager.instance?.SelectCounter(counterId);
        SetPriceMessage();
    }

   
}
