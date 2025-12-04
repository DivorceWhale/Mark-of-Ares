using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    private bool hasDied = false;

    public DeathUIManager uiManager;   // <-- Assign in Inspector

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy took damage, health now: " + currentHealth);

        if (currentHealth <= 0 && !hasDied)
            Die();
    }

    void Die()
    {
        hasDied = true;
        Debug.Log(gameObject.name + " defeated!");

        if (uiManager != null)
            uiManager.ShowDeathScreen();
        else
            Debug.LogWarning("EnemyHealth: No UIManager assigned!");

        Destroy(gameObject);
    }
}
