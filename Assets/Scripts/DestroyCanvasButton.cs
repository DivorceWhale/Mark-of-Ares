using UnityEngine;

public class DestroyCanvasButton : MonoBehaviour
{
    private const string MenuClosedKey = "VRMenuClosed";

    [Header("Menu Objects")]
    public GameObject pauseMenuCanvas;
    public GameObject pauseMenu;

    [Header("VR Interaction Objects")]
    public GameObject surface;
    public GameObject interactionArea;

    private bool menuVisible = false;

    void Start()
    {
        // Check if menu was permanently closed before
        if (PlayerPrefs.GetInt(MenuClosedKey, 0) == 1)
        {
            SetMenuActive(false);
            enabled = false; // disable script so menu never shows
            return;
        }

        SetMenuActive(false);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            menuVisible = !menuVisible;
            SetMenuActive(menuVisible);
        }
    }

    private void SetMenuActive(bool active)
    {
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(active);
        if (pauseMenu != null) pauseMenu.SetActive(active);
        if (surface != null) surface.SetActive(active);
        if (interactionArea != null) interactionArea.SetActive(active);
    }

    // Permanently close menu
    public void CloseMenuPermanently()
    {
        menuVisible = false;
        SetMenuActive(false);

        PlayerPrefs.SetInt(MenuClosedKey, 1);
        PlayerPrefs.Save();

        enabled = false; // disable script so menu never appears
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
