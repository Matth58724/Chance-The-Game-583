using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    // REFERENCES
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;


    void Update()
    {
        if (weaponManager.currentWeapon == null)
        {
            ammoText.text = "-- / --";
            reloadText.gameObject.SetActive(false);
            return;
        }

        Weapon w = weaponManager.currentWeapon;

        // Show current ammo / max ammo
        ammoText.text = w.currentAmmo + " / " + w.maxAmmo;

        // Show RELOADING text when reloading
        reloadText.gameObject.SetActive(w.isReloading);
    }
}