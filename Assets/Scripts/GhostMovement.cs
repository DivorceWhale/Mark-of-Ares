using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Header("Floating Movement")]
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;

    [Header("Drifting Movement")]
    public float moveSpeed = 1f;
    public float driftRadius = 3f;

    [Header("Rotation (Ghostly Spin)")]
    public float rotationSpeed = 15f; // degrees per second
    public Vector3 rotationAxis = new Vector3(0, 1, 0.2f); // slightly off-axis for a natural feel

    [Header("Spawner Bounds (Optional)")]
    public string spawnerTag = "Spawner"; // Tag of the spawner in the scene
    private BoxCollider spawnerCollider;

    [Header("Height Limits")]
    public float minHeight = 1f;
    public float maxHeight = 3f;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;

        // Automatically find the spawner in the scene if not assigned
        GameObject spawnerObj = GameObject.FindWithTag(spawnerTag);
        if (spawnerObj != null)
            spawnerCollider = spawnerObj.GetComponent<BoxCollider>();

        PickNewTarget();
    }

    void Update()
    {
        // Floating height with min/max clamp
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        newY = Mathf.Clamp(newY, minHeight, maxHeight);

        // Drift toward target
        Vector3 horizontalTarget = new Vector3(targetPos.x, newY, targetPos.z);
        transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, moveSpeed * Time.deltaTime);

        // Gentle ghostly spin
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);

        // Pick new target if close
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(targetPos.x, 0, targetPos.z)) < 0.2f)
        {
            PickNewTarget();
        }

        // Clamp X and Z inside spawner bounds
        if (spawnerCollider != null)
        {
            Bounds b = spawnerCollider.bounds;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, b.min.x, b.max.x);
            pos.z = Mathf.Clamp(pos.z, b.min.z, b.max.z);
            transform.position = pos;
        }
    }

    void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * driftRadius;
        targetPos = new Vector3(startPos.x + randomCircle.x, startPos.y, startPos.z + randomCircle.y);

        // Ensure target stays inside spawner bounds
        if (spawnerCollider != null)
        {
            Bounds b = spawnerCollider.bounds;
            targetPos.x = Mathf.Clamp(targetPos.x, b.min.x, b.max.x);
            targetPos.z = Mathf.Clamp(targetPos.z, b.min.z, b.max.z);
        }
    }
}
