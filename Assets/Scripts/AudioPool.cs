// AudioPool.cs
using UnityEngine;
using System.Collections.Generic;

public class AudioPool : MonoBehaviour
{
    public static AudioPool Instance;

    [Header("Pool Settings")]
    public int poolSize = 20;
    public GameObject audioSourcePrefab;
    public bool expandable = true;
    public int maxPoolSize = 50;

    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private List<AudioSource> allSources = new List<AudioSource>();
    private Transform poolContainer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePool()
    {
        poolContainer = new GameObject("Audio Pool Container").transform;
        poolContainer.SetParent(transform);

        // Create initial pool
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    AudioSource CreateNewAudioSource()
    {
        GameObject sourceObj = audioSourcePrefab != null ?
            Instantiate(audioSourcePrefab, poolContainer) :
            new GameObject($"PooledAudioSource_{allSources.Count}");

        if (audioSourcePrefab == null)
        {
            sourceObj.transform.SetParent(poolContainer);
        }

        AudioSource source = sourceObj.GetComponent<AudioSource>();
        if (source == null)
        {
            source = sourceObj.AddComponent<AudioSource>();
        }

        // Default settings
        source.playOnAwake = false;
        source.spatialBlend = 1f; // 3D by default

        sourceObj.SetActive(false);
        availableSources.Enqueue(source);
        allSources.Add(source);

        return source;
    }

    public AudioSource GetAudioSource()
    {
        // Clean up finished sources
        ReturnFinishedSources();

        AudioSource source = null;

        // Try to get available source
        while (availableSources.Count > 0 && source == null)
        {
            source = availableSources.Dequeue();
            if (source == null) // Source was destroyed
            {
                allSources.Remove(source);
                source = null;
            }
        }

        // Create new source if needed and allowed
        if (source == null)
        {
            if (expandable && allSources.Count < maxPoolSize)
            {
                source = CreateNewAudioSource();
                availableSources.Dequeue(); // Remove from queue since we're using it
            }
            else
            {
                Debug.LogWarning("Audio pool exhausted!");
                return null;
            }
        }

        // Activate and reset source
        source.gameObject.SetActive(true);
        ResetAudioSource(source);

        return source;
    }

    public void ReturnAudioSource(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        source.gameObject.SetActive(false);

        if (!availableSources.Contains(source))
        {
            availableSources.Enqueue(source);
        }
    }

    void ReturnFinishedSources()
    {
        foreach (AudioSource source in allSources)
        {
            if (source != null && source.gameObject.activeInHierarchy &&
                !source.isPlaying && !availableSources.Contains(source))
            {
                ReturnAudioSource(source);
            }
        }
    }

    void ResetAudioSource(AudioSource source)
    {
        source.clip = null;
        source.volume = 1f;
        source.pitch = 1f;
        source.spatialBlend = 1f;
        source.minDistance = 1f;
        source.maxDistance = 500f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.loop = false;
        source.priority = 128;
        //source.dopperLevel = 1f;
        source.spread = 0f;
        source.panStereo = 0f;
    }

    // Convenience methods
    public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAudioSource();
        if (source != null)
        {
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }
    }

    public AudioSource PlayClipAtPointWithReturn(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return null;

        AudioSource source = GetAudioSource();
        if (source != null)
        {
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.Play();
            return source;
        }
        return null;
    }

    public void PlayRandomClipAtPoint(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlayClipAtPoint(clip, position, volume);
    }

    void Update()
    {
        // Periodically clean up pool
        if (Time.frameCount % 60 == 0) // Every 60 frames
        {
            ReturnFinishedSources();
        }
    }

    public int GetAvailableSourceCount()
    {
        return availableSources.Count;
    }

    public int GetTotalSourceCount()
    {
        return allSources.Count;
    }

    public int GetActiveSourceCount()
    {
        return allSources.Count - availableSources.Count;
    }
}

// Extension class for easy audio pooling
public static class AudioPoolExtensions
{
    public static void PlayPooled(this AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (AudioPool.Instance != null)
        {
            AudioPool.Instance.PlayClipAtPoint(clip, position, volume);
        }
    }

    public static void PlayPooled(this AudioClip clip, Transform transform, float volume = 1f)
    {
        if (AudioPool.Instance != null && transform != null)
        {
            AudioPool.Instance.PlayClipAtPoint(clip, transform.position, volume);
        }
    }
}