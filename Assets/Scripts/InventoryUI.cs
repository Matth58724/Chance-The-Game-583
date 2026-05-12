using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; // The parent UI object
    public WeaponManager playerWeaponManager; // Drag your Player here

    [Header("Grids")]
    public Transform weaponGrid; // A UI object with 'Grid Layout Group'
    public Transform engramGrid; // A UI object with 'Grid Layout Group'
    public GameObject slotPrefab; // The Button prefab we will make in Step 3

    private bool isOpen = false;

void Update()
    {
        // Open/close inventory with I or Tab
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();
    }
    public static bool isInventoryOpen = false;

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        isInventoryOpen = isOpen;
        inventoryPanel.SetActive(isOpen);

        // Unlock mouse so you can click items
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen) RefreshUI();
    }

public void RefreshUI()
    {
        // Clear both grids to avoid duplicates
        foreach (Transform child in weaponGrid) Destroy(child.gameObject);
        foreach (Transform child in engramGrid) Destroy(child.gameObject);

        // Spawn weapons into weapon grid
        foreach (WeaponManager.WeaponEntry wep in playerWeaponManager.inventory)
        {
            if (wep != null)
            {
                GameObject slot = Instantiate(slotPrefab, weaponGrid);
                slot.GetComponent<InventorySlot>().Setup(wep);
            }
        }

        // Group engrams by name so duplicates stack into one slot
        Dictionary<string, (EngramData data, int count)> engramStacks =
            new Dictionary<string, (EngramData, int)>();

        foreach (EngramData eng in playerWeaponManager.engramInventory)
        {
            if (eng == null) continue;
            if (engramStacks.ContainsKey(eng.engramName))
            {
                var entry = engramStacks[eng.engramName];
                engramStacks[eng.engramName] = (entry.data, entry.count + 1);
            }
            else
            {
                engramStacks[eng.engramName] = (eng, 1);
            }
        }

        // Spawn one slot per unique engram type with stack count
        foreach (var kvp in engramStacks)
        {
            GameObject slot = Instantiate(slotPrefab, engramGrid);
            slot.GetComponent<InventorySlot>().SetupEngram(kvp.Value.data, kvp.Value.count);
        }

        // Update section labels with capacity counts
        UpdateLabel("WeaponLabel", "WEAPONS", playerWeaponManager.inventory.Count, playerWeaponManager.maxWeaponSlots);
        UpdateLabel("EngramLabel", "ENGRAMS", playerWeaponManager.engramInventory.Count, playerWeaponManager.maxEngramSlots);

        // Force full canvas rebuild so grid layout applies correctly
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(weaponGrid.GetComponent<RectTransform>());
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(engramGrid.GetComponent<RectTransform>());
    }

    void UpdateLabel(string labelName, string title, int current, int max)
    {
        var label = inventoryPanel.transform.Find(labelName);
        if (label == null) return;
        var tmp = label.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmp == null) return;
        tmp.text = title + "  <size=14><color=#aaaaaa>" + current + "/" + max + "</color></size>";
        // Turn red if full
        tmp.color = current >= max ? new UnityEngine.Color(1f, 0.4f, 0.4f, 1f) : new UnityEngine.Color(0.8f, 0.8f, 1f, 1f);
    }
}