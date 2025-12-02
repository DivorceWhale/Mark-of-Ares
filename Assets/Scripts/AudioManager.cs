using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        public bool playOnAwake = false;

        [HideInInspector] public AudioSource source;
    }

    public Sound[] sounds;

    private Dictionary<string, Sound> soundDictionary;

    // The currently playing music track
    private AudioSource currentMusicSource;

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
            return;
        }

        InitializeSounds();
    }

    void InitializeSounds()
    {
        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;


            // 🔥 VR FIX: Force 2D audio so Meta Spatializer doesn’t kill volume
            s.source.spatialBlend = 0f;

            soundDictionary[s.name] = s;

            if (s.playOnAwake)
            {
                currentMusicSource = s.source;
                s.source.Play();
            }
        }
    }

    // -------------------------------------------------------
    // PLAY SOUND (SFX)
    // -------------------------------------------------------
    public void Play(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            if (!s.loop)
            {
                s.source.PlayOneShot(s.clip, s.volume);
            }
            else
            {
                s.source.Play();
            }
        }
        else
        {
            Debug.LogWarning("Sound not found: " + name);
        }
    }

    // -------------------------------------------------------
    // PLAY MUSIC WITH FADE + STOP CURRENT MUSIC
    // -------------------------------------------------------
    public void PlayMusic(string name, float fadeTime = 1f)
    {
        if (!soundDictionary.TryGetValue(name, out Sound newSound))
        {
            Debug.LogWarning("Music not found: " + name);
            return;
        }

        // If the same music is already playing, don't restart it
        if (currentMusicSource == newSound.source)
            return;

        StartCoroutine(FadeMusic(newSound.source, fadeTime));
    }

    IEnumerator FadeMusic(AudioSource newMusic, float fadeTime)
    {
        AudioSource oldMusic = currentMusicSource;
        currentMusicSource = newMusic;

        // Fade out old music
        if (oldMusic != null)
        {
            float startVol = oldMusic.volume;
            float t = 0;

            while (t < fadeTime)
            {
                t += Time.deltaTime;
                oldMusic.volume = Mathf.Lerp(startVol, 0, t / fadeTime);
                yield return null;
            }

            oldMusic.Stop();
            oldMusic.volume = startVol;
        }

        // Start new music
        newMusic.volume = 0f;
        newMusic.Play();

        // Fade in new music
        float targetVol = soundDictionary[newMusic.clip.name].volume;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            newMusic.volume = Mathf.Lerp(0, targetVol, time / fadeTime);
            yield return null;
        }
    }

    public void Stop(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.Stop();
        }
    }

    public void SetVolume(string name, float volume)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.volume = Mathf.Clamp01(volume);
        }
    }
}
