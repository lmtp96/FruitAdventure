using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_MinimapTilemapDrawer : MonoBehaviour
{
    [SerializeField] private Tilemap[] platformTilemaps;
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
        FindPlatformTilemaps();
        if (platformTilemaps == null || platformTilemaps.Length == 0)
            return;

        CalculateWorldBounds();
        foreach (Tilemap platformTilemap in platformTilemaps)
            DrawPlatformLines(platformTilemap);
    }

    private void FindPlatformTilemaps()
    {
        if (platformTilemaps != null && platformTilemaps.Length > 0)
            return;
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        int groundLayer = LayerMask.NameToLayer("Ground");
        List<Tilemap> results  = new List<Tilemap>();

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (tilemap.gameObject.layer == groundLayer)
            {
               results.Add(tilemap);
            }
        }
        platformTilemaps = results.ToArray();
    }
    private void CalculateWorldBounds()
    {
        worldMin = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        worldMax = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        foreach (Tilemap platformTilemap in platformTilemaps)
        {
            if (platformTilemap == null)
                continue;

            BoundsInt bounds = platformTilemap.cellBounds;

            Vector3 min = platformTilemap.CellToWorld(bounds.min);
            Vector3 max = platformTilemap.CellToWorld(bounds.max);

            worldMin = Vector2.Min(worldMin,new Vector2(min.x,min.y));
            worldMax = Vector2.Max(worldMax,new Vector2(max.x,max.y));
        }
    }

    private void DrawPlatformLines(Tilemap platformTilemap)
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
                    CreateLine(platformTilemap, startX, endX, y);
                    startX = int.MinValue;
                }
            }
        }
    }

    private void CreateLine(Tilemap tilemap, int startX, int endX, int y)
    {
        Vector3 startWorld = tilemap.GetCellCenterWorld(new Vector3Int(startX, y, 0));
        Vector3 endWorld = tilemap.GetCellCenterWorld(new Vector3Int(endX, y, 0));

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
}
