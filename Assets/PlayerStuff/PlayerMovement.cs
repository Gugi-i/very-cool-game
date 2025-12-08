using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Combat Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public float knockbackForce = 5f;
    public Transform attackPoint; // The center of the hitbox
    public float attackRange = 0.7f; // How big the hitbox is
    public int attackDamage = 20;
    public LayerMask enemyLayers; // Who gets hit?
    public GameObject hitEffect;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    // State flags
    public bool IsBusy { get; private set; }
    public bool IsDead { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Prevent movement update if busy or dead
        if (IsBusy || IsDead)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsBusy || IsDead) return;

        if (context.started && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (context.started && !IsBusy && !IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            StartAction("Attack");
        }
    }

    public void AttackHit()
    {
        // 1. Detect enemies in range of the attack point
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 2. Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            if (hitEffect != null) Instantiate(hitEffect, attackPoint.transform.position, Quaternion.identity);

            // OPTION A: If you have a specific Enemy script
            // enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);

            // OPTION B: General approach (Send Message)
            enemy.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 1. Enter Busy/Hit state FIRST
            StartAction("Hit");

            // 2. Calculate Knockback Direction (Away from the enemy)
            // (transform.position - enemy.position) gives the vector pointing away
            Vector2 knockbackDir = (transform.position - damageSource.position).normalized;

            // 3. Apply Knockback
            // We set Y to usually be up a little to prevent ground friction stopping it
            Vector2 force = new Vector2(knockbackDir.x, 0.2f).normalized * knockbackForce;

            rb.linearVelocity = Vector2.zero; // Reset previous movement
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        IsBusy = true; // Lock inputs permanently
        rb.linearVelocity = Vector2.zero; // Stop moving
        animator.SetTrigger("Die");

        // Optional: Disable collision logic here
    }

    private void StartAction(string triggerName)
    {
        IsBusy = true;
        animator.SetTrigger(triggerName);
        animator.SetBool("IsBusy", true); // Sync with your Animator parameter
    }

    // --- CALL THIS VIA ANIMATION EVENT ---
    // Add an event at the end of Attack and Hit animations calling this function.
    public void ResetBusyState()
    {
        if (IsDead) return;

        IsBusy = false;
        animator.SetBool("IsBusy", false);
    }

    void Update()
    {
        if (IsDead) return;

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);

        // Flip character (only if not busy)
        if (!IsBusy)
        {
            if (moveInput.x > 0 && !facingRight) Flip();
            else if (moveInput.x < 0 && facingRight) Flip();
        }
    }

    void FixedUpdate()
    {
        if (IsBusy || IsDead)
        {
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}