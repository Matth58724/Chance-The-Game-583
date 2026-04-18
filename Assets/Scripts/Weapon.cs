using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float fireRate = 0.15f;
    public float range = 100f;
    public int damage = 10;
    public int maxAmmo = 30;

    [Header("Effects")]
    public ParticleSystem muzzleFlash; // Muzzle flash, maybe later
    public GameObject hitEffectPrefab; // Dust or spark effect, maybe later

    public void Fire()
    {
        // Play Muzzle Flash (might implement later)
        if (muzzleFlash != null) muzzleFlash.Play();

        // Raycast from the center of the screen
        // Viewport (0.5, 0.5) is the dead center of the players view
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Spawn a hit effect (sparks/dust) where the bullet landed
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Dealing damage to health if the target has health
        }
    }

    public void Reload()
    {
        Debug.Log("Reloading " + weaponName);
        // Future: add animation or ammo logic here
    }
}