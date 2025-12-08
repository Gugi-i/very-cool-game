using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50f;
    protected float currentHealth;
    public int score = 0;

    [Header("Movement")]
    public float moveSpeed = 2f;

    public bool IsBusy { get; protected set; }

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D mainCollider;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<Collider2D>();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        IsBusy = false;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
        if (mainCollider != null) mainCollider.enabled = true;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    protected virtual void Update()
    {
        if (currentHealth <= 0) return;
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
        if (!rb.simulated) return;

        if (animator != null) animator.SetTrigger("Die");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        mainCollider.enabled = false;

        this.enabled = false;

        StartCoroutine(DisableAfterDeathRoutine());
    }

    private IEnumerator DisableAfterDeathRoutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    public void ResetBusyState()
    {
        if (currentHealth <= 0) return;
        IsBusy = false;
    }
}