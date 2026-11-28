using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // =========================================================
    // CHANGE HEALTH
    // Positive value = Heal
    // Negative value = Damage
    // =========================================================

    public void ChangeHealth(int amount)
    {
        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found!");
            return;
        }

        if (amount < 0)
        {
            // Convert negative value to positive damage
            StatManager.Instance.TakeDamage(-amount);
        }
        else if (amount > 0)
        {
            // Heal
            StatManager.Instance.Heal(amount);
        }

        Debug.Log(
            "Player Health: " +
            StatManager.Instance.GetCurrentHealth() +
            " / " +
            StatManager.Instance.GetMaxHealth()
        );

        // Check death
        if (StatManager.Instance.GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    // =========================================================
    // TAKE DAMAGE
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found!");
            return;
        }

        StatManager.Instance.TakeDamage(damage);

        Debug.Log(
            "Player Health: " +
            StatManager.Instance.GetCurrentHealth() +
            " / " +
            StatManager.Instance.GetMaxHealth()
        );

        if (StatManager.Instance.GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    // =========================================================
    // HEAL
    // =========================================================

    public void Heal(int amount)
    {
        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found!");
            return;
        }

        StatManager.Instance.Heal(amount);

        Debug.Log(
            "Player Health: " +
            StatManager.Instance.GetCurrentHealth() +
            " / " +
            StatManager.Instance.GetMaxHealth()
        );
    }

    // =========================================================
    // DIE
    // =========================================================

    private void Die()
    {
        Debug.Log("Player Died!");

        // Add death animation here later
        // anim.SetTrigger("Die");
    }
}