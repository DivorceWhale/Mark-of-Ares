using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class CutsceneOrchestrator : MonoBehaviour
{
    [Header("Bindings")]
    public PlayableDirector director;
    public SceneFader fader;
    public TMP_Text subtitleText;
    //public string nextSceneName = "Half-Blood";

    [Header("Participants / Systems")]
    public GameObject shieldProp;
    public EnemySpawner spawner;
    public Animator kidAnim;
    public Animator momAnim;
    public MomSimpleDeath momDeath;


    bool cutsceneRunning;

    void OnEnable()
    {
        if (director != null)
            director.stopped += OnDirectorStopped; // safety: load next scene if timeline ends
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

    public void StartCutscene()
    {
        cutsceneRunning = true;

        if (fader) { fader.InstantSet(1f); fader.FadeTo(0f, 1f); }

        if (director)
        {
            director.time = 0;
            director.Play();
        }
    }

    void OnDirectorStopped(PlayableDirector _)  // fallback in case you forget the final signal
    {
        if (!cutsceneRunning) return;
        EndCutsceneAndLoad();
    }

    public void EndCutsceneAndLoad()
    {
        cutsceneRunning = false;
        if (fader) fader.FadeTo(1f, 0.75f, () => SceneManager.LoadScene(2));
        else SceneManager.LoadScene(2);
    }

    // ===== Timeline Signals (call these from Signal Receiver) =====
    public void Signal_ShieldGive()
    {
        if (shieldProp)
        {
            shieldProp.SetActive(true);
        }
    }

    public void Signal_SpawnMonsters()
    {
        if (spawner) spawner.Spawn();
    }

    public void Signal_MomDeath()
    {
        if (momAnim) momAnim.SetTrigger("Death");   // optional, if you have an Animator
        if (momDeath) momDeath.Die();              // our simple fall-over script
    }


    public void Signal_Teleport()
    {
        EndCutsceneAndLoad();
    }

    // Subtitles (optional)
    public void ShowSubtitle(string line)
    {
        if (subtitleText) subtitleText.text = line;
    }
    public void ClearSubtitle()
    {
        if (subtitleText) subtitleText.text = "";
    }
}
