using UnityEngine;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] private UI_MinimapTilemapDrawer tilemapDrawer;

    [SerializeField] private RectTransform mapArea;
    [SerializeField] private RectTransform playerIcon;
    [SerializeField] private RectTransform startIcon;
    [SerializeField] private RectTransform finishIcon;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform finishPoint;

    private Vector2 worldMin;
    private Vector2 worldMax;

    [SerializeField] private float finishIconYOffset = -1f;



    private void Start()
    {
        if(tilemapDrawer == null)
            tilemapDrawer = GetComponent<UI_MinimapTilemapDrawer>();

        if (tilemapDrawer !=null)
        {
            worldMin = tilemapDrawer.WorldMin;
            worldMax = tilemapDrawer.WorldMax;
        }

        if (startPoint == null)
            startPoint = FindFirstObjectByType<StartPoint>()?.transform;
        if (finishPoint == null)
            finishPoint = FindFirstObjectByType<FinishPoint>()?.transform;

        if(startPoint != null)
            SetIconPosition(startIcon, startPoint.position);

        if(finishPoint != null)
        {
            Vector3 finishPosition = finishPoint.position;
            finishPosition.y += finishIconYOffset;
            SetIconPosition(finishIcon, finishPosition);
        }

    }


    private void Update()
    {
        if (mapArea == null || playerIcon == null || startPoint == null || finishPoint == null)
            return;
        Player player = PlayerManager.instance != null ? PlayerManager.instance.player : null;

        if (player == null)
            return;

        SetIconPosition(playerIcon, player.transform.position);
    }

    private void SetIconPosition(RectTransform icon, Vector3 worldPosition)
    {
        if (icon == null || mapArea == null)
            return;

        float progressX = Mathf.InverseLerp(worldMin.x,worldMax.x, worldPosition.x);   
        float progressY = Mathf.InverseLerp(worldMin.y,worldMax.y, worldPosition.y);
        
        progressX = Mathf.Clamp01(progressX);
        progressY = Mathf.Clamp01(progressY);

        float paddingX = icon.rect.width / 2f;
        float paddingY = icon.rect.height / 2f;

        float leftX = -mapArea.rect.width / 2f + paddingX;
        float rightX = mapArea.rect.width / 2f - paddingX;

        float bottomY = -mapArea.rect.height / 2f + paddingY;
        float topY = mapArea.rect.height / 2f - paddingY;

        float x = Mathf.Lerp(leftX, rightX, progressX);
        float y = Mathf.Lerp(bottomY, topY, progressY);

        icon.anchoredPosition = new Vector2(x, y);
    }
}
