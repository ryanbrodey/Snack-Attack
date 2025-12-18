using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButtonFlash : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Button startButton;

    [Header("Start Menu Objects")]
    public GameObject startMenuCanvas;     // Canvas with title, button, filters
    public Camera startMenuCamera;          // Camera with blur / post-processing

    [Header("Timing")]
    public float initialDelay = 5f;
    public float flashInterval = 0.5f;

    [Header("Flash look")]
    [Range(0f, 1f)] public float offAlpha = 0f;
    public bool keepClickableWhileFlashing = true;

    CanvasGroup cg;
    Coroutine flashRoutine;

    void Awake()
    {
        if (startButton == null)
        {
            Debug.LogError("StartButtonFlash: Please assign Start Button.");
            enabled = false;
            return;
        }

        cg = startButton.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = startButton.gameObject.AddComponent<CanvasGroup>();

        // Hook up click
        startButton.onClick.AddListener(OnStartPressed);
    }

    System.Collections.IEnumerator Start()
    {
        // Hide initially
        SetVisible(false);

        // Wait before showing
        yield return new WaitForSeconds(initialDelay);

        // Show, then flash
        SetVisible(true);

        flashRoutine = StartCoroutine(FlashLoop());
    }

    System.Collections.IEnumerator FlashLoop()
    {
        while (true)
        {
            cg.alpha = (cg.alpha > 0.9f) ? offAlpha : 1f;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    void SetVisible(bool visible)
    {
        cg.alpha = visible ? 1f : 0f;

        bool interact = keepClickableWhileFlashing ? true : visible;
        cg.interactable = interact;
        cg.blocksRaycasts = interact;
    }

    // 🔥 THIS IS THE IMPORTANT PART 🔥
    void OnStartPressed()
    {
        // Stop flashing
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        // Disable UI
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        // Disable start menu camera (removes blur / filters)
        if (startMenuCamera != null)
            startMenuCamera.enabled = false;

        // Optional (later, not required now):
        // SceneManager.UnloadSceneAsync("StartMenu");
    }
}
