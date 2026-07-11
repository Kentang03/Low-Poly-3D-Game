using UnityEngine;
using Invector.vCharacterController; // Add Invector namespace

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameStarted = false;
    public bool isPaused = false;
    
    [Header("Player Controller References")]
    public vThirdPersonController vThirdPersonController;  // Invector controller
    public vThirdPersonInput vThirdPersonInput;            // Invector input
    public InvectorControllerAdapter invectorAdapter;      // Optional adapter (if still needed)
    
    [Header("Legacy Support (Deprecated)")]
    public CharacterController legacyPlayerController;     // Keep for backward compatibility
    [Header("Spawn Management")]
    public PlayerSpawnManager playerSpawnManager;
    public SceneTransitionManager sceneTransitionManager;
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void InitializeGame()
    {
        // Set initial game state
        isGameStarted = false;
        isPaused = false;
        
        // Get scene transition manager reference
        if (sceneTransitionManager == null)
            sceneTransitionManager = SceneTransitionManager.Instance;
        
        // Get player spawn manager reference
        if (playerSpawnManager == null)
            playerSpawnManager = PlayerSpawnManager.Instance;
            
        // Auto-find Invector components if not assigned
        if (vThirdPersonController == null)
            vThirdPersonController = FindObjectOfType<vThirdPersonController>();
            
        if (vThirdPersonInput == null)
            vThirdPersonInput = FindObjectOfType<vThirdPersonInput>();
            
        // Optional: Auto-find Invector adapter if still being used
        if (invectorAdapter == null)
            invectorAdapter = FindObjectOfType<InvectorControllerAdapter>();
    }
    
    public void StartGame()
    {
        isGameStarted = true;
        isPaused = false;
        
        // Ensure player is properly spawned and can move
        EnsurePlayerReadyToPlay();
            
        Debug.Log("Game Started - Player ready to move!");
    }
    
    public void SetPauseState(bool paused)
    {
        isPaused = paused;
        
        // Control Invector controller and input
        if (vThirdPersonController != null && vThirdPersonInput != null)
        {
            // Disable/enable input when paused/unpaused
            vThirdPersonInput.enabled = !paused;
            
            if (paused)
            {
                // Stop character movement when paused
                vThirdPersonController.input = Vector3.zero;
                vThirdPersonController.isSprinting = false;
            }
            
            Debug.Log($"Invector Controller pause state: {paused}");
        }
        // Fallback to adapter
        else if (invectorAdapter != null)
        {
            invectorAdapter.SetPauseState(paused);
        }
            
        Debug.Log($"GameManager: Pause state set to {paused}");
    }
    
    // Deprecated - use SetPauseState instead
    public void PauseGame()
    {
        SetPauseState(true);
    }
    
    // Deprecated - use SetPauseState instead  
    public void ResumeGame()
    {
        SetPauseState(false);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        // Reset game state
        isGameStarted = false;
        isPaused = false;
        
        // Return to main menu using scene transition manager
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.ReturnToMainMenu();
        }
        else
        {
            // Fallback: restart current scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
            
        Debug.Log("Game Restarted - Returning to main menu!");
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Check which controller type is currently active
    /// </summary>
    /// <returns>String describing active controller</returns>
    public string GetActiveControllerType()
    {
        if (vThirdPersonController != null && vThirdPersonInput != null)
            return "Invector vThirdPersonController";
        else if (invectorAdapter != null)
            return "InvectorControllerAdapter";
        else if (legacyPlayerController != null)
            return "Legacy CharacterController";
        else
            return "No Controller Found";
    }
    
    /// <summary>
    /// Get reference to the active player transform
    /// </summary>
    /// <returns>Transform of the active player controller</returns>
    public Transform GetPlayerTransform()
    {
        if (vThirdPersonController != null)
            return vThirdPersonController.transform;
        else if (invectorAdapter != null)
            return invectorAdapter.transform;
        else if (legacyPlayerController != null)
            return legacyPlayerController.transform;
        else
            return null;
    }
    
    /// <summary>
    /// Debug method to validate controller setup
    /// </summary>
    [ContextMenu("Validate Controller Setup")]
    public void ValidateControllerSetup()
    {
        Debug.Log("=== CONTROLLER SETUP VALIDATION ===");
        
        bool hasInvectorController = vThirdPersonController != null;
        bool hasInvectorInput = vThirdPersonInput != null;
        bool hasAdapter = invectorAdapter != null;
        bool hasLegacy = legacyPlayerController != null;
        
        Debug.Log($"vThirdPersonController: {(hasInvectorController ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"vThirdPersonInput: {(hasInvectorInput ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"InvectorAdapter: {(hasAdapter ? "✅ Found" : "❌ Not Found")}");
        Debug.Log($"Legacy Controller: {(hasLegacy ? "⚠️ Found (Deprecated)" : "✅ Not Found")}");
        
        Debug.Log($"\nActive Controller Type: {GetActiveControllerType()}");
        
        if (hasInvectorController && hasInvectorInput)
        {
            Debug.Log("🎉 SETUP COMPLETE: Using Invector vThirdPersonController");
        }
        else if (hasAdapter)
        {
            Debug.Log("⚠️ Using InvectorControllerAdapter (consider upgrading to direct Invector integration)");
        }
        else
        {
            Debug.LogWarning("❌ No proper controller setup found!");
        }
    }
    
    /// <summary>
    /// Ensure player is properly spawned and ready to play
    /// </summary>
    void EnsurePlayerReadyToPlay()
    {
        // Ensure player spawn manager has spawned player
        if (playerSpawnManager != null)
        {
            // Player should already be spawned on scene load
            // Just ensure movement is enabled
            playerSpawnManager.EnablePlayerMovement();
        }
        
        // Enable Invector controller and input
        if (vThirdPersonController != null && vThirdPersonInput != null)
        {
            vThirdPersonController.enabled = true;
            vThirdPersonInput.enabled = true;
            
            // Ensure no movement locks
            vThirdPersonController.lockMovement = false;
            vThirdPersonController.lockRotation = false;
            
            Debug.Log("Invector Third Person Controller ready for gameplay");
        }
        // Fallback to adapter if available
        else if (invectorAdapter != null)
        {
            invectorAdapter.StartGame();
            Debug.Log("Invector Adapter started");
        }
        // Legacy support
        else if (legacyPlayerController != null)
        {
            legacyPlayerController.enabled = true;
            Debug.Log("Legacy Character Controller enabled");
        }
        else
        {
            Debug.LogWarning("No player controller found!");
        }
        
        // Ensure proper game state for movement
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Remove duplicate pause handling - now handled by PauseMenuManager
    }
}