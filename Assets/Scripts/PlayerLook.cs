using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 25f;
    public Transform playerCamera;

    private float xRotation = 0f;
    private Vector2 lookInput;

    // Recoil offset added on top of normal look rotation
    private float recoilX = 0f;

    void Start()
    {
        // Locks the cursor to the center of the screen and hides it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // If the inventory is open, do NOT process mouse movement
        if (InventoryUI.isInventoryOpen) return;
        Look();
    }

    // Matches the Look action name in Input Action Asset
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Smoothly recover recoil back to zero over time
        recoilX = Mathf.Lerp(recoilX, 0f, 8f * Time.deltaTime);

        // Apply BOTH mouse look and recoil offset to camera
        playerCamera.localRotation = Quaternion.Euler(xRotation + recoilX, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    // Called by Weapon.cs to add upward camera kick
    public void AddRecoil(float amount)
    {
        recoilX -= amount;
    }
}