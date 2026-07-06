using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Chicken : Enemy
{
    [Header("Chicken details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;

    private float aggroTimer;
    private bool canFlip = true;

    protected override void Update()
    {
        base.Update();

        aggroTimer -= Time.deltaTime;

        if (isDead)
            return;

        if (isGrounded && HandleTurnAround())
            return;

        if (isPlayerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if (aggroTimer < 0)
            canMove = false;

        HandleMovement();
    }

    private bool HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {
            CancelInvoke(nameof(Flip));
            canFlip = true;

            Flip();

            canMove = false;
            aggroTimer = 0;
            rb.linearVelocity = Vector2.zero;
            return true;
        }
        return false;
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        if (player == null)
            return;

        HandleFlip(player.transform.position.x);

        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), .3f);
            }
        }
    }

    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
    }
}
