using UnityEngine;
using UnityEngine.SceneManagement; // <-- ADD THIS

public class PlayerHealth : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float fallY = -10f;

    [Header("Health Settings")]
    public HealthBar healthBar;
    public int maxHealth = 100;

    [Header("Death Scene")]  // <-- ADD THIS
    public string deathSceneName = "MainMenu";  // Set this in the Inspector

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (transform.position.y < fallY)
        {
            Respawn();
            RestoreHealth();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ----------- CHANGED THIS METHOD ------------
    private void Die()
    {
        Debug.Log("Player has died! Loading scene...");

        // Load the selected scene
        SceneManager.LoadScene(deathSceneName);
    }
    // --------------------------------------------

    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("No respawn point assigned!");
            return;
        }

        var ui = FindObjectOfType<RiseAgainUI>();
        if (ui != null)
            ui.Show("Rise again – keep going", 1.5f);

        transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
