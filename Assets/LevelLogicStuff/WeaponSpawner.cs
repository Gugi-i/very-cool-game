using UnityEngine;
using System.Collections;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject weaponPrefab; // The weapon to give
    public float respawnTime = 5f;  // How long to wait
    public SpriteRenderer visualRenderer; // Drag the sprite renderer here

    private bool isAvailable = true;

    private void Start()
    {
        // Set the visual to match the weapon automatically
        if (weaponPrefab != null && visualRenderer != null)
        {
            visualRenderer.sprite = weaponPrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    // Called by the Player when they Interact
    public GameObject PickupWeapon()
    {
        if (!isAvailable) return null;

        StartCoroutine(RespawnRoutine());
        return weaponPrefab;
    }

    private IEnumerator RespawnRoutine()
    {
        isAvailable = false;
        visualRenderer.enabled = false; // Hide visual

        yield return new WaitForSeconds(respawnTime);

        isAvailable = true;
        visualRenderer.enabled = true; // Show visual
    }
}