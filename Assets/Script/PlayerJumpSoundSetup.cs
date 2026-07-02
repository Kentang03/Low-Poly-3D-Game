using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Setup wizard untuk PlayerJumpSoundController
/// Membantu setup dan konfigurasi jump sound system
/// </summary>
public class PlayerJumpSoundSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupOnStart = false;
    
    [Header("Default Sound Settings")]
    public AudioClip[] defaultJumpSounds;
    public AudioClip defaultLandingSound;
    
    [Header("Setup Status")]
    [SerializeField] private bool playerControllerFound = false;
    [SerializeField] private bool jumpSoundControllerReady = false;
    [SerializeField] private bool audioSourceReady = false;
    [SerializeField] private bool soundsAssigned = false;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupPlayerJumpSoundSystem();
        }
    }
    
    /// <summary>
    /// Setup complete jump sound system
    /// </summary>
    [ContextMenu("🔊 Setup Player Jump Sound System")]
    public void SetupPlayerJumpSoundSystem()
    {
        Debug.Log("=== Player Jump Sound Setup Starting ===");
        
        FindPlayerController();
        SetupJumpSoundController();
        ConfigureAudioSource();
        AssignDefaultSounds();
        
        ValidateSetup();
        PrintSetupSummary();
        
        Debug.Log("=== Player Jump Sound Setup Complete! ===");
    }
    
    /// <summary>
    /// Find vThirdPersonController in scene
    /// </summary>
    [ContextMenu("🎯 Find Player Controller")]
    public void FindPlayerController()
    {
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerController != null)
        {
            Debug.Log($"✅ Found vThirdPersonController on: {playerController.gameObject.name}");
            playerControllerFound = true;
            
            // Ensure setup script is on same GameObject for convenience
            if (transform.gameObject != playerController.gameObject)
            {
                Debug.Log($"ℹ️ Consider moving this setup script to the player GameObject: {playerController.gameObject.name}");
            }
        }
        else
        {
            Debug.LogError("❌ vThirdPersonController not found in scene!");
            Debug.LogError("   Please ensure you have a player with vThirdPersonController component.");
            playerControllerFound = false;
        }
    }
    
    /// <summary>
    /// Setup PlayerJumpSoundController component
    /// </summary>
    [ContextMenu("🔊 Setup Jump Sound Controller")]
    public void SetupJumpSoundController()
    {
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ Cannot setup: vThirdPersonController not found!");
            return;
        }
        
        PlayerJumpSoundController existingController = playerController.GetComponent<PlayerJumpSoundController>();
        
        if (existingController == null)
        {
            // Add PlayerJumpSoundController to player
            PlayerJumpSoundController jumpSoundController = playerController.gameObject.AddComponent<PlayerJumpSoundController>();
            
            // Configure default settings
            jumpSoundController.jumpSoundVolume = 0.7f;
            jumpSoundController.landingSoundVolume = 0.8f;
            jumpSoundController.randomizePitch = true;
            jumpSoundController.minPitch = 0.9f;
            jumpSoundController.maxPitch = 1.1f;
            jumpSoundController.jumpSoundCooldown = 0.2f;
            jumpSoundController.landingSoundCooldown = 0.3f;
            jumpSoundController.showDebugLogs = false;
            
            Debug.Log("✅ PlayerJumpSoundController added to player");
        }
        else
        {
            Debug.Log("✅ PlayerJumpSoundController already exists");
        }
        
        jumpSoundControllerReady = true;
    }
    
    /// <summary>
    /// Configure AudioSource component
    /// </summary>
    [ContextMenu("🎵 Configure Audio Source")]
    public void ConfigureAudioSource()
    {
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ Cannot configure: vThirdPersonController not found!");
            return;
        }
        
        AudioSource audioSource = playerController.GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            // Add AudioSource
            audioSource = playerController.gameObject.AddComponent<AudioSource>();
            Debug.Log("✅ AudioSource added to player");
        }
        else
        {
            Debug.Log("✅ AudioSource already exists");
        }
        
        // Configure AudioSource for jump sounds
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound for jump effects
        audioSource.volume = 0.7f;
        audioSource.pitch = 1f;
        
        Debug.Log("✅ AudioSource configured for jump sounds");
        audioSourceReady = true;
    }
    
    /// <summary>
    /// Assign default sounds if provided
    /// </summary>
    [ContextMenu("🎶 Assign Default Sounds")]
    public void AssignDefaultSounds()
    {
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ Cannot assign sounds: vThirdPersonController not found!");
            return;
        }
        
        PlayerJumpSoundController jumpSoundController = playerController.GetComponent<PlayerJumpSoundController>();
        
        if (jumpSoundController == null)
        {
            Debug.LogError("❌ Cannot assign sounds: PlayerJumpSoundController not found!");
            Debug.LogError("   Please run 'Setup Jump Sound Controller' first.");
            return;
        }
        
        // Assign jump sounds
        if (defaultJumpSounds != null && defaultJumpSounds.Length > 0)
        {
            jumpSoundController.jumpSounds = defaultJumpSounds;
            Debug.Log($"✅ Assigned {defaultJumpSounds.Length} jump sounds");
            soundsAssigned = true;
        }
        else
        {
            Debug.LogWarning("⚠️ No default jump sounds provided. Please assign jump sounds manually.");
            soundsAssigned = false;
        }
        
        // Assign landing sound
        if (defaultLandingSound != null)
        {
            jumpSoundController.landingSound = defaultLandingSound;
            Debug.Log("✅ Assigned landing sound");
        }
        else
        {
            Debug.LogWarning("⚠️ No default landing sound provided. Please assign landing sound manually.");
        }
    }
    
    /// <summary>
    /// Create example/test sounds (simple beep sounds for testing)
    /// </summary>
    [ContextMenu("🧪 Create Test Sounds")]
    public void CreateTestSounds()
    {
        Debug.Log("Creating test sounds for jump system...");
        
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ Cannot create test sounds: vThirdPersonController not found!");
            return;
        }
        
        PlayerJumpSoundController jumpSoundController = playerController.GetComponent<PlayerJumpSoundController>();
        
        if (jumpSoundController == null)
        {
            Debug.LogError("❌ Cannot create test sounds: PlayerJumpSoundController not found!");
            return;
        }
        
        Debug.Log("✅ Test sounds would be created here (requires audio generation)");
        Debug.Log("   For now, please assign your own AudioClip files to the jump sound controller.");
        Debug.Log("   Recommended: Use short sound effects (0.1-0.5 seconds) for best results.");
    }
    
    /// <summary>
    /// Validate complete setup
    /// </summary>
    void ValidateSetup()
    {
        Debug.Log("=== Validating Jump Sound System ===");
        
        bool allValid = true;
        
        if (playerControllerFound)
            Debug.Log("✅ Player Controller: Found");
        else
        {
            Debug.LogError("❌ Player Controller: Missing");
            allValid = false;
        }
        
        if (jumpSoundControllerReady)
            Debug.Log("✅ Jump Sound Controller: Ready");
        else
        {
            Debug.LogError("❌ Jump Sound Controller: Not Ready");
            allValid = false;
        }
        
        if (audioSourceReady)
            Debug.Log("✅ Audio Source: Ready");
        else
        {
            Debug.LogError("❌ Audio Source: Not Ready");
            allValid = false;
        }
        
        if (soundsAssigned)
            Debug.Log("✅ Sounds: Assigned");
        else
        {
            Debug.LogWarning("⚠️ Sounds: Not Assigned (Manual assignment needed)");
        }
        
        if (allValid && soundsAssigned)
        {
            Debug.Log("🎉 Jump Sound System validated successfully!");
        }
        else
        {
            Debug.LogWarning("⚠️ Jump Sound System setup incomplete. Check warnings above.");
        }
    }
    
    /// <summary>
    /// Print setup summary
    /// </summary>
    void PrintSetupSummary()
    {
        Debug.Log("=== Jump Sound System Summary ===");
        Debug.Log($"✅ Player Controller: {(playerControllerFound ? "Found" : "Missing")}");
        Debug.Log($"✅ Jump Sound Controller: {(jumpSoundControllerReady ? "Ready" : "Missing")}");
        Debug.Log($"✅ Audio Source: {(audioSourceReady ? "Ready" : "Missing")}");
        Debug.Log($"✅ Sounds Assigned: {(soundsAssigned ? "Yes" : "Manual needed")}");
        
        if (IsSystemReady())
        {
            Debug.Log("🚀 JUMP SOUND SYSTEM READY! 🚀");
            Debug.Log("Player will now play sound effects when jumping and landing!");
        }
        else
        {
            Debug.Log("⚠️ System needs attention. Please complete missing steps.");
        }
    }
    
    /// <summary>
    /// Check if system is completely ready
    /// </summary>
    public bool IsSystemReady()
    {
        return playerControllerFound && jumpSoundControllerReady && audioSourceReady;
    }
    
    /// <summary>
    /// Test the jump sound system
    /// </summary>
    [ContextMenu("🧪 Test Jump Sounds")]
    public void TestJumpSounds()
    {
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ Cannot test: vThirdPersonController not found!");
            return;
        }
        
        PlayerJumpSoundController jumpSoundController = playerController.GetComponent<PlayerJumpSoundController>();
        
        if (jumpSoundController == null)
        {
            Debug.LogError("❌ Cannot test: PlayerJumpSoundController not found!");
            return;
        }
        
        Debug.Log("🧪 Testing jump sounds...");
        jumpSoundController.TestJumpSound();
        
        // Test landing sound after a delay
        Invoke(nameof(TestLandingDelayed), 1f);
    }
    
    void TestLandingDelayed()
    {
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        PlayerJumpSoundController jumpSoundController = playerController?.GetComponent<PlayerJumpSoundController>();
        
        if (jumpSoundController != null)
        {
            Debug.Log("🧪 Testing landing sound...");
            jumpSoundController.TestLandingSound();
        }
    }
    
    /// <summary>
    /// Check current system status
    /// </summary>
    [ContextMenu("📊 Check System Status")]
    public void CheckSystemStatus()
    {
        Debug.Log("=== Current Jump Sound System Status ===");
        
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        PlayerJumpSoundController jumpSoundController = playerController?.GetComponent<PlayerJumpSoundController>();
        AudioSource audioSource = playerController?.GetComponent<AudioSource>();
        
        Debug.Log($"Player Controller: {(playerController != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Jump Sound Controller: {(jumpSoundController != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Audio Source: {(audioSource != null ? "✅ Found" : "❌ Missing")}");
        
        if (jumpSoundController != null)
        {
            bool hasJumpSounds = jumpSoundController.jumpSounds != null && jumpSoundController.jumpSounds.Length > 0;
            bool hasLandingSound = jumpSoundController.landingSound != null;
            
            Debug.Log($"Jump Sounds: {(hasJumpSounds ? $"✅ {jumpSoundController.jumpSounds.Length} sounds assigned" : "❌ No sounds assigned")}");
            Debug.Log($"Landing Sound: {(hasLandingSound ? "✅ Assigned" : "❌ Not assigned")}");
        }
        
        if (playerController != null && jumpSoundController != null && audioSource != null)
        {
            Debug.Log("🎉 System is active and ready!");
        }
        else
        {
            Debug.Log("⚠️ System needs setup. Run 'Setup Player Jump Sound System'.");
        }
    }
}