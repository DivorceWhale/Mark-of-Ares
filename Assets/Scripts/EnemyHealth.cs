using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Scene to load on defeat")]
    public string sceneToLoad = "MainMenu";  // Change this to your desired scene name

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this when the enemy takes damage
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " defeated!");

        // Load the desired scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        // Destroy enemy object
        Destroy(gameObject);
    }
}
