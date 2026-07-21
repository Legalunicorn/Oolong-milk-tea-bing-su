using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroundShopItem : MonoBehaviour
{

    // very similar to counter and wall but perhaps i want more customization with the Image
    [SerializeField] private Button button;
    [SerializeField] private Image preview;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI price;

    private int groundId;
    private int priceCache;

    public void Setup(GroundData data, int id)

    {
        // id -> ref back to game manager
        groundId = id;
        preview.sprite = data.preview;
        name.text = data.name;
        priceCache = data.price;
        SetPriceMessage();
        button.onClick.AddListener(OnClick);
    }

    public void SetPriceMessage()
    {
        if (GameManager.instance.ownedGroundIds.Contains(groundId))
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
        CafeCustomisationManager.instance?.SelectGround(groundId);
        SetPriceMessage();
    }
}
