using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [Header("Footstep Sound Names (AudioManager)")]
    public string[] footstepSoundNames;

    [Header("Jump & Land Sounds (AudioManager)")]
    public string jumpSoundName = "Jump";
    public string landSoundName = "Land";

    [Header("Settings")]
    public float stepInterval = 0.45f;
    public float speedThreshold = 0.1f;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;

    private CharacterController controller;
    private Vector3 lastPosition;
    private float stepTimer;
    private bool wasGrounded = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (controller == null)
            return;

        bool grounded = controller.isGrounded;

        // --- LAND SOUND ---
        if (!wasGrounded && grounded)
        {
            PlayLandSound();
        }

        // --- JUMP SOUND ---
        if (wasGrounded && !grounded)
        {
            PlayJumpSound();
        }

        // --- FOOTSTEPS ---
        if (grounded)
            HandleFootsteps();

        wasGrounded = grounded;
        lastPosition = transform.position;
    }

    private void HandleFootsteps()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (speed > speedThreshold)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }

    private void PlayFootstep()
    {
        if (footstepSoundNames.Length == 0)
            return;

        string soundToPlay =
            footstepSoundNames[Random.Range(0, footstepSoundNames.Length)];

        float pitch = Random.Range(pitchMin, pitchMax);

        AudioSource src = AudioManager.Instance.GetSource(soundToPlay);
        if (src != null)
            src.pitch = pitch;

        AudioManager.Instance.Play(soundToPlay);
    }


    private void PlayJumpSound()
    {
        if (!string.IsNullOrEmpty(jumpSoundName))
            AudioManager.Instance.Play(jumpSoundName);
    }

    private void PlayLandSound()
    {
        if (!string.IsNullOrEmpty(landSoundName))
            AudioManager.Instance.Play(landSoundName);
    }
}
