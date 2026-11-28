using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public Animator anim;

    [Header("Attack Point")]
    public Transform attackPoint;

    [Header("Enemy Detection")]
    public LayerMask enemyLayer;

    private Collider2D[] enemiesInRange;

    private bool canAttack = true;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Left mouse button
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    // =========================================================
    // ATTACK
    // =========================================================

    private IEnumerator AttackCoroutine()
    {
        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found!");
            yield break;
        }

        canAttack = false;

        playerMovement.SetAttacking(true);

        anim.SetBool("IsAttacking", true);

        // Use attack duration from animation timing.
        yield return new WaitForSeconds(0.45f);

        anim.SetBool("IsAttacking", false);

        playerMovement.SetAttacking(false);

        // Use cooldown from StatManager
        yield return new WaitForSeconds(
            StatManager.Instance.attackCooldown
        );

        canAttack = true;
    }

    // =========================================================
    // DEAL DAMAGE
    // =========================================================
    // Call this using an Animation Event
    // on the hit frame of the Slash animation.
    // =========================================================

    public void DealDamage()
    {
        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found!");
            return;
        }

        if (attackPoint == null)
        {
            Debug.LogWarning("Attack Point is not assigned!");
            return;
        }

        int damage = StatManager.Instance.attackDamage;
        float attackRange = StatManager.Instance.attackRange;

        enemiesInRange = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        if (enemiesInRange.Length > 0)
        {
            foreach (Collider2D enemyCollider in enemiesInRange)
            {
                EnemyHealth enemyHealth =
                    enemyCollider.GetComponentInParent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    // Deal damage
                    enemyHealth.TakeDamage(damage);
                }

                // Enemy knockback
                EnemyKnockback enemyKnockback =
                    enemyCollider.GetComponentInParent<EnemyKnockback>();

                if (enemyKnockback != null)
                {
                    Vector2 direction =
                        ((Vector2)enemyCollider.transform.position -
                         (Vector2)attackPoint.position).normalized;

                    enemyKnockback.Knockback(direction);
                }
            }
        }
    }

    // =========================================================
    // ATTACK GIZMO
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        float radius = 1f;

        if (StatManager.Instance != null)
        {
            radius = StatManager.Instance.attackRange;
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);

        Gizmos.DrawSphere(
            attackPoint.position,
            radius
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            radius
        );

        Gizmos.DrawSphere(
            attackPoint.position,
            0.08f
        );
    }
}