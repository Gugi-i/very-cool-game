using UnityEngine;
using System.Collections;

public class MachineGun : Weapon
{
    [Header("Burst Settings")]
    public int shotsPerBurst = 5;
    public float timeBetweenBurstShots = 0.1f;

    private bool isBursting = false;

    // 1. FIX: Change return type to bool to match Base Class
    public override bool TryShoot(Vector2 direction, Transform firePoint)
    {
        // 2. LOGIC: Check if we can shoot (Cooldown + Not Bursting + Has Ammo)
        if (!isBursting && Time.time >= lastFireTime + fireRate && currentAmmo > 0)
        {
            lastFireTime = Time.time;
            StartCoroutine(FireBurst(direction, firePoint));
            return true; // Return true to tell PlayerShooting "We fired"
        }
        return false;
    }

    protected override void Shoot(Vector2 direction, Transform firePoint)
    {
        // Intentionally left empty, logic is in Coroutine
    }

    private IEnumerator FireBurst(Vector2 direction, Transform firePoint)
    {
        isBursting = true;

        for (int i = 0; i < shotsPerBurst; i++)
        {
            // 3. AMMO CHECK: Stop bursting if we run out mid-burst
            if (currentAmmo <= 0) break;

            currentAmmo--; // Deduct 1 ammo per shot in the burst

            if (projectilePrefab != null)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                proj.GetComponent<Projectile>().Launch(direction);

                if (shootSound) AudioSource.PlayClipAtPoint(shootSound, firePoint.position, 1f);
            }

            yield return new WaitForSeconds(timeBetweenBurstShots);
        }

        isBursting = false;
    }
}