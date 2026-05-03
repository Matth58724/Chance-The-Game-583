using UnityEngine;

public class EngramPickup : MonoBehaviour
{
    public EngramData engramData; // This can be set by the EnemyHealth script or in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Search for the WeaponManager on the object that touched the trigger
        WeaponManager manager = other.GetComponent<WeaponManager>();

        if (manager != null)
        {
            // Try to add the engram to the 5-slot inventory
            bool wasAdded = manager.AddEngram(engramData);

            if (wasAdded)
            {
                // Play a sound or effect here
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory Full! Leave the engram on the ground.");
            }
        }
    }
}