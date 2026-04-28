using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    // SETTINGS
    public int damage = 10;              // Damage dealt per shot
    public float fireRate = 1.5f;        // Seconds between shots
    public float shootingRange = 5f;     // Max range enemy can hit player

    // PRIVATE STATE
    private float nextFireTime = 0f;     // Time when enemy can fire next
    private Enemy enemyAI;               // Reference to AI to check attack state
    private Transform playerTransform;   // Reference to player position

    void Start()
    {
        // Cache references
        enemyAI = GetComponent<Enemy>();

        // Find player by tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("Player not found in EnemyShooting!");
    }

    void Update()
    {
        // Only shoot when in attacking state
        if (enemyAI == null || enemyAI.state != Enemy.State.Attacking) return;
        if (playerTransform == null) return;

        // Check fire rate cooldown
        if (Time.time < nextFireTime) return;

        // Shoot at player
        Shoot();
    }


    void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        // Direction from enemy to player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Raycast toward player
        if (Physics.Raycast(transform.position, directionToPlayer,
            out RaycastHit hit, shootingRange))
        {
            // Check if ray hit the player
            PlayerHealth playerHealth = hit.transform.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Enemy shot player for " + damage + " damage!");
            }
        }
    }
}