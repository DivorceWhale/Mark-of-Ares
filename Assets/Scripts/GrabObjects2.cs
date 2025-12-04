using UnityEngine;
using System.Collections;

public class GrabObjects2 : MonoBehaviour
{
    [Header("Hand Anchors")]
    public Transform rightHandAnchor;
    public Transform leftHandAnchor;

    [Header("Grab Settings")]
    public float grabRange = 0.2f;

    [Header("Right Hand Offset")]
    public Vector3 rightPositionOffset = Vector3.zero;
    public Vector3 rightRotationOffset = Vector3.zero;

    [Header("Left Hand Offset")]
    public Vector3 leftPositionOffset = Vector3.zero;
    public Vector3 leftRotationOffset = Vector3.zero;

    [Header("Auto-Return")]
    public string floorTag = "Floor";
    public float returnDelay = 0.5f;

    [Header("Throw Tuning")]
    public float throwPower = 0.15f;
    public float torquePower = 0.05f;
    public float maxThrowSpeed = 18f;
    public float smoothing = 0.15f;

    [Header("Damage Settings")]
    public int damage = 10;
    public Collider damageTrigger;   // ← assign your mesh trigger here

    private Rigidbody rb;
    private bool isHeld = false;

    private Transform currentHand;
    private Transform lastUsedHand;
    private Vector3 currentPosOffset;
    private Vector3 currentRotOffset;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 velocity;
    private Vector3 angularVelocity;

    private bool hasBeenGrabbed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isHeld && currentHand != null)
        {
            HandleMovement();
        }
        else
        {
            TryGrabInput();
        }
    }

    private void HandleMovement()
    {
        // Calculate safe velocity
        Vector3 deltaPos = currentHand.position - lastPosition;
        Vector3 rawVel = Time.deltaTime > 0.0001f ? deltaPos / Time.deltaTime : Vector3.zero;
        velocity = Vector3.Lerp(velocity, rawVel, 1f - smoothing);

        Quaternion deltaRot = currentHand.rotation * Quaternion.Inverse(lastRotation);
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;
        angularVelocity = Time.deltaTime > 0.0001f ? axis * (angle * Mathf.Deg2Rad / Time.deltaTime) : Vector3.zero;

        lastPosition = currentHand.position;
        lastRotation = currentHand.rotation;

        // Drop Input
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) ||
            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Drop();
            return;
        }

        // Follow hand
        transform.position = currentHand.position + currentHand.TransformVector(currentPosOffset);
        transform.rotation = currentHand.rotation * Quaternion.Euler(currentRotOffset);
    }

    private void TryGrabInput()
    {
        // RIGHT HAND GRAB
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            if (Vector3.Distance(transform.position, rightHandAnchor.position) <= grabRange)
            {
                Grab(rightHandAnchor, rightPositionOffset, rightRotationOffset);
                return;
            }
        }

        // LEFT HAND GRAB
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            if (Vector3.Distance(transform.position, leftHandAnchor.position) <= grabRange)
            {
                Grab(leftHandAnchor, leftPositionOffset, leftRotationOffset);
                return;
            }
        }
    }

    private void Grab(Transform hand, Vector3 posOffset, Vector3 rotOffset)
    {
        isHeld = true;
        hasBeenGrabbed = true;
        currentHand = hand;
        currentPosOffset = posOffset;
        currentRotOffset = rotOffset;

        lastUsedHand = hand;
        rb.isKinematic = true;

        lastPosition = hand.position;
        lastRotation = hand.rotation;
    }

    private void Drop()
    {
        isHeld = false;
        rb.isKinematic = false;

        velocity = Vector3.ClampMagnitude(velocity, maxThrowSpeed);

        rb.AddForce(velocity * throwPower, ForceMode.Impulse);
        rb.AddTorque(angularVelocity * torquePower, ForceMode.Impulse);

        currentHand = null;
    }

    // ---------------------- DAMAGE FROM TRIGGER ----------------------
    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Hit enemy! Damage dealt.");
        }
    }


    // ---------------------- PHYSICS COLLISION (floor, walls, etc.) ----------------------
    private void OnCollisionEnter(Collision collision)
    {
        // Only return after being grabbed once
        if (hasBeenGrabbed && !isHeld && collision.gameObject.CompareTag(floorTag))
        {
            StartCoroutine(ReturnToLastHand());
        }
    }

    private IEnumerator ReturnToLastHand()
    {
        if (lastUsedHand == null) yield break;

        yield return new WaitForSeconds(returnDelay);

        Grab(lastUsedHand,
            lastUsedHand == rightHandAnchor ? rightPositionOffset : leftPositionOffset,
            lastUsedHand == rightHandAnchor ? rightRotationOffset : leftRotationOffset);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
