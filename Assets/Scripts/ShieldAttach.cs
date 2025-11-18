using UnityEngine;

public class ShieldAttach : MonoBehaviour
{
    [Header("References")]
    public Transform leftHandAnchor;
    public GameObject shieldPrefab;

    [Header("Offset Settings")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Collision Settings")]
    public LayerMask interactableLayers; // Layers the shield should collide with (e.g., projectiles, enemies)

    private GameObject shieldInstance;
    private Collider shieldCollider;
    private Rigidbody shieldRigidbody;

    void Start()
    {
        if (leftHandAnchor == null || shieldPrefab == null)
        {
            Debug.LogWarning("ShieldAttach: Missing reference!");
            return;
        }

        // Instantiate shield once
        shieldInstance = Instantiate(shieldPrefab, leftHandAnchor.position, leftHandAnchor.rotation);
        shieldInstance.transform.SetParent(leftHandAnchor, true);
        shieldInstance.transform.localPosition = positionOffset;
        shieldInstance.transform.localRotation = Quaternion.Euler(rotationOffset);

        // Setup Rigidbody
        shieldRigidbody = shieldInstance.GetComponent<Rigidbody>();
        if (shieldRigidbody == null)
            shieldRigidbody = shieldInstance.AddComponent<Rigidbody>();

        shieldRigidbody.isKinematic = true;      // Avoid physics overhead
        shieldRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous; // Good for fast projectiles

        // Setup Collider
        shieldCollider = shieldInstance.GetComponent<Collider>();
        if (shieldCollider == null)
            shieldCollider = shieldInstance.AddComponent<BoxCollider>();

        // Only collide with specified layers
        shieldCollider.gameObject.layer = LayerMaskToLayer(interactableLayers);
    }

    // Convert LayerMask to first layer in mask
    private int LayerMaskToLayer(LayerMask mask)
    {
        int layer = 0;
        int maskValue = mask.value;
        while (maskValue > 0)
        {
            if ((maskValue & 1) == 1)
                return layer;
            maskValue >>= 1;
            layer++;
        }
        return 0; // Default to layer 0 if none found
    }

    // Optional: detect hits
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & interactableLayers) != 0)
        {
            Debug.Log("Shield hit: " + collision.gameObject.name);
            // Here you can block or destroy the projectile
        }
    }
}
