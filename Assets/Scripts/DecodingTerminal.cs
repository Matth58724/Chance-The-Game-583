using UnityEngine;

public class DecodingTerminal : MonoBehaviour
{
    public void DecodeEngram(WeaponManager playerManager, int engramIndex)
    {
        if (playerManager.engramInventory.Count <= engramIndex) return;

        EngramData engram = playerManager.engramInventory[engramIndex];

        // Pick a random weapon from the engram's tier pool
        int randomIndex = Random.Range(0, engram.possibleDrops.Count);
        WeaponData reward = engram.possibleDrops[randomIndex];

        // Add the new weapon to the player's gun inventory
        playerManager.AddToInventory(reward);

        // Remove the used blueprint
        playerManager.engramInventory.RemoveAt(engramIndex);

        Debug.Log($"Decoded {engram.engramName} into a {reward.weaponName}!");
    }
}