using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Inventory")]
    // This is the inventory. It stores the blueprints of everything we've picked up
    public List<WeaponData> inventory = new List<WeaponData>();
    public Transform weaponHolder;

    [Header("Current State")]
    public Weapon currentWeapon;
    private int currentWeaponIndex = 0;
    private bool isFiring = false;
    private float nextFireTime = 0f;

    void Start()
    {
        if (inventory.Count > 0)
        {
            EquipWeapon(0);
        }
    }

    void Update()
    {
        if (isFiring && currentWeapon != null && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + currentWeapon.fireRate;
            currentWeapon.Fire();
        }
    }

    // Input System Messages

    void OnShoot() => isFiring = true;
    void OnShootRelease() => isFiring = false;

    // Placeholder for switching weapons (like using Scroll Wheel or 1/2 keys)
    void OnNextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % inventory.Count;
        EquipWeapon(nextIndex);
    }

    void OnReload()
    {
        if (currentWeapon != null) currentWeapon.Reload();
    }

    // Weapon Logic

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= inventory.Count) return;

        // We only destroy the the visual model but 
        // the actual WeaponData remains safely inside the inventory List.
        if (weaponHolder.childCount > 0)
        {
            foreach (Transform child in weaponHolder)
            {
                Destroy(child.gameObject);
            }
        }

        // Pull the data from your inventory placeholder
        WeaponData data = inventory[index];

        // Instantiate the visual model from the data blueprint
        GameObject newWepObj = Instantiate(data.modelPrefab, weaponHolder);
        newWepObj.transform.localPosition = Vector3.zero;
        newWepObj.transform.localRotation = Quaternion.identity;

        currentWeapon = newWepObj.GetComponent<Weapon>();

        if (currentWeapon != null)
        {
            // Inject the stats from the inventory data into the physical gun
            currentWeapon.weaponName = data.weaponName;
            currentWeapon.fireRate = data.fireRate;
            currentWeapon.damage = data.damage;
            currentWeapon.range = data.range;
            currentWeapon.maxAmmo = data.magSize;
        }

        currentWeaponIndex = index;
    }

    public void AddToInventory(WeaponData newLoot)
    {
        // This adds the new blueprint to your collection
        inventory.Add(newLoot);

        Debug.Log($"Picked up: {newLoot.weaponName} ({newLoot.rarity})");

        if (inventory.Count == 1) EquipWeapon(0);
    }
}