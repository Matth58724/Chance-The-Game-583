using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    // ── FULL INVENTORY ───────────────────────────────────────────
    public List<WeaponData> inventory = new List<WeaponData>();
    public WeaponData startingWeapon;

    // ── EQUIP SLOTS ──────────────────────────────────────────────
    public WeaponData[] equipSlots = new WeaponData[3];
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

    // ── AMMO PERSISTENCE ─────────────────────────────────────────
    // Keyed by WeaponData instance ID so ammo persists when switching
    private Dictionary<int, int> savedAmmo = new Dictionary<int, int>();

    // ── UNITY METHODS ────────────────────────────────────────────

    void Awake()
    {
        if (startingWeapon != null && !inventory.Contains(startingWeapon))
        {
            inventory.Add(startingWeapon);
            equipSlots[0] = startingWeapon;
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

        // Continuously save current weapon ammo so it persists on switch
        if (currentWeapon != null && equipSlots[currentWeaponIndex] != null)
            savedAmmo[equipSlots[currentWeaponIndex].GetInstanceID()] = currentWeapon.currentAmmo;

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

    // ── INPUT SYSTEM MESSAGES ────────────────────────────────────

    void OnShoot() => isFiring = true;
    void OnShootRelease() => isFiring = false;

    void OnReload()
    {
        if (currentWeapon != null)
            currentWeapon.StartReload();
    }

    // ── PUBLIC METHODS ───────────────────────────────────────────

public void EquipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlots.Length) return;
        if (equipSlots[slotIndex] == null) return;

        // Already on this slot - do nothing
        if (slotIndex == currentWeaponIndex && currentWeapon != null) return;

        // Save current ammo before switching
        if (currentWeapon != null && equipSlots[currentWeaponIndex] != null)
            savedAmmo[equipSlots[currentWeaponIndex].GetInstanceID()] = currentWeapon.currentAmmo;

        currentWeaponIndex = slotIndex;
        WeaponData data = equipSlots[slotIndex];

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

            // Restore saved ammo or start full
            int id = data.GetInstanceID();
            currentWeapon.currentAmmo = savedAmmo.ContainsKey(id) ? savedAmmo[id] : data.magSize;
        }

        Debug.Log("Equipped slot " + (slotIndex + 1) + ": " + data.weaponName);
    }

    public void AssignToSlot(WeaponData data, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlots.Length) return;

        // Remove from any other slot
        for (int i = 0; i < equipSlots.Length; i++)
            if (i != slotIndex && equipSlots[i] == data)
                equipSlots[i] = null;

        equipSlots[slotIndex] = data;

        if (slotIndex == currentWeaponIndex)
            EquipWeapon(slotIndex);

        Debug.Log("Assigned " + data.weaponName + " to slot " + (slotIndex + 1));
    }

    public void AddToInventory(WeaponData data)
    {
        inventory.Add(data);
        Debug.Log($"Picked up: {data.weaponName} ({data.rarity})");

        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i] == null)
            {
                AssignToSlot(data, i);
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
