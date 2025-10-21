using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerCTF : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Capture the Flag");
        }
    }
}
