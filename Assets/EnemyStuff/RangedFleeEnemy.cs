using System.Collections;
using UnityEngine;

public class RangedFleeEnemy : EnemyBase
{
    [Header("Target")]
    public Transform player;

    [Header("Flee Settings")]
    public float fleeRange = 5f;
    public float moveSpeedFlee = 3f;

    [Header("Environment Check")]
    public Transform edgeCheck;
    public LayerMask groundLayer;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2f;

    [Header("Combat Feedback")]
    public float knockbackForce = 3f;

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
        facingRight = transform.localScale.x > 0;
    }

    protected override void Update()
    {
        base.Update();
        if (currentHealth <= 0) return;
        if (player == null) return;

        if (IsBusy)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 1. Calculate Distances
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        float diffY = Mathf.Abs(player.position.y - transform.position.y);
        float dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);

        // 2. Platform/Height Check (FIX: Don't react if player is on different floor)
        if (diffY > 0.5f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 3. Flee Logic
        // We want to flee AWAY from player (-dirToPlayerX)
        float fleeDir = -dirToPlayerX;

        // Check if there is ground where we want to run
        bool safeToFlee = CheckGroundInFleeDirection(fleeDir);

        if (distToPlayer < fleeRange && safeToFlee)
        {
            Flee(fleeDir);
        }
        else
        {
            // Stop, Face Player, Shoot
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            FaceDirection(dirToPlayerX);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
        }

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void Flee(float dirX)
    {
        Debug.Log($"Fleeing: Dir {dirX} | Desired Speed: {dirX * moveSpeedFlee} | Current X Vel: {rb.linearVelocity.x}");

        rb.linearVelocity = new Vector2(dirX * moveSpeedFlee, rb.linearVelocity.y);
        FaceDirection(dirX);
    }

    bool CheckGroundInFleeDirection(float dirX)
    {
        float checkY = edgeCheck.position.y;
        float checkX = transform.position.x + (dirX * 0.2f);

        Vector2 checkPos = new Vector2(checkX, checkY);

        return Physics2D.OverlapCircle(checkPos, 0.3f, groundLayer);
    }

    void FaceDirection(float dir)
    {
        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    void StartAttack()
    {
        IsBusy = true;
        lastAttackTime = Time.time;
        if (animator != null) animator.SetTrigger("Attack");
    }

    public void SpawnProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            EnemyProjectile projScript = projObj.GetComponent<EnemyProjectile>();

            if (projScript != null)
            {
                float dirX = facingRight ? 1f : -1f;
                Vector2 launchDir = new Vector2(dirX, 0);
                projScript.Launch(launchDir);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (currentHealth <= 0) return;

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
        Gizmos.DrawWireSphere(transform.position, fleeRange);

        Gizmos.color = Color.blue;
        Vector3 originRight = transform.position + (Vector3.right * 0.8f) + (Vector3.up * 0.5f);
        Vector3 originLeft = transform.position + (Vector3.left * 0.8f) + (Vector3.up * 0.5f);
        Gizmos.DrawLine(originRight, originRight + Vector3.down * 2f);
        Gizmos.DrawLine(originLeft, originLeft + Vector3.down * 2f);
    }
}