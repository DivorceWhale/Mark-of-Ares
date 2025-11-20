// AudioManager.cs
using UnityEngine;
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
        [HideInInspector]
        public AudioSource source;
    }

    public Sound[] sounds;
    private Dictionary<string, Sound> soundDictionary;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
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

            soundDictionary[s.name] = s;

            if (s.playOnAwake)
                s.source.Play();
        }
    }

    public void Play(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    public void Stop(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].source.Stop();
        }
    }

    public void SetVolume(string name, float volume)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].source.volume = Mathf.Clamp01(volume);
        }
    }

    // -------------------------------------------------------
    //  ✔ ADDED: Safe access wrappers for other scripts
    // -------------------------------------------------------

    public AudioSource GetSource(string name)
    {
        if (soundDictionary.ContainsKey(name))
            return soundDictionary[name].source;

        return null;
    }

    public bool TryGetSource(string name, out AudioSource source)
    {
        source = null;
        if (soundDictionary.ContainsKey(name))
        {
            source = soundDictionary[name].source;
            return true;
        }
        return false;
    }
}
