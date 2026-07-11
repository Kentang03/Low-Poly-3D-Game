using UnityEngine;

namespace Invector.vCharacterController
{
    /// <summary>
    /// Manages all character audio effects including footsteps, jumps, and landings
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class CharacterAudioController : MonoBehaviour
    {
        [Header("Audio Sources")]
        [Tooltip("Main audio source for character sounds")]
        public AudioSource audioSource;
        
        [Header("Jump & Landing Sounds")]
        [Tooltip("Sound played when character jumps")]
        public AudioClip jumpSound;
        [Range(0f, 1f)]
        public float jumpVolume = 0.7f;
        
        [Tooltip("Sound played when character lands")]
        public AudioClip landingSound;
        [Range(0f, 1f)]
        public float landingVolume = 0.6f;
        
        [Header("Footstep Sounds")]
        [Tooltip("Sounds played randomly for walking")]
        public AudioClip[] walkFootsteps;
        [Range(0f, 1f)]
        public float walkVolume = 0.5f;
        
        [Tooltip("Sounds played randomly for running")]
        public AudioClip[] runFootsteps;
        [Range(0f, 1f)]
        public float runVolume = 0.6f;
        
        [Tooltip("Sounds played randomly for sprinting")]
        public AudioClip[] sprintFootsteps;
        [Range(0f, 1f)]
        public float sprintVolume = 0.7f;
        
        [Header("Footstep Settings")]
        [Tooltip("Time between footsteps when walking")]
        public float walkStepInterval = 0.5f;
        [Tooltip("Time between footsteps when running")]
        public float runStepInterval = 0.35f;
        [Tooltip("Time between footsteps when sprinting")]
        public float sprintStepInterval = 0.25f;
        [Tooltip("Minimum input magnitude to play footsteps")]
        public float minInputForFootsteps = 0.1f;
        
        [Header("Pitch Variation")]
        [Tooltip("Add random pitch variation to footsteps for more natural sound")]
        public bool useRandomPitch = true;
        [Range(0f, 0.3f)]
        public float pitchVariation = 0.1f;
        
        // Internal variables
        private vThirdPersonController controller;
        private float footstepTimer = 0f;
        private bool wasGrounded = true;
        private bool wasJumping = false;
        
        void Awake()
        {
            // Get or add audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Configure audio source
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 15f;
            
            // Get controller reference
            controller = GetComponent<vThirdPersonController>();
            
            if (controller == null)
            {
                Debug.LogWarning("CharacterAudioController: No vThirdPersonController found on " + gameObject.name);
            }
        }
        
        void Update()
        {
            if (controller == null) return;
            
            // Handle footsteps
            HandleFootsteps();
            
            // Handle landing sound
            HandleLanding();
            
            // Update previous states
            wasGrounded = controller.isGrounded;
            wasJumping = controller.isJumping;
        }
        
        /// <summary>
        /// Plays footstep sounds based on movement speed and input
        /// </summary>
        private void HandleFootsteps()
        {
            // Only play footsteps when grounded and moving
            if (!controller.isGrounded || controller.input.magnitude < minInputForFootsteps)
            {
                footstepTimer = 0f;
                return;
            }
            
            // Don't play footsteps while jumping
            if (controller.isJumping)
            {
                footstepTimer = 0f;
                return;
            }
            
            // Update timer
            footstepTimer += Time.deltaTime;
            
            // Determine current speed and interval
            float currentInterval;
            AudioClip[] currentFootsteps;
            float currentVolume;
            
            if (controller.isSprinting)
            {
                currentInterval = sprintStepInterval;
                currentFootsteps = sprintFootsteps.Length > 0 ? sprintFootsteps : runFootsteps;
                currentVolume = sprintVolume;
            }
            else if (controller.inputMagnitude > 0.5f) // Running
            {
                currentInterval = runStepInterval;
                currentFootsteps = runFootsteps;
                currentVolume = runVolume;
            }
            else // Walking
            {
                currentInterval = walkStepInterval;
                currentFootsteps = walkFootsteps;
                currentVolume = walkVolume;
            }
            
            // Play footstep sound
            if (footstepTimer >= currentInterval)
            {
                PlayFootstep(currentFootsteps, currentVolume);
                footstepTimer = 0f;
            }
        }
        
        /// <summary>
        /// Plays a random footstep sound from the given array
        /// </summary>
        private void PlayFootstep(AudioClip[] footsteps, float volume)
        {
            if (footsteps == null || footsteps.Length == 0) return;
            
            // Get random footstep sound
            AudioClip clip = footsteps[Random.Range(0, footsteps.Length)];
            if (clip == null) return;
            
            // Apply random pitch if enabled
            if (useRandomPitch)
            {
                audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            }
            else
            {
                audioSource.pitch = 1f;
            }
            
            // Play sound
            audioSource.PlayOneShot(clip, volume);
        }
        
        /// <summary>
        /// Handles landing sound when character hits the ground
        /// </summary>
        private void HandleLanding()
        {
            // Detect landing (was in air, now grounded)
            if (!wasGrounded && controller.isGrounded && !controller.isJumping)
            {
                PlayLandingSound();
            }
        }
        
        /// <summary>
        /// Plays the jump sound effect
        /// Called from vThirdPersonController.Jump()
        /// </summary>
        public void PlayJumpSound()
        {
            if (jumpSound != null)
            {
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(jumpSound, jumpVolume);
            }
        }
        
        /// <summary>
        /// Plays the landing sound effect
        /// </summary>
        public void PlayLandingSound()
        {
            if (landingSound != null)
            {
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(landingSound, landingVolume);
            }
        }
        
        /// <summary>
        /// Stops all character audio
        /// </summary>
        public void StopAllAudio()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        
        /// <summary>
        /// Sets the master volume for all character sounds
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Clamp01(volume);
            }
        }
    }
}
