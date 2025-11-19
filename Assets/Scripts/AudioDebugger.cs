// AudioDebugger.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AudioDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool showDebugInfo = true;
    public bool visualizeAudioSources = true;
    public bool showSoundWaves = true;
    public bool logAudioEvents = false;
    public KeyCode toggleDebugKey = KeyCode.F12;
    
    [Header("Visualization")]
    public float visualizationRadius = 1f;
    public Color audioSourceColor = Color.cyan;
    public Color activeSourceColor = Color.green;
    public Color inactiveSourceColor = Color.red;
    public float waveAnimationSpeed = 2f;
    
    [Header("Performance Monitoring")]
    public bool monitorPerformance = true;
    public float updateInterval = 0.5f;
    
    // Runtime data
    private List<AudioSource> allAudioSources = new List<AudioSource>();
    private Dictionary<AudioSource, AudioInfo> audioInfoMap = new Dictionary<AudioSource, AudioInfo>();
    private float lastUpdateTime;
    private int activeSourceCount;
    private int totalSourceCount;
    private float totalAudioVolume;
    
    [System.Serializable]
    public class AudioInfo
    {
        public string name;
        public string clipName;
        public float volume;
        public float pitch;
        public bool isPlaying;
        public bool is3D;
        public float distance;
        public Vector3 position;
    }
    
    void Start()
    {
        RefreshAudioSources();
        InvokeRepeating(nameof(RefreshAudioSources), 1f, 2f);
    }
    
    void Update()
    {
        // Toggle debug display
        if (Input.GetKeyDown(toggleDebugKey))
        {
            showDebugInfo = !showDebugInfo;
        }
        
        // Update monitoring
        if (monitorPerformance && Time.time - lastUpdateTime > updateInterval)
        {
            UpdateAudioInfo();
            lastUpdateTime = Time.time;
        }
    }
    
    void RefreshAudioSources()
    {
        allAudioSources = FindObjectsOfType<AudioSource>().ToList();
        
        if (logAudioEvents)
        {
            Debug.Log($"Found {allAudioSources.Count} audio sources in scene");
        }
    }
    
    void UpdateAudioInfo()
    {
        audioInfoMap.Clear();
        activeSourceCount = 0;
        totalAudioVolume = 0f;
        
        foreach (AudioSource source in allAudioSources)
        {
            if (source == null) continue;
            
            AudioInfo info = new AudioInfo
            {
                name = source.gameObject.name,
                clipName = source.clip != null ? source.clip.name : "None",
                volume = source.volume,
                pitch = source.pitch,
                isPlaying = source.isPlaying,
                is3D = source.spatialBlend > 0.5f,
                position = source.transform.position
            };
            
            if (source.isPlaying)
            {
                activeSourceCount++;
                totalAudioVolume += source.volume;
            }
            
            // Calculate distance to listener
            AudioListener listener = FindObjectOfType<AudioListener>();
            if (listener != null)
            {
                info.distance = Vector3.Distance(source.transform.position, listener.transform.position);
            }
            
            audioInfoMap[source] = info;
        }
        
        totalSourceCount = allAudioSources.Count;
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        // Create debug window
        float windowWidth = 400f;
        float windowHeight = 500f;
        
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = MakeTexture(2, 2, new Color(0, 0, 0, 0.8f));
        
        GUILayout.BeginArea(new Rect(10, 10, windowWidth, windowHeight), boxStyle);
        
        // Title
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 18;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;
        GUILayout.Label("Audio System Debug", titleStyle);
        
        GUILayout.Space(10);
        
        // Summary info
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = Color.white;
        
        GUILayout.Label($"Total Audio Sources: {totalSourceCount}", labelStyle);
        GUILayout.Label($"Active Sources: {activeSourceCount}", labelStyle);
        GUILayout.Label($"Combined Volume: {totalAudioVolume:F2}", labelStyle);
        
        // Audio Pool info
        if (AudioPool.Instance != null)
        {
            GUILayout.Label($"Pool Available: {AudioPool.Instance.GetAvailableSourceCount()}", labelStyle);
            GUILayout.Label($"Pool Active: {AudioPool.Instance.GetActiveSourceCount()}", labelStyle);
        }
        
        // Music System info
        if (MusicSystem.Instance != null)
        {
            GUILayout.Label($"Current Music: {MusicSystem.Instance.GetCurrentTrackName()}", labelStyle);
            GUILayout.Label($"Music Intensity: {MusicSystem.Instance.GetCurrentIntensity():F2}", labelStyle);
        }
        
        // Zone Manager info
        if (SoundZoneManager.Instance != null)
        {
            GUILayout.Label($"Active Zones: {SoundZoneManager.Instance.activeZones.Count}", labelStyle);
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Active Audio Sources:", titleStyle);
        
        // Scrollable list of active sources
        if (audioInfoMap.Count > 0)
        {
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.Height(250));
            
            foreach (var kvp in audioInfoMap)
            {
                if (kvp.Value.isPlaying)
                {
                    GUILayout.BeginHorizontal();
                    
                    // Color indicator
                    Color indicatorColor = kvp.Value.is3D ? Color.cyan : Color.yellow;
                    DrawColorBox(indicatorColor, 10, 10);
                    
                    // Source info
                    string sourceInfo = $"{kvp.Value.name}: {kvp.Value.clipName} " +
                                      $"(Vol: {kvp.Value.volume:F2}, Pitch: {kvp.Value.pitch:F2}";
                    
                    if (kvp.Value.is3D)
                    {
                        sourceInfo += $", Dist: {kvp.Value.distance:F1}m";
                    }
                    sourceInfo += ")";
                    
                    GUILayout.Label(sourceInfo, labelStyle);
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndScrollView();
        }
        
        // Controls
        GUILayout.Space(10);
        
        if (GUILayout.Button("Refresh Audio Sources"))
        {
            RefreshAudioSources();
        }
        
        if (GUILayout.Button("Stop All Sounds"))
        {
            StopAllSounds();
        }
        
        GUILayout.Label($"Press {toggleDebugKey} to toggle debug display", labelStyle);
        
        GUILayout.EndArea();
    }
    
    void StopAllSounds()
    {
        foreach (AudioSource source in allAudioSources)
        {
            if (source != null)
            {
                source.Stop();
            }
        }
        
        Debug.Log("Stopped all audio sources");
    }
    
    void OnDrawGizmos()
    {
        if (!visualizeAudioSources) return;
        
        foreach (AudioSource source in allAudioSources)
        {
            if (source == null) continue;
            
            // Set color based on state
            if (source.isPlaying)
            {
                Gizmos.color = activeSourceColor;
                
                // Draw animated waves for playing sources
                if (showSoundWaves)
                {
                    float waveSize = visualizationRadius + Mathf.Sin(Time.time * waveAnimationSpeed) * 0.5f;
                    Gizmos.DrawWireSphere(source.transform.position, waveSize);
                    
                    waveSize = visualizationRadius + Mathf.Sin(Time.time * waveAnimationSpeed + 1f) * 0.5f;
                    Gizmos.color = new Color(activeSourceColor.r, activeSourceColor.g, activeSourceColor.b, 0.5f);
                    Gizmos.DrawWireSphere(source.transform.position, waveSize * 1.5f);
                }
            }
            else
            {
                Gizmos.color = inactiveSourceColor;
            }
            
            // Draw source indicator
            Gizmos.DrawWireCube(source.transform.position, Vector3.one * 0.3f);
            
            // Draw 3D sound range
            if (source.spatialBlend > 0.5f)
            {
                Gizmos.color = new Color(audioSourceColor.r, audioSourceColor.g, audioSourceColor.b, 0.3f);
                Gizmos.DrawWireSphere(source.transform.position, source.minDistance);
                
                Gizmos.color = new Color(audioSourceColor.r, audioSourceColor.g, audioSourceColor.b, 0.1f);
                Gizmos.DrawWireSphere(source.transform.position, source.maxDistance);
            }
            
            // Draw line to listener
            AudioListener listener = FindObjectOfType<AudioListener>();
            if (listener != null && source.isPlaying)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                Gizmos.DrawLine(source.transform.position, listener.transform.position);
            }
        }
    }
    
    Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }
    
    void DrawColorBox(Color color, float width, float height)
    {
        Texture2D texture = MakeTexture(1, 1, color);
        GUIStyle style = new GUIStyle();
        style.normal.background = texture;
        GUILayout.Box("", style, GUILayout.Width(width), GUILayout.Height(height));
    }
    
    // Public methods for external logging
    public void LogAudioEvent(string eventName, AudioSource source = null)
    {
        if (!logAudioEvents) return;
        
        string message = $"[Audio Event] {eventName}";
        if (source != null)
        {
            message += $" - Source: {source.gameObject.name}";
            if (source.clip != null)
            {
                message += $", Clip: {source.clip.name}";
            }
        }
        
        Debug.Log(message);
    }
    
    public void LogCollisionSound(Collision collision, AudioClip clipPlayed)
    {
        if (!logAudioEvents) return;
        
        Debug.Log($"[Collision Sound] {collision.gameObject.name} - Force: {collision.relativeVelocity.magnitude:F2}, Clip: {clipPlayed?.name ?? "None"}");
    }
}

