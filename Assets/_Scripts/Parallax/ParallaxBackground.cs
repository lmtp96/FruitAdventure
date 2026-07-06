using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private ParallaxLayer[] backgroundLayers; // Danh sach cac layer nen dung cho parallax

    private Camera mainCamera;
    private float lastCameraPosX; // Vi tri X truoc do cua camera chinh
    private float cameraHalfWidth; // Nua chieu rong cua camera

    private void Awake()
    {
        SetupCamera();
        SetupBackgroundLayers();
    }

    private void OnEnable()
    {
        SetupCamera();
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            SetupCamera();

            if (mainCamera == null)
                return;
        }

        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        float currentCameraPosX = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPosX - lastCameraPosX;
        lastCameraPosX = currentCameraPosX;

        float cameraLeftEdge = currentCameraPosX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPosX + cameraHalfWidth;

        if (backgroundLayers == null)
            return;

        foreach (ParallaxLayer layer in backgroundLayers)
        {
            if (layer == null)
                continue;

            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    private void SetupCamera()
    {
        mainCamera = targetCamera != null ? targetCamera : Camera.main;

        if (mainCamera == null)
            return;

        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        lastCameraPosX = mainCamera.transform.position.x;
    }

    private void SetupBackgroundLayers()
    {
        if (backgroundLayers == null)
            return;

        foreach (ParallaxLayer layer in backgroundLayers)
        {
            if (layer == null)
                continue;

            layer.CalculateImageWidth();
        }
    }
}
