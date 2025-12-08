using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile settings")]
    public float speed = 15f;
    public int damage = 10;
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
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.TakeDamage(damage, transform);
            if (hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.identity);
            if (shootSound) AudioSource.PlayClipAtPoint(shootSound, player.GetComponent<Transform>().position, 1f);
            Destroy(gameObject);
        }
    }
}
