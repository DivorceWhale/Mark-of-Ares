using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Respawn Settings")]
    public bool respawnOnDeath = true;
    [Tooltip("If left empty, will auto-find an object tagged 'Respawn' or named 'SpawnPoint'.")]
    public Transform respawnPoint;
    [Tooltip("Optional global fall check. Teleports back if player drops below this Y.")]
    public bool enableFallCheck = true;
    public float fallY = -20f;

    [Header("UI")]
    public HealthBar healthBar; // drag your HealthBar here

    // Cached references for safe teleporting
    private Rigidbody _rb;
    private CharacterController _cc;
    private Transform _currentCheckpoint;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Auto-find a default spawn if none assigned
        if (respawnPoint == null)
        {
            var tagged = GameObject.FindGameObjectWithTag("Respawn");
            if (tagged) respawnPoint = tagged.transform;
            else
            {
                var named = GameObject.Find("SpawnPoint");
                if (named) respawnPoint = named.transform;
            }
        }

        // Initialize active checkpoint to respawnPoint (if set)
        _currentCheckpoint = respawnPoint != null ? respawnPoint : transform;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)maxHealth);
            healthBar.SetHealth((int)currentHealth);
        }
    }

    void Update()
    {
        if (enableFallCheck && transform.position.y < fallY)
        {
            // Count as a death-free reset; don�t penalize health
            SafeTeleport(_currentCheckpoint);
            // Optionally, you could also call TakeDamage(maxHealth) to "kill"
        }
    }

    // ===== Public API =====

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar != null) healthBar.SetHealth(Mathf.Max(0, (int)currentHealth));
        if (currentHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (healthBar != null) healthBar.SetHealth((int)currentHealth);
    }

    /// <summary>Set a new checkpoint/respawn location at runtime.</summary>
    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint != null) _currentCheckpoint = checkpoint;
    }

    /// <summary>Teleport back to the active checkpoint (useful for trigger volumes).</summary>
    public void TeleportBack()
    {
        SafeTeleport(_currentCheckpoint);
    }

    // ===== Internals =====

    void Die()
    {
        if (respawnOnDeath && _currentCheckpoint != null)
        {
            currentHealth = maxHealth;
            if (healthBar != null) healthBar.SetHealth((int)maxHealth);
            SafeTeleport(_currentCheckpoint);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void SafeTeleport(Transform target)
    {
        if (target == null) return;

        // Stop motion
        if (_rb)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        // Safest for CharacterController
        if (_cc && _cc.enabled)
        {
            _cc.enabled = false;
            transform.SetPositionAndRotation(target.position, target.rotation);
            _cc.enabled = true;
        }
        else
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }
}
