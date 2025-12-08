using System.Collections;
using UnityEngine;

public class PatrolEnemy : EnemyBase
{
    [Header("Target")]
    public Transform player;

    [Header("Patrol Settings")]
    public Transform edgeCheck;
    public LayerMask groundLayer;
    public float idleTimeAtEdge = 1f;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 15;
    public LayerMask playerLayer;

    [Header("Combat Feedback")]
    public float knockbackForce = 3f;

    private float lastAttackTime;
    private bool movingRight = true;
    private bool isWaiting = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        IsBusy = false;
        isWaiting = false;
        movingRight = transform.localScale.x > 0;

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

        if (IsBusy || isWaiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (CheckForPlayer())
        {
            AttemptAttack();
            return;
        }

        Patrol();

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void Patrol()
    {
        float dirX = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

        bool groundAhead = Physics2D.OverlapCircle(edgeCheck.position, 0.3f, groundLayer);

        if (!groundAhead)
        {
            StartCoroutine(TurnAroundRoutine());
        }
    }

    private bool CheckForPlayer()
    {
        if (player == null) return false;

        float yDiff = Mathf.Abs(player.position.y - transform.position.y);
        if (yDiff > 0.5f) return false;

        float dirX = movingRight ? 1f : -1f;
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, Vector2.right * dirX, attackRange, playerLayer);
        return hit.collider != null;
    }

    private void AttemptAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            IsBusy = true;
            if (animator != null) animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator TurnAroundRoutine()
    {
        isWaiting = true;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetFloat("Speed", 0);

        yield return new WaitForSeconds(idleTimeAtEdge);

        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        isWaiting = false;
    }

    public void AttackHit()
    {
        if (currentHealth <= 0) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange * 0.5f, playerLayer);

        if (hitPlayer != null)
        {
            PlayerMovement pm = hitPlayer.GetComponent<PlayerMovement>();
            if (pm != null) pm.TakeDamage(attackDamage, transform);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (currentHealth <= 0) return;

        // Only playing Hit animation if not already busy attacking could be considered, 
        // but usually taking damage interrupts everything:
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
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            float dir = transform.localScale.x > 0 ? 1 : -1;
            Gizmos.DrawRay(attackPoint.position, Vector2.right * dir * attackRange);
        }
        if (edgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector2 origin = new Vector2(edgeCheck.position.x, edgeCheck.position.y + 0.5f);
            Gizmos.DrawLine(origin, origin + Vector2.down * 1.5f);
        }
    }
}