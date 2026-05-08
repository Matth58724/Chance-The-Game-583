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

void RefreshUI()
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

        // Force full canvas rebuild so grid layout applies correctly
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(weaponGrid.GetComponent<RectTransform>());
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(engramGrid.GetComponent<RectTransform>());
    }
}