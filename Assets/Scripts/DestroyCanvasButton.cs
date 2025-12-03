using UnityEngine;

public class DestroyCanvasButton: MonoBehaviour
{
    // Assign this in the Button's OnClick()
    public void DestroyCanvas()
    {
        Destroy(gameObject);
    }
}
