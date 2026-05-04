using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image iconImage;

    public void Setup(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("Slot received NULL data!");
            return;
        }
        Debug.Log("Setting up slot for: " + data.weaponName);
        nameText.text = data.weaponName;
        iconImage.sprite = data.weaponIcon; // Shows the weapon PNG
    }

    public void Setup(EngramData data)
    {
        nameText.text = data.engramName;
        iconImage.sprite = data.engramIcon; // Shows the engram PNG
        iconImage.color = data.engramColor; // Optional: Tint it white/green/blue
    }
}