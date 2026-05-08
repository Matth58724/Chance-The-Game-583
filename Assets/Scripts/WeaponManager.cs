using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    // ── FULL INVENTORY ───────────────────────────────────────────
    // All weapons the player has collected
    public List<WeaponData> inventory = new List<WeaponData>();
    public WeaponData startingWeapon;

    // ── EQUIP SLOTS ─────────────────────────────────────────────
    // 3 active weapon slots the player can swap between
    public WeaponData[] equipSlots = new WeaponData[3];
    public int currentWeaponIndex = 0;
    public Transform weaponHolder;

    // ── ENGRAM INVENTORY ─────────────────────────────────────────
    [Header("Engram Inventory")]
    public List<EngramData> engramInventory = new List<EngramData>();
    public int maxEngramSlots = 5;

    // ── CURRENT STATE ───────────────────────────────────────────
    public Weapon currentWeapon;
    private bool isFiring = false;
    private float nextFireTime = 0f;
    private float scrollCooldown = 0f;

    // ── UNITY METHODS ───────────────────────────────────────────

    void Awake()
    {
        // Add starting weapon to inventory and equip slot 0
        if (startingWeapon != null && !inventory.Contains(startingWeapon))
        {
            inventory.Add(startingWeapon);
            equipSlots[0] = startingWeapon;
        }
    }

    void Start()
    {
        // Equip whatever is in slot 0
        if (equipSlots[0] != null)
            EquipWeapon(0);
    }

    void Update()
    {
        // Block input when UI is open
        if (InventoryUI.isInventoryOpen) return;
        if (DecodingTerminalUI.isTerminalOpen) return;

        // Keys 1/2/3 to switch equip slots
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        // Scroll wheel to cycle through equip slots
        scrollCooldown -= Time.deltaTime;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scrollCooldown <= 0f && Mathf.Abs(scroll) > 0.01f)
        {
            scrollCooldown = 0.2f;
            int dir = scroll > 0 ? -1 : 1;
            int next = (currentWeaponIndex + dir + 3) % 3;
            EquipWeapon(next);
        }

        // Fire
        if (isFiring && currentWeapon != null &&
            Time.time >= nextFireTime && !currentWeapon.isReloading)
        {
            if (currentWeapon.currentAmmo > 0)
            {
                nextFireTime = Time.time + currentWeapon.fireRate;
                currentWeapon.Fire();
            }
        }
    }

    // ── INPUT SYSTEM MESSAGES ─────────────────────────────────────

    void OnShoot() => isFiring = true;
    void OnShootRelease() => isFiring = false;

    void OnReload()
    {
        if (currentWeapon != null)
            currentWeapon.StartReload();
    }

    // ── PUBLIC METHODS ───────────────────────────────────────────

    // Equip the weapon in the given slot index (0-2)
    public void EquipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlots.Length) return;
        if (equipSlots[slotIndex] == null) return;

        currentWeaponIndex = slotIndex;
        WeaponData data = equipSlots[slotIndex];

        // Destroy current weapon model
        if (weaponHolder.childCount > 0)
            foreach (Transform child in weaponHolder)
                Destroy(child.gameObject);

        // Spawn new weapon model
        GameObject newWepObj = Instantiate(data.modelPrefab, weaponHolder);
        newWepObj.transform.localPosition = Vector3.zero;
        newWepObj.transform.localRotation = Quaternion.identity;

        currentWeapon = newWepObj.GetComponent<Weapon>();
        if (currentWeapon != null)
        {
            currentWeapon.weaponName  = data.weaponName;
            currentWeapon.fireRate    = data.fireRate;
            currentWeapon.damage      = data.damage;
            currentWeapon.range       = data.range;
            currentWeapon.maxAmmo     = data.magSize;
            currentWeapon.reloadTime  = data.reloadTime;
            currentWeapon.currentAmmo = data.magSize;
        }

        Debug.Log("Equipped slot " + (slotIndex + 1) + ": " + data.weaponName);
    }

    // Assign a weapon from inventory to an equip slot
    public void AssignToSlot(WeaponData data, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlots.Length) return;
        equipSlots[slotIndex] = data;

        // If this is the currently active slot re-equip it
        if (slotIndex == currentWeaponIndex)
            EquipWeapon(slotIndex);

        Debug.Log("Assigned " + data.weaponName + " to slot " + (slotIndex + 1));
    }

    // Add weapon to full inventory
    public void AddToInventory(WeaponData newLoot)
    {
        inventory.Add(newLoot);
        Debug.Log($"Picked up: {newLoot.weaponName} ({newLoot.rarity})");

        // Auto assign to first empty equip slot
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i] == null)
            {
                AssignToSlot(newLoot, i);
                break;
            }
        }
    }

    public bool AddEngram(EngramData newEngram)
    {
        if (engramInventory.Count < maxEngramSlots)
        {
            engramInventory.Add(newEngram);
            Debug.Log($"Acquired {newEngram.engramName} Engram!");
            return true;
        }
        Debug.Log("Engram inventory full!");
        return false;
    }
}