using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public Animator anim;

    [Header("Attack Settings")]
    public int damage = 25;
    public float attackDuration = 0.45f;
    public float attackCooldown = 0.1f;

    [Header("Attack Point")]
    public Transform attackPoint;

    [Tooltip("Radius of the area that can receive damage.")]
    public float attackRadius = 1.0f;

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

    // ==========================================
    // ATTACK
    // ==========================================

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;

        playerMovement.SetAttacking(true);

        anim.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(attackDuration);

        anim.SetBool("IsAttacking", false);

        playerMovement.SetAttacking(false);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    // ==========================================
    // DEAL DAMAGE
    // ==========================================
    // Call this from the Slash animation event
    // ==========================================
public void DealDamage()
{
    if (attackPoint == null)
        return;

    enemiesInRange = Physics2D.OverlapCircleAll(
        attackPoint.position,
        attackRadius,
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
                enemyHealth.TakeDamage(damage);
            }

            EnemyKnockback enemyKnockback =
                enemyCollider.GetComponentInParent<EnemyKnockback>();

            if (enemyKnockback != null)
            {
                Vector2 direction =
                    (enemyCollider.transform.position -
                     attackPoint.position).normalized;

                enemyKnockback.Knockback(direction);
            }
        }
    }
}

    // ==========================================
    // RED ATTACK GIZMO
    // ==========================================

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        // Filled transparent red circle
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);

        Gizmos.DrawSphere(
            attackPoint.position,
            attackRadius
        );

        // Red outline
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );

        // Small red center point
        Gizmos.DrawSphere(
            attackPoint.position,
            0.08f
        );
    }
}