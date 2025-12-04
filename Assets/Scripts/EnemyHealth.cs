using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Scene & Canvas Settings")]
    public string sceneToLoad = "MainMenu";
    public Canvas deathCanvas;
    public GameObject surface;
    public GameObject interactionArea;

    [Header("Optional Settings")]
    public bool pauseGame = true;

    private bool hasDied = false;

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

        // 🔥 Hide ALL other canvases except the deathCanvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);

        foreach (Canvas c in allCanvases)
        {
            if (c != deathCanvas)
                c.gameObject.SetActive(false);
        }

        // Show the death screen
        if (deathCanvas != null)
            deathCanvas.gameObject.SetActive(true);

        if (surface != null)
            surface.SetActive(true);

        if (interactionArea != null)
            interactionArea.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0f;

        Destroy(gameObject);
    }

    // ✅ Load next scene
    public void LoadNextScene()
    {
        if (deathCanvas != null)
            deathCanvas.gameObject.SetActive(false);

        if (surface != null)
            surface.SetActive(false);

        if (interactionArea != null)
            interactionArea.SetActive(false);

        if (pauseGame)
            Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }

    // 🟥 NEW — End Game button
    public void EndGame()
    {
        Debug.Log("Game Quit");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
