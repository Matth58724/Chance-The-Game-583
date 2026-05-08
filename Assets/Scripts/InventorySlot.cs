using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ── REFERENCES ───────────────────────────────────────────────
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public TextMeshProUGUI stackText;

    // ── PRIVATE STATE ────────────────────────────────────────────
    private WeaponData weaponData;
    private WeaponManager weaponManager;
    private bool isHovered = false;
    private Image bgImage;

    // ── COLORS ───────────────────────────────────────────────────
    private Color normalColor  = new Color(1f, 1f, 1f, 1f);
    private Color hoveredColor = new Color(0.75f, 0.9f, 1f, 1f);

    // ── UNITY METHODS ────────────────────────────────────────────

    void Awake()
    {
        bgImage = GetComponent<Image>();
        weaponManager = FindFirstObjectByType<WeaponManager>();
    }

    void Update()
    {
        if (!isHovered || weaponData == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) AssignToSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AssignToSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AssignToSlot(2);
    }

    // ── HOVER EVENTS ─────────────────────────────────────────────

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

    // ── SETUP FOR WEAPONS ────────────────────────────────────────

    public void Setup(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("Slot received NULL WeaponData!");
            return;
        }

        weaponData = data;

        nameText.text    = data.weaponName;
        iconImage.sprite = data.weaponIcon;
        iconImage.color  = Color.white;

        if (stackText != null)
            stackText.gameObject.SetActive(false);
    }

    // ── SETUP FOR ENGRAMS ────────────────────────────────────────

    public void SetupEngram(EngramData data, int count = 1)
    {
        if (data == null)
        {
            Debug.LogError("Slot received NULL EngramData!");
            return;
        }

        weaponData = null;
        nameText.text = data.engramName;

        if (data.engramIcon != null)
        {
            iconImage.sprite = data.engramIcon;
            iconImage.color  = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color  = data.engramColor;
        }

        if (stackText != null)
        {
            stackText.gameObject.SetActive(count > 1);
            stackText.text = "x" + count;
        }
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void AssignToSlot(int slotIndex)
    {
        if (weaponManager == null || weaponData == null) return;
        weaponManager.AssignToSlot(weaponData, slotIndex);
        Debug.Log(weaponData.weaponName + " assigned to slot " + (slotIndex + 1));
    }
}
