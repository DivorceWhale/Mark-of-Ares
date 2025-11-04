using UnityEngine;

public class SimpleFallRespawn : MonoBehaviour
{
    [Tooltip("Where the player will respawn.")]
    public Transform respawnPoint;

    [Tooltip("Y position threshold that triggers respawn.")]
    public float fallY = -10f;

    void Update()
    {
        if (transform.position.y < fallY)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("No respawn point assigned!");
            return;
        }

        // Reset position and rotation
        transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        // If using Rigidbody, stop velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Player respawned.");
    }
}
