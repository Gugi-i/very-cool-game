using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile settings")]
    public float speed = 15f;
    public float damage = 25f;
    public float lifetime = 5f;

    [Header("Projectile sound effect")]
    public AudioClip shootSound;

    [Header("Particle effect")]
    public GameObject hitEffect;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        RotateTowardsVelocity();
    }

    void FixedUpdate()
    {
        RotateTowardsVelocity();
    }

    void RotateTowardsVelocity()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemy = collision.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.identity);
            if (shootSound) AudioSource.PlayClipAtPoint(shootSound, enemy.GetComponent<Transform>().position, 1f);
            Destroy(gameObject);
        }
    }
}
