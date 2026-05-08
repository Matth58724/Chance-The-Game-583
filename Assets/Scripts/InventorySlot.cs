using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    // ── REFERENCES ───────────────────────────────────────────────
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public TextMeshProUGUI stackText; // Shows count e.g. "x3"
    //public Image borderImage; // Border outline for slots

    // ── SETUP FOR WEAPONS ────────────────────────────────────────
    public void Setup(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("Slot received NULL WeaponData!");
            return;
        }
        nameText.text = data.weaponName;
        iconImage.sprite = data.weaponIcon;
        iconImage.color = Color.white;

        // Set border color from WeaponDatas rarityColor
        //if (borderImage != null)
        //    borderImage.color = data.rarityColor;

        // Weapons don't stack so hide stack text
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

        nameText.text = data.engramName;

        // Use sprite if assigned, otherwise solid color block
        if (data.engramIcon != null)
        {
            iconImage.sprite = data.engramIcon;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = data.engramColor;
        }

        // Engrams border color
        //if (borderImage != null)
        //    borderImage.color = data.engramColor;


        // Show stack count if more than one
        if (stackText != null)
        {
            stackText.gameObject.SetActive(count > 1);
            stackText.text = "x" + count;
        }
    }
}