using UnityEngine;

public class Break_Ghost : MonoBehaviour
{
    [Header("Ghost References")]
    public GameObject ghost_normal;
    public GameObject ghost_Parts;
    public Animator ghost;

    [Header("Scoring")]
    public int pointsForGhost = 1;

    [Header("Floor Detection")]
    public string floorTag = "Floor"; // Tag of the floor object

    private bool isBroken = false;
    private Rigidbody[] partRigidbodies;
    private int counter;

    void Start()
    {
        if (ghost_normal != null)
            ghost_normal.SetActive(true);

        if (ghost_Parts != null)
        {
            ghost_Parts.SetActive(false);
            partRigidbodies = ghost_Parts.GetComponentsInChildren<Rigidbody>(true);

            // Disable gravity and kinematic on all parts initially
            foreach (Rigidbody rb in partRigidbodies)
            {
                rb.useGravity = false;
                rb.isKinematic = true;

                // Add a small script to detect collision with floor
                FloorCollision fc = rb.gameObject.AddComponent<FloorCollision>();
                fc.Setup(this, floorTag);
            }
        }
    }

    public void break_Ghost()
    {
        if (isBroken) return;

        isBroken = true;

        if (ghost_Parts != null)
        {
            ghost_Parts.SetActive(true);
            ghost_normal.SetActive(false);

            foreach (Rigidbody rb in partRigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    // Called by FloorCollision when any part hits the floor
    public void DestroyGhost()
    {
        // Award points when ghost finally hits the floor
        if (GameManager.Instance != null && pointsForGhost > 0)
        {
            GameManager.Instance.AddScore(pointsForGhost);
            pointsForGhost = 0; // prevent double counting
        }

        Destroy(gameObject);
    }

    public void play_anim()
    {
        counter++;
        if (counter % 2 == 0)
            ghost.Play("idle");
        else
            ghost.Play("attack");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detect hits from Spear or Axe
        if (collision.gameObject.CompareTag("Spear") || collision.gameObject.CompareTag("Axe"))
        {
            break_Ghost();
        }
    }
}

// Helper class to detect floor contact
public class FloorCollision : MonoBehaviour
{
    private Break_Ghost parentGhost;
    private string floorTag;

    public void Setup(Break_Ghost ghost, string tag)
    {
        parentGhost = ghost;
        floorTag = tag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(floorTag))
        {
            if (parentGhost != null)
            {
                parentGhost.DestroyGhost();
            }
        }
    }
}
