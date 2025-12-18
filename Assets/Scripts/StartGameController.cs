using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject startMenuCanvas;
    public Camera startMenuCamera;        // MenuCam (blur)
    public Camera startMenuMainCamera;    // Wall-facing camera

    private GameObject player;

    void Start()
    {
        // Find inactive player in Map scene
        Scene mapScene = SceneManager.GetSceneByName("Map");

        if (!mapScene.isLoaded)
        {
            Debug.LogError("Map scene is not loaded!");
            return;
        }

        foreach (GameObject obj in mapScene.GetRootGameObjects())
        {
            if (obj.CompareTag("Player"))
            {
                player = obj;
                break;
            }
        }

        if (player == null)
        {
            Debug.LogError("Player not found in Map scene.");
            return;
        }

        // Ensure player starts OFF
        player.SetActive(false);
    }

    // Called by Start button
    public void StartGame()
    {
        // Hide menu UI
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        // Disable BOTH menu cameras
        if (startMenuCamera != null)
            startMenuCamera.enabled = false;

        if (startMenuMainCamera != null)
            startMenuMainCamera.enabled = false;

        // Activate player
        if (player != null)
            player.SetActive(true);

        // ✅ START GAME VIA WAVEMANAGER ONLY
        Scene spawnerScene = SceneManager.GetSceneByName("Spawners");

        if (!spawnerScene.isLoaded)
        {
            Debug.LogWarning("Spawners scene is not loaded.");
            return;
        }

        foreach (GameObject obj in spawnerScene.GetRootGameObjects())
        {
            WaveManager waveManager = obj.GetComponent<WaveManager>();
            if (waveManager != null)
            {
                waveManager.StartRound1();
                Debug.Log("WaveManager: Round 1 started");
                break;
            }
        }
    }
}
