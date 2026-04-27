using UnityEngine;

public class Weapon : MonoBehaviour
{
    // WEAPON IDENTITY
    public string weaponName;
    public float fireRate = 0.15f;
    public float range = 100f;
    public int damage = 10;

    // AMMO
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    public bool isReloading = false;

    // RECOIL SETTINGS
    [Header("Recoil")]
    public float recoilUp = 2f;        // How much camera kicks upward
    public float recoilSide = 0.5f;    // How much camera kicks sideways
    public float recoilReturn = 5f;    // How fast camera returns to original position
    public float weaponKickBack = 0.1f; // How far gun model kicks back
    public float weaponKickReturn = 8f; // How fast gun model returns

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;

    private Vector3 weaponOriginalPosition;
    private PlayerLook playerLook;


    void Start()
{
    currentAmmo = maxAmmo;
    weaponOriginalPosition = transform.localPosition;

    // Find RecoilController directly in the scene instead of via Camera.main
    playerLook = FindFirstObjectByType<PlayerLook>();

    if (playerLook == null)
        Debug.LogError("PlayerLook not found in scene!");
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

    // PUBLIC METHODS

    public bool CanFire()
    {
        return !isReloading && currentAmmo > 0;
    }

    public void Fire()
    {
        if (!CanFire()) return;

        currentAmmo--;

        // Apply recoil
        ApplyRecoil();

        // Play muzzle flash
        if (muzzleFlash != null) muzzleFlash.Play();

        // Raycast from center of screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
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

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log(weaponName + " reloaded!");
    }

    // PRIVATE METHODS

    void ApplyRecoil()
{
    // Kick gun model backward
    transform.localPosition -= Vector3.forward * weaponKickBack;

    // Add recoil to the camera via PlayerLook
    if (playerLook != null)
        playerLook.AddRecoil(recoilUp);
}
}