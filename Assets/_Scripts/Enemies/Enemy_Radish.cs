using System.Collections;
using UnityEngine;

public class Enemy_Radish : Enemy
{
    [Header("Leaf Attack")]
    [SerializeField] private Enemy_Bullet leafPrefab;
    [SerializeField] private float leafSpeed = 5f;
    [SerializeField] private float leafLifeTime = 3f;
    [SerializeField] private Transform leafSpawnPoint;
    [Header("Radish details")]
    [SerializeField] private float flyForce;
    [SerializeField] private float walkDuration = 2f;

    private float xOriginalPosition;

    private float walkTimer;
    private float minFlyDistance;

    private RaycastHit2D groundBelowDetected;
    private bool isFlying;

    [Space]
    [SerializeField] private Material flashMaterial;
    private Material originalMaterial;

    protected override void Start()
    {
        base.Start();
        originalMaterial = sr.material;

        xOriginalPosition = transform.position.x;
        isFlying = true;
        minFlyDistance = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, whatIsGround).distance;
    }
    protected override void Update()
    {
        base.Update();


        if (isDead) return;

        walkTimer -= Time.deltaTime;

        if (isFlying)
        {
            HandleFlying();
        }
        else
        {
            float xDifference = Mathf.Abs(transform.position.x - xOriginalPosition);

            if (walkTimer < 0 && xDifference < .1f)
            {
                rb.gravityScale = 1f;
                isFlying = true;
            }

            HandleMovement();
            HandleTurnAround();
        }
    }

    private void HandleFlying()
    {
        if (groundBelowDetected.distance < minFlyDistance)
        {
            rb.linearVelocity = new Vector2(0, flyForce);
        }
    }

    private void HandleTurnAround()
    {
        if (isGrounded == false) { return; }
        if (!isGroundInfrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (isGrounded == false) { return; }

        if (idleTimer > 0) { return; }
        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }
    protected override void HandleAnimator()
    {
        base.HandleAnimator();

        anim.SetBool("isFlying", isFlying);
    }
    protected override void HandleCollision()
    {
        base.HandleCollision();
        groundBelowDetected = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, whatIsGround);
    }
    public override void Die()
    {
        if (isFlying)
        {
            StartCoroutine(FlashFxCo());

            SpawnLeaves();

            isFlying = false;
            walkTimer = walkDuration;
            rb.gravityScale = 3f;
        }
        else
            base.Die();
    }

    private IEnumerator FlashFxCo()
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(.1f);

        sr.material = originalMaterial;
    }

    private void SpawnLeaves()
    {
        if(leafPrefab == null) return;

        Vector3 spawnPosition = leafSpawnPoint != null ? leafSpawnPoint.position : transform.position;

        Enemy_Bullet leftLeaf = Instantiate(leafPrefab, spawnPosition , Quaternion.identity);
        leftLeaf.SetVelocity(Vector2.left * leafSpeed);
        Destroy(leftLeaf.gameObject, leafLifeTime);

        Enemy_Bullet rightLeaf = Instantiate(leafPrefab, spawnPosition , Quaternion.identity);
        rightLeaf.SetVelocity(Vector2.right * leafSpeed);
        rightLeaf.FlipSprite();
        Destroy(rightLeaf.gameObject, leafLifeTime);
    }
}
