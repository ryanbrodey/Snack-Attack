using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("Spawners")]
    public EnemySpawner chiliSpawner;
    public EnemySpawner kiwiSpawner;
    public EnemySpawner chickSpawner;   // 🆕 Round 3

    [Header("UI")]
    public RoundUIController roundUI;

    private int enemiesKilled = 0;
    private int currentRound = 0;

    // ---------- ROUND 1 ----------
    public void StartRound1()
    {
        currentRound = 1;
        enemiesKilled = 0;

        roundUI?.PlayRoundIntro(1);

        chiliSpawner.Init(this);
        chiliSpawner.StartSpawning();
    }

    // ---------- DEATH CALLBACK ----------
    public void OnEnemyKilled()
    {
        enemiesKilled++;

        if (currentRound == 1 && enemiesKilled >= 10)
        {
            chiliSpawner.StopSpawning();
            StartRound2();
        }
        else if (currentRound == 2 && enemiesKilled >= 10)
        {
            kiwiSpawner.StopSpawning();
            StartRound3();
        }
        else if (currentRound == 3 && enemiesKilled >= 10)
        {
            chickSpawner.StopSpawning();
            SceneManager.LoadScene("EndMenu");
        }
    }

    // ---------- ROUND 2 ----------
    private void StartRound2()
    {
        currentRound = 2;
        enemiesKilled = 0;

        roundUI?.PlayRoundIntro(2);

        kiwiSpawner.Init(this);
        kiwiSpawner.StartSpawning();
    }

    // ---------- ROUND 3 ----------
    private void StartRound3()
    {
        currentRound = 3;
        enemiesKilled = 0;

        roundUI?.PlayRoundIntro(3);

        chickSpawner.Init(this);
        chickSpawner.StartSpawning();
    }
}
