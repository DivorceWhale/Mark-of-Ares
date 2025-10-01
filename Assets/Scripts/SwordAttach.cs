using UnityEngine;

public class SwordAttach : MonoBehaviour
{
    [Header("References")]
    public Transform rightHandAnchor;
    public GameObject swordPrefab;

    [Header("Offset Settings")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    private GameObject swordInstance;
    private bool isHoldingSword = true;

    void Start()
    {
        // Spawn sword and attach it
        AttachSword();
    }

    void Update()
    {
        // Check if holding sword and trigger is pressed -> drop sword
        if (isHoldingSword && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            DropSword();
        }
    }

    void AttachSword()
    {
        swordInstance = Instantiate(swordPrefab, rightHandAnchor);
        swordInstance.transform.localPosition = positionOffset;
        swordInstance.transform.localRotation = Quaternion.Euler(rotationOffset);

        // Ensure physics is disabled while held
        Rigidbody rb = swordInstance.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isHoldingSword = true;
    }

    void DropSword()
    {
        // Unparent so it falls
        swordInstance.transform.parent = null;

        // Enable physics
        Rigidbody rb = swordInstance.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        isHoldingSword = false;
    }
}
