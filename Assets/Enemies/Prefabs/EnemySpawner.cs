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
    [SerializeField] private float spawnInterval = 5f;   // seconds between spawns
    [SerializeField] private int maxAliveEnemies = 10;   // cap on enemies alive
    [Tooltip("-1 = infinite enemies")]
    [SerializeField] private int totalToSpawn = -1;      // total enemies over whole game

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int totalSpawned = 0;
    private bool spawning = true;

    private void Start()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner is not configured correctly!", this);
            enabled = false;
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            // Clean out destroyed enemies
            aliveEnemies.RemoveAll(e => e == null);

            bool underAliveLimit = aliveEnemies.Count < maxAliveEnemies;
            bool underTotalLimit = (totalToSpawn < 0) || (totalSpawned < totalToSpawn);

            if (underAliveLimit && underTotalLimit)
            {
                SpawnOneEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOneEnemy()
    {
        // pick a random spawn point
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, point.position, point.rotation);
        aliveEnemies.Add(enemy);
        totalSpawned++;
    }

    // Optional: allow other scripts to stop spawning (e.g. when wave ends)
    public void StopSpawning()
    {
        spawning = false;
    }

    public void StartSpawning()
    {
        if (!spawning)
        {
            spawning = true;
            StartCoroutine(SpawnLoop());
        }
    }
}

