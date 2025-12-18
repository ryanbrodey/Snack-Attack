using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawners")]
    public EnemySpawner chiliSpawner;
    public EnemySpawner kiwiSpawner;

    [Header("UI")]
    public RoundUIController roundUI;

    private int enemiesKilled = 0;

    public void StartRound1()
    {
        enemiesKilled = 0;

        if (roundUI != null)
            roundUI.PlayRoundIntro(1);

        chiliSpawner.Init(this);
        chiliSpawner.StartSpawning();
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;

        if (enemiesKilled >= 10)
        {
            chiliSpawner.StopSpawning();
            StartRound2();
        }
    }

    private void StartRound2()
    {
        enemiesKilled = 0;
        
        if (roundUI != null)
            roundUI.PlayRoundIntro(2);

        kiwiSpawner.Init(this);
        kiwiSpawner.StartSpawning();
    }
}
