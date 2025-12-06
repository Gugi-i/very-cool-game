using System.Collections;
using UnityEngine;

public class ChaserEnemy : EnemyBase
{
    [Header("Target")]
    public Transform player;

    [Header("AI Settings")]
    public float chaseRange = 6f;
    public float attackTriggerDistance = 1.2f;
    public float attackCooldown = 1f;

    [Header("Combat Hitbox")]
    public Transform attackPoint;
    public float attackHitboxRadius = 0.5f;
    public int attackDamage = 10;
    public LayerMask playerLayer;

    [Header("Combat Feedback")]
    public float knockbackForce = 5f;

    [Header("Ground & Edge Detection")]
    public Transform groundCheck;
    public Transform edgeCheck;
    public float checkRadius = 0.15f;
    public LayerMask groundLayer;

    private float lastAttackTime;
    private bool facingRight = true;

    public bool IsBusy { get; private set; }

    protected override void OnEnable()
    {
        base.OnEnable(); // Resets Health and Physics

        // FIX 1: Ensure we are upright (just in case)
        transform.rotation = Quaternion.identity;

        // FIX 2: Find the player automatically
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }

        IsBusy = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (player == null) return;

        // 1. Busy Check: If attacking or hit, do not move or start new logic
        if (IsBusy) return;

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        bool isGroundAhead = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, groundLayer);

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float horizontalDir = Mathf.Sign(player.position.x - transform.position.x);

        // Update animator speed (Capitalized)
        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // 2. Safety Check: Stop if not grounded or about to fall
        if (!isGrounded || !isGroundAhead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 3. Attack Logic
        if (distanceToPlayer <= attackTriggerDistance)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttackAnimation();
                lastAttackTime = Time.time;
            }
            return;
        }

        // 4. Chase Logic
        if (distanceToPlayer <= chaseRange)
        {
            rb.linearVelocity = new Vector2(horizontalDir * moveSpeed, rb.linearVelocity.y);
            HandleFlip(horizontalDir);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void StartAttackAnimation()
    {
        IsBusy = true;
        rb.linearVelocity = Vector2.zero; // Stop moving immediately
        if (animator != null)
            animator.SetTrigger("Attack"); // Capitalized
    }

    // --- ANIMATION EVENT calls this function ---
    public void AttackHit()
    {
        if (attackPoint == null) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackHitboxRadius, playerLayer);

        if (hitPlayer != null)
        {
            PlayerMovement pm = hitPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                // Pass 'transform' (this enemy) as the damage source
                pm.TakeDamage(attackDamage, transform);
            }
        }
    }

    // --- ANIMATION EVENT calls this function (ADD TO END OF HIT & ATTACK CLIPS) ---
    public void ResetBusyState()
    {
        IsBusy = false;
        // Stop any residual knockback sliding when recovering
        rb.linearVelocity = Vector2.zero;
    }

    void HandleFlip(float dir)
    {
        // Prevent flipping if busy (optional, but usually looks better)
        if (IsBusy) return;

        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    public override void TakeDamage(float damage)
    {
        // 1. Process Health reduction (and calling Die() if health <= 0)
        base.TakeDamage(damage);

        // 2. IMPORTANT CHECK:
        // If we died in the line above, STOP here. Do not play Hit animation.
        if (currentHealth <= 0) return;

        // 3. Alive? Then play Hit animation and apply knockback
        IsBusy = true;
        if (animator != null) animator.SetTrigger("Hit");

        // KNOCKBACK LOGIC
        if (player != null)
        {
            // Calculate direction away from player
            Vector2 direction = (transform.position - player.position).normalized;

            // Add a slight upward lift (y: 0.2) to reduce friction issues
            Vector2 knockback = new Vector2(direction.x, 0.2f).normalized * knockbackForce;

            rb.linearVelocity = Vector2.zero; // Reset existing velocity first
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }

    protected override void Die()
    {
        if (animator != null) animator.SetTrigger("Die");

        IsBusy = true;
        rb.linearVelocity = Vector2.zero;

        // Turn off physics so it doesn't block the player while dying
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        // POOLING CHANGE: Start routine instead of Destroy
        StartCoroutine(DisableAfterDeathRoutine());
    }

    private IEnumerator DisableAfterDeathRoutine()
    {
        // Wait for death animation to finish
        yield return new WaitForSeconds(2f);

        // Return to pool
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitboxRadius);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}