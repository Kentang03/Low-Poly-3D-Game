using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Initializes gameplay scene with proper player spawn and system setup
/// </summary>
public class GameplaySceneInitializer : MonoBehaviour
{
    [Header("Initialization Settings")]
    public bool autoInitializeOnStart = true;
    public float initializationDelay = 0.2f;
    
    [Header("Required Managers")]
    public GameManager gameManager;
    public PlayerSpawnManager playerSpawnManager;
    
    [Header("Player Setup")]
    public bool ensurePlayerCanMove = true;
    public bool setCursorForGameplay = true;
    public bool setTimeScale = true;
    
    void Start()
    {
        if (autoInitializeOnStart)
        {
            Invoke(nameof(InitializeGameplayScene), initializationDelay);
        }
    }
    
    /// <summary>
    /// Initialize the gameplay scene with proper setup
    /// </summary>
    public void InitializeGameplayScene()
    {
        Debug.Log("=== INITIALIZING GAMEPLAY SCENE ===");
        
        // Find required managers
        FindRequiredManagers();
        
        // Initialize spawn system
        InitializeSpawnSystem();
        
        // Setup player for immediate gameplay
        SetupPlayerForGameplay();
        
        // Configure game state
        ConfigureGameState();
        
        Debug.Log("🎮 GAMEPLAY SCENE READY - Player can move!");
    }
    
    void FindRequiredManagers()
    {
        // Find GameManager
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
            if (gameManager != null)
                Debug.Log("✅ GameManager found");
            else
                Debug.LogWarning("⚠️ GameManager not found - some features may not work");
        }
        
        // Find PlayerSpawnManager
        if (playerSpawnManager == null)
        {
            playerSpawnManager = PlayerSpawnManager.Instance;
            if (playerSpawnManager != null)
                Debug.Log("✅ PlayerSpawnManager found");
            else
                Debug.LogWarning("⚠️ PlayerSpawnManager not found - player may not spawn correctly");
        }
    }
    
    void InitializeSpawnSystem()
    {
        if (playerSpawnManager == null) return;
        
        // Ensure player is spawned at spawn point
        playerSpawnManager.SpawnPlayer();
        
        // Enable movement immediately after spawn
        if (ensurePlayerCanMove)
        {
            playerSpawnManager.EnablePlayerMovement();
        }
        
        Debug.Log("✅ Player spawn system initialized");
    }
    
    void SetupPlayerForGameplay()
    {
        // Find player controller components
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        vThirdPersonInput playerInput = FindObjectOfType<vThirdPersonInput>();
        
        if (playerController != null && playerInput != null)
        {
            // Ensure components are enabled
            playerController.enabled = true;
            playerInput.enabled = true;
            
            // Remove any movement restrictions
            playerController.lockMovement = false;
            playerController.lockRotation = false;
            // Note: stopMove is handled internally by Invector
            
            // Clear any existing input
            playerController.input = Vector3.zero;
            playerController.isSprinting = false;
            playerController.isJumping = false;
            
            // Ensure rigidbody is not frozen
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = false;
            }
            
            Debug.Log("✅ Player controller setup for immediate gameplay");
        }
        else
        {
            Debug.LogWarning("⚠️ Player controller components not found!");
        }
    }
    
    void ConfigureGameState()
    {
        // Set proper time scale
        if (setTimeScale)
        {
            Time.timeScale = 1f;
        }
        
        // Configure cursor for gameplay
        if (setCursorForGameplay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Update GameManager state if available
        if (gameManager != null)
        {
            gameManager.isGameStarted = true;
            gameManager.isPaused = false;
        }
        
        Debug.Log("✅ Game state configured for gameplay");
    }
    
    /// <summary>
    /// Manual initialization method
    /// </summary>
    [ContextMenu("Initialize Gameplay Scene")]
    public void ManualInitialize()
    {
        InitializeGameplayScene();
    }
    
    /// <summary>
    /// Validate that all systems are ready for gameplay
    /// </summary>
    [ContextMenu("Validate Gameplay Setup")]
    public void ValidateGameplaySetup()
    {
        Debug.Log("=== GAMEPLAY SETUP VALIDATION ===");
        
        // Check managers
        bool hasGameManager = GameManager.Instance != null;
        bool hasSpawnManager = PlayerSpawnManager.Instance != null;
        
        Debug.Log($"GameManager: {(hasGameManager ? "✅" : "❌")}");
        Debug.Log($"PlayerSpawnManager: {(hasSpawnManager ? "✅" : "❌")}");
        
        // Check player controller
        vThirdPersonController controller = FindObjectOfType<vThirdPersonController>();
        vThirdPersonInput input = FindObjectOfType<vThirdPersonInput>();
        
        bool hasController = controller != null;
        bool hasInput = input != null;
        bool controllerEnabled = hasController && controller.enabled;
        bool inputEnabled = hasInput && input.enabled;
        bool canMove = hasController && !controller.lockMovement && !controller.stopMove;
        
        Debug.Log($"vThirdPersonController: {(hasController ? "✅" : "❌")}");
        Debug.Log($"vThirdPersonInput: {(hasInput ? "✅" : "❌")}");
        Debug.Log($"Controller Enabled: {(controllerEnabled ? "✅" : "❌")}");
        Debug.Log($"Input Enabled: {(inputEnabled ? "✅" : "❌")}");
        Debug.Log($"Can Move: {(canMove ? "✅" : "❌")}");
        
        // Check spawn point
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        bool hasSpawnPoint = spawnPoint != null;
        Debug.Log($"Spawn Point: {(hasSpawnPoint ? "✅ " + spawnPoint.name : "❌ Not Found")}");
        
        // Check game state
        bool properTimeScale = Time.timeScale == 1f;
        bool properCursor = Cursor.lockState == CursorLockMode.Locked && !Cursor.visible;
        
        Debug.Log($"Time Scale (1.0): {(properTimeScale ? "✅" : "❌ " + Time.timeScale)}");
        Debug.Log($"Cursor Locked: {(properCursor ? "✅" : "❌")}");
        
        // Overall status
        bool readyForGameplay = hasController && hasInput && controllerEnabled && 
                               inputEnabled && canMove && properTimeScale;
        
        if (readyForGameplay)
        {
            Debug.Log("\n🎉 GAMEPLAY READY! Player can move and play.");
        }
        else
        {
            Debug.Log("\n⚠️ GAMEPLAY NOT READY. Check failed items above.");
        }
    }
}