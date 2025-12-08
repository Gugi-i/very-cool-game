using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Setup")]
    public Transform weaponHolder; // Create an empty Child GameObject called "WeaponPosition" and assign here
    public Transform firePoint;    // Keep this or make it a child of WeaponHolder
    public Transform crosshair;

    private Weapon activeWeapon;
    private WeaponSpawner currentNearbySpawner; // Tracks spawner we are standing on

    private void Update()
    {
        if (activeWeapon != null && activeWeapon.currentAmmo <= 0)
        {
            UnequipWeapon();
        }
    }

    // 1. INPUT: FIRE
    public void OnFire(InputAction.CallbackContext context)
    {
        if (activeWeapon != null && context.started)
        {
            Vector2 direction = (crosshair.position - firePoint.position).normalized;

            // Try to shoot. If successful, check ammo.
            activeWeapon.TryShoot(direction, firePoint);
        }
    }

    // 2. INPUT: INTERACT (Add "Interact" Action to your Input System Map)
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && currentNearbySpawner != null)
        {
            GameObject newWeaponPrefab = currentNearbySpawner.PickupWeapon();
            if (newWeaponPrefab != null)
            {
                EquipWeapon(newWeaponPrefab);
            }
        }
    }

    // 3. EQUIP LOGIC
    public void EquipWeapon(GameObject weaponPrefab)
    {
        // Remove old weapon if exists
        UnequipWeapon();

        // Instantiate new weapon as a child of the WeaponHolder
        GameObject newWeaponObj = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.identity);
        newWeaponObj.transform.SetParent(weaponHolder);

        // Reset local rotation/position so it sits correctly in the hand
        newWeaponObj.transform.localPosition = Vector3.zero;
        newWeaponObj.transform.localRotation = Quaternion.identity;

        activeWeapon = newWeaponObj.GetComponent<Weapon>();
    }

    // 4. UNEQUIP LOGIC
    public void UnequipWeapon()
    {
        if (activeWeapon != null)
        {
            Destroy(activeWeapon.gameObject);
            activeWeapon = null;
        }
    }

    // 5. DETECT SPAWNER
    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponSpawner spawner = other.GetComponent<WeaponSpawner>();
        if (spawner != null)
        {
            currentNearbySpawner = spawner;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        WeaponSpawner spawner = other.GetComponent<WeaponSpawner>();
        if (spawner != null && currentNearbySpawner == spawner)
        {
            currentNearbySpawner = null;
        }
    }
}