// Audio testing utilities
public class AudioTestUtility : MonoBehaviour
{
    [Header("Test Settings")]
    public AudioClip testClip;
    public float testVolume = 1f;
    public bool test3DSound = true;
    
    [Header("Test Controls")]
    public KeyCode playTestSound = KeyCode.T;
    public KeyCode playAtRandomPosition = KeyCode.R;
    public KeyCode testDopplerFlyby = KeyCode.D;
    
    void Update()
    {
        if (Input.GetKeyDown(playTestSound))
        {
            PlayTestSound();
        }
        
        if (Input.GetKeyDown(playAtRandomPosition))
        {
            PlayAtRandomPosition();
        }
        
        if (Input.GetKeyDown(testDopplerFlyby))
        {
            TestDopplerFlyby();
        }
    }
    
    void PlayTestSound()
    {
        if (testClip == null) return;
        
        if (AudioPool.Instance != null)
        {
            AudioPool.Instance.PlayClipAtPoint(testClip, transform.position, testVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(testClip, transform.position, testVolume);
        }
        
        Debug.Log($"Played test sound: {testClip.name}");
    }
    
    void PlayAtRandomPosition()
    {
        if (testClip == null) return;
        
        Vector3 randomPos = transform.position + Random.insideUnitSphere * 10f;
        randomPos.y = transform.position.y;
        
        if (AudioPool.Instance != null)
        {
            AudioPool.Instance.PlayClipAtPoint(testClip, randomPos, testVolume);
        }
        
        Debug.Log($"Played test sound at random position: {randomPos}");
    }
    
    void TestDopplerFlyby()
    {
        if (testClip == null) return;
        
        GameObject flybyObject = new GameObject("Doppler Test Object");
        flybyObject.transform.position = transform.position + Vector3.left * 20f;
        
        AudioSource source = flybyObject.AddComponent<AudioSource>();
        source.clip = testClip;
        source.loop = true;
        source.spatialBlend = 1f;
        source.dopplerLevel = 2f;
        source.volume = testVolume;
        source.Play();
        
        Rigidbody rb = flybyObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = Vector3.right * 30f;
        
        Destroy(flybyObject, 5f);
        
        Debug.Log("Started Doppler flyby test");
    }
}