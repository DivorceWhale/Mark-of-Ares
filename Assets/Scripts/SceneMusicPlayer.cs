using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicPlayer : MonoBehaviour
{
    [Header("Music Names Per Scene")]
    public string MainMenu = "Main Menu";
    public string HalfBlood = "Half-Blood";
    public string SpearThrowing = "Spear Throwing";
    public string FinalBoss = "Final Boss";

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
        Debug.Log("Playing music in scene: " + SceneManager.GetActiveScene().name);

        if (AudioManager.Instance != null)
        {
            Debug.Log("AudioManager instance found");

            AudioManager.Instance.PlayMusic("MainMenuMusic"); // match name exactly
        }
        else
        {
            Debug.LogWarning("AudioManager instance is null!");
        }
    }

    private void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                AudioManager.Instance.PlayMusic(MainMenu);
                break;

            case "Half-Blood":
                AudioManager.Instance.PlayMusic(HalfBlood);
                break;

            case "Spear Throwing":
                AudioManager.Instance.PlayMusic(SpearThrowing);
                break;

            case "Final Boss":
                AudioManager.Instance.PlayMusic(FinalBoss);
                break;

            default:
                Debug.Log("No music assigned for scene: " + sceneName);
                break;
        }
    }
}
