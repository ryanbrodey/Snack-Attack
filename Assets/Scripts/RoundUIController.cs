using UnityEngine;
using TMPro;
using System.Collections;

public class RoundUIController : MonoBehaviour
{
    public TextMeshProUGUI centerText;
    public TextMeshProUGUI cornerText;

    private void Awake()
    {
        SetAlpha(centerText, 0f);
        SetAlpha(cornerText, 0f);
    }

    public void PlayRoundIntro(int round)
    {
        StopAllCoroutines();
        StartCoroutine(RoundSequence(round));
    }

    private IEnumerator RoundSequence(int round)
    {
        centerText.text = $"ROUND {round}";
        cornerText.text = $"Round {round}";

        // Fade in big center text
        yield return Fade(centerText, 0f, 1f, 1f);

        // Hold
        yield return new WaitForSeconds(2f);

        // Fade out
        yield return Fade(centerText, 1f, 0f, 1f);

        // Show small corner indicator
        SetAlpha(cornerText, 1f);
    }

    private IEnumerator Fade(TextMeshProUGUI text, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(text, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        SetAlpha(text, to);
    }

    private void SetAlpha(TextMeshProUGUI text, float a)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = a;
        text.color = c;
    }
}
