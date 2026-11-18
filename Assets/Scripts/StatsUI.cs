using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [Header("Stats Panel")]
    public CanvasGroup statsPanel;

    [Header("Stat Slots")]
    public Transform statsSlotParent;

    private Transform[] statsSlots;

    private TMP_Text damageText;
    private TMP_Text speedText;
    private TMP_Text rangeText;
    private TMP_Text cooldownText;
    private TMP_Text healthText;

    private void Awake()
    {
        // ---------------------------------------------------------
        // Find CanvasGroup automatically
        // ---------------------------------------------------------

        if (statsPanel == null)
        {
            statsPanel = GetComponent<CanvasGroup>();
        }

        // ---------------------------------------------------------
        // Find Stat Slot Parent automatically
        // ---------------------------------------------------------

        if (statsSlotParent == null)
        {
            statsSlotParent = transform;
        }

        // ---------------------------------------------------------
        // Get all StatsSlot children
        // ---------------------------------------------------------

        statsSlots = new Transform[statsSlotParent.childCount];

        for (int i = 0; i < statsSlotParent.childCount; i++)
        {
            statsSlots[i] = statsSlotParent.GetChild(i);
        }

        // ---------------------------------------------------------
        // Find text components inside StatsSlots
        // ---------------------------------------------------------

        FindStatTexts();

        // ---------------------------------------------------------
        // Hide panel when game starts
        // ---------------------------------------------------------

        HideStatsPanel();
    }

    private void Update()
    {
        // Press Q to toggle stats panel
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleStatsPanel();
        }
    }

    // =========================================================
    // FIND STAT TEXTS
    // =========================================================

    private void FindStatTexts()
    {
        foreach (Transform slot in statsSlots)
        {
            string slotName = slot.name.ToLower();

            TMP_Text text = slot.GetComponentInChildren<TMP_Text>();

            if (text == null)
                continue;

            if (slotName.Contains("damage"))
            {
                damageText = text;
            }
            else if (slotName.Contains("speed"))
            {
                speedText = text;
            }
            else if (slotName.Contains("range"))
            {
                rangeText = text;
            }
            else if (slotName.Contains("cooldown"))
            {
                cooldownText = text;
            }
            else if (slotName.Contains("health"))
            {
                healthText = text;
            }
        }
    }

    // =========================================================
    // TOGGLE PANEL
    // =========================================================

    private void ToggleStatsPanel()
    {
        if (statsPanel == null)
            return;

        bool currentlyVisible = statsPanel.alpha > 0.5f;

        if (currentlyVisible)
        {
            HideStatsPanel();
        }
        else
        {
            ShowStatsPanel();
        }
    }

    // =========================================================
    // SHOW PANEL
    // =========================================================

    public void ShowStatsPanel()
    {
        if (statsPanel == null)
            return;

        UpdateStats();

        statsPanel.alpha = 1f;
        statsPanel.interactable = true;
        statsPanel.blocksRaycasts = true;
    }

    // =========================================================
    // HIDE PANEL
    // =========================================================

    public void HideStatsPanel()
    {
        if (statsPanel == null)
            return;

        statsPanel.alpha = 0f;
        statsPanel.interactable = false;
        statsPanel.blocksRaycasts = false;
    }

    // =========================================================
    // UPDATE ALL STATS
    // =========================================================

    public void UpdateStats()
    {
        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found!");
            return;
        }

        UpdateDamage();
        UpdateSpeed();
        UpdateRange();
        UpdateCooldown();
        UpdateHealth();
    }

    // =========================================================
    // UPDATE DAMAGE
    // =========================================================

    public void UpdateDamage()
    {
        if (damageText == null || StatManager.Instance == null)
            return;

        damageText.text =
            "Damage: " +
            StatManager.Instance.attackDamage;
    }

    // =========================================================
    // UPDATE SPEED
    // =========================================================

    public void UpdateSpeed()
    {
        if (speedText == null || StatManager.Instance == null)
            return;

        speedText.text =
            "Movement Speed: " +
            StatManager.Instance.movementSpeed.ToString("0.0");
    }

    // =========================================================
    // UPDATE RANGE
    // =========================================================

    public void UpdateRange()
    {
        if (rangeText == null || StatManager.Instance == null)
            return;

        rangeText.text =
            "Attack Range: " +
            StatManager.Instance.attackRange.ToString("0.0");
    }

    // =========================================================
    // UPDATE COOLDOWN
    // =========================================================

    public void UpdateCooldown()
    {
        if (cooldownText == null || StatManager.Instance == null)
            return;

        cooldownText.text =
            "Attack Cooldown: " +
            StatManager.Instance.attackCooldown.ToString("0.00");
    }

    // =========================================================
    // UPDATE HEALTH
    // =========================================================

    public void UpdateHealth()
    {
        if (healthText == null || StatManager.Instance == null)
            return;

        healthText.text =
            "Health: " +
            StatManager.Instance.GetCurrentHealth() +
            " / " +
            StatManager.Instance.GetMaxHealth();
    }
}