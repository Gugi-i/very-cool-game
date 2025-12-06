using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50f;
    protected float currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D mainCollider;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<Collider2D>();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;

        // Ensure physics are back on
        if (rb != null) rb.simulated = true;
        if (mainCollider != null) mainCollider.enabled = true;
    }

    protected virtual void Update()
    {
        // Override in subclasses
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Default behavior for pooling
        gameObject.SetActive(false);
    }
}