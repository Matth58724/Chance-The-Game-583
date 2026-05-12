using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotsUI : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    private const int SLOT_COUNT = 3;
    private float slotWidth  = 160f;
    private float slotHeight = 50f;
    private float spacing    = 4f;

    // ── COLORS ───────────────────────────────────────────────────
    private Color activeColor  = new Color(0.20f, 0.20f, 0.25f, 0.95f); // Dark when active
    private Color inactiveColor= new Color(0.10f, 0.10f, 0.13f, 0.80f); // Darker when inactive
    private Color emptyColor   = new Color(0.06f, 0.06f, 0.08f, 0.70f); // Darkest when empty
    private Color activeBorder = new Color(0.55f, 0.85f, 1.00f, 1.00f); // Cyan border when active
    private Color inactiveBorder=new Color(0.25f, 0.25f, 0.30f, 1.00f); // Gray border when inactive
    private Color activeText   = new Color(1.00f, 1.00f, 1.00f, 1.00f); // White text active
    private Color inactiveText = new Color(0.55f, 0.55f, 0.60f, 1.00f); // Gray text inactive

    // ── PRIVATE REFS ─────────────────────────────────────────────
    private WeaponManager weaponManager;
    private Image[]           slotBgs      = new Image[SLOT_COUNT];
    private Image[]           slotBorders  = new Image[SLOT_COUNT];
    private TextMeshProUGUI[] slotNumbers  = new TextMeshProUGUI[SLOT_COUNT];
    private TextMeshProUGUI[] slotNames    = new TextMeshProUGUI[SLOT_COUNT];
    private TextMeshProUGUI   ammoText;
    private TextMeshProUGUI   reloadText;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        weaponManager = FindFirstObjectByType<WeaponManager>();
        BuildUI();
    }

    void Update()
    {
        RefreshUI();
    }

    // ── BUILD ────────────────────────────────────────────────────

