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
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
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
        // Clear both grids so we don't get duplicates
        foreach (Transform child in weaponGrid) Destroy(child.gameObject);
        foreach (Transform child in engramGrid) Destroy(child.gameObject);

        // Spawn Weapons into the Weapon Grid
        foreach (WeaponData wep in playerWeaponManager.inventory)
        {
            if (wep != null)
            {
                GameObject slot = Instantiate(slotPrefab, weaponGrid);
                slot.GetComponent<InventorySlot>().Setup(wep); // Calls the Weapon version
            }
        }

        // Spawn Engrams into the Engram Grid
        foreach (EngramData eng in playerWeaponManager.engramInventory)
        {
            if (eng != null)
            {
                GameObject slot = Instantiate(slotPrefab, engramGrid);
                slot.GetComponent<InventorySlot>().Setup(eng); // Calls the Engram version
            }
        }

        // Clean up layout
        Canvas.ForceUpdateCanvases();
        weaponGrid.GetComponent<UnityEngine.UI.LayoutGroup>().enabled = false;
        weaponGrid.GetComponent<UnityEngine.UI.LayoutGroup>().enabled = true;

        if (engramGrid.GetComponent<UnityEngine.UI.LayoutGroup>() != null)
        {
            engramGrid.GetComponent<UnityEngine.UI.LayoutGroup>().enabled = false;
            engramGrid.GetComponent<UnityEngine.UI.LayoutGroup>().enabled = true;
        }
    }
}