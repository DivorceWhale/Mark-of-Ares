using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 5f;
    public float moveSpeed = 2f;
    public bool pingPong = true;

    // Exposed read-only per-physics-step motion
    public Vector3 FrameDelta { get; private set; }
    public Vector3 Velocity { get; private set; }

    private Rigidbody rb;
    private Vector3 startPos;
    private float t; // normalized along the path 0..1
    private int dir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;     // important
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        startPos = transform.position;
        t = 0f;
    }

    void FixedUpdate()
    {
        // advance t
        float dt = moveSpeed * Time.fixedDeltaTime / Mathf.Max(moveDistance, 0.0001f);
        t += dir * dt;

        if (pingPong)
        {
            if (t >= 1f) { t = 1f; dir = -1; }
            else if (t <= 0f) { t = 0f; dir = 1; }
        }
        else
        {
            // wrap-around loop
            if (t > 1f) t -= 1f;
            if (t < 0f) t += 1f;
        }

        Vector3 target = startPos + moveDirection.normalized * (t * moveDistance);
        Vector3 next = target;

        // compute deltas before moving
        Vector3 before = rb.position;
        rb.MovePosition(next);

        FrameDelta = rb.position - before;
        Velocity = FrameDelta / Time.fixedDeltaTime;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = Application.isPlaying ? startPos : transform.position;
        Gizmos.DrawLine(a, a + moveDirection.normalized * moveDistance);
    }
#endif
}
