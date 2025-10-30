using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveDirection = Vector3.right; // Direction to move
    public float moveDistance = 5f;               // Distance to move
    public float moveSpeed = 2f;                  // Speed of movement

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
    }

    void Update()
    {
        // Move between start and target positions
        if (movingToTarget)
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);

        // Switch directions when reaching the end points
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            movingToTarget = false;
        else if (Vector3.Distance(transform.position, startPos) < 0.01f)
            movingToTarget = true;
    }

    // Optional: Draw gizmos in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection.normalized * moveDistance);
    }
}
