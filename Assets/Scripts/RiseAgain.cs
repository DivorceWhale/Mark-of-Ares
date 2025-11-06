using UnityEngine;
using TMPro;

public class RiseAgainUI : MonoBehaviour
{
    public CanvasGroup group;
    public TextMeshProUGUI messageText;
    [Range(0.1f, 3f)] public float fadeTime = 0.35f;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        group.alpha = 0f; // start hidden
    }

    public void Show(string msg, float holdSeconds = 1.25f)
    {
        if (messageText) messageText.text = msg;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(holdSeconds));
    }

    System.Collections.IEnumerator FadeRoutine(float hold)
    {
        // fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }
        group.alpha = 1f;

        yield return new WaitForSeconds(hold);

        // fade out
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
        group.alpha = 0f;
    }
}
