using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused = false;

    [Header("VR Interaction Objects")]
    public GameObject surface;
    public GameObject interactionArea;

    void Start()
    {
        // Ensure everything is hidden at start
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (surface != null) surface.SetActive(false);
        if (interactionArea != null) interactionArea.SetActive(false);
    }

    void Update()
    {
        // Press Oculus Menu Button to pause
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pauseMenu != null) pauseMenu.SetActive(isPaused);
        if (surface != null) surface.SetActive(isPaused);
        if (interactionArea != null) interactionArea.SetActive(isPaused);

        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (surface != null) surface.SetActive(false);
        if (interactionArea != null) interactionArea.SetActive(false);

        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
