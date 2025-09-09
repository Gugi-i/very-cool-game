using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour 
{
    [Header("Stats")]
    public float health = 100f;
    public float moveSpeed = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ===== Behavior Hooks =====
        // Example behaviors you can add later:
        // Behavior 1: Patrol()
        // Behavior 2: ChasePlayer()
        // Behavior 3: ShootAtPlayer()
        // Call only one or combine based on enemy type
    }

    #region Behaviors (stubs)
    void Patrol()
    {
        // TODO: Add patrol logic
    }

    void ChasePlayer()
    {
        // TODO: Add chase logic
    }

    void ShootAtPlayer()
    {
        // TODO: Add shooting logic
    }
    #endregion

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die()
    {
        // TODO: add death animation / destroy prefab
        Destroy(gameObject);
    }
}
