using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UI_MinimapTilemapDrawer : MonoBehaviour
{
    [SerializeField] private Tilemap platformTilemap;
    [SerializeField] private RectTransform mapArea;
    [SerializeField] private RectTransform lineParent;
    [SerializeField] private Image linePrefab;

    private Vector2 worldMin;
    private Vector2 worldMax;
    public Vector2 WorldMin => worldMin;
    public Vector2 WorldMax => worldMax;

    [SerializeField] private float lineHeight = 3f;
    [SerializeField] private float lineExtraLength = 8f;

    private void Awake()
    {
        FindPlatformTilemap();
        if (platformTilemap == null)
            return;

        CalculateWorldBounds();
        DrawPlatformLines();
    }

    private void DrawPlatformLines()
    {
        if (platformTilemap == null || mapArea == null || lineParent == null || linePrefab == null)
            return;

        BoundsInt bounds = platformTilemap.cellBounds;

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            int startX = int.MinValue;

            for(int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                Vector3Int cellAbove = new Vector3Int(x, y + 1, 0);

                bool hasTile = platformTilemap.HasTile(cell);
                bool hasTileAbove = platformTilemap.HasTile(cellAbove);
                bool isTopPlatform = hasTile && !hasTileAbove;

                if (isTopPlatform && startX == int.MinValue)
                    startX = x;

                bool endOfSegment = (!isTopPlatform || x == bounds.xMax - 1) && startX != int.MinValue;

                if(endOfSegment)
                {
                    int endX = isTopPlatform && x == bounds.xMax - 1 ? x : x - 1;
                    CreateLine(startX, endX, y);
                    startX = int.MinValue;
                }
            }
        }
    }

    private void CreateLine(int startX, int endX, int y)
    {
        Vector3 startWorld = platformTilemap.GetCellCenterWorld(new Vector3Int(startX, y, 0));
        Vector3 endWorld = platformTilemap.GetCellCenterWorld(new Vector3Int(endX, y, 0));

        Vector2 startUI = WorldToMapPosition(startWorld);
        Vector2 endUI = WorldToMapPosition(endWorld);

        Image newLine = Instantiate(linePrefab, lineParent);
        RectTransform lineRect = newLine.rectTransform;

        float length = Mathf.Abs(endUI.x - startUI.x) + lineExtraLength;

        lineRect.anchoredPosition = new Vector2((startUI.x + endUI.x) / 2f, startUI.y);

        lineRect.sizeDelta = new Vector2(length, lineHeight);
    }

    private Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        float progressX = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPosition.x);
        float progressY = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPosition.y);

        progressX = Mathf.Clamp01(progressX);
        progressY = Mathf.Clamp01(progressY);

        float x = Mathf.Lerp(-mapArea.rect.width / 2f, mapArea.rect.width / 2f, progressX);
        float y = Mathf.Lerp(-mapArea.rect.height / 2f, mapArea.rect.height / 2f, progressY);

        return new Vector2(x, y);
    }

    private void CalculateWorldBounds()
    {
        BoundsInt cellBounds = platformTilemap.cellBounds;

        Vector3 minWorld = platformTilemap.CellToWorld
            (new Vector3Int(cellBounds.xMin, cellBounds.yMin, 0));
        Vector3 maxWorld = platformTilemap.CellToWorld
            (new Vector3Int(cellBounds.xMax, cellBounds.yMax, 0));

        worldMin = minWorld;
        worldMax = maxWorld;
    }

    private void FindPlatformTilemap()
    {
        if (platformTilemap != null)
            return;
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

        foreach(Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.name == "Ground")
            {
                platformTilemap = tilemap;
                return;
            }
        }
    }
}
