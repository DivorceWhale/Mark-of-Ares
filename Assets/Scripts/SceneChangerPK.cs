using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerPK : MonoBehaviour
{
    public string triggerTag = "[Building Blocks] Camera Rig";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SceneManager.LoadScene("Parkour");
        }
    }
}
