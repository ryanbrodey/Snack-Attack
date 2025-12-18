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
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxAliveEnemies = 10;
    [SerializeField] private int totalToSpawn = 10;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int totalSpawned = 0;

    private bool spawning = false;   // 🔒 locked by default
    private Coroutine spawnRoutine;

    private WaveManager waveManager;

    public void Init(WaveManager manager)
    {
        waveManager = manager;
    }

    public void StartSpawning()
    {
        if (spawning) return;

        spawning = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        spawning = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            aliveEnemies.RemoveAll(e => e == null);

            if (aliveEnemies.Count < maxAliveEnemies &&
                totalSpawned < totalToSpawn)
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

        EnemyDeathReporter reporter = enemy.GetComponent<EnemyDeathReporter>();
        if (reporter != null && waveManager != null)
        {
            reporter.Init(waveManager);
        }
    }

    public bool IsWaveComplete()
    {
        aliveEnemies.RemoveAll(e => e == null);
        return totalSpawned >= totalToSpawn && aliveEnemies.Count == 0;
    }
}
