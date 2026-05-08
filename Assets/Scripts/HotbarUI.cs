using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HotbarUI : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    private const int SLOT_COUNT = 5;

    // ── COLORS ───────────────────────────────────────────────────
    private Color normalColor    = new Color(0.15f, 0.15f, 0.2f,  0.85f); // Dark slot
    private Color equippedColor  = new Color(0.25f, 0.55f, 0.95f, 1.0f);  // Blue when selected
    private Color emptyColor     = new Color(0.08f, 0.08f, 0.12f, 0.7f);  // Darker when empty

    // ── PRIVATE REFS ─────────────────────────────────────────────
    private WeaponManager weaponManager;
    private List<Image> slotBgs      = new List<Image>();
    private List<Image> slotIcons    = new List<Image>();
    private List<TextMeshProUGUI> slotNumbers = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> slotNames   = new List<TextMeshProUGUI>();

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        weaponManager = FindFirstObjectByType<WeaponManager>();
        BuildHotbar();
    }

    void Update()
    {
        RefreshHotbar();
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void BuildHotbar()
    {
        var canvasRect = GetComponent<RectTransform>();

        float slotSize   = 70f;
        float spacing    = 6f;
        float totalWidth = SLOT_COUNT * slotSize + (SLOT_COUNT - 1) * spacing;

        // Position hotbar at bottom center
        canvasRect.anchorMin = new Vector2(0.5f, 0f);
        canvasRect.anchorMax = new Vector2(0.5f, 0f);
        canvasRect.pivot     = new Vector2(0.5f, 0f);
        canvasRect.anchoredPosition = new Vector2(0f, 15f);
        canvasRect.sizeDelta = new Vector2(totalWidth, slotSize);

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            float xPos = i * (slotSize + spacing);

            // ── Slot background ──────────────────────────────────
            var bgObj  = new GameObject("Slot_" + (i + 1));
            bgObj.transform.SetParent(transform, false);
            var bgImg  = bgObj.AddComponent<Image>();
            bgImg.color = emptyColor;
            slotBgs.Add(bgImg);

            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin        = new Vector2(0f, 0f);
            bgRect.anchorMax        = new Vector2(0f, 0f);
            bgRect.pivot            = new Vector2(0f, 0f);
            bgRect.anchoredPosition = new Vector2(xPos, 0f);
            bgRect.sizeDelta        = new Vector2(slotSize, slotSize);

            // ── Slot number label (top left) ─────────────────────
            var numObj  = new GameObject("Number");
            numObj.transform.SetParent(bgObj.transform, false);
            var numTmp  = numObj.AddComponent<TextMeshProUGUI>();
            numTmp.text      = (i + 1).ToString();
            numTmp.fontSize  = 12;
            numTmp.color     = new Color(1f, 1f, 1f, 0.6f);
            numTmp.alignment = TextAlignmentOptions.TopLeft;
            slotNumbers.Add(numTmp);

            var numRect = numObj.GetComponent<RectTransform>();
            numRect.anchorMin        = Vector2.zero;
            numRect.anchorMax        = Vector2.one;
            numRect.offsetMin        = new Vector2(4f, 0f);
            numRect.offsetMax        = new Vector2(-4f, -4f);

            // ── Weapon icon (center) ─────────────────────────────
            var iconObj  = new GameObject("Icon");
            iconObj.transform.SetParent(bgObj.transform, false);
            var iconImg  = iconObj.AddComponent<Image>();
            iconImg.color = Color.white;
            iconImg.gameObject.SetActive(false);
            slotIcons.Add(iconImg);

            var iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin  = new Vector2(0.1f, 0.25f);
            iconRect.anchorMax  = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin  = Vector2.zero;
            iconRect.offsetMax  = Vector2.zero;

            // ── Weapon name label (bottom) ───────────────────────
            var nameObj  = new GameObject("Name");
            nameObj.transform.SetParent(bgObj.transform, false);
            var nameTmp  = nameObj.AddComponent<TextMeshProUGUI>();
            nameTmp.fontSize        = 8;
            nameTmp.color           = Color.white;
            nameTmp.alignment       = TextAlignmentOptions.Center;
            nameTmp.enableAutoSizing = true;
            nameTmp.fontSizeMin     = 6;
            nameTmp.fontSizeMax     = 9;
            nameTmp.overflowMode    = TextOverflowModes.Ellipsis;
            slotNames.Add(nameTmp);

            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin  = new Vector2(0f, 0f);
            nameRect.anchorMax  = new Vector2(1f, 0.28f);
            nameRect.offsetMin  = new Vector2(2f, 2f);
            nameRect.offsetMax  = new Vector2(-2f, 0f);
        }
    }

    void RefreshHotbar()
    {
        if (weaponManager == null) return;

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            bool hasWeapon  = i < weaponManager.inventory.Count;
            bool isEquipped = i == weaponManager.currentWeaponIndex;

            // Background color
            slotBgs[i].color = !hasWeapon  ? emptyColor :
                                isEquipped ? equippedColor : normalColor;

            if (hasWeapon)
            {
                var data = weaponManager.inventory[i];

                // Show icon if available
                if (data.weaponIcon != null)
                {
                    slotIcons[i].gameObject.SetActive(true);
                    slotIcons[i].sprite = data.weaponIcon;
                }
                else
                {
                    slotIcons[i].gameObject.SetActive(false);
                }

                slotNames[i].text = data.weaponName;
            }
            else
            {
                slotIcons[i].gameObject.SetActive(false);
                slotNames[i].text = "";
            }
        }
    }
}
