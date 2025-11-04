using UnityEngine;
using UnityEngine.InputSystem; // For new Input System (XR controllers)

[RequireComponent(typeof(CharacterController))]
public class VRDoubleJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public int maxJumps = 2; // Allows double jump

    [Header("Input")]
    public InputActionProperty jumpAction; // Assign your controller button here

    private CharacterController characterController;
    private Vector3 velocity;
    private int jumpCount = 0;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCount = 0;
        }

        // Handle jump input (button press)
        if (jumpAction.action.WasPressedThisFrame() && jumpCount < maxJumps)
        {
            Jump();
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move character
        characterController.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        jumpCount++;
    }
}
