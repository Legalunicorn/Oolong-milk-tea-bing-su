using UnityEngine;
using UnityEngine.Tilemaps;

public class CafeCustomisationManager : MonoBehaviour
{
    [Header("Wallpaper Tilemap")]
    [SerializeField] private Tilemap wallpaperTilemap;

    [Header("Wallpaper Options")]
    [Tooltip("Element 0 should be the wallpaper painted in the scene.")]
    [SerializeField] private TileBase[] wallpaperOptions;

    [Header("Default Selection")]
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

        // The scene should initially be painted using this wallpaper.
        currentWallpaperIndex = Mathf.Clamp(
            defaultWallpaperIndex,
            0,
            wallpaperOptions.Length - 1
        );

        // Load the wallpaper previously selected by the player.
        int savedWallpaperIndex = PlayerPrefs.GetInt(
            WallpaperSaveKey,
            currentWallpaperIndex
        );

        savedWallpaperIndex = Mathf.Clamp(
            savedWallpaperIndex,
            0,
            wallpaperOptions.Length - 1
        );

        ApplyWallpaper(savedWallpaperIndex, false);
    }

    public void SelectWallpaper(int wallpaperIndex)
    {
        ApplyWallpaper(wallpaperIndex, true);
    }

    private void ApplyWallpaper(int wallpaperIndex, bool saveSelection)
    {
        if (wallpaperIndex < 0 || wallpaperIndex >= wallpaperOptions.Length)
        {
            Debug.LogWarning("Invalid wallpaper index: " + wallpaperIndex);
            return;
        }

        TileBase currentTile = wallpaperOptions[currentWallpaperIndex];
        TileBase newTile = wallpaperOptions[wallpaperIndex];

        if (currentTile == null || newTile == null)
        {
            Debug.LogWarning("One of the wallpaper Tile assets is missing.");
            return;
        }

        if (currentTile != newTile)
        {
            wallpaperTilemap.SwapTile(currentTile, newTile);
        }

        currentWallpaperIndex = wallpaperIndex;

        if (saveSelection)
        {
            PlayerPrefs.SetInt(WallpaperSaveKey, currentWallpaperIndex);
            PlayerPrefs.Save();
        }
    }
}
