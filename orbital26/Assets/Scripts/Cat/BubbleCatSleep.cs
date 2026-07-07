using UnityEngine;

public class BubbleCatSleep : BubbleCatInteraction
{
    protected override void TriggerBubbleAction()
    {
        TutorialManager.instance?.OnSleepCat();
        cat.SleepCat();
    }
}
