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
        // Clear the grids
        foreach (Transform child in weaponGrid)
        {
            Destroy(child.gameObject);
        }

        // Loop through the actual live inventory
        foreach (WeaponData wep in playerWeaponManager.inventory)
        {
            if (wep != null)
            {
                GameObject slot = Instantiate(slotPrefab, weaponGrid);
                slot.GetComponent<InventorySlot>().Setup(wep);
            }
        }

        // Force the UI to refresh its layout
        Canvas.ForceUpdateCanvases();
        weaponGrid.GetComponent<UnityEngine.UI.LayoutGroup>().enabled = false;
        weaponGrid.GetComponent<UnityEngine.UI.LayoutGroup>().enabled = true;
    }
}