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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.ChangeHealth(-damage);

                Debug.Log("Player took damage!");
            }
        }
    }

    public void Attack()
    {
        // Detect players inside attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.ChangeHealth(-damage);

                Debug.Log("Enemy attacked player!");
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