using UnityEngine;

public class EnemyDeathReporter : MonoBehaviour
{
    private WaveManager waveManager;

    public void Init(WaveManager manager)
    {
        waveManager = manager;
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }
    }
}
