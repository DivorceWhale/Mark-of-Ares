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

    private Vector3 lastPosition; // For tracking platform movement

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Move between start and target positions
        if (movingToTarget)
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);

        // Switch directions when reaching the endpoints
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            movingToTarget = false;
        else if (Vector3.Distance(transform.position, startPos) < 0.01f)
            movingToTarget = true;

        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If player stands on platform, make player a child
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Remove parent when player leaves the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    // Optional: Draw gizmos in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection.normalized * moveDistance);
    }
}
