using UnityEngine;

public class StartGameController : MonoBehaviour
{
    [Header("Start Menu Objects")]
    public Canvas startMenuCanvas;
    public Camera startMenuCamera;

    [Header("Player")]
    public string playerTag = "Player";

    GameObject player;

    void Start()
    {
        // Player starts disabled
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
            player.SetActive(false);
    }

    public void StartGame()
    {
        // Enable player (activates player camera)
        if (player == null)
            player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
            player.SetActive(true);

        // Disable start menu visuals
        if (startMenuCanvas != null)
            startMenuCanvas.enabled = false;

        if (startMenuCamera != null)
            startMenuCamera.gameObject.SetActive(false);

        // Optional: unlock mouse if needed
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
