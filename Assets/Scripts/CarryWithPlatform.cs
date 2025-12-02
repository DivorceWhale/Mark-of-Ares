using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlatformRider : MonoBehaviour
{
    [Header("Platform Setup")]
    public string platformTag = "MovingPlatform";

    private CharacterController cc;
    private Transform currentPlatform;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Only care about things tagged as moving platforms
        if (!hit.collider.CompareTag(platformTag))
            return;

        // We only want to "stand" on top, not on the side
        if (hit.normal.y > 0.5f) // surface is mostly facing up
        {
            currentPlatform = hit.collider.transform;
            transform.SetParent(currentPlatform);
        }
    }

    void Update()
    {
        // If we had a platform but we're no longer grounded, let go
        if (currentPlatform != null && !cc.isGrounded)
        {
            transform.SetParent(null);
            currentPlatform = null;
        }
    }

    void OnDisable()
    {
        // Safety: unparent if player object is disabled
        if (transform.parent != null)
            transform.SetParent(null);
        currentPlatform = null;
    }
}
