using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject projectilePrefab;
    public float fireRate = 0.3f;
    public int magazineSize = 10; // New: Max Ammo
    public AudioClip shootSound;

    [HideInInspector] public int currentAmmo; // Track current ammo
    protected float lastFireTime;

    private void Awake()
    {
        currentAmmo = magazineSize; // Refill on create
    }

    public virtual bool TryShoot(Vector2 direction, Transform firePoint)
    {
        if (Time.time >= lastFireTime + fireRate && currentAmmo > 0)
        {
            lastFireTime = Time.time;
            currentAmmo--; // Deduct ammo
            Shoot(direction, firePoint);
            return true; // Shot fired
        }
        return false; // Did not fire
    }

    protected abstract void Shoot(Vector2 direction, Transform firePoint);
}