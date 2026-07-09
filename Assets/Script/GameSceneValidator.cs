using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Comprehensive validation for GameScene setup
/// </summary>
public class GameSceneValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool validateOnStart = false;
    public bool showDetailedLogs = true;
    
    [Header("Expected Components")]
    public bool requireGameManager = true;
    public bool requireSpawnManager = true;
    public bool requireSceneInitializer = true;
    public bool requirePlayerController = true;
    public bool requireSpawnPoint = true;
    
    void Start()
    {
        if (validateOnStart)
        {
            Invoke(nameof(ValidateGameSceneSetup), 1f); // Wait a second for initialization
        }
    }
    
    /// <summary>
    /// Comprehensive GameScene validation
    /// </summary>
    [ContextMenu("Validate GameScene Setup")]
    public void ValidateGameSceneSetup()
    {
        Debug.Log("=== GAMESCENE SETUP VALIDATION ===");
        
        bool overallValid = true;
        
        // Validate scene name
        overallValid &= ValidateSceneName();
        
        // Validate managers
        overallValid &= ValidateManagers();
        
        // Validate player setup
        overallValid &= ValidatePlayerSetup();
        
        // Validate spawn system
        overallValid &= ValidateSpawnSystem();
        
        // Validate game state
        overallValid &= ValidateGameState();
        
        // Print final result
        PrintFinalResult(overallValid);
    }
    
    bool ValidateSceneName()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (showDetailedLogs)
            Debug.Log($"Scene Name: {currentScene}");
        
        if (currentScene == "GameScene")
        {
            Debug.Log("✅ Scene Name: Correct (GameScene)");
            return true;
        }
        else
        {
            Debug.LogWarning($"⚠️ Scene Name: Expected 'GameScene', found '{currentScene}'");
            Debug.LogWarning("  - Ensure scene is saved as 'GameScene.unity'");
            Debug.LogWarning("  - Update MainMenuManager.gameSceneName if needed");
            return false;
        }
    }
    
    bool ValidateManagers()
    {
        bool managersValid = true;
        
        Debug.Log("\n--- MANAGERS VALIDATION ---");
        
        // Game Manager
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            Debug.Log("✅ GameManager: Found");
            
            // Check GameManager references
            bool hasPlayerController = gameManager.vThirdPersonController != null;
            bool hasPlayerInput = gameManager.vThirdPersonInput != null;
            bool hasSpawnManager = gameManager.playerSpawnManager != null;
            
            if (showDetailedLogs)
            {
                Debug.Log($"  - Player Controller: {(hasPlayerController ? "✅" : "❌")}");
                Debug.Log($"  - Player Input: {(hasPlayerInput ? "✅" : "❌")}");
                Debug.Log($"  - Spawn Manager: {(hasSpawnManager ? "✅" : "❌")}");
            }
            
            if (!hasPlayerController || !hasPlayerInput || !hasSpawnManager)
                managersValid = false;
        }
        else
        {
            Debug.LogError("❌ GameManager: Missing");
            managersValid = false;
        }
        
        // Player Spawn Manager
        PlayerSpawnManager spawnManager = PlayerSpawnManager.Instance;
        if (spawnManager != null)
        {
            Debug.Log("✅ PlayerSpawnManager: Found");
            
            if (showDetailedLogs)
            {
                Debug.Log($"  - Default Spawn Point: {(spawnManager.defaultSpawnPoint != null ? "✅" : "❌")}");
                Debug.Log($"  - Spawn On Load: {(spawnManager.spawnOnSceneLoad ? "✅" : "❌")}");
                Debug.Log($"  - Enable Movement: {(spawnManager.enableMovementAfterSpawn ? "✅" : "❌")}");
            }
        }
        else if (requireSpawnManager)
        {
            Debug.LogError("❌ PlayerSpawnManager: Missing");
            managersValid = false;
        }
        
        // Scene Initializer
        GameplaySceneInitializer sceneInit = FindObjectOfType<GameplaySceneInitializer>();
        if (sceneInit != null)
        {
            Debug.Log("✅ GameplaySceneInitializer: Found");
            
            if (showDetailedLogs)
            {
                Debug.Log($"  - Auto Initialize: {(sceneInit.autoInitializeOnStart ? "✅" : "❌")}");
                Debug.Log($"  - Ensure Can Move: {(sceneInit.ensurePlayerCanMove ? "✅" : "❌")}");
            }
        }
        else if (requireSceneInitializer)
        {
            Debug.LogWarning("⚠️ GameplaySceneInitializer: Missing (Recommended)");
        }
        
        // Scene Transition Manager
        SceneTransitionManager transitionManager = SceneTransitionManager.Instance;
        if (transitionManager != null)
        {
            Debug.Log("✅ SceneTransitionManager: Found");
        }
        else
        {
            Debug.LogWarning("⚠️ SceneTransitionManager: Missing (Needed for return to main menu)");
        }
        
        return managersValid;
    }
    
    bool ValidatePlayerSetup()
    {
        bool playerValid = true;
        
        Debug.Log("\n--- PLAYER SETUP VALIDATION ---");
        
        // Find player controller
        vThirdPersonController playerController = FindObjectOfType<vThirdPersonController>();
        if (playerController != null)
        {
            Debug.Log("✅ vThirdPersonController: Found");
            
            // Check player components
            vThirdPersonInput playerInput = playerController.GetComponent<vThirdPersonInput>();
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            Animator animator = playerController.GetComponent<Animator>();
            
            if (showDetailedLogs)
            {
                Debug.Log($"  - vThirdPersonInput: {(playerInput != null ? "✅" : "❌")}");
                Debug.Log($"  - Rigidbody: {(rb != null ? "✅" : "❌")}");
                Debug.Log($"  - Animator: {(animator != null ? "✅" : "❌")}");
                Debug.Log($"  - Player Tag: {(playerController.CompareTag("Player") ? "✅" : "❌")}");
            }
            
            // Check component states
            if (playerController.enabled && (playerInput?.enabled ?? false))
            {
                Debug.Log("✅ Player Components: Enabled");
            }
            else
            {
                Debug.LogWarning("⚠️ Player Components: Some disabled");
                playerValid = false;
            }
            
            // Check movement locks
            bool hasMovementLocks = playerController.lockMovement || playerController.lockRotation;
            if (!hasMovementLocks)
            {
                Debug.Log("✅ Player Movement: No locks");
            }
            else
            {
                Debug.LogWarning("⚠️ Player Movement: Has movement locks");
                if (showDetailedLogs)
                {
                    Debug.LogWarning($"  - Lock Movement: {playerController.lockMovement}");
                    Debug.LogWarning($"  - Lock Rotation: {playerController.lockRotation}");
                }
                playerValid = false;
            }
            
            // Check rigidbody
            if (rb != null)
            {
                bool properConstraints = rb.constraints == (RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ);
                bool notKinematic = !rb.isKinematic;
                
                if (properConstraints && notKinematic)
                {
                    Debug.Log("✅ Player Physics: Configured correctly");
                }
                else
                {
                    Debug.LogWarning("⚠️ Player Physics: Incorrect configuration");
                    if (showDetailedLogs)
                    {
                        Debug.LogWarning($"  - Constraints: {rb.constraints}");
                        Debug.LogWarning($"  - Is Kinematic: {rb.isKinematic}");
                    }
                    playerValid = false;
                }
            }
        }
        else if (requirePlayerController)
        {
            Debug.LogError("❌ vThirdPersonController: Missing");
            playerValid = false;
        }
        
        // Check camera
        vThirdPersonCamera playerCamera = FindObjectOfType<vThirdPersonCamera>();
        if (playerCamera != null)
        {
            Debug.Log("✅ vThirdPersonCamera: Found");
        }
        else
        {
            Debug.LogWarning("⚠️ vThirdPersonCamera: Missing (Camera won't follow player)");
        }
        
        return playerValid;
    }
    
    bool ValidateSpawnSystem()
    {
        bool spawnValid = true;
        
        Debug.Log("\n--- SPAWN SYSTEM VALIDATION ---");
        
        // Check spawn points
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            Debug.Log($"✅ Spawn Points: {spawnPoints.Length} found");
            
            foreach (GameObject sp in spawnPoints)
            {
                SpawnPoint spComponent = sp.GetComponent<SpawnPoint>();
                if (showDetailedLogs)
                {
                    Debug.Log($"  - {sp.name}: {(spComponent != null ? "✅ Has SpawnPoint component" : "❌ Missing component")}");
                }
            }
        }
        else if (requireSpawnPoint)
        {
            Debug.LogError("❌ Spawn Points: None found with 'SpawnPoint' tag");
            spawnValid = false;
        }
        
        // Check spawn manager setup
        PlayerSpawnManager spawnManager = PlayerSpawnManager.Instance;
        if (spawnManager != null && spawnManager.defaultSpawnPoint != null)
        {
            Debug.Log("✅ Spawn Manager: Has default spawn point assigned");
        }
        else if (spawnManager != null)
        {
            Debug.LogWarning("⚠️ Spawn Manager: No default spawn point assigned");
            spawnValid = false;
        }
        
        return spawnValid;
    }
    
    bool ValidateGameState()
    {
        bool stateValid = true;
        
        Debug.Log("\n--- GAME STATE VALIDATION ---");
        
        // Check time scale
        if (Time.timeScale == 1f)
        {
            Debug.Log("✅ Time Scale: Normal (1.0)");
        }
        else
        {
            Debug.LogWarning($"⚠️ Time Scale: Abnormal ({Time.timeScale})");
            stateValid = false;
        }
        
        // Check cursor state (only if in play mode)
        if (Application.isPlaying)
        {
            bool properCursor = Cursor.lockState == CursorLockMode.Locked && !Cursor.visible;
            if (properCursor)
            {
                Debug.Log("✅ Cursor State: Locked for gameplay");
            }
            else
            {
                Debug.LogWarning("⚠️ Cursor State: Not locked for gameplay");
                if (showDetailedLogs)
                {
                    Debug.LogWarning($"  - Lock State: {Cursor.lockState}");
                    Debug.LogWarning($"  - Visible: {Cursor.visible}");
                }
            }
        }
        
        return stateValid;
    }
    
    void PrintFinalResult(bool overallValid)
    {
        Debug.Log("\n=== VALIDATION SUMMARY ===");
        
        if (overallValid)
        {
            Debug.Log("🎉 GAMESCENE VALIDATION PASSED!");
            Debug.Log("✅ Scene is ready for gameplay");
            Debug.Log("✅ Player should spawn and be able to move immediately");
        }
        else
        {
            Debug.LogWarning("⚠️ GAMESCENE VALIDATION FAILED");
            Debug.LogWarning("❌ Some issues need to be addressed");
            Debug.LogWarning("📝 Check the warnings and errors above");
        }
        
        Debug.Log("\n💡 TIP: Use 'Tools > Kiro > Spawn System Setup Wizard' for automatic setup");
    }
    
    /// <summary>
    /// Quick validation for runtime use
    /// </summary>
    public bool IsGameSceneReady()
    {
        // Quick checks for essential components
        bool hasGameManager = GameManager.Instance != null;
        bool hasSpawnManager = PlayerSpawnManager.Instance != null;
        bool hasPlayer = FindObjectOfType<vThirdPersonController>() != null;
        bool hasSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint") != null;
        
        return hasGameManager && hasSpawnManager && hasPlayer && hasSpawnPoint;
    }
    
    /// <summary>
    /// Validate only player movement capability
    /// </summary>
    [ContextMenu("Validate Player Movement Only")]
    public void ValidatePlayerMovementOnly()
    {
        Debug.Log("=== PLAYER MOVEMENT VALIDATION ===");
        
        vThirdPersonController player = FindObjectOfType<vThirdPersonController>();
        if (player == null)
        {
            Debug.LogError("❌ No vThirdPersonController found");
            return;
        }
        
        vThirdPersonInput input = player.GetComponent<vThirdPersonInput>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        
        Debug.Log($"Controller Enabled: {(player.enabled ? "✅" : "❌")}");
        Debug.Log($"Input Enabled: {(input?.enabled ?? false ? "✅" : "❌")}");
        Debug.Log($"Lock Movement: {(player.lockMovement ? "❌" : "✅")}");
        Debug.Log($"Lock Rotation: {(player.lockRotation ? "❌" : "✅")}");
        Debug.Log($"Rigidbody Not Kinematic: {(rb != null && !rb.isKinematic ? "✅" : "❌")}");
        Debug.Log($"Time Scale Normal: {(Time.timeScale == 1f ? "✅" : "❌")}");
        
        bool canMove = player.enabled && (input?.enabled ?? false) && !player.lockMovement && 
                      !player.lockRotation && (rb != null && !rb.isKinematic) && Time.timeScale == 1f;
        
        if (canMove)
        {
            Debug.Log("🎮 PLAYER CAN MOVE!");
        }
        else
        {
            Debug.LogWarning("⚠️ PLAYER MOVEMENT ISSUES DETECTED");
        }
    }
}