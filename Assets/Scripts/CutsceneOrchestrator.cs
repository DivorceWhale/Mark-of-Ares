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
    public FreezeCamera freezeCam;

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

        // Lock VR / camera look
        if (freezeCam) freezeCam.freeze = true;

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
        // Re-enable camera look when leaving the cutscene
        if (freezeCam) freezeCam.freeze = false;

        // Try scene name first
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            try
            {
                SceneManager.LoadScene(nextSceneName);
                return;
            }
            catch
            {
                // fall through to index
            }
        }

        // Fallback by build index
        SceneManager.LoadScene(fallbackSceneIndex);
    }

    // -------------------------------------------------------------
    // Timeline Signals
    // -------------------------------------------------------------
    public void Signal_ShieldGive()
    {
        if (shieldProp) shieldProp.SetActive(true);

        // Optional: play give/receive animations if you hook them up
        // if (momAnim) momAnim.SetTrigger("GiveShield");
        // if (kidAnim) kidAnim.SetTrigger("ReceiveShield");
    }

    public void Signal_SpawnMonsters()
    {
        if (spawner) spawner.Spawn();
    }

    public void Signal_MomDeath()
    {
        // Animator "Death" not needed if MomSimpleDeath handles the fall
        // if (momAnim) momAnim.SetTrigger("Death");

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
