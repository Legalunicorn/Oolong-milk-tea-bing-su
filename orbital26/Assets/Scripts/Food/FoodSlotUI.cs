using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manage the food slot UI
// the inventory count, cook timers etc
public class FoodSlotUI : MonoBehaviour
{
    
    [SerializeField] private FoodData foodData;
    [SerializeField] private Image foodIcon;
    [SerializeField] private TextMeshProUGUI foodNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI price; // revenue
    [SerializeField] private TextMeshProUGUI foodFeedbackText;
    [SerializeField] private Button cookButton;
    [SerializeField] private Image cookFillImage;
    private bool isCooking = false;
    private float timeLeftToCook = 0f;
    // maybe we rect fill the time left to cook
    
    // idea: majk

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foodIcon.sprite = foodData.icon;
        foodNameText.text= foodData.foodName;
        //costText.text = "Cook: $" + foodData.cookCost;
        costText.text = ""+foodData.cookCost;
        timerText.text = "Cook time: " + foodData.cookTime + "s";

        // idk if timer should be static information like (5s)
        // timerText.gameObject.SetActive(false);
        cookButton.onClick.AddListener(OnCookClicked);
    }
    void OnCookClicked()
    {
        if (isCooking) return;

        // Stop locked food from cooking.
        if (GameManager.instance.currLevel < foodData.unlockLevel)
        {
            foodFeedbackText.text =
                foodData.foodName +
                " is unlocked in Level " +
                foodData.unlockLevel;

            AudioManager.instance?.PlayUIClickSound();
            return;
        }

        // Clear the previous message.
        foodFeedbackText.text = "";

        TutorialManager.instance?.OnFoodCooked(foodData.foodName);

        Debug.Log(
            "Cash bef: " +
            GameManager.instance.cash +
            " Cost: " +
            foodData.cookCost
        );

        if (!GameManager.instance.SpendCash(foodData.cookCost))
        {
            Debug.Log(
                "Not enough to cook food " +
                foodData.foodName
            );
            return;
        }

        Debug.Log(
         "Cook success: " +
            GameManager.instance.cash
        );

        StartCoroutine(Cook());
    }   
    
    //IEnumarator -> an interface 
    // let us carry an action over multiple frame ahead\
    IEnumerator Cook()
    {
        isCooking = true;
        cookButton.interactable = false;
        cookFillImage.fillAmount = 0f;
        cookFillImage.gameObject.SetActive(true);


        // TODO: add a fill rect for timer or something later on, finish functionality first 
        // TODO: for now just use a timer text
        float total = foodData.cookTime;
        float rem = foodData.cookTime;
        while(rem > 0f)
        {
            // timerText.
            rem -= Time.deltaTime;
            // adjust the full amount
            cookFillImage.fillAmount = 1f - (rem/total); 
            // skip ahead until rem is done then proceed below
            yield return null;
        }
        // costText.text = "Cook: $" + foodData.cookCost;
        costText.text =  ""+foodData.cookCost;
        cookFillImage.fillAmount =1f;
        cookFillImage.gameObject.SetActive(false);
        FoodManager.instance.IncreaseStock(foodData);
        isCooking = false;
        cookButton.interactable = true;
    }

    // Update is called once per frame
    // TODO: add a fill rect that adjust based on cook time left 

    void Update()
    {
        // updat the inventory count 
        if (FoodManager.instance == null) return;
        countText.text = "Owned: " + FoodManager.instance.GetStock(foodData);
    }
}
