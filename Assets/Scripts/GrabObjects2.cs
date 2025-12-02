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
    public float throwPower = 0.15f;
    public float torquePower = 0.05f;
    public float maxThrowSpeed = 18f;
    public float smoothing = 0.15f;

    [Header("Damage")]
    public int damage = 10;          // Damage dealt to enemies
    public Collider damageCollider;       // Trigger collider for damage only

    private Rigidbody rb;
    private bool isHeld = false;

    private Transform currentHand;
    private Vector3 currentPosOffset;
    private Vector3 currentRotOffset;

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
            HandleMovement();
        }
        else
        {
            TryGrabInput();
        }
    }

    private void HandleMovement()
    {
        // --- Velocity (smoothed & safe) ---
        Vector3 deltaPos = currentHand.position - lastPosition;

        Vector3 rawVel = Vector3.zero;
        if (Time.deltaTime > 0.0001f)
            rawVel = deltaPos / Time.deltaTime;

        velocity = Vector3.Lerp(velocity, rawVel, 1f - smoothing);
        if (!IsValidVector(velocity)) velocity = Vector3.zero;

        // --- Angular velocity ---
        Quaternion deltaRotation = currentHand.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        if (Time.deltaTime > 0.0001f)
            angularVelocity = axis * (angle * Mathf.Deg2Rad / Time.deltaTime);
        else
            angularVelocity = Vector3.zero;

        if (!IsValidVector(angularVelocity)) angularVelocity = Vector3.zero;

        lastPosition = currentHand.position;
        lastRotation = currentHand.rotation;

        // Drop input
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
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
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            float dist = Vector3.Distance(transform.position, rightHandAnchor.position);
            if (dist <= grabRange)
                Grab(rightHandAnchor, rightPositionOffset, rightRotationOffset);
        }
    }

    private void Grab(Transform hand, Vector3 posOffset, Vector3 rotOffset)
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

    private void Drop()
    {
        isHeld = false;

        if (rb != null)
        {
            rb.isKinematic = false;

            if (!IsValidVector(velocity)) velocity = Vector3.zero;
            velocity = Vector3.ClampMagnitude(velocity, maxThrowSpeed);

            if (!IsValidVector(angularVelocity)) angularVelocity = Vector3.zero;

            rb.AddForce(velocity * throwPower, ForceMode.Impulse);
            rb.AddTorque(angularVelocity * torquePower, ForceMode.Impulse);
        }

        currentHand = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Auto-return to hand
        if (!isHeld && collision.gameObject.CompareTag(floorTag))
        {
            StartCoroutine(ReturnToHand());
        }

        // Damage enemies
        EnemyHealth enemy = collision.collider.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Enemy hit for " + damage + " damage!");
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

    bool IsValidVector(Vector3 v)
    {
        return !(float.IsNaN(v.x) || float.IsInfinity(v.x) ||
                 float.IsNaN(v.y) || float.IsInfinity(v.y) ||
                 float.IsNaN(v.z) || float.IsInfinity(v.z));
    }
}
