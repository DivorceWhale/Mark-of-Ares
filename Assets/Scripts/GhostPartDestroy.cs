using UnityEngine;

public class GhostPartDestroy : MonoBehaviour
{
    public float destroyDelay = .5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
