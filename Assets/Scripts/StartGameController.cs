using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject startMenuCanvas;    // Start menu UI
    public Camera startMenuCamera;         // MenuCam
    public Camera startMenuMainCamera;     // Wall-facing camera

    GameObject player;

    void Start()
    {
        // Get the Map scene (it is already loaded additively)
        Scene mapScene = SceneManager.GetSceneByName("Map");

        if (!mapScene.isLoaded)
        {
            Debug.LogError("Map scene is not loaded!");
            return;
        }

        // Find the player EVEN IF IT IS INACTIVE
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

        // Make sure player starts OFF
        player.SetActive(false);
    }

    // Called when Start button is pressed
    public void StartGame()
    {
        // Hide menu UI
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        // Turn off BOTH StartMenu cameras
        if (startMenuCamera != null)
            startMenuCamera.enabled = false;

        if (startMenuMainCamera != null)
            startMenuMainCamera.enabled = false;

        // Turn on the player (activates its camera)
        if (player != null)
            player.SetActive(true);
    }
}
