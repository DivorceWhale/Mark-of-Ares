using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriggerShowCanvas : MonoBehaviour
{
    [Header("Canvas Settings")]
    public Canvas canvasToShow;
    public GameObject surface;
    public GameObject interactionArea;

    [Header("Other Canvases to Hide")]
    public Canvas[] canvasesToHide;

    [Header("Scene Settings")]
    public string sceneToLoad = "";          // Scene name to load

    [Header("Optional Settings")]
    public bool pauseGame = false;
    public bool onlyOnce = true;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && onlyOnce) return;

        // Hide other canvases
        foreach (var canvas in canvasesToHide)
        {
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }

        // Show this canvas and VR objects
        if (canvasToShow != null)
            canvasToShow.gameObject.SetActive(true);
        if (surface != null)
            surface.SetActive(true);
        if (interactionArea != null)
            interactionArea.SetActive(true);

        // Optionally pause the game
        if (pauseGame)
            Time.timeScale = 0f;

        triggered = true;
    }

    // Close canvas normally
    public void CloseCanvas()
    {
        if (canvasToShow != null)
            canvasToShow.gameObject.SetActive(false);
        if (surface != null)
            surface.SetActive(false);
        if (interactionArea != null)
            interactionArea.SetActive(false);

        if (pauseGame)
            Time.timeScale = 1f;
    }

    // ✅ NEW — Load next scene when button pressed
    public void LoadNextScene()
    {
        if (pauseGame)
            Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
        else
            Debug.LogWarning("No scene name assigned in TriggerShowCanvas!");
    }
}
