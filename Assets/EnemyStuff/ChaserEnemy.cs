using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class ChaserEnemy : EnemyBase
{
    [Header("Target")]
    public Transform player;

    [Header("AI Settings")]
    public float chaseRange = 6f;
    public float attackTriggerDistance = 1.2f;
    public float verticalAttackRange = 1.5f; // Max height diff to attack
    public float attackCooldown = 1f;
    public float dropCheckDistance = 4f;     // Max height to drop down

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

    protected override void OnEnable()
    {
        base.OnEnable();
        transform.rotation = Quaternion.identity;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
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
        if (currentHealth <= 0) return;

        base.Update();

        if (player == null) return;

        // 1. Busy Check
        if (IsBusy) return;

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        bool isGroundAhead = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, groundLayer);

        // Calculate Distances
        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distY = Mathf.Abs(player.position.y - transform.position.y);
        float dirX = Mathf.Sign(player.position.x - transform.position.x);

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // 2. Handle Flip BEFORE checking edges
        // This ensures the enemy turns around if the player is behind them,
        // even if they are currently standing at an edge.
        if (distX <= chaseRange)
        {
            HandleFlip(dirX);
        }

        // 3. Drop Logic (Raycast down to see if there is a platform below)
        bool safeToDrop = false;
        if (!isGroundAhead)
        {
            Vector2 origin = new Vector2(edgeCheck.position.x, edgeCheck.position.y + 0.5f);
            RaycastHit2D groundBelow = Physics2D.Raycast(origin, Vector2.down, dropCheckDistance, groundLayer);
            if (groundBelow.collider != null)
            {
                safeToDrop = true;
            }
        }

        // 4. Safety Stop
        if (isGrounded && !isGroundAhead && !safeToDrop)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 5. Attack Logic
        // Must be close on X AND close on Y to attack
        if (distX <= attackTriggerDistance && distY <= verticalAttackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttackAnimation();
                lastAttackTime = Time.time;
            }
            return;
        }

        // 6. Chase Logic
        if (distX <= chaseRange)
        {
            rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void StartAttackAnimation()
    {
        IsBusy = true;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetTrigger("Attack");
    }

    public void AttackHit()
    {
        if (attackPoint == null) return;
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackHitboxRadius, playerLayer);

        if (hitPlayer != null)
        {
            PlayerMovement pm = hitPlayer.GetComponent<PlayerMovement>();
            if (pm != null) pm.TakeDamage(attackDamage, transform);
        }
    }

    void HandleFlip(float dir)
    {
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
        base.TakeDamage(damage);
        if (currentHealth <= 0) return;

        IsBusy = true;
        if (animator != null) animator.SetTrigger("Hit");

        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            Vector2 knockback = new Vector2(direction.x, 0.2f).normalized * knockbackForce;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }

    // --- Debugging ---

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

        if (edgeCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * dropCheckDistance);
        }
    }
}