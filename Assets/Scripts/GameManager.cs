using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score Settings")]
    public int currentScore = 0;
    public int scoreToWin = 5;
    public string sceneToLoad = "MainMenu";

    [Header("UI Elements")]
    public TMP_Text scoreText;  // Assign your world-space TMP text here

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log("Score: " + currentScore);

        // Update UI safely
        UpdateScoreText();

        if (currentScore >= scoreToWin)
        {
            LoadNextScene();
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            // Overwrite previous text
            scoreText.text = $"{currentScore}";
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
