using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    public int damage = 10;              // Damage dealt per shot
    public float fireRate = 1.5f;        // Seconds between shots
    public float shootingRange = 5f;     // Max range enemy can hit player

    // ── EFFECTS ──────────────────────────────────────────────────
    public GameObject weaponFlashPrefab; // Muzzle flash prefab
    public Transform bulletSpawnPoint;   // Barrel tip of enemy weapon

    // ── PRIVATE STATE ────────────────────────────────────────────
    private float nextFireTime = 0f;
    private Enemy enemyAI;
    private Transform playerTransform;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        enemyAI = GetComponent<Enemy>();

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

        Shoot();
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        // Spawn muzzle flash at barrel tip if assigned
        if (weaponFlashPrefab != null && bulletSpawnPoint != null)
            Instantiate(weaponFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // Direction from enemy to player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Raycast toward player
        if (Physics.Raycast(transform.position, directionToPlayer,
            out RaycastHit hit, shootingRange))
        {
            PlayerHealth playerHealth = hit.transform.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Enemy shot player for " + damage + " damage!");
            }
        }
    }
}