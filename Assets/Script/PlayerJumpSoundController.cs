using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Mengelola sound effects untuk jump player
/// Script ini mengintegrasikan dengan Invector Third Person Controller
/// </summary>
public class PlayerJumpSoundController : MonoBehaviour
{
    [Header("Jump Sound Settings")]
    public AudioClip[] jumpSounds;
    public AudioClip landingSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float jumpSoundVolume = 0.7f;
    [Range(0f, 1f)]
    public float landingSoundVolume = 0.8f;
    
    [Header("Sound Variation")]
    public bool randomizePitch = true;
    [Range(0.8f, 1.2f)]
    public float minPitch = 0.9f;
    [Range(0.8f, 1.2f)]
    public float maxPitch = 1.1f;
    
    [Header("Cooldown Settings")]
    public float jumpSoundCooldown = 0.2f; // Prevent spam clicking
    public float landingSoundCooldown = 0.3f;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    // Component references
    private AudioSource audioSource;
    private vThirdPersonController playerController;
    
    // State tracking
    private bool wasJumping = false;
    private bool wasGrounded = true;
    private float lastJumpSoundTime = 0f;
    private float lastLandingSoundTime = 0f;
    
    void Start()
    {
        InitializeComponents();
    }
    
    void Update()
    {
        if (playerController != null)
        {
            CheckJumpState();
            CheckLandingState();
        }
    }
    
    /// <summary>
    /// Initialize required components
    /// </summary>
    void InitializeComponents()
    {
        // Get or create AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            
            // Configure AudioSource for sound effects
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.volume = jumpSoundVolume;
        }
        
        // Find player controller
        playerController = GetComponent<vThirdPersonController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<vThirdPersonController>();
            
            if (playerController == null)
            {
                Debug.LogError("PlayerJumpSoundController: vThirdPersonController not found! Please ensure this script is on the player or a vThirdPersonController exists in the scene.");
                return;
            }
        }
        
        // Initialize state
        wasGrounded = playerController.isGrounded;
        wasJumping = playerController.isJumping;
        
        if (showDebugLogs)
            Debug.Log("PlayerJumpSoundController initialized successfully");
    }
    
    /// <summary>
    /// Check for jump state changes
    /// </summary>
    void CheckJumpState()
    {
        // Detect jump start
        if (!wasJumping && playerController.isJumping)
        {
            OnJumpStart();
        }
        
        // Update state
        wasJumping = playerController.isJumping;
    }
    
    /// <summary>
    /// Check for landing state changes
    /// </summary>
    void CheckLandingState()
    {
        // Detect landing (was airborne, now grounded)
        if (!wasGrounded && playerController.isGrounded && !playerController.isJumping)
        {
            OnLanding();
        }
        
        // Update state
        wasGrounded = playerController.isGrounded;
    }
    
    /// <summary>
    /// Called when player starts jumping
    /// </summary>
    void OnJumpStart()
    {
        // Check cooldown
        if (Time.time - lastJumpSoundTime < jumpSoundCooldown)
            return;
            
        PlayJumpSound();
        lastJumpSoundTime = Time.time;
        
        if (showDebugLogs)
            Debug.Log("Player jumped!");
    }
    
    /// <summary>
    /// Called when player lands
    /// </summary>
    void OnLanding()
    {
        // Check cooldown
        if (Time.time - lastLandingSoundTime < landingSoundCooldown)
            return;
            
        PlayLandingSound();
        lastLandingSoundTime = Time.time;
        
        if (showDebugLogs)
            Debug.Log("Player landed!");
    }
    
    /// <summary>
    /// Play jump sound effect
    /// </summary>
    void PlayJumpSound()
    {
        if (jumpSounds == null || jumpSounds.Length == 0)
        {
            if (showDebugLogs)
                Debug.LogWarning("No jump sounds assigned!");
            return;
        }
        
        // Select random sound
        AudioClip soundToPlay = jumpSounds[Random.Range(0, jumpSounds.Length)];
        
        if (soundToPlay != null)
        {
            // Apply pitch variation
            if (randomizePitch)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }
            else
            {
                audioSource.pitch = 1f;
            }
            
            // Play sound
            audioSource.volume = jumpSoundVolume;
            audioSource.PlayOneShot(soundToPlay);
            
            if (showDebugLogs)
                Debug.Log($"Played jump sound: {soundToPlay.name}");
        }
    }
    
    /// <summary>
    /// Play landing sound effect
    /// </summary>
    void PlayLandingSound()
    {
        if (landingSound == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("No landing sound assigned!");
            return;
        }
        
        // Apply pitch variation
        if (randomizePitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            audioSource.pitch = 1f;
        }
        
        // Play sound
        audioSource.volume = landingSoundVolume;
        audioSource.PlayOneShot(landingSound);
        
        if (showDebugLogs)
            Debug.Log($"Played landing sound: {landingSound.name}");
    }
    
    /// <summary>
    /// Add jump sound to the array
    /// </summary>
    public void AddJumpSound(AudioClip newSound)
    {
        if (newSound == null) return;
        
        if (jumpSounds == null)
        {
            jumpSounds = new AudioClip[] { newSound };
        }
        else
        {
            AudioClip[] newArray = new AudioClip[jumpSounds.Length + 1];
            for (int i = 0; i < jumpSounds.Length; i++)
            {
                newArray[i] = jumpSounds[i];
            }
            newArray[jumpSounds.Length] = newSound;
            jumpSounds = newArray;
        }
        
        if (showDebugLogs)
            Debug.Log($"Added jump sound: {newSound.name}");
    }
    
    /// <summary>
    /// Set landing sound
    /// </summary>
    public void SetLandingSound(AudioClip newLandingSound)
    {
        landingSound = newLandingSound;
        
        if (showDebugLogs)
            Debug.Log($"Set landing sound: {newLandingSound.name}");
    }
    
    /// <summary>
    /// Enable/disable debug logging
    /// </summary>
    public void SetDebugMode(bool enabled)
    {
        showDebugLogs = enabled;
    }
    
    /// <summary>
    /// Test jump sound manually
    /// </summary>
    [ContextMenu("Test Jump Sound")]
    public void TestJumpSound()
    {
        PlayJumpSound();
    }
    
    /// <summary>
    /// Test landing sound manually
    /// </summary>
    [ContextMenu("Test Landing Sound")]
    public void TestLandingSound()
    {
        PlayLandingSound();
    }
    
    /// <summary>
    /// Enable debug mode for testing
    /// </summary>
    [ContextMenu("Enable Debug Mode")]
    public void EnableDebugMode()
    {
        SetDebugMode(true);
    }
    
    /// <summary>
    /// Disable debug mode
    /// </summary>
    [ContextMenu("Disable Debug Mode")]
    public void DisableDebugMode()
    {
        SetDebugMode(false);
    }
    
    // Unity Event integration methods
    public void OnPlayerJump()
    {
        PlayJumpSound();
    }
    
    public void OnPlayerLand()
    {
        PlayLandingSound();
    }
}