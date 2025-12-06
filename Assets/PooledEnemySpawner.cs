using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; // Required for Tilemaps

public class PooledEnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2.0f;
    public int maxEnemies = 10;

    [Header("Map Settings")]
    [Tooltip("Drag your Ground/Platform Tilemaps here")]
    public List<Tilemap> tilemaps;

    // The Pool
    private List<GameObject> enemyPool = new List<GameObject>();

    // Valid Spawn Locations Cache
    private List<Vector3> validSpawnPositions = new List<Vector3>();

    private void Start()
    {
        CalculateSpawnPoints();
        StartCoroutine(SpawnRoutine());
    }

    private void CalculateSpawnPoints()
    {
        validSpawnPositions.Clear();

        foreach (Tilemap map in tilemaps)
        {
            // Scan every coordinate inside the tilemap's bounds
            foreach (var pos in map.cellBounds.allPositionsWithin)
            {
                // 1. Is there a tile here? (The ground)
                if (map.HasTile(pos))
                {
                    // 2. Is the space ABOVE it empty? (So they don't spawn inside a wall)
                    Vector3Int posAbove = new Vector3Int(pos.x, pos.y + 1, pos.z);
                    if (!map.HasTile(posAbove))
                    {
                        // 3. Convert to World Position (Center of the tile)
                        Vector3 worldPos = map.GetCellCenterWorld(pos);

                        // Adjust Y to sit on top of the tile (assuming 1 unit tiles)
                        worldPos.y += 0.5f;

                        validSpawnPositions.Add(worldPos);
                    }
                }
            }
        }

        Debug.Log($"Found {validSpawnPositions.Count} valid spawn points.");
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (validSpawnPositions.Count > 0)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = GetPooledEnemy();

        if (enemy != null)
        {
            // Pick a random valid tile position from our cache
            Vector3 spawnPos = validSpawnPositions[Random.Range(0, validSpawnPositions.Count)];

            enemy.transform.position = spawnPos;
            enemy.SetActive(true);
        }
    }

    private GameObject GetPooledEnemy()
    {
        foreach (GameObject e in enemyPool)
        {
            if (!e.activeInHierarchy) return e;
        }

        if (enemyPool.Count < maxEnemies)
        {
            GameObject newObj = Instantiate(enemyPrefab);
            newObj.SetActive(false);
            enemyPool.Add(newObj);
            return newObj;
        }

        return null;
    }
}