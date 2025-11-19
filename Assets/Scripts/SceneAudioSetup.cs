// SceneAudioSetup.cs
// This script demonstrates how to set up all audio systems in a Unity scene

using UnityEngine;
using UnityEditor;

public class SceneAudioSetup : MonoBehaviour
{
    [Header("Scene Setup Guide")]
    [TextArea(10, 20)]
    public string setupInstructions = @"
UNITY 6.2 AUDIO SYSTEM SETUP GUIDE
===================================

1. HIERARCHY SETUP:
   □ Create empty GameObject named 'Audio Systems'
   □ Add AudioManager component
   □ Add MusicSystem component
   □ Add SoundZoneManager component
   □ Add AudioPool component
   □ Add AudioDebugger component (optional)

2. PLAYER SETUP:
   □ Add PlayerActionSounds to player GameObject
   □ Ensure player has CharacterController
   □ Ensure main camera has AudioListener

3. ENVIRONMENT SETUP:
   □ Create trigger zones for ambient sounds
   □ Add LocationSoundTrigger or ManagedLocationTrigger
   □ Set up colliders as triggers
   □ Assign ambient audio clips

4. VEHICLES/MOVING OBJECTS:
   □ Add Rigidbody component
   □ Add CarAudioController for vehicles
   □ Add CustomDopplerEffect for Doppler
   □ Set up WheelColliders for cars

5. INTERACTABLES:
   □ Add DoorInteractable to doors
   □ Add CollisionSoundController to physics objects
   □ Set up material tags for different sounds

6. AUDIO FILES ORGANIZATION:
   Resources/
   ├── Sounds/
   │   ├── Ambient/
   │   ├── Footsteps/
   │   ├── Impacts/
   │   ├── UI/
   │   └── Vehicle/
   └── Music/
       ├── Menu/
       ├── Gameplay/
       └── Cinematics/";

    [Header("Quick Setup")]
    public bool autoSetupScene = false;
    
    void Start()
    {
        if (autoSetupScene)
        {
            SetupAudioSystems();
        }
    }
    
    [ContextMenu("Setup Audio Systems")]
    public void SetupAudioSystems()
    {
        // Check if audio systems already exist
        if (GameObject.Find("Audio Systems") != null)
        {
            Debug.LogWarning("Audio Systems already exists in scene!");
            return;
        }
        
        // Create main audio systems object
        GameObject audioSystems = new GameObject("Audio Systems");
        
        // Add core components
        audioSystems.AddComponent<AudioManager>();
        audioSystems.AddComponent<MusicSystem>();
        audioSystems.AddComponent<SoundZoneManager>();
        audioSystems.AddComponent<AudioPool>();
        
        // Add debug component
        AudioDebugger debugger = audioSystems.AddComponent<AudioDebugger>();
        debugger.showDebugInfo = false; // Start with debug off
        
        Debug.Log("Audio Systems setup complete!");
        
        // Check for player
        CheckPlayerSetup();
        
        // Check for audio listener
        CheckAudioListener();
    }
    
    void CheckPlayerSetup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No GameObject with 'Player' tag found. Please tag your player object.");
            return;
        }
        
        
        if (player.GetComponent<CharacterController>() == null && 
            player.GetComponent<Rigidbody>() == null)
        {
            Debug.LogWarning("Player has no CharacterController or Rigidbody. Movement sounds may not work properly.");
        }
    }
    
    void CheckAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        
        if (listeners.Length == 0)
        {
            Debug.LogError("No AudioListener found! Add one to your main camera.");
            
            // Try to add to main camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.gameObject.AddComponent<AudioListener>();
                Debug.Log("Added AudioListener to main camera.");
            }
        }
        else if (listeners.Length > 1)
        {
            Debug.LogWarning($"Multiple AudioListeners found ({listeners.Length}). This may cause audio issues.");
        }
    }
}

// Example implementation for a complete game scene
public class GameAudioIntegration : MonoBehaviour
{
    [Header("Audio References")]
    public AudioManager audioManager;
    public MusicSystem musicSystem;
    public SoundZoneManager zoneManager;
    
    [Header("Game States")]
    public bool inMenu = true;
    public bool inCombat = false;
    public bool isPaused = false;
    
    [Header("Music Tracks")]
    public string menuMusic = "MainTheme";
    public string explorationMusic = "Exploration";
    public string combatMusic = "BattleTheme";
    public string victoryMusic = "Victory";
    
    void Start()
    {
        InitializeAudio();
    }
    
    void InitializeAudio()
    {
        // Get references if not assigned
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        if (musicSystem == null)
            musicSystem = FindObjectOfType<MusicSystem>();
        if (zoneManager == null)
            zoneManager = FindObjectOfType<SoundZoneManager>();
        
        // Start appropriate music
        if (inMenu)
        {
            musicSystem?.PlayMusic(menuMusic);
        }
    }
    
