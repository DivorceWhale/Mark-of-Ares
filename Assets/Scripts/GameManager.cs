using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score Settings")]
    public int currentScore = 0;
    public int scoreToWin = 5;
    public string sceneToLoad = "MainMenu";

    [Header("UI Settings")]
    public TMP_Text scoreText;               // Assign world-space TMP text
    public Canvas nextSceneCanvasPrefab;     // Assign a prefab with Canvas + Button

    [Header("VR Interaction Objects")]
    public GameObject surface;
    public GameObject interactionArea;

    private Canvas nextSceneCanvasInstance;

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

        UpdateScoreText();

        if (currentScore >= scoreToWin)
        {
            ShowNextSceneCanvas();
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"{currentScore}";
    }

    private void ShowNextSceneCanvas()
    {
        // Instantiate canvas if it doesn't exist
        if (nextSceneCanvasInstance == null && nextSceneCanvasPrefab != null)
        {
            nextSceneCanvasInstance = Instantiate(nextSceneCanvasPrefab);
            nextSceneCanvasInstance.worldCamera = Camera.main;
        }

        if (nextSceneCanvasInstance != null)
            nextSceneCanvasInstance.gameObject.SetActive(true);

        // Show interaction objects
        if (surface != null)
            surface.SetActive(true);
        if (interactionArea != null)
            interactionArea.SetActive(true);

        // Pause the game while waiting for button click
        Time.timeScale = 0f;
    }

    // This method should be assigned to the Button's OnClick in the Canvas prefab
    public void LoadNextScene()
    {
        Time.timeScale = 1f; // Resume game
        if (nextSceneCanvasInstance != null)
            nextSceneCanvasInstance.gameObject.SetActive(false);

        // Hide interaction objects
        if (surface != null)
            surface.SetActive(false);
        if (interactionArea != null)
            interactionArea.SetActive(false);

        SceneManager.LoadScene(sceneToLoad);
    }
}
