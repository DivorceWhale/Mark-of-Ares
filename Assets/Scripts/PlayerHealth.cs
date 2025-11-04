using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Death Bounce (FX)")]
    [Tooltip("Enable a quick up-down bounce when the player dies.")]
    public bool enableDeathBounce = true;
    [Tooltip("Peak height of each bounce, in meters.")]
    public float bounceHeight = 0.6f;
    [Tooltip("Time for ONE up-down bounce.")]
    public float bounceDuration = 0.35f;
    [Tooltip("How many bounces to play before respawn.")]
    public int bounceCount = 1;
    [Tooltip("Temporarily disable CharacterController during bounce to avoid interference.")]
    public bool disableControllerDuringBounce = true;

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
            // Count as a death-free reset; don’t penalize health
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
        if (respawnOnDeath)
        {
            // Start the bounce + respawn sequence
            StartCoroutine(DeathSequence());
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator DeathSequence()
    {
        // Stop motion immediately
        ZeroMotion();

        // Optional: quick bounce where the player died (visual feedback)
        if (enableDeathBounce && bounceDuration > 0f && bounceCount > 0)
        {
            yield return StartCoroutine(PlayDeathBounce());
        }

        // 🕒 NEW: show a delay before respawn
        Debug.Log("You died! Respawning in 2 seconds...");
        yield return new WaitForSeconds(2f);  // pause for 2 seconds

        // Respawn
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetHealth((int)maxHealth);
        SafeTeleport(_currentCheckpoint);
    }


    IEnumerator PlayDeathBounce()
    {
        bool ccWasEnabled = _cc && _cc.enabled;

        if (_cc && disableControllerDuringBounce) _cc.enabled = false;

        Vector3 basePos = transform.position;

        // Use a simple sine arch: y = sin(pi * t) scaled by bounceHeight
        for (int i = 0; i < bounceCount; i++)
        {
            float t = 0f;
            while (t < bounceDuration)
            {
                t += Time.deltaTime;
                float norm = Mathf.Clamp01(t / bounceDuration);
                float yOffset = Mathf.Sin(norm * Mathf.PI) * bounceHeight; // up then down
                Vector3 target = new Vector3(basePos.x, basePos.y + yOffset, basePos.z);
                MoveTransformSafely(target);
                yield return null;
            }

            // Reset to base position after each bounce to avoid drift
            MoveTransformSafely(basePos);
        }

        if (_cc && disableControllerDuringBounce && ccWasEnabled)
            _cc.enabled = true;
    }

    void ZeroMotion()
    {
        if (_rb)
        {
            _rb.linearVelocity = Vector3.zero;        // fixed: linearVelocity -> velocity
            _rb.angularVelocity = Vector3.zero;
        }
    }

    void SafeTeleport(Transform target)
    {
        if (target == null) return;

        // Stop motion
        ZeroMotion();

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

    // Moves transform, handling CharacterController if needed (no rotation change)
    void MoveTransformSafely(Vector3 position)
    {
        if (_cc && _cc.enabled)
        {
            _cc.enabled = false;
            transform.position = position;
            _cc.enabled = true;
        }
        else
        {
            transform.position = position;
        }
    }
}
