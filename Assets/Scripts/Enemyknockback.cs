using UnityEngine;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.15f;
    public float stunDuration = 0.5f;

    public enum EnemyState
    {
        Normal,
        Knockback,
        Stunned
    }

    public EnemyState currentState = EnemyState.Normal;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    // ============================================================
    // KNOCKBACK
    // ============================================================

    public void Knockback(Vector2 direction)
    {
        if (currentState == EnemyState.Knockback)
            return;

        StartCoroutine(KnockbackCoroutine(direction));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        // ============================================
        // ENTER KNOCKBACK
        // ============================================

        currentState = EnemyState.Knockback;

        rb.linearVelocity = Vector2.zero;

        direction.Normalize();

        rb.linearVelocity = direction * knockbackForce;

        // Let the Rigidbody move naturally
        yield return new WaitForSeconds(knockbackDuration);

        // ============================================
        // STOP KNOCKBACK
        // ============================================

        rb.linearVelocity = Vector2.zero;

        // ============================================
        // ENTER STUN
        // ============================================

        currentState = EnemyState.Stunned;

        yield return StartCoroutine(StunTimer());

        // ============================================
        // RETURN TO NORMAL
        // ============================================

        rb.linearVelocity = Vector2.zero;

        currentState = EnemyState.Normal;
    }

    // ============================================================
    // STUN TIMER
    // ============================================================

    private IEnumerator StunTimer()
    {
        float timer = stunDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            rb.linearVelocity = Vector2.zero;

            yield return null;
        }
    }
}