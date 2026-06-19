using UnityEngine;
using System; // round function
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    // TMP tutorial: https://www.youtube.com/watch?v=bR0clpZvjXo

    [SerializeField] private TextMeshProUGUI textElement;

    private string GetClockText(float totalTime, float currTime)
    {
        // 1,2,3,4,5,6,7,8,9,10
        int hoursPassed = Mathf.FloorToInt((1f-(currTime / totalTime))*9);
        int time = 8 + hoursPassed;
        if (time < 12)
        {
            return time + ":00 AM";
        }
        else
        {
            // 13 -> 1
            if (time > 12) time -= 12;
            return time + ":00 PM";
        }
    }

    void Update()
    {
        // just defensive checks, it should exist
        if (LevelManager.instance == null) return;
        float totalTime = LevelManager.instance.timeLimit;
        float timeLeft = LevelManager.instance.timeLeft;
        textElement.text = GetClockText(totalTime, timeLeft);
    }
}
