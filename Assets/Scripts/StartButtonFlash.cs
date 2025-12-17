using UnityEngine;
using UnityEngine.UI;

public class StartButtonFlash : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Button startButton;

    [Header("Timing")]
    public float initialDelay = 5f;
    public float flashInterval = 0.5f;

    [Header("Flash look")]
    [Range(0f, 1f)] public float offAlpha = 0f;   // 0 = invisible, try 0.3 for “pulse”
    public bool keepClickableWhileFlashing = true;

    CanvasGroup cg;

    void Awake()
    {
        if (startButton == null)
        {
            Debug.LogError("StartButtonFlash: Please assign Start Button.");
            enabled = false; 
            return;
        }

        cg = startButton.GetComponent<CanvasGroup>();
        if (cg == null) cg = startButton.gameObject.AddComponent<CanvasGroup>();
    }

    System.Collections.IEnumerator Start()
    {
        // Hide initially
        SetVisible(false);

        // Wait before showing
        yield return new WaitForSeconds(initialDelay);

        // Show, then flash
        SetVisible(true);

        while (true)
        {
            // Toggle alpha
            cg.alpha = (cg.alpha > 0.9f) ? offAlpha : 1f;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    void SetVisible(bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        // You can keep it clickable while flashing if you like:
        bool interact = keepClickableWhileFlashing ? true : visible;
        cg.interactable = interact;
        cg.blocksRaycasts = interact;
    }
}
