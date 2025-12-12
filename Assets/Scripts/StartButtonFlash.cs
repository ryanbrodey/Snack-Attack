using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartButtonFlash : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Button startButton;

    [Header("Timing")]
    public float initialDelay = 3f;
    public float flashInterval = 0.5f;
    public int flashCount = 3;  // Number of times to flash

    [Header("Flash look")]
    [Range(0f, 1f)] public float offAlpha = 0f;   // 0 = invisible, try 0.3 for "pulse"
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

    IEnumerator Start()
    {
        // Hide initially
        SetVisible(false);

        // Wait before showing
        yield return new WaitForSeconds(initialDelay);

        // Flash the button the specified number of times
        for (int i = 0; i < flashCount; i++)
        {
            // Flash ON
            cg.alpha = 1f;
            yield return new WaitForSeconds(flashInterval);
            
            // Flash OFF
            cg.alpha = offAlpha;
            yield return new WaitForSeconds(flashInterval);
        }

        // After flashing, make button fully visible and stay visible
        SetVisible(true);
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