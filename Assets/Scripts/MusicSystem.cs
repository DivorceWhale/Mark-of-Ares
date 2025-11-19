// MusicSystem.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicSystem : MonoBehaviour
{
    public static MusicSystem Instance;
    
    [System.Serializable]
    public class MusicTrack
    {
        public string trackName;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = true;
        public float fadeInTime = 2f;
        public float fadeOutTime = 1f;
        [Range(0f, 1f)]
        public float intensityLevel = 0.5f; // For dynamic music
    }
    
    [System.Serializable]
    public class MusicLayer
    {
        public string layerName;
        public AudioClip clip;
        public float targetVolume;
        public bool isActive;
        [HideInInspector]
        public AudioSource source;
    }
    
    [Header("Music Tracks")]
    public MusicTrack[] musicTracks;
    public string startingTrack = "";
    
    [Header("Dynamic Music Layers")]
    public MusicLayer[] dynamicLayers;
    public bool useDynamicMusic = false;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float masterMusicVolume = 0.7f;
    public float defaultFadeTime = 2f;
    public bool playOnStart = true;
    
    // Audio sources for crossfading
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;
    private AudioSource activeSource;
    private AudioSource inactiveSource;
    
    // Current state
    private MusicTrack currentTrack;
    private Coroutine fadeCoroutine;
    private Dictionary<string, MusicTrack> trackDictionary;
    
    // Dynamic music state
    private float currentIntensity = 0.5f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMusicSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeMusicSystem()
    {
        // Create audio sources
        GameObject sourceA = new GameObject("Music Source A");
        sourceA.transform.SetParent(transform);
        musicSourceA = sourceA.AddComponent<AudioSource>();
        musicSourceA.spatialBlend = 0f; // 2D sound
        musicSourceA.priority = 0; // Highest priority
        
        GameObject sourceB = new GameObject("Music Source B");
        sourceB.transform.SetParent(transform);
        musicSourceB = sourceB.AddComponent<AudioSource>();
        musicSourceB.spatialBlend = 0f;
        musicSourceB.priority = 0;
        
        activeSource = musicSourceA;
        inactiveSource = musicSourceB;
        
        // Build track dictionary
        trackDictionary = new Dictionary<string, MusicTrack>();
        foreach (MusicTrack track in musicTracks)
        {
            if (!string.IsNullOrEmpty(track.trackName))
            {
                trackDictionary[track.trackName] = track;
            }
        }
        
        // Setup dynamic layers
        if (useDynamicMusic)
        {
            SetupDynamicLayers();
        }
    }
    
    void Start()
    {
        if (playOnStart && !string.IsNullOrEmpty(startingTrack))
        {
            PlayMusic(startingTrack);
        }
    }
    
    void SetupDynamicLayers()
    {
        foreach (MusicLayer layer in dynamicLayers)
        {
            GameObject layerObj = new GameObject($"Music Layer - {layer.layerName}");
            layerObj.transform.SetParent(transform);
            
            layer.source = layerObj.AddComponent<AudioSource>();
            layer.source.clip = layer.clip;
            layer.source.loop = true;
            layer.source.spatialBlend = 0f;
            layer.source.volume = 0f;
            layer.source.priority = 10;
            
            if (layer.isActive)
            {
                layer.source.Play();
            }
        }
    }
    
    public void PlayMusic(string trackName, bool immediate = false)
    {
        if (!trackDictionary.ContainsKey(trackName))
        {
            Debug.LogWarning($"Music track '{trackName}' not found!");
            return;
        }
        
        MusicTrack newTrack = trackDictionary[trackName];
        
        // Don't restart if already playing
        if (currentTrack == newTrack && activeSource.isPlaying)
        {
            return;
        }
        
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        if (immediate || !activeSource.isPlaying)
        {
            // Immediate switch
            activeSource.Stop();
            activeSource.clip = newTrack.clip;
            activeSource.volume = newTrack.volume * masterMusicVolume;
            activeSource.loop = newTrack.loop;
            activeSource.Play();
            currentTrack = newTrack;
        }
        else
        {
            // Crossfade
            fadeCoroutine = StartCoroutine(CrossfadeToTrack(newTrack));
        }
    }
    
    IEnumerator CrossfadeToTrack(MusicTrack newTrack)
    {
        // Setup inactive source with new track
        inactiveSource.clip = newTrack.clip;
        inactiveSource.volume = 0f;
        inactiveSource.loop = newTrack.loop;
        inactiveSource.Play();
        
        float fadeTime = currentTrack != null ? 
            Mathf.Max(currentTrack.fadeOutTime, newTrack.fadeInTime) : 
            newTrack.fadeInTime;
        
        // Crossfade
        float elapsed = 0f;
        float startVolumeActive = activeSource.volume;
        float targetVolumeInactive = newTrack.volume * masterMusicVolume;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            
            // Use curves for smooth fading
            float fadeOutCurve = 1f - Mathf.Pow(t, 2f); // Quadratic out
            float fadeInCurve = Mathf.Pow(t, 2f); // Quadratic in
            
            activeSource.volume = startVolumeActive * fadeOutCurve;
            inactiveSource.volume = targetVolumeInactive * fadeInCurve;
            
            yield return null;
        }
        
        // Finalize
        activeSource.Stop();
        activeSource.volume = 0f;
        inactiveSource.volume = targetVolumeInactive;
        
        // Swap sources
        AudioSource temp = activeSource;
        activeSource = inactiveSource;
        inactiveSource = temp;
        
        currentTrack = newTrack;
    }
    
    public void StopMusic(float fadeTime = -1f)
    {
        if (fadeTime < 0)
        {
            fadeTime = defaultFadeTime;
        }
        
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        fadeCoroutine = StartCoroutine(FadeOutMusic(fadeTime));
    }
    
    IEnumerator FadeOutMusic(float fadeTime)
    {
        float startVolume = activeSource.volume;
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
            yield return null;
        }
        
        activeSource.Stop();
        activeSource.volume = 0f;
        currentTrack = null;
    }
    
    public void SetMusicVolume(float volume)
    {
        masterMusicVolume = Mathf.Clamp01(volume);
        
        if (currentTrack != null && activeSource.isPlaying)
        {
            activeSource.volume = currentTrack.volume * masterMusicVolume;
        }
        
        // Update dynamic layers
        if (useDynamicMusic)
        {
            UpdateDynamicLayers();
        }
    }
    
    public void SetMusicIntensity(float intensity)
    {
        currentIntensity = Mathf.Clamp01(intensity);
        
        if (useDynamicMusic)
        {
            UpdateDynamicLayers();
        }
        else if (currentTrack != null)
        {
            // Find appropriate track for intensity
            MusicTrack bestTrack = null;
            float closestDiff = float.MaxValue;
            
            foreach (MusicTrack track in musicTracks)
            {
                float diff = Mathf.Abs(track.intensityLevel - currentIntensity);
                if (diff < closestDiff)
                {
                    closestDiff = diff;
                    bestTrack = track;
                }
            }
            
            if (bestTrack != null && bestTrack != currentTrack)
            {
                PlayMusic(bestTrack.trackName);
            }
        }
    }
    
    void UpdateDynamicLayers()
    {
        foreach (MusicLayer layer in dynamicLayers)
        {
            if (layer.source != null)
            {
                float targetVolume = layer.isActive ? layer.targetVolume * masterMusicVolume : 0f;
                
                // Adjust volume based on intensity if needed
                if (layer.layerName.ToLower().Contains("intense") || 
                    layer.layerName.ToLower().Contains("action"))
                {
                    targetVolume *= currentIntensity;
                }
                else if (layer.layerName.ToLower().Contains("calm") || 
                         layer.layerName.ToLower().Contains("ambient"))
                {
                    targetVolume *= (1f - currentIntensity);
                }
                
                layer.source.volume = Mathf.MoveTowards(layer.source.volume, targetVolume, 
                    Time.deltaTime * 2f);
            }
        }
    }
    
    public void ToggleDynamicLayer(string layerName, bool active)
    {
        foreach (MusicLayer layer in dynamicLayers)
        {
            if (layer.layerName == layerName)
            {
                layer.isActive = active;
                if (active && !layer.source.isPlaying)
                {
                    layer.source.Play();
                }
                break;
            }
        }
    }
    
    public void PlayStinger(AudioClip stinger, float volume = 1f)
    {
        if (stinger == null) return;
        
        GameObject stingerObj = new GameObject("Music Stinger");
        stingerObj.transform.SetParent(transform);
        
        AudioSource stingerSource = stingerObj.AddComponent<AudioSource>();
        stingerSource.clip = stinger;
        stingerSource.volume = volume * masterMusicVolume;
        stingerSource.spatialBlend = 0f;
        stingerSource.priority = 0;
        stingerSource.Play();
        
        Destroy(stingerObj, stinger.length);
    }
    
    public bool IsMusicPlaying()
    {
        return activeSource != null && activeSource.isPlaying;
    }
    
    public string GetCurrentTrackName()
    {
        return currentTrack != null ? currentTrack.trackName : "";
    }
    
    public float GetCurrentIntensity()
    {
        return currentIntensity;
    }
}