using UnityEngine;

public class pauseMenuC : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public GameObject pauseMenu;
    public bool isPaused = false;

    [Header("VR Interaction Objects")]
    public GameObject surface;
    public GameObject interactionArea;

    void Start()
    {
        // Ensure everything is hidden at start
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
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
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(isPaused);
        if (pauseMenu != null) pauseMenu.SetActive(isPaused);
        if (surface != null) surface.SetActive(isPaused);
        if (interactionArea != null) interactionArea.SetActive(isPaused);

        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (surface != null) surface.SetActive(false);
        if (interactionArea != null) interactionArea.SetActive(false);

        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        // Reset the splash/canvas counter so it can appear next time
        PlayerPrefs.SetInt("CanvasShowCount", 0);
        PlayerPrefs.Save();

        Time.timeScale = 1;
        Application.Quit();
    }

}
