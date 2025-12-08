using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun Specifics")]
    public int projectileCount = 3;
    public float spreadAngle = 10f; // Total spread (e.g., 10 degrees)

    public override bool TryShoot(Vector2 direction, Transform firePoint)
    {
        // 1. Check Cooldown AND if we have enough ammo for the full spread
        if (Time.time >= lastFireTime + fireRate && currentAmmo >= projectileCount)
        {
            lastFireTime = Time.time;

            // 2. Deduct ammo based on how many projectiles we shoot
            currentAmmo -= projectileCount;

            Shoot(direction, firePoint);
            return true;
        }
        return false;
    }

    protected override void Shoot(Vector2 direction, Transform firePoint)
    {
        if (shootSound) AudioSource.PlayClipAtPoint(shootSound, firePoint.position, 1f);
        for (int i = 0; i < projectileCount; i++)
        {
            // 1. Get the base rotation from the direction
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 2. Calculate a random offset within the spread (-5 to +5 if spread is 10)
            float randomOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);

            // 3. Create the new rotation
            Quaternion newRotation = Quaternion.Euler(0, 0, baseAngle + randomOffset);

            // 4. Instantiate and launch
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, newRotation);

            // Derive the new direction vector from the rotation
            Vector2 randomizedDirection = newRotation * Vector2.right;

            proj.GetComponent<Projectile>().Launch(randomizedDirection);
        }
    }
}