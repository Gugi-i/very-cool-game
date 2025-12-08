using System.Collections;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    [Header("Targeting")]
    public Transform player;
    public float chaseRange = 10f;

    [Header("Burst Movement Settings")]
    public float burstSpeed = 8f;
    public float burstInterval = 2f;
    public float linearDrag = 2f;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public Transform attackPoint;
    public float attackHitboxRadius = 0.5f;
    public LayerMask playerLayer;

    [Header("Combat Feedback")]
    public float knockbackForce = 4f;

    private float lastBurstTime;
    private float lastAttackTime;
    private bool facingRight = true;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearDamping = linearDrag;
        }

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
        if (IsBusy) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float dirX = Mathf.Sign(player.position.x - transform.position.x);

        // 1. Flip Logic (Always face player)
        if (distanceToPlayer <= chaseRange)
        {
            HandleFlip(dirX);
        }

        // 2. Attack Logic
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttackAnimation();
                lastAttackTime = Time.time;
            }
            return;
        }

        // 3. Burst Chase Logic
        if (distanceToPlayer <= chaseRange)
        {
            if (Time.time >= lastBurstTime + burstInterval)
            {
                PerformBurstMove();
                lastBurstTime = Time.time;
            }
        }
    }

    void PerformBurstMove()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = direction * burstSpeed;
    }

    void StartAttackAnimation()
    {
        IsBusy = true;

        if (animator != null)
            animator.SetTrigger("Attack");
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
        if (currentHealth <= 0)
        {
            rb.gravityScale = 1f;
            return;
        }

        IsBusy = true;
        if (animator != null) animator.SetTrigger("Hit");
        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }

    // --- Debugging ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(attackPoint.position, attackHitboxRadius);
        }
    }
}