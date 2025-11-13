using UnityEngine;
using System.Collections;

public class MomSimpleDeath : MonoBehaviour
{
    [Header("Death Animation")]
    public float fallDuration = 0.4f;               // How fast she falls
    public Vector3 deadEulerAngles = new Vector3(90f, 0f, 0f);  // Lying on her back
    public float delayBeforeFreeze = 0.1f;

    bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(deadEulerAngles);

        float t = 0f;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / fallDuration);
            transform.rotation = Quaternion.Slerp(startRot, endRot, lerp);
            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeFreeze);
        // She just stays there. If you want to fade her out later, you can do it here.
    }
}
