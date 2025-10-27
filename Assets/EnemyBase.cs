using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50f;
    protected float currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("References")]
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        // Base enemies don’t move — override in subclasses
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Could play VFX / sound here
        Destroy(gameObject, 0.5f); // small delay for death anim
    }
}
