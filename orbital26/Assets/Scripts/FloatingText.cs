using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float lifespan = 1f;
    
    private float timePast = 0f;
    private Color textColor;

    public void Setup(string msg, Color color, Sprite  sprite= null)
    {
        // init the text, icon and color
        text.text = msg;
        text.color = color;
        textColor = color;
        if (sprite != null && icon != null) icon.sprite = sprite;
        else if (icon != null) icon.gameObject.SetActive(false);
    }

    void Update()
    {
        timePast += Time.deltaTime;
        transform.position += Vector3.up * speed * Time.deltaTime;
        // reduce opac of text + icon
        float alpha = 1f - (timePast  / lifespan);
        Color nxtCol = new Color(textColor.r, textColor.g, textColor.b, alpha);
        text.color = nxtCol;
        if (icon != null && icon.gameObject.activeSelf) icon.color = new Color(1,1,1,alpha);
        if (timePast >= lifespan) Destroy(gameObject);
    }
}
