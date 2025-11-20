using UnityEngine;

public class FreezeCamera : MonoBehaviour
{
    public bool freeze = false;

    private Quaternion frozenRot;

    void Start()
    {
        // Save starting rotation
        frozenRot = transform.rotation;
    }

    void LateUpdate()
    {
        if (freeze)
        {
            // Lock rotation
            transform.rotation = frozenRot;
        }
        else
        {
            // Update stored rotation for when player regains control
            frozenRot = transform.rotation;
        }
    }
}
