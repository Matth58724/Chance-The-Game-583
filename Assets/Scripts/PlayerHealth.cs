using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    public int maxHealth = 100;
    public int currentHealth;

    // ── UI REFERENCES ────────────────────────────────────────────
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image damageFlash;   // Full screen red overlay

    // ── HIT EFFECTS ─────────────────────────────────────────────
    [Header("Hit Effects")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float flashDuration   = 0.15f;
    [SerializeField] private float flinchAmount    = 3f;    // Degrees of camera kick
    [SerializeField] private float flinchDuration  = 0.1f;

    // ── HEALTH BAR COLORS ────────────────────────────────────────
    private Color highHealthColor = new Color(0.0f, 0.9f, 0.2f, 1f);
    private Color midHealthColor  = new Color(1.0f, 0.7f, 0.0f, 1f);
    private Color lowHealthColor  = new Color(0.9f, 0.1f, 0.1f, 1f);

    // ── PRIVATE REFS ──────────────────────────────────────────────
    private AudioSource audioSource;
    private PlayerLook playerLook;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        playerLook = GetComponentInChildren<PlayerLook>();

        // Hide flash overlay on start
        if (damageFlash != null)
            damageFlash.color = Color.clear;
    }

    // ── PUBLIC METHODS ──────────────────────────────────────────

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthUI();
        PlayHitEffects();

        if (currentHealth <= 0)
            Die();
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void PlayHitEffects()
    {
        // Play hit sound - skip leading silence
        if (hitSound != null && audioSource != null)
        {
            audioSource.clip = hitSound;
            audioSource.time = 0.7f;
            audioSource.Play();
        }

        // Red screen flash
        if (damageFlash != null)
            StartCoroutine(FlashRoutine());

        // Camera flinch
        if (playerLook != null)
            StartCoroutine(FlinchRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Fade in red flash
        damageFlash.color = new Color(1f, 0f, 0f, 0.35f);
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.35f, 0f, elapsed / flashDuration);
            damageFlash.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
        damageFlash.color = Color.clear;
    }

    IEnumerator FlinchRoutine()
    {
        // Kick camera down then recover
        playerLook.AddRecoil(flinchAmount);
        yield return new WaitForSeconds(flinchDuration);
        playerLook.AddRecoil(-flinchAmount * 0.5f);
    }

    void UpdateHealthUI()
    {
        float percentage = (float)currentHealth / maxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = percentage;
            if (percentage > 0.6f)
                healthBarFill.color = highHealthColor;
            else if (percentage > 0.3f)
                healthBarFill.color = midHealthColor;
            else
                healthBarFill.color = lowHealthColor;
        }

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        Debug.Log("Player died!");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }
}