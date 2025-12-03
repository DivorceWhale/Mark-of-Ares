using UnityEngine;

public class HideCanvasButton : MonoBehaviour
{
    private const string CanvasCounterKey = "CanvasShowCount";

    [Header("Assign objects to hide")]
    public GameObject canvasToHide;
    public GameObject surfaceToHide;
    public GameObject interactionAreaToHide;

    private void Start()
    {
        // Show canvas only if counter is 0
        int counter = PlayerPrefs.GetInt(CanvasCounterKey, 0);
        bool show = counter == 0;

        if (canvasToHide != null) canvasToHide.SetActive(show);
        if (surfaceToHide != null) surfaceToHide.SetActive(show);
        if (interactionAreaToHide != null) interactionAreaToHide.SetActive(show);
    }

    // Called by button OnClick
    public void HideObjects()
    {
        if (canvasToHide != null) canvasToHide.SetActive(false);
        if (surfaceToHide != null) surfaceToHide.SetActive(false);
        if (interactionAreaToHide != null) interactionAreaToHide.SetActive(false);

        // Increment counter so it won't show again
        PlayerPrefs.SetInt(CanvasCounterKey, 1);
        PlayerPrefs.Save();
    }

    // Call this when quitting the game to reset counter
    public void ResetCanvasCounter()
    {
        PlayerPrefs.SetInt(CanvasCounterKey, 0);
        PlayerPrefs.Save();
    }
}
