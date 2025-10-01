using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    [Header("Hand Anchors")]
    public Transform rightHandAnchor;
    public Transform leftHandAnchor;

    [Header("Grab Settings")]
    public float grabRange = 0.2f; // How close hand must be

    [Header("Right Hand Offset")]
    public Vector3 rightPositionOffset = Vector3.zero;
    public Vector3 rightRotationOffset = Vector3.zero;

    [Header("Left Hand Offset")]
    public Vector3 leftPositionOffset = Vector3.zero;
    public Vector3 leftRotationOffset = Vector3.zero;

    private Rigidbody rb;
    private bool isHeld = false;

    private Transform currentHand;
    private Vector3 currentPosOffset;
    private Vector3 currentRotOffset;

    // For throwing
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 velocity;
    private Vector3 angularVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isHeld && currentHand != null)
        {
            // Track hand velocity for throwing
            velocity = (currentHand.position - lastPosition) / Time.deltaTime;

            Quaternion deltaRotation = currentHand.rotation * Quaternion.Inverse(lastRotation);
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            angularVelocity = axis * (angle * Mathf.Deg2Rad / Time.deltaTime);

            lastPosition = currentHand.position;
            lastRotation = currentHand.rotation;

            // Drop input (trigger)
            if ((currentHand == rightHandAnchor && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) ||
                (currentHand == leftHandAnchor && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)))
            {
                Drop();
            }

            // Update position & rotation
            transform.position = currentHand.position + currentHand.TransformVector(currentPosOffset);
            transform.rotation = currentHand.rotation * Quaternion.Euler(currentRotOffset);
        }
        else
        {
            // Try grab right hand
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                TryGrab(rightHandAnchor, rightPositionOffset, rightRotationOffset);
            }
            // Try grab left hand
            else if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                TryGrab(leftHandAnchor, leftPositionOffset, leftRotationOffset);
            }
        }
    }

    void TryGrab(Transform hand, Vector3 posOffset, Vector3 rotOffset)
    {
        if (hand == null) return;

        float dist = Vector3.Distance(transform.position, hand.position);
        if (dist <= grabRange)
        {
            Grab(hand, posOffset, rotOffset);
        }
    }

    void Grab(Transform hand, Vector3 posOffset, Vector3 rotOffset)
    {
        isHeld = true;
        currentHand = hand;
        currentPosOffset = posOffset;
        currentRotOffset = rotOffset;

        if (rb != null)
            rb.isKinematic = true;

        lastPosition = hand.position;
        lastRotation = hand.rotation;
    }

    void Drop()
    {
        isHeld = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;
        }

        currentHand = null;
    }
}
