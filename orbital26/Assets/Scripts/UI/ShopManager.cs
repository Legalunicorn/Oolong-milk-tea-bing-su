using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Manages the adoption shop and cafe customization menu
public class ShopManager : MonoBehaviour
{
    [Header("Main Menu Panel")]
    [SerializeField] private GameObject shopPanel;

    [Header("Menu Pages")]
    [SerializeField] private GameObject adoptCatPage;
    [SerializeField] private GameObject customizationPage;

    [Header("Cat Shop")]
    [SerializeField] private List<CatData> cats;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private void Start()
    {
        // Prepare the default page before hiding the full menu.
        SetPage(adoptCatPage);

        shopPanel.SetActive(false);

        FillShop();
    }

    // Create all cat shop items.
    private void FillShop()
    {
        foreach (CatData cat in cats)
        {
            GameObject item = Instantiate(shopItemPrefab, itemContainer);

            CatShopItem ui = item.GetComponent<CatShopItem>();

            if (ui != null)
            {
                ui.Setup(cat, this);
            }
            else
            {
                Debug.LogError(
                    "CatShopItem component is missing from the shop item prefab."
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

        // SpendCash returns true when the purchase is successful.
        if (GameManager.instance.SpendCash(catData.price))
        {
            AudioManager.instance?.PlaySpendMoneySound();

            GameManager.instance.AddCat(catData);
            LevelManager.instance.SpawnCat(catData.prefab);

            feedbackText.text = catData.name = " purchased!";
        }
        else
        {
            feedbackText.text =
                "Not enough cash to buy " + catData.name + " :(";
        }
    }

    public void OpenShopUI()
    {
        AudioManager.instance?.PlayUIClickSound();

        TutorialManager.instance?.OnShopOpen();

        shopPanel.SetActive(true);

        // Always open the cat adoption page first.
        SetPage(adoptCatPage);

        feedbackText.text = "";
    }

    public void CloseShopUI()
    {
        AudioManager.instance?.PlayUIClickSound();

        shopPanel.SetActive(false);
    }

    // Called by the Adopt Cat tab button.
    public void ShowAdoptCatPage()
    {
        AudioManager.instance?.PlayUIClickSound();

        SetPage(adoptCatPage);
    }

    // Called by the Cafe Customization tab button.
    public void ShowCustomizationPage()
    {
        AudioManager.instance?.PlayUIClickSound();

        SetPage(customizationPage);
    }

    private void SetPage(GameObject pageToShow)
    {
        adoptCatPage.SetActive(pageToShow == adoptCatPage);
        customizationPage.SetActive(pageToShow == customizationPage);
    }
}
