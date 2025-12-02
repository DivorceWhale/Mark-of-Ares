using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerDUEL : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("1v1 Duels");
        }
    }
}
