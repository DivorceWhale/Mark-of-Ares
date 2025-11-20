using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CutsceneOrchestrator : MonoBehaviour
{
    [Header("Bindings")]
    public PlayableDirector director;
    public SceneFader fader;
    public TMP_Text subtitleText;

    [Header("Scene Settings")]
    public string nextSceneName = "CreditsScene";
    public int fallbackSceneIndex = 2;
    public float fadeOutDuration = 0.75f;

    [Header("Participants")]
    public GameObject shieldProp;
    public EnemySpawner spawner;
    public Animator kidAnim;
    public Animator momAnim;
    public MomSimpleDeath momDeath;

    bool cutsceneRunning = false;
    bool isLoading = false;
    Coroutine loadRoutine;

    void OnEnable()
    {
        if (director != null)
            director.stopped += OnDirectorStopped;
    }

    void OnDisable()
    {
        if (director != null)
            director.stopped -= OnDirectorStopped;
    }

    void Start()
    {
        StartCutscene();
    }

    // -------------------------------------------------------------
    // Cutscene Flow
    // -------------------------------------------------------------
    public void StartCutscene()
    {
        cutsceneRunning = true;

        if (fader)
        {
            fader.InstantSet(1f);
            fader.FadeTo(0f, fadeOutDuration);
        }

        if (director)
        {
            director.time = 0;
            director.Play();
        }
    }

    void OnDirectorStopped(PlayableDirector _)
    {
        if (cutsceneRunning)
            EndCutsceneAndLoad();
    }

    public void EndCutsceneAndLoad()
    {
        if (isLoading) return;

        isLoading = true;
        cutsceneRunning = false;

        if (fader)
        {
            // Start fade-out
            fader.FadeTo(1f, fadeOutDuration);

            // Wait then load
            if (loadRoutine != null) StopCoroutine(loadRoutine);
            loadRoutine = StartCoroutine(LoadAfterDelay(fadeOutDuration));
        }
        else
        {
            LoadSceneNow();
        }
    }

    IEnumerator LoadAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadSceneNow();
    }

    void LoadSceneNow()
    {
        // Try scene name
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            try
            {
                SceneManager.LoadScene(nextSceneName);
                return;
            }
            catch { }
        }

        // Fallback
        SceneManager.LoadScene(fallbackSceneIndex);
    }

    // -------------------------------------------------------------
    // Timeline Signals
    // -------------------------------------------------------------
    public void Signal_ShieldGive()
    {
        if (shieldProp) shieldProp.SetActive(true);
    }

    public void Signal_SpawnMonsters()
    {
        if (spawner) spawner.Spawn();
    }

    public void Signal_MomDeath()
    {
        if (momAnim) momAnim.SetTrigger("Death");
        if (momDeath) momDeath.Die();
    }

    public void Signal_Teleport()
    {
        EndCutsceneAndLoad();
    }

    public void Signal_EndCutsceneWithName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
            nextSceneName = sceneName;

        EndCutsceneAndLoad();
    }

    // -------------------------------------------------------------
    // Subtitles
    // -------------------------------------------------------------
    public void ShowSubtitle(string line)
    {
        if (subtitleText) subtitleText.text = line;
    }

    public void ClearSubtitle()
    {
        if (subtitleText) subtitleText.text = "";
    }
}