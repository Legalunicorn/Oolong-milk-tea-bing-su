using UnityEngine;

public class CustomerTimer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private Color fullCol = Color.green;
    // [SerializeField] private Color medColor = Color.yellow;
    [SerializeField] private Color lowColor = Color.red;

    private float mxWidth;

    void Awake()
    {
        mxWidth = fill.transform.localScale.x;
    }

    public void SetProgress(float left)
    {
        left = Mathf.Clamp01(left);
        Vector3 scale = fill.transform.localScale;
        scale.x = mxWidth * left;
        fill.transform.localScale = scale;
        fill.color = Color.Lerp(lowColor,fullCol, left);
    }

}
