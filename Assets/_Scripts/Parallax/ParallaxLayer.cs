using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background; // Sprite/mesh cua layer nen

    [Range(0f, 1f)]
    [SerializeField] private float parallaxMultiplier = 0.5f; // So cang nho thi layer di chuyen cang cham

    [SerializeField] private float loopOffset = 0.5f; // Khoang dem khi lap lai anh nen

    private float imageFullWidth; // Chieu rong day du cua anh nen
    private float imageHalfWidth; // Nua chieu rong cua anh nen

    public void CalculateImageWidth()
    {
        if (background == null)
        {
            ResetImageWidth();
            return;
        }

        Renderer renderer = background.GetComponent<Renderer>();

        if (renderer == null)
        {
            ResetImageWidth();
            Debug.LogWarning($"Parallax layer '{background.name}' needs a SpriteRenderer or MeshRenderer.", background);
            return;
        }

        imageFullWidth = renderer.bounds.size.x;
        imageHalfWidth = imageFullWidth / 2f;
    }

    public void Move(float distanceToMove)
    {
        if (background == null)
            return;

        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        if (background == null || imageFullWidth <= 0f)
            return;

        float imageRightEdge = background.position.x + imageHalfWidth - loopOffset;
        float imageLeftEdge = background.position.x - imageHalfWidth + loopOffset;

        while (imageRightEdge < cameraLeftEdge)
        {
            background.position += Vector3.right * imageFullWidth;
            imageRightEdge += imageFullWidth;
            imageLeftEdge += imageFullWidth;
        }

        while (imageLeftEdge > cameraRightEdge)
        {
            background.position -= Vector3.right * imageFullWidth;
            imageLeftEdge -= imageFullWidth;
            imageRightEdge -= imageFullWidth;
        }
    }

    private void ResetImageWidth()
    {
        imageFullWidth = 0f;
        imageHalfWidth = 0f;
    }
}
