// SoundZoneManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SoundZoneManager : MonoBehaviour
{
    public static SoundZoneManager Instance;
    
    [System.Serializable]
    public class SoundZone
    {
        public string zoneName;
        public AudioSource audioSource;
        public float targetVolume;
        public float priority;
        public float fadeSpeed = 1f;
        public bool isActive = false;
        [HideInInspector]
        public float currentVolume = 0f;
    }
    
    [Header("Zone Management")]
    public List<SoundZone> activeZones = new List<SoundZone>();
    public bool allowMultipleZones = true;
    public float masterVolume = 1f;
    
    [Header("Transition Settings")]
    public float defaultFadeSpeed = 2f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
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
        }
    }
    
    void Update()
    {
        UpdateZoneVolumes();
    }
    
    public void EnterZone(string zoneName, AudioSource source, float targetVolume = 1f, float priority = 0f)
    {
        // Check if zone already exists
        SoundZone existingZone = activeZones.FirstOrDefault(z => z.zoneName == zoneName);
        
        if (existingZone != null)
        {
            existingZone.isActive = true;
            return;
        }
        
        // Create new zone
        SoundZone newZone = new SoundZone
        {
            zoneName = zoneName,
            audioSource = source,
            targetVolume = targetVolume,
            priority = priority,
            fadeSpeed = defaultFadeSpeed,
            isActive = true,
            currentVolume = 0f
        };
        
        activeZones.Add(newZone);
        
        // Sort by priority
        activeZones = activeZones.OrderByDescending(z => z.priority).ToList();
        
        // Start playing if not already
        if (!source.isPlaying)
        {
            source.volume = 0f;
            source.Play();
        }
        
        Debug.Log($"Entered sound zone: {zoneName}");
    }
    
    public void ExitZone(string zoneName)
    {
        SoundZone zone = activeZones.FirstOrDefault(z => z.zoneName == zoneName);
        if (zone != null)
        {
            zone.isActive = false;
            Debug.Log($"Exited sound zone: {zoneName}");
        }
    }
    
    void UpdateZoneVolumes()
    {
        if (!allowMultipleZones && activeZones.Count > 1)
        {
            // Only play highest priority zone
            for (int i = 0; i < activeZones.Count; i++)
            {
                SoundZone zone = activeZones[i];
                float targetVol = (i == 0 && zone.isActive) ? zone.targetVolume : 0f;
                UpdateZoneVolume(zone, targetVol);
            }
        }
        else
        {
            // Allow multiple overlapping zones
            foreach (SoundZone zone in activeZones)
            {
                float targetVol = zone.isActive ? zone.targetVolume : 0f;
                UpdateZoneVolume(zone, targetVol);
            }
        }
        
        // Remove inactive zones that have faded out
        activeZones.RemoveAll(z => !z.isActive && z.currentVolume <= 0.01f);
    }
    
    void UpdateZoneVolume(SoundZone zone, float targetVolume)
    {
        // Smooth volume transition
        if (zone.currentVolume != targetVolume)
        {
            float fadeDirection = targetVolume > zone.currentVolume ? 1f : -1f;
            zone.currentVolume = Mathf.MoveTowards(zone.currentVolume, targetVolume, 
                zone.fadeSpeed * Time.deltaTime);
            
            // Apply fade curve
            float curveValue = fadeDirection > 0 ? 
                fadeInCurve.Evaluate(zone.currentVolume) : 
                fadeOutCurve.Evaluate(zone.currentVolume);
            
            // Set actual volume
            if (zone.audioSource != null)
            {
                zone.audioSource.volume = curveValue * masterVolume;
                
                // Stop audio when fully faded out
                if (zone.currentVolume <= 0.01f && zone.audioSource.isPlaying)
                {
                    zone.audioSource.Stop();
                }
            }
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
    
    public void FadeOutAllZones(float duration = 1f)
    {
        foreach (SoundZone zone in activeZones)
        {
            zone.isActive = false;
            zone.fadeSpeed = zone.targetVolume / duration;
        }
    }
    
    public bool IsInZone(string zoneName)
    {
        SoundZone zone = activeZones.FirstOrDefault(z => z.zoneName == zoneName);
        return zone != null && zone.isActive;
    }
    
    public float GetZoneVolume(string zoneName)
    {
        SoundZone zone = activeZones.FirstOrDefault(z => z.zoneName == zoneName);
        return zone != null ? zone.currentVolume : 0f;
    }
}

// Enhanced Location Trigger that works with the manager
public class ManagedLocationTrigger : MonoBehaviour
{
    [Header("Zone Settings")]
    public string zoneName;
    public AudioClip ambientClip;
    public float volume = 0.7f;
    public float priority = 0f;
    
    [Header("Trigger Settings")]
    public bool triggerOnce = false;
    public LayerMask triggerLayers = -1;
    
    private AudioSource audioSource;
    private bool hasTriggered = false;
    
    void Start()
    {
        // Create audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ambientClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound for ambient
        audioSource.volume = 0f;
        
        // Auto-generate zone name if not set
        if (string.IsNullOrEmpty(zoneName))
        {
            zoneName = gameObject.name + "_Zone";
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;
        
        if (((1 << other.gameObject.layer) & triggerLayers) != 0)
        {
            if (SoundZoneManager.Instance != null)
            {
                SoundZoneManager.Instance.EnterZone(zoneName, audioSource, volume, priority);
                hasTriggered = true;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & triggerLayers) != 0)
        {
            if (SoundZoneManager.Instance != null)
            {
                SoundZoneManager.Instance.ExitZone(zoneName);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
        
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.8f);
            Gizmos.DrawWireCube(box.center, box.size);
            Gizmos.matrix = oldMatrix;
        }
        
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius * transform.lossyScale.x);
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.8f);
            Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius * transform.lossyScale.x);
        }
    }
    
    /*void OnDrawGizmosSelected()
    {
        // Draw zone name
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
            $"Zone: {zoneName}\nPriority: {priority}\nVolume: {volume}");
    }*/
}