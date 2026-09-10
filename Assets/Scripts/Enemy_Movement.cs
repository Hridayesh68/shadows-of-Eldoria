using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Attack")]
    public float attackRange = 1.2f;

    [Header("References")]
    public Transform player;
    public Transform frontObject;

    private Rigidbody2D rb;
    private Animator anim;

    private int facingDirection = -1;

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

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
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

    // =========================
    // IDLE
    // =========================

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);

        // If player somehow disappears
        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
        }
        else
        {
            ChangeState(EnemyState.Chasing);
        }
    }

    // =========================
    // CHASE
    // =========================

    private void Chase()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // Calculate distance to player
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        // Face player
        FacePlayer();

        // If close enough, attack
        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
            return;
        }

        // Otherwise continue chasing
        Vector2 direction =
            (player.position - transform.position).normalized;

        rb.linearVelocity = direction * speed;

        anim.SetBool("IsChasing", true);
        anim.SetBool("IsAttacking", false);
    }

    // =========================
    // ATTACK
    // =========================

    private void Attack()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // Stop moving while attacking
        rb.linearVelocity = Vector2.zero;

        // Keep facing the player
        FacePlayer();

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        // Player moved out of attack range
        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", true);
    }

    // =========================
    // FACE PLAYER
    // =========================

    private void FacePlayer()
    {
        if (player == null)
            return;

        float playerX = player.position.x;
        float enemyX = transform.position.x;

        if (playerX > enemyX)
        {
            facingDirection = 1;
        }
        else if (playerX < enemyX)
        {
            facingDirection = -1;
        }

        // IMPORTANT:
        // Flip ONLY the front object.
        // Do NOT flip the enemy root.
        if (frontObject != null)
        {
            Vector3 scale = frontObject.localScale;

            scale.x = Mathf.Abs(scale.x) * facingDirection;

            frontObject.localScale = scale;
        }
    }

    // =========================
    // PLAYER ENTERS DETECTION
    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;

            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            if (distance <= attackRange)
            {
                ChangeState(EnemyState.Attacking);
            }
            else
            {
                ChangeState(EnemyState.Chasing);
            }
        }
    }

    // =========================
    // PLAYER LEAVES DETECTION
    // =========================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform == player)
            {
                player = null;

                ChangeState(EnemyState.Idle);
            }
        }
    }

    // =========================
    // STATE CHANGE
    // =========================

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
}