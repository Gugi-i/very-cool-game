using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint; // assign empty object at muzzle
    public Transform crosshair; // assign Crosshair object in inspector
    public float fireRate = 0.3f;

    private PlayerAudio playerAudio;

    private float lastFireTime;

    private void Awake()
    {
        playerAudio = GetComponentInChildren<PlayerAudio>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= lastFireTime + fireRate)
        {
            Shoot();
            playerAudio.PlayShoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || crosshair == null) return;

        // Get direction towards crosshair
        Vector2 direction = (crosshair.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Launch(direction);
    }
}
