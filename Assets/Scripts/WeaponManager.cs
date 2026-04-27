using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    // INVENTORY
    // Stores WeaponData blueprints for all collected weapons
    public List<WeaponData> inventory = new List<WeaponData>();
    public Transform weaponHolder;

    // CURRENT STATE
    public Weapon currentWeapon;
    private int currentWeaponIndex = 0;
    private bool isFiring = false;
    private float nextFireTime = 0f;

    // UNITY METHODS
    void Start()
    {
        if (inventory.Count > 0)
            EquipWeapon(0);
    }

    void Update()
    {
        // Fire if holding shoot button, weapon exists, fire rate allows it
        // and weapon is not reloading
        if (isFiring && currentWeapon != null &&
            Time.time >= nextFireTime && !currentWeapon.isReloading)
        {
            // Only fire if there is ammo
            if (currentWeapon.currentAmmo > 0)
            {
                nextFireTime = Time.time + currentWeapon.fireRate;
                currentWeapon.Fire();
            }
        }
    }

    // INPUT SYSTEM MESSAGES

    void OnShoot() => isFiring = true;
    void OnShootRelease() => isFiring = false;

    void OnNextWeapon()
    {
        // Cycle to next weapon in inventory
        int nextIndex = (currentWeaponIndex + 1) % inventory.Count;
        EquipWeapon(nextIndex);
    }

    void OnReload()
    {
        // Manually trigger reload with R key
        if (currentWeapon != null)
            currentWeapon.StartReload();
    }

    // WEAPON LOGIC

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= inventory.Count) return;

        // We only destroy the the visual model but 
        // the actual WeaponData remains safely inside the inventory List.
        if (weaponHolder.childCount > 0)
        {
            foreach (Transform child in weaponHolder)
                Destroy(child.gameObject);
        }

        // Get weapon data from inventory
        WeaponData data = inventory[index];

        // Spawn the weapon model
        GameObject newWepObj = Instantiate(data.modelPrefab, weaponHolder);
        newWepObj.transform.localPosition = Vector3.zero;
        newWepObj.transform.localRotation = Quaternion.identity;

        // Get the Weapon component and inject stats from WeaponData
        currentWeapon = newWepObj.GetComponent<Weapon>();

        if (currentWeapon != null)
        {
            currentWeapon.weaponName = data.weaponName;
            currentWeapon.fireRate = data.fireRate;
            currentWeapon.damage = data.damage;
            currentWeapon.range = data.range;
            currentWeapon.maxAmmo = data.magSize;
            currentWeapon.reloadTime = data.reloadTime;

            // Start with full ammo when equipping
            currentWeapon.currentAmmo = data.magSize;
        }

        currentWeaponIndex = index;
        Debug.Log("Equipped: " + data.weaponName);
    }

    public void AddToInventory(WeaponData newLoot)
    {
        inventory.Add(newLoot);
        Debug.Log($"Picked up: {newLoot.weaponName} ({newLoot.rarity})");

        // Auto equip if this is the first weapon
        if (inventory.Count == 1) EquipWeapon(0);
    }
}