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

    [Header("Other Canvases to Hide")]
    public Canvas[] canvasesToHide;         // Assign any other canvases you want hidden

    [Header("VR Interaction Objects")]
    public GameObject surface;
    public GameObject interactionArea;

    private Canvas nextSceneCanvasInstance;

    void Awake()
    {
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
        // Hide other canvases
        foreach (var canvas in canvasesToHide)
        {
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }

        // Instantiate the next-scene canvas if not already
        if (nextSceneCanvasInstance == null && nextSceneCanvasPrefab != null)
        {
            nextSceneCanvasInstance = Instantiate(nextSceneCanvasPrefab);
            nextSceneCanvasInstance.worldCamera = Camera.main;

            // Make sure the button calls LoadNextScene
            var button = nextSceneCanvasInstance.GetComponentInChildren<Button>();
            if (button != null)
                button.onClick.AddListener(LoadNextScene);
        }

        // Show next-scene canvas & VR objects
        if (nextSceneCanvasInstance != null)
            nextSceneCanvasInstance.gameObject.SetActive(true);
        if (surface != null) surface.SetActive(true);
        if (interactionArea != null) interactionArea.SetActive(true);
    }

    public void LoadNextScene()
    {
        // Hide next-scene canvas & VR objects
        if (nextSceneCanvasInstance != null)
            nextSceneCanvasInstance.gameObject.SetActive(false);
        if (surface != null) surface.SetActive(false);
        if (interactionArea != null) interactionArea.SetActive(false);

        SceneManager.LoadScene(sceneToLoad);
    }
}
