using UnityEngine;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    public int maxHealth = 100;
    public int currentHealth;

    // ── UI REFERENCES ────────────────────────────────────────────
    [SerializeField] private Image healthBarFill;       // The filled portion of the HP bar
    [SerializeField] private TextMeshProUGUI healthText; // Optional HP text label

    // ── HEALTH BAR COLORS ────────────────────────────────────────
    private Color highHealthColor  = new Color(0.0f, 0.9f, 0.2f, 1f);  // Green  > 60%
    private Color midHealthColor   = new Color(1.0f, 0.7f, 0.0f, 1f);  // Orange > 30%
    private Color lowHealthColor   = new Color(0.9f, 0.1f, 0.1f, 1f);  // Red   <= 30%

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // ── PUBLIC METHODS ───────────────────────────────────────────

    // Called by EnemyShooting when player is hit
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player took " + damage + " damage. Health: " + currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void UpdateHealthUI()
    {
        float percentage = (float)currentHealth / maxHealth;

        // Update fill amount
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = percentage;

            // Change color based on health percentage
            if (percentage > 0.6f)
                healthBarFill.color = highHealthColor;
            else if (percentage > 0.3f)
                healthBarFill.color = midHealthColor;
            else
                healthBarFill.color = lowHealthColor;
        }

        // Update text label if assigned
        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: Add death screen later
    }
}