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
        cat.FeedCat();
    }
}