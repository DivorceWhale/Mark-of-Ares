using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;       // World-space canvas for pause menu
    public GameObject startButton;       // Start/Resume button
    public GameObject settingsButton;    // Settings button
    public GameObject quitButton;        // Quit button

    private bool isPaused = false;

    void Update()
    {
        // Check for Start/Menu button on right Touch controller
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        // Enable the pause menu and its buttons
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        if (startButton) startButton.SetActive(true);
        if (settingsButton) settingsButton.SetActive(true);
        if (quitButton) quitButton.SetActive(true);

        // Stop the game time
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        // Disable the pause menu and its buttons
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (startButton) startButton.SetActive(false);
        if (settingsButton) settingsButton.SetActive(false);
        if (quitButton) quitButton.SetActive(false);

        // Resume game time
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Change this to your main menu scene name
    }
}
