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
        // Find player in map scene
        Scene mapScene = SceneManager.GetSceneByName("Map");

        if (!mapScene.isLoaded) return;

        foreach (GameObject obj in mapScene.GetRootGameObjects())
        {
            if (obj.CompareTag("Player"))
            {
                player = obj;
                break;
            }
        }

        if (player != null)
        {
            player.SetActive(false);
        }
    }

    public void StartGame()
    {
        // Hide menu
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        if (startMenuCamera != null)
            startMenuCamera.enabled = false;

        if (startMenuMainCamera != null)
            startMenuMainCamera.enabled = false;

        if (player != null)
            player.SetActive(true);

        // Start wave manager
        Scene spawnerScene = SceneManager.GetSceneByName("Spawners");
        if (!spawnerScene.isLoaded) return;

        foreach (GameObject obj in spawnerScene.GetRootGameObjects())
        {
            WaveManager waveManager = obj.GetComponent<WaveManager>();
            if (waveManager != null)
            {
                waveManager.StartRound1();
                break;
            }
        }
    }
}
