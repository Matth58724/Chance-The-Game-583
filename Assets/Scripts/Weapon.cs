using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    // ── WEAPON IDENTITY ──────────────────────────────────────────
    public string weaponName;
    public float fireRate = 0.15f;
    public float range = 100f;
    public int damage = 10;

    // ── AMMO ─────────────────────────────────────────────────────
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    public bool isReloading = false;

    // ── RECOIL SETTINGS ──────────────────────────────────────────
    [Header("Recoil")]
    public float recoilUp = 2f;
    public float weaponKickBack = 0.1f;
    public float weaponKickReturn = 8f;

    // ── EFFECTS ──────────────────────────────────────────────────
    [Header("Effects")]
    public GameObject weaponFlashPrefab;  // Point light prefab to spawn on fire
    public Transform bulletSpawnPoint;    // Where flash spawns — barrel tip

    // ── PRIVATE STATE ────────────────────────────────────────────
    private Vector3 weaponOriginalPosition;
    private PlayerLook playerLook;

    // ── UNITY METHODS ────────────────────────────────────────────

void Start()
    {
        currentAmmo = maxAmmo;
        weaponOriginalPosition = transform.localPosition;

        // Find PlayerLook for camera recoil
        playerLook = FindFirstObjectByType<PlayerLook>();
        if (playerLook == null)
            Debug.LogError("PlayerLook not found!");

        // Auto-find BulletSpawnPoint child if not manually assigned
        if (bulletSpawnPoint == null)
        {
            Transform found = transform.Find("BulletSpawnPoint");
            if (found != null)
                bulletSpawnPoint = found;
            else
                Debug.LogWarning("BulletSpawnPoint not found on " + weaponName);
        }
    }

    void Update()
    {
        // Smoothly return gun model to original position after kick
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            weaponOriginalPosition,
            weaponKickReturn * Time.deltaTime
        );
    }

    // ── PUBLIC METHODS ───────────────────────────────────────────

    public bool CanFire()
    {
        // Can only fire if not reloading and has ammo
        return !isReloading && currentAmmo > 0;
    }

public void Fire()
    {
        if (!CanFire()) return;

        currentAmmo--;

        // Apply camera and weapon recoil
        ApplyRecoil();

        // Spawn muzzle flash at barrel tip
        if (weaponFlashPrefab != null && bulletSpawnPoint != null)
            Instantiate(weaponFlashPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // Raycast from center of screen for hit detection
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Deal damage if target has EnemyHealth component
            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage);
        }

        // Auto reload when empty
        if (currentAmmo <= 0)
            StartReload();
    }

    public void StartReload()
    {
        if (isReloading || currentAmmo == maxAmmo) return;

        Debug.Log("Reloading " + weaponName + "...");
        isReloading = true;
        Invoke(nameof(FinishReload), reloadTime);
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void ApplyRecoil()
    {
        // Kick gun model backward
        transform.localPosition -= Vector3.forward * weaponKickBack;

        // Add upward camera kick via PlayerLook
        if (playerLook != null)
            playerLook.AddRecoil(recoilUp);
    }

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log(weaponName + " reloaded!");
    }
}