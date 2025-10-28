using UnityEngine;

public class VRUIFollow : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerHead;      // Usually the CenterEyeAnchor
    public float distanceFromHead = 0.5f;   // Distance in meters
    public Vector3 localOffset = Vector3.zero; // Optional offset for positioning

    void LateUpdate()
    {
        if (playerHead == null) return;

        // Fully follow player position (including up/down)
        transform.position = playerHead.position + playerHead.forward * distanceFromHead + localOffset;

        // Rotate to match player's full head orientation
        transform.rotation = Quaternion.LookRotation(playerHead.forward, playerHead.up);
    }
}
