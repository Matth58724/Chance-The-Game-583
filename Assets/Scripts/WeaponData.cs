using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Loot/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Visuals")]
    public string weaponName;
    public Sprite weaponIcon; // This is 2D picture for the inventory
    public GameObject modelPrefab; // 3D gun model

    [Header("Rarity Settings")]
    public Rarity rarity;
    public Color rarityColor; // Grey, Green, Blue, Yellow, Rainbow

    [Header("Stats")]
    public float fireRate = 0.2f;
    public int damage = 10;
    public int magSize = 12;
    public float reloadTime = 2f;
    public float range = 100f;

    public enum Rarity { Common, Uncommon, Rare, UltraRare, Legendary }
}