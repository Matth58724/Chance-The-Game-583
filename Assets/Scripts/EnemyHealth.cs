using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    // HEALTH SETTINGS
    public int maxHealth = 100;          // Maximum health
    public int currentHealth;            // Current health

    // HIT BLINK 
    public Material hitMat;              // White flash material
    private Renderer rend;
    private Material originalMaterial;


    private Rigidbody rb;
    private Enemy enemyAI;              // Reference to AI script to disable on death

    // Engram Drops
    public GameObject engramPickupPrefab; // A physical loot crate/orb
    public EngramData engramToDrop;
    [Range(0, 100)] public float dropChance = 25f;


    void Start()
    {
        // Start at full health
        currentHealth = maxHealth;

        // Cache components
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        enemyAI = GetComponent<Enemy>();

        // Store original material for blink reset
        originalMaterial = rend.material;
    }


    // Called by Weapon.cs when raycast hits this enemy
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(Blink());
    }


    void Die()
    {
        Debug.Log(gameObject.name + " died!");

        // Disable AI script so enemy stops moving
        if (enemyAI != null)
            enemyAI.enabled = false;

        // Unfreeze rotation so enemy falls over
        rb.freezeRotation = false;

        // Tilt slightly so physics tips it over naturally
        transform.rotation = new Quaternion(
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z + 5,
            transform.rotation.w
        );

        // Destroy after 3 seconds to clean up scene
        Destroy(gameObject, 3f);

        // Engram Drop Chance
        if (Random.Range(0f, 100f) <= dropChance && engramToDrop != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f; // Spawn slightly above the enemy's feet
            GameObject drop = Instantiate(engramPickupPrefab, spawnPos, Quaternion.identity);

            // PASS THE DATA TO THE PICKUP SCRIPT
            EngramPickup pickupScript = drop.GetComponent<EngramPickup>();
            if (pickupScript != null)
            {
                pickupScript.engramData = engramToDrop;
            }
        }
    }

    IEnumerator Blink()
    {
        // Flash white material briefly on hit
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);

        // Restore original material
        rend.material = originalMaterial;
    }

}