using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Canvas deathCanvas;
    public GameObject surface;
    public GameObject interactionArea;

    [Header("Scene Settings")]
    public string sceneToLoad = "MainMenu";
    public bool pauseGame = true;

    // Called by EnemyHealth when the enemy dies
    public void ShowDeathScreen()
    {
        Debug.Log("DeathUIManager: Showing death UI");

        // Hide all other canvases
        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
        foreach (Canvas c in allCanvases)
        {
            if (c != deathCanvas)
                c.gameObject.SetActive(false);
        }

        // Show death UI
        if (deathCanvas != null)
            deathCanvas.gameObject.SetActive(true);

        if (surface != null)
            surface.SetActive(true);

        if (interactionArea != null)
            interactionArea.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0f;
    }

    public void LoadNextScene()
    {
        if (pauseGame)
            Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }

    public void EndGame()
    {
        PlayerPrefs.SetInt("CanvasShowCount", 0);
        PlayerPrefs.Save();
        Time.timeScale = 1;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
