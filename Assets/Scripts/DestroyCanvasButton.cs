using UnityEngine;

public class HideCanvasButton : MonoBehaviour
{
    [Header("Assign objects to hide")]
    public GameObject canvasToHide;
    public GameObject surfaceToHide;
    public GameObject interactionAreaToHide;

    // Called by the button's OnClick
    public void HideObjects()
    {
        if (canvasToHide != null) canvasToHide.SetActive(false);
        if (surfaceToHide != null) surfaceToHide.SetActive(false);
        if (interactionAreaToHide != null) interactionAreaToHide.SetActive(false);

        // No PlayerPrefs saving → will reappear on next launch
    }
}
