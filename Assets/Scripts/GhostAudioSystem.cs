using UnityEngine;
using System.Collections.Generic;

public class GhostAudioSystem : MonoBehaviour
{
    public static GhostAudioSystem Instance;

    [System.Serializable]
    public class GhostSound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        [HideInInspector]
        public AudioSource source;
    }

    [Header("Ghost Sounds")]
    public GhostSound[] sounds;

    private Dictionary<string, GhostSound> soundDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSounds()
    {
        soundDictionary = new Dictionary<string, GhostSound>();

        foreach (GhostSound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            soundDictionary[s.name] = s;
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
            Debug.LogWarning($"Ghost sound '{name}' not found!");
        }
    }

    public void Stop(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].source.Stop();
        }
    }
}
