using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ── REFERENCES ───────────────────────────────────────────────
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public TextMeshProUGUI stackText;

    // ── PRIVATE STATE ────────────────────────────────────────────
    private WeaponManager.WeaponEntry weaponEntry;
    private EngramData engramData;
    private WeaponManager weaponManager;
    private InventoryUI inventoryUI;
    private bool isHovered = false;
    private Image bgImage;
    private UnityEngine.UI.Outline outline;

    // ── COLORS ───────────────────────────────────────────────────
    private Color normalColor  = new Color(1f, 1f, 1f, 1f);
    private Color hoveredColor = new Color(0.75f, 0.9f, 1f, 1f);

    // ── UNITY METHODS ────────────────────────────────────────────

    void Awake()
    {
        bgImage       = GetComponent<Image>();
        weaponManager = FindFirstObjectByType<WeaponManager>();
        inventoryUI   = FindFirstObjectByType<InventoryUI>();

        // Add outline component for rarity border
        outline = GetComponent<UnityEngine.UI.Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<UnityEngine.UI.Outline>();
        outline.effectDistance = new Vector2(3f, 3f);
        outline.effectColor = Color.clear;
    }

    void Update()
    {
        if (!isHovered || weaponEntry == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) AssignToSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AssignToSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AssignToSlot(2);
    }

    // ── POINTER EVENTS ───────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (bgImage != null) bgImage.color = hoveredColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (bgImage != null) bgImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (weaponEntry != null) DiscardWeapon();
        else if (engramData != null) DiscardEngram();
    }

    // ── SETUP ────────────────────────────────────────────────────

    public void Setup(WeaponManager.WeaponEntry entry)
    {
        if (entry == null || entry.data == null)
        {
            Debug.LogError("Slot received NULL WeaponEntry!");
            return;
        }
        weaponEntry = entry;
        engramData  = null;

        nameText.text    = entry.data.weaponName;
        iconImage.sprite = entry.data.weaponIcon;
        iconImage.color  = Color.white;
        if (stackText != null) stackText.gameObject.SetActive(false);

        // Color name and border by rarity
        Color rc = entry.data.rarityColor;
        nameText.color = rc;
        if (outline != null) outline.effectColor = rc;
    }

    public void SetupEngram(EngramData data, int count = 1)
    {
        if (data == null) { Debug.LogError("Slot received NULL EngramData!"); return; }
        weaponEntry = null;
        engramData  = data;

        nameText.text  = data.engramName;
        nameText.color = Color.white;

        if (data.engramIcon != null) { iconImage.sprite = data.engramIcon; iconImage.color = Color.white; }
        else { iconImage.sprite = null; iconImage.color = data.engramColor; }

        if (stackText != null) { stackText.gameObject.SetActive(count > 1); stackText.text = "x" + count; }

        // No rarity border for engrams
        if (outline != null) outline.effectColor = Color.clear;
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void AssignToSlot(int slotIndex)
    {
        if (weaponManager == null || weaponEntry == null) return;
        weaponManager.AssignToSlot(weaponEntry, slotIndex);
    }

    void DiscardWeapon()
    {
        if (weaponManager == null || weaponEntry == null) return;

        for (int i = 0; i < weaponManager.equipSlots.Length; i++)
            if (weaponManager.equipSlots[i] == weaponEntry)
            {
                weaponManager.equipSlots[i] = null;
                if (i == weaponManager.currentWeaponIndex)
                {
                    foreach (Transform child in weaponManager.weaponHolder)
                        Destroy(child.gameObject);
                    weaponManager.currentWeapon = null;
                }
            }

        weaponManager.inventory.Remove(weaponEntry);
        Debug.Log("Discarded: " + weaponEntry.data.weaponName);
        if (inventoryUI != null) inventoryUI.RefreshUI();
    }

    void DiscardEngram()
    {
        if (weaponManager == null || engramData == null) return;
        weaponManager.engramInventory.Remove(engramData);
        Debug.Log("Discarded: " + engramData.engramName);
        if (inventoryUI != null) inventoryUI.RefreshUI();
    }
}
