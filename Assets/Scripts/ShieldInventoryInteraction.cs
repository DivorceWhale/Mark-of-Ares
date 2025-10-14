using UnityEngine;

public class ShieldInventoryTrigger : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private bool menuOpen = false;

    void Update()
    {
        // Example: open with left controller grip
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            if (menuOpen)
            {
                inventoryUI.Close();
                menuOpen = false;
            }
            else
            {
                inventoryUI.Open();
                menuOpen = true;
            }
        }
    }
}
