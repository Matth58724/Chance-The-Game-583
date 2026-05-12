using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    // ── INVENTORY ENTRY ──────────────────────────────────────────
    // Wraps WeaponData with a unique runtime ID so duplicates work
    public class WeaponEntry
    {
        public WeaponData data;
        public int uid;        // Unique per pickup, not per asset
        public int savedAmmo;

        public WeaponEntry(WeaponData data, int uid)
        {
            this.data     = data;
            this.uid      = uid;
            this.savedAmmo = data.magSize;
        }
    }

    // ── FULL INVENTORY ───────────────────────────────────────────
    public List<WeaponEntry> inventory = new List<WeaponEntry>();
    public WeaponData startingWeapon;
    public int maxWeaponSlots = 3;
    private int nextUID = 0;

    // ── EQUIP SLOTS ──────────────────────────────────────────────
    public WeaponEntry[] equipSlots = new WeaponEntry[3];
    public int currentWeaponIndex = 0;
    public Transform weaponHolder;

    // ── ENGRAM INVENTORY ─────────────────────────────────────────
    [Header("Engram Inventory")]
    public List<EngramData> engramInventory = new List<EngramData>();
    public int maxEngramSlots = 5;

    // ── CURRENT STATE ────────────────────────────────────────────
    public Weapon currentWeapon;
    private bool isFiring = false;
    private float nextFireTime = 0f;
    private float scrollCooldown = 0f;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Awake()
    {
        if (startingWeapon != null)
        {
            var entry = new WeaponEntry(startingWeapon, nextUID++);
            inventory.Add(entry);
            equipSlots[0] = entry;
        }
    }

    void Start()
    {
        if (equipSlots[0] != null)
            EquipWeapon(0);
    }

    void Update()
    {
        if (InventoryUI.isInventoryOpen) return;
        if (DecodingTerminalUI.isTerminalOpen) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        scrollCooldown -= Time.deltaTime;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scrollCooldown <= 0f && Mathf.Abs(scroll) > 0.01f)
        {
            scrollCooldown = 0.2f;
            int dir  = scroll > 0 ? -1 : 1;
            int next = (currentWeaponIndex + dir + 3) % 3;
            EquipWeapon(next);
        }

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


    void LateUpdate()
    {
        // Save ammo every frame in LateUpdate so it always runs after shooting
        if (currentWeapon != null && equipSlots[currentWeaponIndex] != null)
            equipSlots[currentWeaponIndex].savedAmmo = currentWeapon.currentAmmo;
    }
    // ── INPUT MESSAGES ───────────────────────────────────────────

    void OnShoot() => isFiring = true;
    void OnShootRelease() => isFiring = false;
    void OnReload() { if (currentWeapon != null) currentWeapon.StartReload(); }

    // ── PUBLIC METHODS ───────────────────────────────────────────

    public void EquipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlots.Length) return;
        if (equipSlots[slotIndex] == null) return;
        if (slotIndex == currentWeaponIndex && currentWeapon != null) return;

        // Save current ammo
        if (currentWeapon != null && equipSlots[currentWeaponIndex] != null)
            equipSlots[currentWeaponIndex].savedAmmo = currentWeapon.currentAmmo;

        currentWeaponIndex = slotIndex;
        WeaponEntry entry = equipSlots[slotIndex];
        WeaponData data   = entry.data;

        if (weaponHolder.childCount > 0)
            foreach (Transform child in weaponHolder)
                Destroy(child.gameObject);

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
            currentWeapon.currentAmmo = entry.savedAmmo;
        }

        Debug.Log("Equipped slot " + (slotIndex + 1) + ": " + data.weaponName + " [uid=" + entry.uid + "]");
    }

    // Assign a specific WeaponEntry to a slot
    public void AssignToSlot(WeaponEntry entry, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlots.Length) return;
        if (entry == null) return;

        // Remove this exact entry (by uid) from any other slot
        for (int i = 0; i < equipSlots.Length; i++)
            if (i != slotIndex && equipSlots[i] != null && equipSlots[i].uid == entry.uid)
                equipSlots[i] = null;

        equipSlots[slotIndex] = entry;

        if (slotIndex == currentWeaponIndex)
            EquipWeapon(slotIndex);

        Debug.Log("Assigned " + entry.data.weaponName + " [uid=" + entry.uid + "] to slot " + (slotIndex + 1));
    }

public void AddToInventory(WeaponData data)
    {
        if (inventory.Count >= maxWeaponSlots)
        {
            Debug.Log("Weapon inventory full! Discard a weapon to make room.");
            return;
        }

        var entry = new WeaponEntry(data, nextUID++);
        inventory.Add(entry);
        Debug.Log($"Picked up: {data.weaponName} ({data.rarity})");

        // Auto assign to first empty equip slot
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i] == null)
            {
                AssignToSlot(entry, i);
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
