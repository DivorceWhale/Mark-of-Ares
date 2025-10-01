using UnityEngine;

public class ShieldAttach : MonoBehaviour
{
    [Header("References")]
    public Transform leftHandAnchor;
    public GameObject shieldPrefab;

    [Header("Offset Settings")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    private GameObject shieldInstance;

    void Start()
    {
        if (leftHandAnchor == null || shieldPrefab == null)
        {

            return;
        }

        // Spawn and parent to hand anchor
        shieldInstance = Instantiate(shieldPrefab, leftHandAnchor);

        // Apply offsets
        shieldInstance.transform.localPosition = positionOffset;
        shieldInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
    }
}
