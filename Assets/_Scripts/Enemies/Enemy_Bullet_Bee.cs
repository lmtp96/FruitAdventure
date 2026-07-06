using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet_Bee : MonoBehaviour
{
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string groundLayerName = "Ground";

    private Transform target;
    private List<Vector3> wayPoints = new List<Vector3>();
    private int wayIndex;

    [SerializeField] private GameObject pickupVfx;
    [SerializeField] private float wayPointUpdateCooldown;

    [Header("Explosion Shards")]
    [SerializeField] private GameObject shardAPrefab;
    [SerializeField] private GameObject shardBPrefab;
    [SerializeField] private float shardLifetime = 0.5f;

    private float speed;
    private bool isBreaking;

    public void SetupBullet(Transform newTarget, float newSpeed, float lifeDuration)
    {
        speed = newSpeed;
        target = newTarget;

        transform.up = transform.position - target.position;


        StartCoroutine(AddWayPointCo());
        StartCoroutine(LifeRoutine(lifeDuration));
    }

    private void Update()
    {
        if (wayPoints.Count <= 0)
            return;

        if (wayIndex >= wayPoints.Count)
        {
            BreakBullet();
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayIndex]) < .1f)
        {
            wayIndex++;

            if (wayIndex >= wayPoints.Count)
            {
                BreakBullet();
                return;
            }

            transform.up = transform.position - wayPoints[wayIndex];
        }
    }

    private IEnumerator LifeRoutine(float lifeDuration)
    {
        yield return new WaitForSeconds(lifeDuration);

        BreakBullet();
    }

    private IEnumerator AddWayPointCo()
    {
        while (true)
        {
            AddWayPoint();
            yield return new WaitForSeconds(wayPointUpdateCooldown);
        }
    }

    private void AddWayPoint()
    {
        if (target == null)
            return;

        foreach (Vector3 wayPoint in wayPoints)
        {
            if (wayPoint == target.position)
                return;
        }

        wayPoints.Add(target.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            collision.GetComponent<Player>()?.Knockback(transform.position.x);
            BreakBullet();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayerName))
        {
            BreakBullet();
        }
    }

    private void BreakBullet()
    {
        if (isBreaking)
            return;

        isBreaking = true;
        SpitShards();
        Destroy(gameObject);
    }

    private void SpitShards()
    {
        if (shardAPrefab == null || shardBPrefab == null)
            return;

        GameObject shardA = Instantiate(shardAPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rbA = shardA.GetComponent<Rigidbody2D>();
        if (rbA != null)
        {
            rbA.linearVelocity = new Vector2(Random.Range(-4f, -1f), Random.Range(3f, 6f));
            rbA.AddTorque(Random.Range(-300f, 300f));
        }
        Destroy(shardA, shardLifetime);

        GameObject shardB = Instantiate(shardBPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rbB = shardB.GetComponent<Rigidbody2D>();
        if (rbB != null)
        {
            rbB.linearVelocity = new Vector2(Random.Range(1f, 4f), Random.Range(3f, 6f));
            rbB.AddTorque(Random.Range(-300f, 300f));
        }
        Destroy(shardB, shardLifetime);
    }

    private void OnDestroy()
    {
        if (pickupVfx == null)
            return;

        GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
        newFx.transform.localScale = new Vector3(.6f, .6f, .6f);
    }
}