void BuildUI()
    {
        var root = GetComponent<RectTransform>();

        float totalHeight = SLOT_COUNT * slotHeight + (SLOT_COUNT - 1) * spacing + 40f;

        // Anchor to bottom right
        root.anchorMin        = new Vector2(1f, 0f);
        root.anchorMax        = new Vector2(1f, 0f);
        root.pivot            = new Vector2(1f, 0f);
        root.anchoredPosition = new Vector2(-20f, 80f);
        root.sizeDelta        = new Vector2(slotWidth, totalHeight);

        // Build slots top to bottom (slot 1 at top)
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            float yPos = (SLOT_COUNT - 1 - i) * (slotHeight + spacing);

            // Border
            var borderObj = new GameObject("Border_" + (i + 1));
            borderObj.transform.SetParent(transform, false);
            var borderImg = borderObj.AddComponent<Image>();
            borderImg.color = inactiveBorder;
            borderImg.raycastTarget = false;
            slotBorders[i] = borderImg;

            var borderRect = borderObj.GetComponent<RectTransform>();
            borderRect.anchorMin        = Vector2.zero;
            borderRect.anchorMax        = Vector2.zero;
            borderRect.pivot            = Vector2.zero;
            borderRect.anchoredPosition = new Vector2(-2f, yPos - 2f);
            borderRect.sizeDelta        = new Vector2(slotWidth + 4f, slotHeight + 4f);

            // Slot background
            var bgObj = new GameObject("Slot_" + (i + 1));
            bgObj.transform.SetParent(transform, false);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = emptyColor;
            bgImg.raycastTarget = false;
            slotBgs[i] = bgImg;

            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin        = Vector2.zero;
            bgRect.anchorMax        = Vector2.zero;
            bgRect.pivot            = Vector2.zero;
            bgRect.anchoredPosition = new Vector2(0f, yPos);
            bgRect.sizeDelta        = new Vector2(slotWidth, slotHeight);

            // Slot number
            var numObj = new GameObject("Num");
            numObj.transform.SetParent(bgObj.transform, false);
            var numTmp = numObj.AddComponent<TextMeshProUGUI>();
            numTmp.text      = (i + 1).ToString();
            numTmp.fontSize  = 13;
            numTmp.fontStyle = TMPro.FontStyles.Bold;
            numTmp.color     = inactiveText;
            numTmp.alignment = TextAlignmentOptions.MidlineLeft;
            slotNumbers[i]   = numTmp;

            var numRect = numObj.GetComponent<RectTransform>();
            numRect.anchorMin  = Vector2.zero;
            numRect.anchorMax  = Vector2.one;
            numRect.offsetMin  = new Vector2(8f, 0f);
            numRect.offsetMax  = new Vector2(-8f, 0f);

            // Weapon name
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(bgObj.transform, false);
            var nameTmp = nameObj.AddComponent<TextMeshProUGUI>();
            nameTmp.fontSize         = 13;
            nameTmp.fontStyle        = TMPro.FontStyles.Bold;
            nameTmp.color            = inactiveText;
            nameTmp.alignment        = TextAlignmentOptions.MidlineLeft;
            nameTmp.enableAutoSizing = true;
            nameTmp.fontSizeMin      = 9;
            nameTmp.fontSizeMax      = 13;
            nameTmp.overflowMode     = TextOverflowModes.Ellipsis;
            slotNames[i]             = nameTmp;

            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin  = Vector2.zero;
            nameRect.anchorMax  = Vector2.one;
            nameRect.offsetMin  = new Vector2(28f, 0f);
            nameRect.offsetMax  = new Vector2(-8f, 0f);
        }

        // Ammo display
        var ammoObj = new GameObject("AmmoDisplay");
        ammoObj.transform.SetParent(transform, false);
        ammoText = ammoObj.AddComponent<TextMeshProUGUI>();
        ammoText.fontSize  = 22;
        ammoText.fontStyle = TMPro.FontStyles.Bold;
        ammoText.color     = Color.white;
        ammoText.alignment = TextAlignmentOptions.Center;

        var ammoRect = ammoObj.GetComponent<RectTransform>();
        ammoRect.anchorMin        = Vector2.zero;
        ammoRect.anchorMax        = Vector2.zero;
        ammoRect.pivot            = Vector2.zero;
        ammoRect.anchoredPosition = new Vector2(0f, -30f);
        ammoRect.sizeDelta        = new Vector2(slotWidth, 30f);

        // Reload text
        var reloadObj = new GameObject("ReloadText");
        reloadObj.transform.SetParent(transform, false);
        reloadText = reloadObj.AddComponent<TextMeshProUGUI>();
        reloadText.text      = "RELOADING...";
        reloadText.fontSize  = 14;
        reloadText.fontStyle = TMPro.FontStyles.Bold;
        reloadText.color     = new Color(1f, 0.8f, 0.2f, 1f);
        reloadText.alignment = TextAlignmentOptions.Center;
        reloadText.gameObject.SetActive(false);

        var reloadRect = reloadObj.GetComponent<RectTransform>();
        reloadRect.anchorMin        = Vector2.zero;
        reloadRect.anchorMax        = Vector2.zero;
        reloadRect.pivot            = Vector2.zero;
        reloadRect.anchoredPosition = new Vector2(0f, -50f);
        reloadRect.sizeDelta        = new Vector2(slotWidth, 20f);
    }

    // ── REFRESH ──────────────────────────────────────────────────

void RefreshUI()
    {
        if (weaponManager == null) return;

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            bool hasWeapon = weaponManager.equipSlots[i] != null;
            bool isActive  = i == weaponManager.currentWeaponIndex;

            // Background color
            slotBgs[i].color = !hasWeapon ? emptyColor :
                                isActive   ? activeColor : inactiveColor;

            // Border: rarity color when filled, default when empty
            if (hasWeapon)
            {
                Color rc = weaponManager.equipSlots[i].data.rarityColor;
                // Active slot: full rarity color, inactive: dimmed
                slotBorders[i].color = isActive
                    ? new Color(rc.r, rc.g, rc.b, 1f)
                    : new Color(rc.r, rc.g, rc.b, 0.4f);
            }
            else
            {
                slotBorders[i].color = inactiveBorder;
            }

            // Text colors
            Color textColor = (!hasWeapon || !isActive) ? inactiveText : activeText;
            slotNumbers[i].color = textColor;
            slotNames[i].color   = textColor;

            // Weapon name
            slotNames[i].text = hasWeapon ? weaponManager.equipSlots[i].data.weaponName : "-- Empty --";
        }

        // Ammo display for active weapon
        if (weaponManager.currentWeapon != null)
        {
            var w = weaponManager.currentWeapon;
            ammoText.text = w.currentAmmo + " / " + w.maxAmmo;
            reloadText.gameObject.SetActive(w.isReloading);
        }
        else
        {
            ammoText.text = "-- / --";
            reloadText.gameObject.SetActive(false);
        }
    }
}
