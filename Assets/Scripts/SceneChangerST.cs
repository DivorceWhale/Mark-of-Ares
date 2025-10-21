using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerST : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Spear Throwing");
        }
    }
}
