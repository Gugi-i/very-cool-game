using UnityEngine;

public class SimpleGun : Weapon
{
    protected override void Shoot(Vector2 direction, Transform firePoint)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        proj.GetComponent<Projectile>().Launch(direction);

        if (shootSound) AudioSource.PlayClipAtPoint(shootSound, firePoint.position, 1f);
    }
}