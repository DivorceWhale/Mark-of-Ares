using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class Weapon
    {
        public string weaponName;
        public GameObject weaponPrefab;
    }

    public List<Weapon> collectedWeapons = new List<Weapon>();
    public Weapon currentWeapon;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddWeapon(Weapon newWeapon)
    {
        if (!collectedWeapons.Contains(newWeapon))
            collectedWeapons.Add(newWeapon);
    }

    public void EquipWeapon(int index, Transform handAnchor)
    {
        if (index < 0 || index >= collectedWeapons.Count) return;

        if (currentWeapon != null && handAnchor.childCount > 0)
        {
            Destroy(handAnchor.GetChild(0).gameObject); // remove current
        }

        currentWeapon = collectedWeapons[index];
        Instantiate(currentWeapon.weaponPrefab, handAnchor.position, handAnchor.rotation, handAnchor);
    }
}
