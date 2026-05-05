using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Degrees per second

    void Update()
    {
        // Rotates the object every frame based on time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}