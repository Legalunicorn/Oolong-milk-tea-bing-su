using UnityEngine;
public class BubbleCatFood: BubbleCatInteraction
{
    protected override void TriggerBubbleAction()
    {
        if (!GameManager.instance.SpendCash(cat.GetFeedCost()))
        {
            Debug.Log("Insufficient cash to feed cat");
            return;
        }
        //if (TutorialManager.instance != null) TutorialManager.instance.OnFeedCat();
        TutorialManager.instance?.OnFeedCat();
        cat.FeedCat();
    }
}