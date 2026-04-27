using UnityEngine;

public class Weapon : MonoBehaviour
{
    // ─WEAPON IDENTITY─
    public string weaponName;
    public float fireRate = 0.15f;
    public float range = 100f;
    public int damage = 10;

    // ─AMMO─
    public int maxAmmo = 30;        // Max ammo per magazine
    public int currentAmmo;         // Current ammo in magazine
    public float reloadTime = 2f;   // How long reload takes in seconds
    public bool isReloading = false; // True while reload animation is playing

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;

    void Start()
    {
        // Start with a full magazine
        currentAmmo = maxAmmo;
    }

    public bool CanFire()
    {
        // Can only fire if not reloading and has ammo
        return !isReloading && currentAmmo > 0;
    }

    public void Fire()
    {
        if (!CanFire()) return;

        // Decrement ammo
        currentAmmo--;

        // Play muzzle flash if assigned
        if (muzzleFlash != null) muzzleFlash.Play();

        // Raycast from the center of the screen
        // Viewport (0.5, 0.5) is the dead center of the players view
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Spawn hit effect if assigned
            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

            // Deal damage if target has a health component

        }

        // Auto reload if magazine is empty after firing
        if (currentAmmo <= 0)
            StartReload();
    }

    public void StartReload()
    {
        // Don't reload if already reloading or magazine is full
        if (isReloading || currentAmmo == maxAmmo) return;

        Debug.Log("Reloading " + weaponName + "...");
        isReloading = true;

        // Wait for reload time then finish reload
        Invoke(nameof(FinishReload), reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log(weaponName + " reloaded!");
    }
}