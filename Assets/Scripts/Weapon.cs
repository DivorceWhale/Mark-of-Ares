using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;                 // Damage per hit
    public Collider weaponCollider;         // Trigger collider for hit detection

    private void Start()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false; // Off by default
    }

    // Called by Animation Event
    public void EnableDamage()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    // Called by Animation Event
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
            PlayerHealth pl = other.GetComponent<PlayerHealth>();
            if (pl != null)
            {
                pl.TakeDamage(damage);
                Debug.Log("Player hit for " + damage + " damage!");
            }
        }
    }
}
