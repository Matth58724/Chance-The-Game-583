using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // SETTINGS
    public int maxHealth = 100;       // Maximum player health
    public int currentHealth;         // Current player health

    // UI REFERENCES
    [SerializeField] private TextMeshProUGUI healthText; // HUD health display


    void Start()
    {
        // Start at full health
        currentHealth = maxHealth;
        UpdateHealthUI();
    }


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


    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: Add death screen later
    }
}