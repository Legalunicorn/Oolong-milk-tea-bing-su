using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button nextButton;


    [SerializeField] private Cat tutorialCat;
    [SerializeField] private Transform CatPos;


    // Just hard code a bunch of arrows seems easiesty 
    [SerializeField] private GameObject SatisfactionBarArrow;
    [SerializeField] private GameObject TimerArrow;
    [SerializeField] private GameObject CashDisplayArrow;
    [SerializeField] private GameObject ShopmenuArrow;
    [SerializeField] private GameObject CatBubbleArrow;
    [SerializeField] private GameObject PrepareFoodArrow;
    [SerializeField] private GameObject DragToCustomerArrow;
    [SerializeField] private GameObject CustomerArrow;

    //invoke name of does not accept parameters
    // im just goung to do a lazy work around and make the parameters 
    private bool isBubble = true;

    private int currStep = 0;

    void Awake()
    {
        // singleton 
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        SatisfactionBarArrow.SetActive(false);
        TimerArrow.SetActive(false);
        CashDisplayArrow.SetActive(false);
        ShopmenuArrow.SetActive(false);
        CatBubbleArrow.SetActive(false);
        PrepareFoodArrow.SetActive(false);
        DragToCustomerArrow.SetActive(false);
        CustomerArrow.SetActive(false);
        instance = this;
    }

    void Start()
    {
        nextButton.onClick.AddListener(NextStep);
        ShowCurrentStep();
    }

    public void NextStep()
    {
        currStep++;
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        // Case spam, from step 0 to ~20 ish
        // we inplement the text based ones first and test
        switch (currStep)
        {
            case 0:
                SetMessage("Welcome to Oolong Milk Tea Bingsu! This is your cafe to manage for the next three days (levels).");
                ShowNextButton(true);
                break;
            case 1:
                SetMessage("Your goal is to keep both customers and cats happy. Lets walk through the basics.");
                ShowNextButton(true);
                break;
            case 2:
                SatisfactionBarArrow.SetActive(true);
                SetMessage("This is the satisfaction bar. If it returnes zero, you lose the level.");
                ShowNextButton(true);
                break;
            case 3:
                SatisfactionBarArrow.SetActive(false);
                TimerArrow.SetActive(true);
                SetMessage("This is the clock. Each level lasts from 8am to 5pm.");
                ShowNextButton(true);
                break;
            case 4:
                TimerArrow.SetActive(false);
                CashDisplayArrow.SetActive(true);
                SetMessage("This is your cash. Earn money from customers and use it to buy new cats");
                ShowNextButton(true);
                break;
            case 5:
                CashDisplayArrow.SetActive(false);
                SetMessage("This cat is hungry! Click and hold the bubble to feed it.");
                ShowNextButton(false);
                tutorialCat.SkipToHungry();
                SetCatArrow();
                break;
            case 6:
                CatBubbleArrow.SetActive(false);
                SetMessage("This cat is sleepy zzz... Click and hold the bubble to put it to sleep");
                ShowNextButton(false);
                tutorialCat.SkipToTired();
                Invoke(nameof(SetCatArrow), 0.5f);
                break;
            case 7:
                CatBubbleArrow.SetActive(false);
                SetMessage("Taking care of cats keep your satisfaction high. Ignoring them deduct satisfaction points");
                ShowNextButton(true);
                break;
            case 8:
                SetMessage("A customer has arrived. Let's see what they want");
                ShowNextButton(false);
                LevelManager.instance.SpawnCustomer();
                break;
            case 9:
                // FoodRequestArrow.SetActive(true);
                // InSetCustomerArrow(true);
                Invoke(nameof(SetCustomerArrow), 1.5f);
                SetMessage("Click the food request bubble to open the food menu.");
                ShowNextButton(false);
                break;
            case 10:
                PrepareFoodArrow.SetActive(true);
                CustomerArrow.SetActive(false);
                // Customer.SetActive(false);
                SetMessage("Click this button to prepare the Cake. ");
                ShowNextButton(false);
                break;
            case 11:
                PrepareFoodArrow.SetActive(false);
                Invoke(nameof(ShowDragArrow), 1f);
                // DragToCustomerArrow.SetActive(true);
                SetMessage("Drag the food icon from the Food menu onto the customer who requested it.");
                ShowNextButton(false);
                break;
            case 12:
                DragToCustomerArrow.SetActive(false);
                SetMessage("Nice work! Customers pay after receiving the right order. Now he walks to a chair to eat. ");
                ShowNextButton(false);
                break;
            case 13: 
                SetMessage("After eating, a customer may request for a cat. Now, he wants a brown cat!");
                isBubble = true;
                ShowNextButton(false);
                // ForceCustomerCatRequest();
                break;
            case 14:
                // CustomerBubbleArrow.SetActive(true);   
                SetCustomerArrow();
                SetMessage("click the customer's request bubble to enter a Cat selection mode.");
                ShowNextButton(false);
                break;
            case 15:
                CustomerArrow.SetActive(false);
                SetMessage("Find the brown cat and click the cat.");
                ShowNextButton(false);
                break;
            case 16:
                ShopmenuArrow.SetActive(true);
                SetMessage("Customers may request cats you dont own. Click the shop menu.");
                ShowNextButton(false);
                break;
            case 17: 
                ShopmenuArrow.SetActive(false);
                SetMessage("You can buy new cats here using the cash earned : )");
                ShowNextButton(true);
                break;
            case 18:
                SetMessage("That's everything you need to start! Good luck!");
                // nextButton. = "Finish";
                // TODO: chane "Next" to "Finish" button text, then skip to nextleve l
                ShowNextButton(true);
                break;
            default:
                GameManager.instance.StartNewGame();
                break;
        }
    }

    private void ShowDragArrow()
    {
        DragToCustomerArrow.SetActive(true);
    }

    private void SetCustomerArrow()
    {
        // 1. find the customer 
        CustomerController customer = FindObjectOfType<CustomerController>();
        if (customer != null)
        {
            CustomerArrow.transform.position = customer.transform.position + Vector3.right + Vector3.right;
            if (isBubble) CustomerArrow.transform.position += Vector3.up + Vector3.left;
            CustomerArrow.SetActive(true);
        }

    }

    private void SetCatArrow()
    {
        CatBubbleArrow.transform.position = CatPos.position + Vector3.up + Vector3.right + Vector3.right;
        CatBubbleArrow.SetActive(true);
    }

    private void SetMessage(string msg)
    {
        message.text = msg;
    }

    private void ShowNextButton(bool show)
    {
        nextButton.gameObject.SetActive(show);
    }

    // callback methods for completing interactions on tutorials 
    // find the scripts and functions where user interactions is taken, add the call back to them
    // harded coded the steps in whatecer keep it simple stupid i guess
    public void OnFeedCat() // added call back
    {
        if (currStep == 5) NextStep();
    }
    
    public void OnSleepCat() // added call back
    {
        if (currStep == 6) NextStep();
    }

    public void OnReachCounter()
    {
        if (currStep == 8) NextStep();
    }

    public void OnFoodMenuOpened() // added
    {
        if (currStep == 9) NextStep();
    }

    public void OnFoodCooked(string foodName) // added
    {
        // tutorial: customer only requests for cake
        if (currStep == 10 && foodName == "Cake") NextStep();
    }
    public void OnFoodServed() // aded
    {
        if (currStep == 11) NextStep();
    }

    public void OnFinishEating()
    {
        if (currStep == 12) NextStep();
    }


    public void OnCustomerStartedCatReq()
    {
        if (currStep == 13) NextStep();
    }

    public void OnCustomerCatReqClicked() //  done 
    {
        if (currStep == 14) NextStep();
    }

    public void OnCustomerCatReqMet() // done
    {
        if (currStep == 15) NextStep();
    }

    public void OnShopOpen() // added
    {
        if (currStep == 16) NextStep();
    }

}
