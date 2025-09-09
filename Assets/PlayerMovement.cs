using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float coyoteTime = 0.1f; // small grace period after leaving ground
    public LayerMask groundLayer;
    public Transform groundCheck;   // empty GameObject under player feet
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private float lastGroundedTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpQueued = true;
    }

    void Update()
    {
        // Ground check every frame
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (grounded)
            lastGroundedTime = coyoteTime;

        lastGroundedTime -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Jump logic
        if (jumpQueued && lastGroundedTime > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpQueued = false;
            lastGroundedTime = 0;
        }
        else
        {
            jumpQueued = false; // reset if pressed midair
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
