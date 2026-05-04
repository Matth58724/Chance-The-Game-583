using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEngram", menuName = "Loot/Engram Data")]
public class EngramData : ScriptableObject
{
    public string engramName;
    public Sprite engramIcon; // Sprite for the inventory
    public WeaponData.Rarity tier;
    public Color engramColor;
    public List<WeaponData> possibleDrops; // The pool of weapons this can decode into
}