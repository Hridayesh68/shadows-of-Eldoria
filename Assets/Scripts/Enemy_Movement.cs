using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Player Detection")]
    public Transform detectionPoint;
    public float detectionRange = 5f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("References")]
    public Transform player;
    public SpriteRenderer spriteRenderer;
    public Animator anim;

    [Header("Knockback Reference")]
    public EnemyKnockback enemyKnockback;

    private Rigidbody2D rb;

    private float attackCooldownTimer;

    private EnemyState currentState;

    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (anim == null)
            anim = GetComponent<Animator>();

        if (enemyKnockback == null)
            enemyKnockback = GetComponent<EnemyKnockback>();

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        // ============================================
        // ATTACK COOLDOWN
        // ============================================

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        // ============================================
        // KNOCKBACK / STUN CHECK
        // ============================================

        if (enemyKnockback != null)
        {
            if (enemyKnockback.currentState ==
                EnemyKnockback.EnemyState.Knockback ||
                enemyKnockback.currentState ==
                EnemyKnockback.EnemyState.Stunned)
            {
                // DO NOT run normal enemy movement
                return;
            }
        }

        // ============================================
        // DETECT PLAYER
        // ============================================

        DetectPlayer();

        // ============================================
        // STATE MACHINE
        // ============================================

        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Chasing:
                Chase();
                break;

            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    // ============================================================
    // DETECT PLAYER
    // ============================================================

    private void DetectPlayer()
    {
        if (detectionPoint == null)
        {
            Debug.LogWarning("Detection Point not assigned!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            detectionPoint.position,
            detectionRange,
            playerLayer
        );

        if (hits.Length > 0)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    player = hit.transform;
                    return;
                }
            }
        }

        if (currentState != EnemyState.Attacking)
        {
            player = null;
        }
    }

    // ============================================================
    // IDLE
    // ============================================================

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);

        if (player != null)
        {
            ChangeState(EnemyState.Chasing);
        }
    }

    // ============================================================
    // CHASE
    // ============================================================

    private void Chase()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // ============================================
        // FACE PLAYER
        // ============================================

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }

        // ============================================
        // CHECK DISTANCE
        // ============================================

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        // ============================================
        // ATTACK
        // ============================================

        if (distance <= attackRange &&
            attackCooldownTimer <= 0f)
        {
            ChangeState(EnemyState.Attacking);

            attackCooldownTimer = attackCooldown;

            return;
        }

        // ============================================
        // CHASE
        // ============================================

        if (distance > attackRange)
        {
            Vector2 direction =
                ((Vector2)player.position -
                 (Vector2)transform.position).normalized;

            rb.linearVelocity = direction * speed;

            anim.SetBool("IsChasing", true);
            anim.SetBool("IsAttacking", false);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ============================================================
    // ATTACK
    // ============================================================

    private void Attack()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // ============================================
        // FACE PLAYER
        // ============================================

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", true);
    }

    // ============================================================
    // ATTACK ANIMATION FINISHED
    // ============================================================

    public void AttackAnimationFinished()
    {
        anim.SetBool("IsAttacking", false);

        ChangeState(EnemyState.Chasing);
    }

    // ============================================================
    // STATE CHANGE
    // ============================================================

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:

                rb.linearVelocity = Vector2.zero;

                anim.SetBool("IsChasing", false);
                anim.SetBool("IsAttacking", false);

                break;

            case EnemyState.Chasing:

                anim.SetBool("IsChasing", true);
                anim.SetBool("IsAttacking", false);

                break;

            case EnemyState.Attacking:

                rb.linearVelocity = Vector2.zero;

                anim.SetBool("IsChasing", false);
                anim.SetBool("IsAttacking", true);

                break;
        }
    }

    // ============================================================
    // GIZMOS
    // ============================================================

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(
                detectionPoint.position,
                detectionRange
            );
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}