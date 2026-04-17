using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 25f;
    public Transform playerCamera;

    private float xRotation = 0f;
    private Vector2 lookInput;

    void Start()
    {
        // Locks the cursor to the center of the screen and hides it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
    }

    // Matches the Look action name in Input Action Asset
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Look()
    {
        // Calculate mouse movement
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Calculate vertical rotation (Up/Down)
        xRotation -= mouseY;
        // Clamp rotation so you can't flip your head upside down (90 degrees up or down)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply vertical rotation to the CAMERA
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation to the PLAYER BODY (Left/Right)
        transform.Rotate(Vector3.up * mouseX);
    }
}