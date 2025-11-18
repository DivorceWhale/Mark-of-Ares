using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerFB : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Final Boss");
        }
    }
}
