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
            enabled = false;
            return;
        }

        cg = startButton.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = startButton.gameObject.AddComponent<CanvasGroup>();

        startButton.onClick.AddListener(OnStartPressed);
    }

    System.Collections.IEnumerator Start()
    {
        SetVisible(false);
        yield return new WaitForSeconds(initialDelay);
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

    // Start button clicked
    void OnStartPressed()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        if (startMenuCamera != null)
            startMenuCamera.enabled = false;
    }
}
