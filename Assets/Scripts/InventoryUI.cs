using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform playerHand; // drag your RightHandAnchor here

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SelectWeapon(int index)
    {
        InventoryManager.Instance.EquipWeapon(index, playerHand);
        Close();
    }
}
