using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    // SETTINGS 
    // How long in seconds before this GameObject destroys itself
    public float time = 0.1f;


    void Start()
    {
        // Destroy this GameObject after the set time
        Destroy(gameObject, time);
    }
}