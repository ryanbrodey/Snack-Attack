using UnityEngine;
using TMPro;
using System.Collections;

public class TitleFade : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    
    [Header("Fade Settings")]
    public float delayBeforeFade = 1f;  // time before fading in
    public float fadeDuration = 1f;     // fade time

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        // Start fully invisible
        Color c = tmp.color;
        c.a = 0f;
        tmp.color = c;

        // Begin fade animation
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Wait before fading
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            Color c = tmp.color;
            c.a = t;   // fade alpha up
            tmp.color = c;

            yield return null;
        }

        // Ensure alpha ends at full
        Color final = tmp.color;
        final.a = 1f;
        tmp.color = final;
    }
}
