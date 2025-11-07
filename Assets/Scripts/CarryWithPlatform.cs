using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CarryWithPlatform : MonoBehaviour
{
    [Header("Detection")]
    public string platformTag = "MovingPlatform";
    public float groundCheckAngle = 50f; // treat hits with normal.y above cos(angle) as ground

    private CharacterController cc;
    private PlatformMover currentPlatform;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        // Apply platform delta AFTER your regular movement each frame
        if (currentPlatform != null)
        {
            // Convert delta from platform space to world (already world), then move
            Vector3 delta = currentPlatform.FrameDelta;
            if (delta.sqrMagnitude > 0f)
                cc.Move(delta);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Is this a platform and roughly below us (ground-like)?
        if (hit.collider.CompareTag(platformTag))
        {
            Vector3 n = hit.normal;
            float upDot = Vector3.Dot(n, Vector3.up);
            if (upDot > Mathf.Cos(groundCheckAngle * Mathf.Deg2Rad))
            {
                currentPlatform = hit.collider.GetComponent<PlatformMover>();
                return;
            }
        }

        // if we hit something else that isn't the platform under our feet, don't clear immediately
        // clearing is done if we detect we're not grounded on the platform in the next frame
    }

    void FixedUpdate()
    {
        // If we’re no longer touching the platform this physics step, clear it
        if (currentPlatform != null)
        {
            // Simple heuristic: if the capsule bottom is above the platform by a gap, or not grounded
            if (!cc.isGrounded)
                currentPlatform = null;
        }
    }
}
