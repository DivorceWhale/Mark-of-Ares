using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Where the player will respawn.")]
    public Transform respawnPoint;
    [Tooltip("Y position threshold that triggers respawn.")]
    public float fallY = -10f;

    [Header("Health Settings")]
    public HealthBar healthBar;       // Reference to your HealthBar
    public int maxHealth = 100;       // Maximum health

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        // Check if player fell below threshold
        if (transform.position.y < fallY)
        {
            Respawn();
            RestoreHealth(); // Restore health on fall
        }
    }

    /// <summary>
    /// Deals damage to the player and checks for death
    /// </summary>
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

    /// <summary>
    /// Handles player death
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died! Respawning...");
        Respawn();
        RestoreHealth();
    }

    /// <summary>
    /// Restores the player's health to max
    /// </summary>
    private void RestoreHealth()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    /// <summary>
    /// Respawns the player at the respawn point and resets physics
    /// </summary>
    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("No respawn point assigned!");
            return;
        }

        // Show respawn UI if available
        var ui = FindObjectOfType<RiseAgainUI>();
        if (ui != null)
            ui.Show("Rise again � keep going", 1.5f);

        // Reset position and rotation
        transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        // Reset Rigidbody velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Player respawned.");
    }
}
