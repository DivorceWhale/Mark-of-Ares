using UnityEngine;
using System.Collections;

public class GrabObjects2 : MonoBehaviour
{
    [Header("Right Hand Anchor")]
    public Transform rightHandAnchor;

    [Header("Grab Settings")]
    public float grabRange = 0.2f;

    [Header("Right Hand Offset")]
    public Vector3 rightPositionOffset = Vector3.zero;
    public Vector3 rightRotationOffset = Vector3.zero;

    [Header("Auto-Return")]
    public string floorTag = "Floor";
    public float returnDelay = 0.5f;

    [Header("Throw Tuning")]
    public float throwPower = 0.15f;          // Impulse strength
    public float torquePower = 0.05f;         // Angular impulse strength
    public float maxThrowSpeed = 18f;         // Prevents superhuman throws
    public float smoothing = 0.15f;           // Smoothing for controller velocity

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
            // --- Velocity (smoothed & safe) ---
            Vector3 deltaPos = currentHand.position - lastPosition;

            Vector3 rawVel = Vector3.zero;
            if (Time.deltaTime > 0.0001f)
                rawVel = deltaPos / Time.deltaTime;

            // Lerp smoothing
            velocity = Vector3.Lerp(velocity, rawVel, 1f - smoothing);

            if (!IsValidVector(velocity))
                velocity = Vector3.zero;

            // --- Angular velocity ---
            Quaternion deltaRotation = currentHand.rotation * Quaternion.Inverse(lastRotation);
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            if (Time.deltaTime > 0.0001f)
                angularVelocity = axis * (angle * Mathf.Deg2Rad / Time.deltaTime);
            else
                angularVelocity = Vector3.zero;

            if (!IsValidVector(angularVelocity))
                angularVelocity = Vector3.zero;

            lastPosition = currentHand.position;
            lastRotation = currentHand.rotation;

            // Drop trigger
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Drop();
                return; // Prevent continuing with null hand
            }

            // Follow hand
            transform.position = currentHand.position + currentHand.TransformVector(currentPosOffset);
            transform.rotation = currentHand.rotation * Quaternion.Euler(currentRotOffset);
        }
        else
        {
            // Try grab
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                TryGrab(rightHandAnchor, rightPositionOffset, rightRotationOffset);
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

            // Validate and clamp velocity
            if (!IsValidVector(velocity)) velocity = Vector3.zero;
            velocity = Vector3.ClampMagnitude(velocity, maxThrowSpeed);

            if (!IsValidVector(angularVelocity)) angularVelocity = Vector3.zero;

            // === REALISTIC IMPULSE-BASED THROW ===
            rb.AddForce(velocity * throwPower, ForceMode.Impulse);
            rb.AddTorque(angularVelocity * torquePower, ForceMode.Impulse);
        }

        currentHand = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHeld && collision.gameObject.CompareTag(floorTag))
        {
            StartCoroutine(ReturnToHand());
        }
    }

    private IEnumerator ReturnToHand()
    {
        yield return new WaitForSeconds(returnDelay);

        Grab(rightHandAnchor, rightPositionOffset, rightRotationOffset);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Vector validation helper
    bool IsValidVector(Vector3 v)
    {
        return !(float.IsNaN(v.x) || float.IsInfinity(v.x) ||
                 float.IsNaN(v.y) || float.IsInfinity(v.y) ||
                 float.IsNaN(v.z) || float.IsInfinity(v.z));
    }
}
