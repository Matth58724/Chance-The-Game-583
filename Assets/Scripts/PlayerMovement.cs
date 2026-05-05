using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze rotation so the capsule doesn't fall over when moving
        rb.freezeRotation = true;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            // Reset Y velocity so double-jumps or 
            // jumping while falling feel consistent
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

void ApplyMovement()
    {
        // Get the direction from WASD
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        // Convert local space to world space
        Vector3 relativeMove = transform.TransformDirection(moveDir) * moveSpeed;

        // Apply velocity while keeping Y for gravity
        rb.linearVelocity = new Vector3(relativeMove.x, rb.linearVelocity.y, relativeMove.z);
    }
}