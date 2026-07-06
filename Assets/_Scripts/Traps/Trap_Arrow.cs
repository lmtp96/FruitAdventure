using UnityEngine;

public class Trap_Arrow : Trap_Trampoline
{
    [Header("Additional Info")]
    [SerializeField] private float cooldown;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private bool rotationRigh;
    private int direction = -1;
    [Space]
    [SerializeField] private float scaleUpSpeed = 10f;
    [SerializeField] private Vector3 targetScale;
    private void Start()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
    private void Update()
    {
        HandleScaleUp();
        HandleRotation();
    }

    private void HandleScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        direction = rotationRigh ? -1 : 1;
        transform.Rotate(0, 0, (rotationSpeed * direction) * Time.deltaTime);
    }

    private void DestroyMe()
    {
        GameObject arrowPrefab = ObjectCreator.instance.arrowPrefab;
        ObjectCreator.instance.CreateObject(arrowPrefab, transform, false, cooldown);

        Destroy(gameObject);
    }
}
