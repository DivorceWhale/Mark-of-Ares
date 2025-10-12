using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Respawn Settings")]
    public bool respawnOnDeath = true;
    public Transform respawnPoint;

    [Header("UI")]
    public HealthBar healthBar; // drag your HealthBar here

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.SetHealth((int)currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");

        if (respawnOnDeath && respawnPoint != null)
        {
            currentHealth = maxHealth;
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
            if (healthBar != null)
                healthBar.SetHealth((int)maxHealth);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (healthBar != null)
        {
            healthBar.SetHealth((int)currentHealth);
        }
        Debug.Log($"Player healed {amount}. Health: {currentHealth}");
    }
}
