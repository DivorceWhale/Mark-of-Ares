using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;                 // Damage amount
    public Collider weaponCollider;         // Trigger collider

    private void Awake()
    {
        // Auto-find if forgotten
        if (weaponCollider == null)
            weaponCollider = GetComponent<Collider>();

        if (weaponCollider != null)
            weaponCollider.enabled = false; // Off by default
        else
            Debug.LogError("Weapon has NO collider assigned!");
    }

    public void EnableDamage()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    public void DisableDamage()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!weaponCollider.enabled) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("Player hit! Damage: " + damage);
            }
        }
    }
}
