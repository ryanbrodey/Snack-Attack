using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Where to spawn")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawning settings")]
    [SerializeField] private float spawnInterval = 2f;   // seconds between spawns
    [SerializeField] private int maxAliveEnemies = 10;   // max alive at once
    [SerializeField] private int totalToSpawn = 30;      // TOTAL enemies

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int totalSpawned = 0;
    private bool spawning = false;

    private void Start()
    {
        // Validate setup only (DO NOT start spawning here)
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner is not configured correctly!", this);
            enabled = false;
        }
    }

    public void StartSpawning()
    {
        if (!spawning)
        {
            spawning = true;
            StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            // Remove dead enemies
            aliveEnemies.RemoveAll(e => e == null);

            bool underAliveLimit = aliveEnemies.Count < maxAliveEnemies;
            bool underTotalLimit = totalSpawned < totalToSpawn;

            if (underAliveLimit && underTotalLimit)
            {
                SpawnOneEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOneEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, point.position, point.rotation);
        aliveEnemies.Add(enemy);
        totalSpawned++;
    }
}
