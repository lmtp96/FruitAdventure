using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;
    [SerializeField] private DifficultyType gameDifficulty;
    private GameManager gameManager;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;
    private bool canBeControlled = false;
    public bool CanBeControlled => canBeControlled;
    private float defaultGravityScale;
    private bool isDead;
    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private int skinId;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;
    public bool IsKnocked => isKnocked;

    private void Awake()
    {
        cd = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        gameManager = GameManager.instance;

        UpdateGameDifficulty();
        RespawnFinished(false);
        UpdateSkin();
    }

    public void Damage()
    {
        if (isDead)
            return;

        if (gameDifficulty == DifficultyType.Normal)
        {

            if (gameManager.FruitsCollected() <= 0)
            {
                Die();
                PlayerManager.instance.RespawnPlayer();
            }
            else
            {
                ObjectCreator.instance.CreateObject(fruitDrop, transform, true);
                gameManager.RemoveFruit();
            }

            return;
        }

        if (gameDifficulty == DifficultyType.Hard)
        {
            Die();
            PlayerManager.instance.RespawnPlayer();
        }
    }



    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null)
            gameDifficulty = difficultyManager.difficulty;
    }

    public void UpdateSkin()
    {
        SkinManager skinManager = SkinManager.instance;

        if (skinManager == null)
            return;

        anim.runtimeAnimatorController = animators[skinManager.choosenSkinId];
    }



    public void RespawnFinished(bool finished)
    {

        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

            AudioManager.instance.PlaySFX(11);
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }

    }

    public void Knockback(float sourceDamageXPosition)
    {
        float knockbackDir = 1;

        if (transform.position.x < sourceDamageXPosition)
            knockbackDir = -1;

        if (isKnocked)
            return;

        AudioManager.instance.PlaySFX(9);
        CameraManager.instance.ScreenShake(knockbackDir);
        StartCoroutine(KnockbackRoutine());

        rb.linearVelocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }
    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        anim.SetBool("isKnocked", false);
    }


    public void Die()
    {
        if(isDead)
            return;

        isDead = true;

        AudioManager.instance.PlaySFX(0);
        GameObject newDeathVfx = Instantiate(deathVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCouroutine(direction, duration));
    }

    private IEnumerator PushCouroutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
    }
}
