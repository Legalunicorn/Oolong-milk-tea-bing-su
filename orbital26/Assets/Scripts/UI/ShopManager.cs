using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Manages the cat adoption and cafe customisation pages.
public class ShopManager : MonoBehaviour
{
    [Header("Main Menu Panel")]
    [SerializeField] private GameObject shopPanel;

    [Header("Menu Pages")]
    [SerializeField] private GameObject adoptCatPage;
    [SerializeField] private GameObject customisationPage;

    [Header("Cat Shop")]
    [SerializeField] private List<CatData> cats;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Wallpaper Shop")]
    [SerializeField]
    private CafeCustomisationManager cafeCustomisationManager;

    [SerializeField] private Transform wallpaperItemContainer;
    [SerializeField] private GameObject wallpaperItemPrefab;

    private void Start()
    {
        // Make the cat adoption page the default page.
        SetPage(adoptCatPage);

        FillCatShop();
        FillWallpaperShop();

        shopPanel.SetActive(false);
    }

    // Creates the cat cards.
    private void FillCatShop()
    {
        if (cats == null)
        {
            Debug.LogError("Cats list is not assigned.");
            return;
        }

        if (itemContainer == null)
        {
            Debug.LogError("Cat Item Container is not assigned.");
            return;
        }

        if (shopItemPrefab == null)
        {
            Debug.LogError("Cat Shop Item Prefab is not assigned.");
            return;
        }

        foreach (CatData cat in cats)
        {
            if (cat == null)
            {
                Debug.LogWarning("A CatData entry is missing.");
                continue;
            }

            GameObject item = Instantiate(
                shopItemPrefab,
                itemContainer
            );

            CatShopItem ui = item.GetComponent<CatShopItem>();

            if (ui != null)
            {
                ui.Setup(cat, this);
            }
            else
            {
                Debug.LogError(
                    "CatShopItem component is missing from " +
                    "the cat shop item prefab."
                );
            }
        }
    }

    // Creates the wallpaper cards.
    private void FillWallpaperShop()
    {
        if (cafeCustomisationManager == null)
        {
            Debug.LogError(
                "Cafe Customisation Manager is not assigned."
            );
            return;
        }

        if (wallpaperItemContainer == null)
        {
            Debug.LogError(
                "Wallpaper Item Container is not assigned."
            );
            return;
        }

        if (wallpaperItemPrefab == null)
        {
            Debug.LogError(
                "Wallpaper Item Prefab is not assigned."
            );
            return;
        }

        for (
            int index = 0;
            index < cafeCustomisationManager.WallpaperCount;
            index++
        )
        {
            CafeCustomisationManager.WallpaperSet wallpaper =
                cafeCustomisationManager.GetWallpaper(index);

            if (wallpaper == null)
            {
                Debug.LogWarning(
                    "Wallpaper data is missing at index " + index
                );
                continue;
            }

            GameObject item = Instantiate(
                wallpaperItemPrefab,
                wallpaperItemContainer
            );

            WallpaperShopItem ui =
                item.GetComponent<WallpaperShopItem>();

            if (ui != null)
            {
                ui.Setup(
                    wallpaper,
                    index,
                    cafeCustomisationManager
                );
            }
            else
            {
                Debug.LogError(
                    "WallpaperShopItem component is missing " +
                    "from the wallpaper item prefab."
                );
            }
        }
    }

    public void BuyCat(CatData catData)
    {
        if (catData == null)
        {
            Debug.LogWarning("No CatData was provided.");
            return;
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance was not found.");
            return;
        }

        if (GameManager.instance.SpendCash(catData.price))
        {
            AudioManager.instance?.PlaySpendMoneySound();

            GameManager.instance.AddCat(catData);

            if (LevelManager.instance != null)
            {
                LevelManager.instance.SpawnCat(catData.prefab);
            }

            if (feedbackText != null)
            {
                feedbackText.text =
                    catData.catName = " purchased!";
            }
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text =
                    "Not enough cash to buy " +
                    catData.catName + " :(";
            }
        }
    }

    public void OpenShopUI()
    {
        AudioManager.instance?.PlayUIClickSound();

        TutorialManager.instance?.OnShopOpen();

        shopPanel.SetActive(true);

        // Always show the cat page first.
        SetPage(adoptCatPage);

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        cafeCustomisationManager?.ClearFeedback();
    }

    public void CloseShopUI()
    {
        AudioManager.instance?.PlayUIClickSound();

        shopPanel.SetActive(false);
    }

    public void ShowAdoptCatPage()
    {
        AudioManager.instance?.PlayUIClickSound();

        SetPage(adoptCatPage);

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    public void ShowCustomisationPage()
    {
        AudioManager.instance?.PlayUIClickSound();

        SetPage(customisationPage);

        cafeCustomisationManager?.ClearFeedback();
    }

    private void SetPage(GameObject pageToShow)
    {
        if (adoptCatPage != null)
        {
            adoptCatPage.SetActive(
                pageToShow == adoptCatPage
            );
        }

        if (customisationPage != null)
        {
            customisationPage.SetActive(
                pageToShow == customisationPage
            );
        }
    }
}
