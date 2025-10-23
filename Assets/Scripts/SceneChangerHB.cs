using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerHB : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Half-Blood");
        }
    }
}
