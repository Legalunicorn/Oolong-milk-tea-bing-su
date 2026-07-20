using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CafeCustomisationManager : MonoBehaviour
{

    // COUNTER CUSTOMIZATION STUFF 
    [SerializeField] private SpriteRenderer counterObj;
    [SerializeField] public CounterData[] counterDatas;

    [System.Serializable]
    public class WallpaperSet
    {
        public string wallpaperName;

        [Min(0)]
        public int price;

        [Tooltip("Sprite displayed on the wallpaper shop card.")]
        public Sprite previewIcon;

        [Tooltip(
            "Use the same tile order for every wallpaper. " +
            "For example: plain, border, striped."
        )]
        public TileBase[] tiles;
    }

    [Header("Wallpaper Tilemap")]
    [SerializeField] private Tilemap wallpaperTilemap;

    [Header("Wallpaper Options")]
    [SerializeField] private WallpaperSet[] wallpaperOptions;

    [Header("Default Wallpaper")]
    [SerializeField] private int defaultWallpaperIndex = 0;

    [Header("Customisation UI")]
    [SerializeField] private TextMeshProUGUI wallFeedbackText;

    private int currentWallpaperIndex;

    private const string SelectedWallpaperKey =
        "SelectedWallpaperIndex";

    private const string OwnedWallpaperKeyPrefix =
        "OwnedWallpaper_";

    // Used by ShopManager to know how many cards to create.
    public int WallpaperCount
    {
        get
        {
            if (wallpaperOptions == null) return 0;
            return wallpaperOptions.Length;
        }
    }

    public static CafeCustomisationManager instance;
    // TODO: make a singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (!ValidateSetup()) return;

        // The scene is originally painted with the default wallpaper.
        currentWallpaperIndex = Mathf.Clamp(
            defaultWallpaperIndex,
            0,
            wallpaperOptions.Length - 1
        );

        // Green/default wallpaper is always owned.
        SetWallpaperOwned(defaultWallpaperIndex);

        int savedIndex = PlayerPrefs.GetInt(
            SelectedWallpaperKey,
            defaultWallpaperIndex
        );

        savedIndex = Mathf.Clamp(
            savedIndex,
            0,
            wallpaperOptions.Length - 1
        );

        // Do not load an unpaid wallpaper.
        if (!IsWallpaperOwned(savedIndex))
        {
            savedIndex = defaultWallpaperIndex;
        }

        ApplyWallpaper(savedIndex);
        ClearFeedback();


        ApplyCounter(GameManager.instance.equippedCounterId);
    }

    public void ApplyCounter(int id)
    {
        counterObj.sprite = counterDatas[id].sprite;
    }

    public void SelectCounter(int id)
    {
        // check if owned or not 
        CounterData data = counterDatas[id];
        bool isOwened = GameManager.instance.ownedCounterIds.Contains(id);
        if (!isOwened)
        {
            // false -> not enough to buy coutner
            if (!GameManager.instance.SpendCash(data.price)) return;
            // buy now 
            AudioManager.instance?.PlaySpendMoneySound();
            GameManager.instance.ownedCounterIds.Add(id);
        }
        

        // equip item, either bought or already owned 
        AudioManager.instance?.PlayUIClickSound();
        ApplyCounter(id);
    }


    // ShopManager uses this to obtain the information for each card.
    public WallpaperSet GetWallpaper(int wallpaperIndex)
    {
        if (!IsValidIndex(wallpaperIndex)) return null;

        return wallpaperOptions[wallpaperIndex];
    }

    // Called automatically by WallpaperShopItem.
    public void SelectWallpaper(int wallpaperIndex)
    {
        if (!IsValidIndex(wallpaperIndex))
        {
            Debug.LogWarning(
                "Invalid wallpaper index: " + wallpaperIndex
            );
            return;
        }

        WallpaperSet selectedWallpaper =
            wallpaperOptions[wallpaperIndex];

        // The wallpaper has already been purchased.
        if (IsWallpaperOwned(wallpaperIndex))
        {
            AudioManager.instance?.PlayUIClickSound();
            ApplyWallpaper(wallpaperIndex);
            SetFeedback(
                selectedWallpaper.wallpaperName +
                " wallpaper selected!"
            );

            return;
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance was not found.");
            return;
        }

        // Purchase the wallpaper for the first time.
        if (GameManager.instance.SpendCash(
                selectedWallpaper.price
            ))
        {
            SetWallpaperOwned(wallpaperIndex);
            AudioManager.instance?.PlaySpendMoneySound();
            ApplyWallpaper(wallpaperIndex);
            SetFeedback(
                selectedWallpaper.wallpaperName +
                " wallpaper purchased!"
            );
        }
        else
        {
            AudioManager.instance?.PlayUIClickSound();
            SetFeedback(
                "Not enough cash to buy " +
                selectedWallpaper.wallpaperName +
                " wallpaper :("
            );
        }
    }


    // interesting code
    private void ApplyWallpaper(int newWallpaperIndex)
    {
        if (!IsValidIndex(newWallpaperIndex)) return;

        WallpaperSet currentWallpaper =wallpaperOptions[currentWallpaperIndex];
        WallpaperSet newWallpaper = wallpaperOptions[newWallpaperIndex];

        if (currentWallpaper.tiles == null || newWallpaper.tiles == null)
        {
            Debug.LogError("A wallpaper tile list is missing.");
            return;
        }

        if (currentWallpaper.tiles.Length != newWallpaper.tiles.Length)
        {
            Debug.LogError(
                "All wallpaper sets must contain the same " +
                "number of tiles."
            );
            return;
        }

        for ( int tileIndex = 0; tileIndex < currentWallpaper.tiles.Length; tileIndex++)
        {
            TileBase oldTile = currentWallpaper.tiles[tileIndex];
            TileBase newTile = newWallpaper.tiles[tileIndex];

            if (oldTile == null || newTile == null)
            {
                Debug.LogWarning(
                    "A wallpaper tile is missing at position " +
                    tileIndex
                );
                continue;
            }

            if (oldTile != newTile)
            {
                wallpaperTilemap.SwapTile( oldTile, newTile);
            }
        }

        currentWallpaperIndex = newWallpaperIndex;

        // ? apparently using player prefs now 
        // TODO: if possible change this to game manager as source of truth 
        // so its more consistent..
        PlayerPrefs.SetInt(
            SelectedWallpaperKey,
            currentWallpaperIndex
        );

        PlayerPrefs.Save();
    }

    public bool IsWallpaperOwned(int wallpaperIndex)
    {
        if (!IsValidIndex(wallpaperIndex))
        {
            return false;
        }

        if (wallpaperIndex == defaultWallpaperIndex) return true;
        string key = OwnedWallpaperKeyPrefix + wallpaperIndex;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void SetWallpaperOwned(int wallpaperIndex)
    {
        string key =
            OwnedWallpaperKeyPrefix + wallpaperIndex;

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    private bool IsValidIndex(int wallpaperIndex)
    {
        return wallpaperOptions != null &&
               wallpaperIndex >= 0 &&
               wallpaperIndex < wallpaperOptions.Length;
    }

    private void SetFeedback(string message)
    {
        if (wallFeedbackText != null)
        {
            wallFeedbackText.text = message;
        }
    }

    public void ClearFeedback()
    {
        if (wallFeedbackText != null)
        {
            wallFeedbackText.text = "";
        }
    }

    private bool ValidateSetup()
    {
        if (wallpaperTilemap == null)
        {
            Debug.LogError("Wallpaper Tilemap has not been assigned.");
            return false;
        }

        if (wallpaperOptions == null ||
            wallpaperOptions.Length == 0)
        {
            Debug.LogError("No wallpaper options have been assigned.");
            return false;
        }

        return true;
    }

    // Called when the player starts a new game.
    public static void ResetCustomisationData()
    {
        // Green wallpaper is index 0.
        PlayerPrefs.SetInt(
            SelectedWallpaperKey,
            0
        );

        // Keep the free default wallpaper owned.
        PlayerPrefs.SetInt(
            OwnedWallpaperKeyPrefix + 0,
            1
        );

        // Remove ownership of paid wallpapers.
        // This supports up to 50 wallpaper slots.
        for (int index = 1; index < 50; index++)
        {
            PlayerPrefs.DeleteKey(
                OwnedWallpaperKeyPrefix + index
            );
        }

        PlayerPrefs.Save();
    }
}
