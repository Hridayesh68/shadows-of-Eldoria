using UnityEngine;
using TMPro;

public class StatManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    public static StatManager Instance { get; private set; }

    // =========================================================
    // MOVEMENT STATS
    // =========================================================

    [Header("Movement Stats")]
    public float movementSpeed = 5f;

    // =========================================================
    // COMBAT STATS
    // =========================================================

    [Header("Combat Stats")]
    public int attackDamage = 25;
    public float attackRange = 1f;
    public float attackCooldown = 0.1f;

    // =========================================================
    // HEALTH STATS
    // =========================================================

    [Header("Health Stats")]
    public int maxHealth = 100;

    [SerializeField]
    private int currentHealth;

    // =========================================================
    // UI REFERENCES
    // =========================================================

    [Header("UI")]
    public TMP_Text movementSpeedText;
    public TMP_Text attackDamageText;
    public TMP_Text attackRangeText;
    public TMP_Text attackCooldownText;
    public TMP_Text healthText;

    // =========================================================
    // SINGLETON SETUP
    // =========================================================

    private void Awake()
    {
        // If another StatManager already exists,
        // destroy this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep StatManager between scenes.
        DontDestroyOnLoad(gameObject);

        // Initialize health
        currentHealth = maxHealth;
    }

    private void Start()
    {
        UpdateUI();
    }

    // =========================================================
    // HEALTH
    // =========================================================

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maxHealth
        );

        UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player died.");
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maxHealth
        );

        UpdateUI();
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = value;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maxHealth
        );

        UpdateUI();
    }

    // =========================================================
    // UI
    // =========================================================

    public void UpdateUI()
    {
        if (movementSpeedText != null)
        {
            movementSpeedText.text =
                "Movement Speed: " + movementSpeed.ToString("0.0");
        }

        if (attackDamageText != null)
        {
            attackDamageText.text =
                "Damage: " + attackDamage;
        }

        if (attackRangeText != null)
        {
            attackRangeText.text =
                "Attack Range: " + attackRange.ToString("0.0");
        }

        if (attackCooldownText != null)
        {
            attackCooldownText.text =
                "Attack Cooldown: " + attackCooldown.ToString("0.00");
        }

        if (healthText != null)
        {
            healthText.text =
                "Health: " + currentHealth + " / " + maxHealth;
        }
    }
}