    public void OnGameStart()
    {
        inMenu = false;
        musicSystem?.PlayMusic(explorationMusic);
        audioManager?.Play("GameStart");
    }
    
    public void OnEnterCombat()
    {
        if (!inCombat)
        {
            inCombat = true;
            musicSystem?.PlayMusic(combatMusic);
            musicSystem?.SetMusicIntensity(1f);
            audioManager?.Play("CombatAlert");
        }
    }
    
    public void OnExitCombat(bool victory)
    {
        if (inCombat)
        {
            inCombat = false;
            
            if (victory)
            {
                musicSystem?.PlayMusic(victoryMusic);
                audioManager?.Play("VictoryFanfare");
                // Return to exploration after victory music
                Invoke(nameof(ReturnToExploration), 5f);
            }
            else
            {
                musicSystem?.PlayMusic(explorationMusic);
            }
            
            musicSystem?.SetMusicIntensity(0.5f);
        }
    }
    
    void ReturnToExploration()
    {
        musicSystem?.PlayMusic(explorationMusic);
    }
    
    public void OnPauseGame()
    {
        isPaused = true;
        
        // Reduce all audio volumes
        AudioListener.volume = 0.3f;
        audioManager?.Play("PauseSound");
    }
    
    public void OnResumeGame()
    {
        isPaused = false;
        
        // Restore audio volumes
        AudioListener.volume = 1f;
        audioManager?.Play("ResumeSound");
    }
    
    public void OnPlayerDeath()
    {
        musicSystem?.StopMusic(2f);
        audioManager?.Play("PlayerDeath");
    }
    
    public void OnItemPickup(string itemType)
    {
        switch (itemType)
        {
            case "Health":
                audioManager?.Play("HealthPickup");
                break;
            case "Weapon":
                audioManager?.Play("WeaponPickup");
                break;
            case "Key":
                audioManager?.Play("KeyPickup");
                break;
            default:
                audioManager?.Play("GenericPickup");
                break;
        }
    }
}

// Scriptable Object for Audio Configuration
[CreateAssetMenu(fileName = "AudioConfig", menuName = "Audio/Configuration")]
public class AudioConfiguration : ScriptableObject
{
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float ambientVolume = 0.5f;
    
    [Header("3D Audio Settings")]
    public float defaultMinDistance = 1f;
    public float defaultMaxDistance = 50f;
    public AudioRolloffMode defaultRolloffMode = AudioRolloffMode.Logarithmic;
    
    [Header("Doppler Settings")]
    [Range(0f, 5f)]
    public float dopplerLevel = 1f;
    public float speedOfSound = 343f;
    
    [Header("Performance")]
    public int maxVirtualVoices = 512;
    public int maxRealVoices = 32;
    public bool disableAudioInBackground = true;
    
    public void ApplySettings()
    {
        AudioListener.volume = masterVolume;
        
        // Apply to all audio sources in scene
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.clip != null)
            {
                // Determine source type by name or tag
                if (source.gameObject.name.Contains("Music"))
                {
                    source.volume *= musicVolume;
                }
                else if (source.gameObject.name.Contains("Ambient"))
                {
                    source.volume *= ambientVolume;
                }
                else
                {
                    source.volume *= sfxVolume;
                }
                
                // Apply 3D settings
                if (source.spatialBlend > 0.5f)
                {
                    source.minDistance = defaultMinDistance;
                    source.maxDistance = defaultMaxDistance;
                    source.rolloffMode = defaultRolloffMode;
                    source.dopplerLevel = dopplerLevel;
                }
            }
        }
        
        Debug.Log("Audio configuration applied!");
    }
}

// Helper class for audio triggers in Timeline or Animations
public class AudioEventTrigger : MonoBehaviour
{
    [Header("Audio Event")]
    public string soundName;
    public AudioClip directClip;
    public float volume = 1f;
    public float delay = 0f;
    public bool use3DPosition = true;
    
    // Can be called from Animation Events
    public void TriggerSound()
    {
        if (delay > 0)
        {
            Invoke(nameof(PlayDelayedSound), delay);
        }
        else
        {
            PlayDelayedSound();
        }
    }
    
    void PlayDelayedSound()
    {
        // Try to use AudioManager first
        if (!string.IsNullOrEmpty(soundName) && AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(soundName);
        }
        // Otherwise use direct clip
        else if (directClip != null)
        {
            if (use3DPosition)
            {
                if (AudioPool.Instance != null)
                {
                    AudioPool.Instance.PlayClipAtPoint(directClip, transform.position, volume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(directClip, transform.position, volume);
                }
            }
            else
            {
                // Play 2D sound
                AudioSource tempSource = gameObject.AddComponent<AudioSource>();
                tempSource.clip = directClip;
                tempSource.volume = volume;
                tempSource.spatialBlend = 0f;
                tempSource.Play();
                Destroy(tempSource, directClip.length);
            }
        }
    }
    
    // For Timeline integration
    public void OnTimelineSignal()
    {
        TriggerSound();
    }
}