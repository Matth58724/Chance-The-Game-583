using UnityEngine;

public class DecodingTerminal : MonoBehaviour
{
    private bool hasHealed = false; // Prevent repeatedly healing every frame
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
    void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null && !hasHealed)
        {
            playerHealth.HealToFull();
            hasHealed = true;
            Debug.Log("Player healed to full at terminal!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Reset so player can be healed again next time they approach
        if (other.GetComponent<PlayerHealth>() != null)
            hasHealed = false;
    }

}