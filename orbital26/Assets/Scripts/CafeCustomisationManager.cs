using UnityEngine;
using UnityEngine.Tilemaps;

public class CafeCustomizationManager : MonoBehaviour
{
    [System.Serializable]
    public class WallpaperSet
    {
        public string wallpaperName;

        // Keep the tiles in the same order for every wallpaper colour.
        public TileBase[] tiles;
    }

    [Header("Wallpaper Tilemap")]
    [SerializeField] private Tilemap wallpaperTilemap;

    [Header("Wallpaper Colour Sets")]
    [SerializeField] private WallpaperSet[] wallpaperOptions;

    [Header("Default Wallpaper")]
    [SerializeField] private int defaultWallpaperIndex = 0;

    private int currentWallpaperIndex;

    private const string WallpaperSaveKey = "SelectedWallpaperIndex";

    private void Start()
    {
        if (wallpaperTilemap == null)
        {
            Debug.LogError("Wallpaper Tilemap has not been assigned.");
            return;
        }

        if (wallpaperOptions == null || wallpaperOptions.Length == 0)
        {
            Debug.LogError("No wallpaper options have been assigned.");
            return;
        }

        currentWallpaperIndex = Mathf.Clamp(
            defaultWallpaperIndex,
            0,
            wallpaperOptions.Length - 1
        );

        int savedIndex = PlayerPrefs.GetInt(
            WallpaperSaveKey,
            currentWallpaperIndex
        );

        savedIndex = Mathf.Clamp(
            savedIndex,
            0,
            wallpaperOptions.Length - 1
        );

        ApplyWallpaper(savedIndex, false);
    }

    public void SelectWallpaper(int newWallpaperIndex)
    {
        ApplyWallpaper(newWallpaperIndex, true);
    }

    private void ApplyWallpaper(
        int newWallpaperIndex,
        bool saveSelection
    )
    {
        if (newWallpaperIndex < 0 ||
            newWallpaperIndex >= wallpaperOptions.Length)
        {
            Debug.LogWarning(
                "Invalid wallpaper index: " + newWallpaperIndex
            );
            return;
        }

        WallpaperSet currentWallpaper =
            wallpaperOptions[currentWallpaperIndex];

        WallpaperSet newWallpaper =
            wallpaperOptions[newWallpaperIndex];

        if (currentWallpaper.tiles.Length != newWallpaper.tiles.Length)
        {
            Debug.LogError(
                "Wallpaper sets do not contain the same number of tiles."
            );
            return;
        }

        for (int i = 0; i < currentWallpaper.tiles.Length; i++)
        {
            TileBase oldTile = currentWallpaper.tiles[i];
            TileBase newTile = newWallpaper.tiles[i];

            if (oldTile == null || newTile == null)
            {
                Debug.LogWarning(
                    "A wallpaper tile is missing at position " + i
                );
                continue;
            }

            wallpaperTilemap.SwapTile(oldTile, newTile);
        }

        currentWallpaperIndex = newWallpaperIndex;

        if (saveSelection)
        {
            PlayerPrefs.SetInt(
                WallpaperSaveKey,
                currentWallpaperIndex
            );

            PlayerPrefs.Save();
        }
    }
}
