using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInputHandler playerInputHandler;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    [Header("Buffer & Coyote jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;


    [Header("Wall interactions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;
    private Coroutine wallJumpCoroutine;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Enemy Detection")]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;


    [SerializeField] private ParticleSystem dustFx;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;
    private bool canDoubleJump;

    private bool facingRight = true;
    private int facingDir = 1;
    private Vector2 moveInput => playerInputHandler.moveInput;

    // Lay cac component can dung cho di chuyen, animation va input
    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    // Cap nhat di chuyen va trang thai Player moi frame
    private void Update()
    {
        HandleCollision();
        UpdateAirbornStatus();
        HandleAnimations();
        if (player.CanBeControlled == false)
            return;

        if (player.IsKnocked)
            return;

        HandleEnemyDetection();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
    }

    // Kiem tra enemy ben duoi de Player co the dap len dau enemy
    private void HandleEnemyDetection()
    {
        if (rb.linearVelocity.y >= 0)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach (var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            if (newEnemy != null)
            {
                AudioManager.instance.PlaySFX(1);
                newEnemy.Die();
                Jump();
            }
        }
    }

    // Kiem tra Player vua roi khoi mat dat hay vua cham dat lai
    private void UpdateAirbornStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isGrounded && !isAirborne)
            BecomeAirborne();
    }

    // Danh dau Player dang o tren khong
    private void BecomeAirborne()
    {
        isAirborne = true;

        if (rb.linearVelocity.y < 0)
            ActivateCoyoteJump();
    }

    // Xu ly khi Player cham dat lai
    private void HandleLanding()
    {
        dustFx.Play();

        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }
    #region Buffer & Coyote Jump

    // ---------- Buffer Jump ----------

    // Luu thoi diem bam jump som khi Player dang tren khong
    private void RequestBufferJump()
    {
        if (isAirborne)
            bufferJumpActivated = Time.time;
    }

    // Neu vua cham dat trong thoi gian buffer thi tu dong jump
    private void AttemptBufferJump()
    {
        if (CanUseBufferJump() == false)
            return;

        CancelBufferJump();
        Jump();

    }

    // Kiem tra cu bam jump som con trong thoi gian cho phep khong
    private bool CanUseBufferJump()
    {
        return Time.time < bufferJumpActivated + bufferJumpWindow;
    }

    // Huy cu bam jump som da luu
    private void CancelBufferJump()
    {
        bufferJumpActivated = Time.time - 1;
    }

    // ---------- Coyote Jump ----------

    // Bat dau tinh thoi gian cho phep jump sau khi roi khoi mep dat
    private void ActivateCoyoteJump()
    {
        coyoteJumpActivated = Time.time;
    }

    // Kiem tra con duoc phep coyote jump sau khi roi khoi dat khong
    private bool CanUseCoyoteJump()
    {
        return Time.time < coyoteJumpActivated + coyoteJumpWindow;
    }

    // Huy coyote jump sau khi da dung hoac het can dung
    private void CancelCoyoteJump()
    {
        coyoteJumpActivated = Time.time - 1;
    }

    #endregion


    // Xu ly nut Jump va chon kieu nhay phu hop
    public void HandleJumpInput()
    {
        if (isGrounded || CanUseCoyoteJump())
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
        else
        {
            RequestBufferJump();
        }
        CancelCoyoteJump();
    }


    // Nhay thuong len tren, giu nguyen toc do ngang hien tai
    private void Jump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Nhay lan hai khi Player dang o tren khong
    private void DoubleJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);
        if(wallJumpCoroutine !=null)
        {
            StopCoroutine(wallJumpCoroutine);
            wallJumpCoroutine = null;
        }
        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
        anim.SetTrigger("doubleJump");
    }

    // Bat nguoc Player ra khoi tuong
    private void WallJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(12);

        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        if(wallJumpCoroutine != null)
        {
            StopCoroutine(wallJumpCoroutine);
        }
        wallJumpCoroutine = StartCoroutine(WallJumpRoutine());
    }

    // Tam khoa dieu khien ngang trong luc wall jump
    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
        wallJumpCoroutine = null;
    }

    // Lam Player truot cham hon khi dang bam vao tuong va roi xuong
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifer = moveInput.y < 0 ? 1 : .05f;

        if (canWallSlide == false)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifer);
    }

    // Kiem tra Player co cham dat hoac cham tuong khong
    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    // Cap nhat cac parameter cho Animator
    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    // Di chuyen Player theo input trai/phai
    private void HandleMovement()
    {
        if (isWallDetected)
            return;

        if (isWallJumping)
            return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    // Kiem tra huong input de lat mat Player
    private void HandleFlip()
    {
        if (moveInput.x < 0 && facingRight || moveInput.x > 0 && !facingRight)
            Flip();
    }

    // Lat Player sang huong nguoc lai va cap nhat facingDir
    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    // Ve cac duong check trong Scene de canh khoang cach raycast
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}
