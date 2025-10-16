using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerDUEL : MonoBehaviour
{
    [Tooltip("Tag of the object that will trigger the scene change (e.g., 'Player' or 'MainCamera')")]
    public string triggerTag = "[Building Blocks] Camera Rig";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SceneManager.LoadScene("1v1 Deuls");
        }
    }
}
