using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Detection")]
    public Transform detectionPoint;
    public float detectionRange = 5f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    private float attackCooldownTimer;

    [Header("References")]
    public Transform player;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator anim;

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
        anim = GetComponent<Animator>();

        attackCooldownTimer = 0f;

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        // Decrease attack cooldown every frame
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        // Check for player every single frame
        DetectPlayer();

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

    // =====================================================
    // DETECT PLAYER
    // =====================================================

    private void DetectPlayer()
    {
        if (detectionPoint == null)
        {
            Debug.LogWarning("Detection Point is not assigned!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            detectionPoint.position,
            detectionRange,
            playerLayer
        );

        if (hits.Length > 0)
        {
            // Take the first player detected
            player = hits[0].transform;

            // If we were idle, start chasing
            if (currentState == EnemyState.Idle)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            // No player detected
            player = null;

            if (currentState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Idle);
            }
        }
    }

    // =====================================================
    // IDLE
    // =====================================================

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);
    }

    // =====================================================
    // CHASE
    // =====================================================

    private void Chase()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // -----------------------------------------
        // FLIP TO FACE PLAYER
        // -----------------------------------------

        if (player.position.x > transform.position.x)
        {
            // Player is on the right
            spriteRenderer.flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            // Player is on the left
            spriteRenderer.flipX = true;
        }

        // -----------------------------------------
        // DISTANCE CHECK
        // -----------------------------------------

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        // Player is inside attack range
        if (distance <= attackRange)
        {
            if (attackCooldownTimer <= 0f)
            {
                ChangeState(EnemyState.Attacking);

                // Start cooldown
                attackCooldownTimer = attackCooldown;

                return;
            }
        }

        // -----------------------------------------
        // MOVE TOWARDS PLAYER
        // -----------------------------------------

        Vector2 direction =
            (player.position - transform.position).normalized;

        rb.linearVelocity = direction * speed;

        anim.SetBool("IsChasing", true);
        anim.SetBool("IsAttacking", false);
    }

    // =====================================================
    // ATTACK
    // =====================================================

    private void Attack()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // Stop moving
        rb.linearVelocity = Vector2.zero;

        // -----------------------------------------
        // KEEP FACING PLAYER
        // -----------------------------------------

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }

        // -----------------------------------------
        // CHECK IF PLAYER LEFT ATTACK RANGE
        // -----------------------------------------

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }

        // Attack animation
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", true);
    }

    // =====================================================
    // ATTACK ANIMATION FINISHED
    // =====================================================

    // Add this as an Animation Event at the END
    // of your attack animation.

    public void AttackAnimationFinished()
    {
        anim.SetBool("IsAttacking", false);

        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chasing);
        }
        else
        {
            ChangeState(EnemyState.Chasing);
        }
    }

    // =====================================================
    // STATE CHANGE
    // =====================================================

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

    // =====================================================
    // DEBUG DETECTION RANGE
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null)
            return;

        Gizmos.DrawWireSphere(
            detectionPoint.position,
            detectionRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}