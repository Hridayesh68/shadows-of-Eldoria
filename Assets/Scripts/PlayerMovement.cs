using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer playerSprite;

    [Header("Facing")]
    public int facingDirection = 1;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.15f;
    public float stunDuration = 0.5f;

    // ==========================================
    // STATE
    // ==========================================

    public bool IsAttacking { get; private set; }

    private bool isKnockback;
    private bool isStunned;

    private float stunTimer;

    private Vector2 movementInput;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // ==========================================
        // STUN TIMER
        // ==========================================

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
        }

        // ==========================================
        // DON'T ACCEPT MOVEMENT INPUT
        // ==========================================

        if (isKnockback || isStunned || IsAttacking)
        {
            movementInput = Vector2.zero;

            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 0f);

            return;
        }

        // ==========================================
        // INPUT
        // ==========================================

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(horizontal, vertical).normalized;

        // ==========================================
        // FLIP
        // ==========================================

        if (horizontal > 0)
        {
            FaceRight();
        }
        else if (horizontal < 0)
        {
            FaceLeft();
        }

        // ==========================================
        // WALKING ANIMATION
        // ==========================================

        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
    }

    private void FixedUpdate()
    {
        // ==========================================
        // KNOCKBACK
        // ==========================================

        if (isKnockback)
        {
            // IMPORTANT:
            // Do NOT set velocity to zero here.
            // We want the knockback velocity to continue.
            return;
        }

        // ==========================================
        // STUN / ATTACK
        // ==========================================

        if (isStunned || IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ==========================================
        // MOVE
        // ==========================================

        rb.linearVelocity = movementInput * speed;
    }

    // ==========================================
    // ATTACK STATE
    // ==========================================

    public void SetAttacking(bool attacking)
    {
        IsAttacking = attacking;

        if (attacking)
        {
            movementInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ==========================================
    // FACE RIGHT
    // ==========================================

    private void FaceRight()
    {
        facingDirection = 1;

        if (playerSprite != null)
        {
            playerSprite.flipX = false;
        }
    }

    // ==========================================
    // FACE LEFT
    // ==========================================

    private void FaceLeft()
    {
        facingDirection = -1;

        if (playerSprite != null)
        {
            playerSprite.flipX = true;
        }
    }

    // ==========================================
    // KNOCKBACK
    // ==========================================

    public void Knockback(Transform enemy)
    {
        if (enemy == null || isKnockback)
            return;

        StartCoroutine(KnockbackCoroutine(enemy));
    }

    private IEnumerator KnockbackCoroutine(Transform enemy)
    {
        isKnockback = true;
        isStunned = true;

        // Stop current movement
        rb.linearVelocity = Vector2.zero;

        // ==========================================
        // CALCULATE DIRECTION
        // ==========================================

        Vector2 direction =
            ((Vector2)transform.position -
             (Vector2)enemy.position).normalized;

        // ==========================================
        // APPLY KNOCKBACK
        // ==========================================

        rb.linearVelocity = direction * knockbackForce;

        // ==========================================
        // WAIT FOR KNOCKBACK
        // ==========================================

        yield return new WaitForSeconds(knockbackDuration);

        // Stop knockback
        rb.linearVelocity = Vector2.zero;

        isKnockback = false;

        // ==========================================
        // STUN
        // ==========================================

        stunTimer = stunDuration;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }
}