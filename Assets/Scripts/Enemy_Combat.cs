using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [Header("Combat")]
    public int damage = 1;

    [Header("Attack Settings")]
    public float attackRange = 1f;

    [Header("Attack Points")]
    public Transform attackPoint;
    public Transform weaponPoint;

    [Header("Player Detection")]
    public LayerMask playerLayer;

    public void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack Point is not assigned!");
            return;
        }

        // Find players inside attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player"))
                continue;

            PlayerHealth playerHealth =
                hit.GetComponent<PlayerHealth>();

            PlayerMovement playerMovement =
                hit.GetComponent<PlayerMovement>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);

                Debug.Log("Enemy attacked player!");
            }

            // Apply knockback
            if (playerMovement != null)
            {
                playerMovement.Knockback(transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}