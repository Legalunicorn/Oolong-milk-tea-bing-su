using UnityEngine;
using UnityEngine.UI;

public class SatisfactionBar : MonoBehaviour
{
    // find the slider
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    
    public void SetMaxSatisfaction(int max)
    {
        slider.maxValue = max;
        slider.value = max;
        fill.color = gradient.Evaluate(1f);

    }
    
    public void DecreaseSatisfaction(int decreaseBy)
    {
        // decrease health but dont make it negative;
        //slider.value = Mathf.max(0, slider.value - decreaseBy);
        slider.value = Mathf.Max(0,slider.value - decreaseBy);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void IncreaseSatisfaction(int increaseBy)
    {
        slider.value = Mathf.Min(slider.maxValue, slider.value + increaseBy);
        fill.color = gradient.Evaluate(slider.normalizedValue);
        //slider.value = Mathf.min(slider.maxValue, slider.value + increaseBy);
    }

 

}
