using UnityEngine;
using TMPro;
using System.Collections;

public class RoundUIController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI roundStartText;
    public TextMeshProUGUI roundIndicatorText;

    [Header("Timing")]
    public float fadeInTime = 1f;
    public float holdTime = 2f;
    public float fadeOutTime = 1f;

    void Awake()
    {
        SetAlpha(roundStartText, 0f);
        roundStartText.gameObject.SetActive(false);

        roundIndicatorText.gameObject.SetActive(false);
    }

    public void PlayRoundIntro(int roundNumber)
    {
        StartCoroutine(RoundSequence(roundNumber));
    }

    IEnumerator RoundSequence(int roundNumber)
    {
        roundStartText.text = $"ROUND {roundNumber}";
        roundIndicatorText.text = $"Round {roundNumber}";

        roundStartText.gameObject.SetActive(true);

        // Fade in
        yield return Fade(roundStartText, 0f, 1f, fadeInTime);

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade out
        yield return Fade(roundStartText, 1f, 0f, fadeOutTime);

        roundStartText.gameObject.SetActive(false);
        roundIndicatorText.gameObject.SetActive(true);
    }

    IEnumerator Fade(TextMeshProUGUI text, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetAlpha(text, Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetAlpha(text, to);
    }

    void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
