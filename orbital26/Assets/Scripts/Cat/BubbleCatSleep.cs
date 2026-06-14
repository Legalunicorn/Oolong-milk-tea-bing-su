using UnityEngine;

public class BubbleCatSleep : BubbleCatInteraction
{
    protected override void TriggerBubbleAction()
    {
        cat.SleepCat();
    }
}
