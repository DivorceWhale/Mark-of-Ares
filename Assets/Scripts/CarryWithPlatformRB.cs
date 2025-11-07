using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarryWithPlatformRB : MonoBehaviour
{
    public string platformTag = "MovingPlatform";
    public float stickDotMin = 0.5f; // require some upward normal

    private Rigidbody rb;
    private PlatformMover currentPlatform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true; // usual for characters
    }

    void FixedUpdate()
    {
        if (currentPlatform != null)
        {
            // Add platform velocity so player keeps up
            rb.linearVelocity += currentPlatform.Velocity;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag(platformTag))
        {
            // Check any contact with upward-ish normal to consider "standing on"
            foreach (var c in collision.contacts)
            {
                if (Vector3.Dot(c.normal, Vector3.up) > stickDotMin)
                {
                    currentPlatform = collision.collider.GetComponent<PlatformMover>();
                    return;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (currentPlatform != null && collision.collider.gameObject == currentPlatform.gameObject)
            currentPlatform = null;
    }
}
