using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manages the SHOP UI
public class ShopManager : MonoBehaviour
{

    
    public GameObject shopPanel;
    public List<CatData> cats; 
    
    // direct UI elements inside shop panel
    public Transform itemContainer;
    public GameObject shopItemPrefab;
    public TextMeshProUGUI feedbackText;


    void Start()
    {
        shopPanel.SetActive(false);
        // Current we only have one 
        // But I design this to handle list of cats
        FillShop();

    }

    // Add cat shop items to the shop
    private void FillShop()
    {
        foreach(CatData cat in cats)
        {   
            // create shop item UI
            GameObject item = Instantiate(shopItemPrefab, itemContainer);
            CatShopItem ui = item.GetComponent<CatShopItem>();
            ui.Setup(cat,this);
        }
    }

    public void BuyCat(CatData catData)
    {
        // SpendCash returns bool (success status), True => auto spends it
        if (GameManager.instance.SpendCash(catData.price))
        {
            // play sound effect if the purchase is successful
            AudioManager.instance?.PlaySpendMoneySound();
            GameManager.instance.AddCat(catData);
            LevelManager.instance.SpawnCat(catData.prefab);
            feedbackText.text = catData.name = " purchased!";
        }
        else
        {
            feedbackText.text = "Not enough cash to buy" + catData.name + " :(";
        }
    }

    public void OpenShopUI()
    {
        // Shop and menu buttons
        // click sound effect
        AudioManager.instance?.PlayUIClickSound();
        //tutorial call back
        TutorialManager.instance?.OnShopOpen();
        

        shopPanel.SetActive(true);
        feedbackText.text = "";
    }

    public void CloseShopUI()
    {
        // click sound effect
        AudioManager.instance?.PlayUIClickSound(); 
        shopPanel.SetActive(false);
    }

}
