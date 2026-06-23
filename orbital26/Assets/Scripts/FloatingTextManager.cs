using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    // singleton to manage all the floating text stuff 

    public static FloatingTextManager instance;

    [SerializeField] private GameObject floatTextPrefab;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite heartSprite;

    void Awake()
    {
        instance = this;
    }

    public void SpawnCoinText(string msg, Vector3 pos)
    {
        GameObject o = Instantiate(floatTextPrefab, pos, Quaternion.identity);
        o.GetComponent<FloatingText>().Setup(msg, Color.yellow , coinSprite);
    }

    public void SpawnHeartText(string msg, Vector3 pos)
    {

        GameObject o = Instantiate(floatTextPrefab, pos, Quaternion.identity);
        o.GetComponent<FloatingText>().Setup(msg, Color.red, heartSprite);
    }

    // i realised its abit werid for te caller to send sprite
    // since there is only coin or heart sprite 
    // public void Spawn(string msg, Vector3 pos, Color color, Sprite sprite = null)
    // {
    //     GameObject o = Instantiate(floatTextPrefab, pos, Quaternion.identity);
    //     o.GetComponent<FloatingText>().Setup(msg, color, sprite);
    // }
    // 
}
