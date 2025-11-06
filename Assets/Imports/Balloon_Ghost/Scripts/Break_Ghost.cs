using UnityEngine;

public class Break_Ghost : MonoBehaviour
{
    public bool Is_Breaked = false;
    public GameObject ghost_normal;
    public GameObject ghost_Parts;
    public Animator ghost;
    int counter;

    private Rigidbody[] partRigidbodies;

    [Header("Score")]
    public int pointsForGhost = 1; // points this ghost gives

    void Start()
    {
        ghost_normal.SetActive(true);
        ghost_Parts.SetActive(false);

        if (ghost_Parts != null)
        {
            partRigidbodies = ghost_Parts.GetComponentsInChildren<Rigidbody>(true);

            foreach (Rigidbody rb in partRigidbodies)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
    }

    void Update()
    {
        if (Is_Breaked)
        {
            ghost_Parts.SetActive(true);
            ghost_normal.SetActive(false);

            foreach (Rigidbody rb in partRigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            // Award points once
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(pointsForGhost);
                pointsForGhost = 0; // prevent double counting
            }
        }
    }

    public void break_Ghost()
    {
        Is_Breaked = true;
    }

    public void play_anim()
    {
        counter += 1;
        if (counter == 2)
        {
            counter = 0;
            ghost.Play("idle");
        }
        else
        {
            ghost.Play("attack");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spear"))
        {
            break_Ghost();
        }
    }
}
