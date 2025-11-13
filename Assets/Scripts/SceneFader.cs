using UnityEngine;
using System;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup cg;

    public void InstantSet(float alpha)
    {
        if (!cg) return;
        cg.alpha = alpha;
        cg.blocksRaycasts = alpha > 0.99f;
        cg.interactable = cg.blocksRaycasts;
    }

    public void FadeTo(float target, float duration, Action onDone = null)
    {
        StartCoroutine(FadeRoutine(target, duration, onDone));
    }

    IEnumerator FadeRoutine(float target, float duration, Action onDone)
    {
        if (!cg) yield break;
        float start = cg.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            cg.blocksRaycasts = cg.alpha > 0.01f;
            cg.interactable = cg.blocksRaycasts;
            yield return null;
        }
        cg.alpha = target;
        cg.blocksRaycasts = target > 0.01f;
        cg.interactable = cg.blocksRaycasts;
        onDone?.Invoke();
    }
}
