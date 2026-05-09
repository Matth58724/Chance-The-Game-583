using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Sprint with Left Shift
        isSprinting = Input.GetKey(KeyCode.LeftShift);
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ApplyMovement()
    {
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 relativeMove = transform.TransformDirection(moveDir) * currentSpeed;
        rb.linearVelocity = new Vector3(relativeMove.x, rb.linearVelocity.y, relativeMove.z);
    }
}