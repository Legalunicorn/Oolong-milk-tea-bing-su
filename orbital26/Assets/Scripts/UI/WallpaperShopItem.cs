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
    private CafeCustomisationManager cafeCustomisationManager;

    public void Setup(
        CafeCustomisationManager.WallpaperSet data,
        int index,
        CafeCustomisationManager manager)
    {
        if (data == null || manager == null)
        {
            Debug.LogError("Wallpaper data or manager is missing.");
            return;
        }

        wallpaperIndex = index;
        cafeCustomisationManager = manager;

        wallpaperIcon.sprite = data.previewIcon;
        wallpaperIcon.preserveAspect = true;

        wallpaperNameText.text = data.wallpaperName;
        wallpaperPriceText.text = data.price.ToString();

        buyButton.onClick.RemoveListener(OnWallpaperButtonClicked);
        buyButton.onClick.AddListener(OnWallpaperButtonClicked);
    }

    private void OnWallpaperButtonClicked()
    {
        cafeCustomisationManager.SelectWallpaper(wallpaperIndex);
    }
}
