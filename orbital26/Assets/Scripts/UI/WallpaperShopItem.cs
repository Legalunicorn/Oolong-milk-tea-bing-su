using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperShopItem : MonoBehaviour
{
    [Header("Wallpaper Card UI")]
    [SerializeField] private Image wallpaperIcon;
    [SerializeField] private TextMeshProUGUI wallpaperNameText;
    [SerializeField] private TextMeshProUGUI wallpaperPriceText;
    [SerializeField] private Button buyButton;

    private int wallpaperIndex;
    private int wallpaperPrice;

    private CafeCustomisationManager cafeCustomisationManager;

    public void Setup(
        CafeCustomisationManager.WallpaperSet data,
        int index,
        CafeCustomisationManager manager)
    {
        if (data == null || manager == null)
        {
            Debug.LogError(
                "Wallpaper data or manager is missing."
            );
            return;
        }

        wallpaperIndex = index;
        wallpaperPrice = data.price;
        cafeCustomisationManager = manager;

        wallpaperIcon.sprite = data.previewIcon;
        wallpaperIcon.preserveAspect = true;

        wallpaperNameText.text = data.wallpaperName;

        // Display the price or "Owned".
        RefreshDisplay();

        buyButton.onClick.RemoveListener(
            OnWallpaperButtonClicked
        );

        buyButton.onClick.AddListener(
            OnWallpaperButtonClicked
        );
    }

    private void OnWallpaperButtonClicked()
    {
        if (cafeCustomisationManager == null)
        {
            Debug.LogError(
                "CafeCustomisationManager is missing."
            );
            return;
        }

        // Buys the wallpaper if unowned,
        // or selects it if already owned.
        cafeCustomisationManager.SelectWallpaper(
            wallpaperIndex
        );

        // Update the price text immediately.
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (cafeCustomisationManager == null)
        {
            return;
        }

        bool isOwned =
            cafeCustomisationManager.IsWallpaperOwned(
                wallpaperIndex
            );

        if (isOwned)
        {
            wallpaperPriceText.text = "Owned";
        }
        else
        {
            wallpaperPriceText.text =
                wallpaperPrice.ToString();
        }
    }
}
