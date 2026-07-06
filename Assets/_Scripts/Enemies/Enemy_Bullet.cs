using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string groundLayerName = "Ground";

    [Header("Explosion Shards")]
    [SerializeField] private GameObject shardAPrefab; 
    [SerializeField] private GameObject shardBPrefab; 
    [SerializeField] private float shardLifetime = 0.5f; 
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
    }

    public void FlipSprite() => sr.flipX = !sr.flipX;
    public void SetVelocity(Vector2 velocity) => rb.linearVelocity = velocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            collision.GetComponent<Player>().Knockback(transform.position.x);
            SpitShards();
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayerName))
        {
            SpitShards();
            Destroy(gameObject);
        }
    }

    private void SpitShards()
    {
        if (shardAPrefab != null && shardBPrefab != null)
        {
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
    }
}
