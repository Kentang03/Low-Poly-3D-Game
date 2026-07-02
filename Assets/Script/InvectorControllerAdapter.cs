using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Adapter class to integrate Invector Third Person Controller with existing game systems
/// This class provides compatibility between the Invector controller and MainMenuManager/GameManager/PauseMenuManager
/// </summary>
public class InvectorControllerAdapter : MonoBehaviour
{
    [Header("Invector Components")]
    public vThirdPersonController invectorController;
    public vThirdPersonInput invectorInput;
    public vThirdPersonCamera invectorCamera;
    
    [Header("Game State")]
    public bool canMove = true;
    private bool isGameStarted = false;
    private bool isPaused = false;
    
    [Header("References")]
    public GameManager gameManager;
    public MainMenuManager mainMenuManager;
    public PauseMenuManager pauseMenuManager;
    
    // Store original enabled states
    private bool originalInputEnabled = true;
    private bool originalControllerEnabled = true;
    
    void Start()
    {
        InitializeComponents();
        InitializeGameState();
    }
    
    void InitializeComponents()
    {
        // Auto-find components if not assigned
        if (invectorController == null)
            invectorController = GetComponent<vThirdPersonController>();
            
        if (invectorInput == null)
            invectorInput = GetComponent<vThirdPersonInput>();
            
        if (invectorCamera == null)
            invectorCamera = FindObjectOfType<vThirdPersonCamera>();
        
        // Store original states
        if (invectorInput != null)
            originalInputEnabled = invectorInput.enabled;
        if (invectorController != null)
            originalControllerEnabled = invectorController.enabled;
            
        // Get game manager references
        if (gameManager == null)
            gameManager = GameManager.Instance;
            
        if (mainMenuManager == null)
            mainMenuManager = FindObjectOfType<MainMenuManager>();
            
        if (pauseMenuManager == null)
            pauseMenuManager = FindObjectOfType<PauseMenuManager>();
    }
    
    void InitializeGameState()
    {
        // Start with movement disabled (menu state)
        SetCanMove(false);
        FreezeCharacter();
    }
    
    void Update()
    {
        // Update movement state based on game state
        if (gameManager != null)
        {
            bool shouldMove = gameManager.isGameStarted && !gameManager.isPaused;
            if (canMove != shouldMove)
            {
                SetCanMove(shouldMove);
            }
        }
        
        // Handle cursor state
        UpdateCursorState();
    }
    
    void UpdateCursorState()
    {
        if (!canMove || isPaused)
        {
            // Menu or paused state
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (canMove && isGameStarted)
        {
            // Gameplay state
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    // Public methods for compatibility with existing systems
    
    /// <summary>
    /// Set whether the character can move (called by menu systems)
    /// </summary>
    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
        
        // Handle Invector Input
        if (invectorInput != null)
        {
            invectorInput.enabled = canMove;
        }
        
        // Handle controller input stopping
        if (!canMove && invectorController != null)
        {
            // Stop all movement
            invectorController.input = Vector3.zero;
            invectorController.inputSmooth = Vector3.zero;
            invectorController.moveDirection = Vector3.zero;
            
            // Stop rigidbody movement
            var rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
    
    /// <summary>
    /// Freeze character completely (menu state)
    /// </summary>
    public void FreezeCharacter()
    {
        SetCanMove(false);
        isPaused = false;
        isGameStarted = false;
        
        // Disable Invector components
        if (invectorInput != null)
        {
            invectorInput.enabled = false;
        }
        
        if (invectorController != null)
        {
            var rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        // Unlock cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Character frozen - Invector input disabled");
    }
    
    /// <summary>
    /// Unfreeze character for gameplay
    /// </summary>
    public void UnfreezeCharacter()
    {
        isPaused = false;
        isGameStarted = true;
        
        // Enable Invector components first
        if (invectorController != null)
        {
            var rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            
            invectorController.enabled = true;
        }
        
        if (invectorInput != null)
        {
            invectorInput.enabled = true;
        }
        
        // Then set can move
        SetCanMove(true);
        
        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Character unfrozen - Invector input enabled");
    }
    
    /// <summary>
    /// Set cursor lock state manually
    /// </summary>
    public void SetCursorLocked(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    /// <summary>
    /// Set pause state (called by PauseMenuManager)
    /// </summary>
    public void SetPauseState(bool paused)
    {
        isPaused = paused;
        SetCanMove(!paused && isGameStarted);
    }
    
    /// <summary>
    /// Start game (called by MainMenuManager)
    /// </summary>
    public void StartGame()
    {
        isGameStarted = true;
        UnfreezeCharacter();
    }
    
    /// <summary>
    /// Return to menu (called by MainMenuManager)
    /// </summary>
    public void ReturnToMenu()
    {
        isGameStarted = false;
        FreezeCharacter();
    }
    
    // Compatibility methods for existing code that expects CharacterController interface
    
    /// <summary>
    /// Force enable all Invector components (untuk respawn)
    /// </summary>
    public void ForceEnableInvectorComponents()
    {
        Debug.Log("Force enabling Invector components...");
        
        // Enable controller first
        if (invectorController != null)
        {
            invectorController.enabled = true;
            Debug.Log("✓ Enabled vThirdPersonController");
        }
        
        // Then enable input
        if (invectorInput != null)
        {
            invectorInput.enabled = true;
            Debug.Log("✓ Enabled vThirdPersonInput");
        }
        
        // Set movement state
        SetCanMove(true);
        
        // Ensure rigidbody is not frozen
        if (invectorController != null)
        {
            var rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                Debug.Log("✓ Unfroze Rigidbody");
            }
        }
        
        Debug.Log("Invector components force enabled complete");
    }
    
    /// <summary>
    /// Validate current player position for debugging respawn issues
    /// </summary>
    public void ValidatePlayerPosition()
    {
        Debug.Log("=== PLAYER POSITION VALIDATION ===");
        Debug.Log($"Transform Position: {transform.position}");
        
        if (invectorController != null)
        {
            Debug.Log($"Controller Enabled: {invectorController.enabled}");
            Debug.Log($"Controller Input: {invectorController.input}");
            Debug.Log($"Controller Input Smooth: {invectorController.inputSmooth}");
            Debug.Log($"Controller Move Direction: {invectorController.moveDirection}");
            Debug.Log($"Controller Vertical Velocity: {invectorController.verticalVelocity}");
            
            // Check animator
            Animator animator = invectorController.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log($"Animator Root Position: {animator.rootPosition}");
                Debug.Log($"Animator Root Rotation: {animator.rootRotation}");
            }
        }
        
        if (invectorInput != null)
        {
            Debug.Log($"Input Enabled: {invectorInput.enabled}");
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log($"Rigidbody Velocity: {rb.linearVelocity}");
            Debug.Log($"Rigidbody Angular Velocity: {rb.angularVelocity}");
            Debug.Log($"Rigidbody Is Kinematic: {rb.isKinematic}");
            Debug.Log($"Rigidbody Constraints: {rb.constraints}");
        }
        
        Debug.Log($"Can Move: {canMove}");
        Debug.Log($"Game Started: {isGameStarted}");
        Debug.Log("=================================");
    }
    
    /// <summary>
    /// Teleport Invector character to position without conflicts
    /// </summary>
    /// <param name="targetPosition">Target position</param>
    /// <param name="targetRotation">Target rotation</param>
    public void TeleportCharacter(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (invectorController == null) 
        {
            Debug.LogError("Cannot teleport: Invector controller not found!");
            return;
        }
        
        StartCoroutine(TeleportCharacterCoroutine(targetPosition, targetRotation));
    }
    
    /// <summary>
    /// Safe teleportation coroutine that prevents position conflicts
    /// </summary>
    System.Collections.IEnumerator TeleportCharacterCoroutine(Vector3 targetPosition, Quaternion targetRotation)
    {
        Debug.Log($"🎯 Starting safe teleport to: {targetPosition}");
        
        // Store original states
        bool wasControllerEnabled = invectorController.enabled;
        bool wasInputEnabled = invectorInput?.enabled ?? false;
        
        // 1. Disable all movement components
        if (invectorInput != null)
            invectorInput.enabled = false;
        invectorController.enabled = false;
        
        // 2. Reset physics
        Rigidbody rb = invectorController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        // 3. Set new position and rotation
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        
        // 4. Reset Invector internal states
        invectorController.input = Vector3.zero;
        invectorController.inputSmooth = Vector3.zero;
        invectorController.moveDirection = Vector3.zero;
        invectorController.verticalVelocity = 0f;
        
        // 5. Force animator update
        Animator animator = invectorController.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        
        // 6. Wait for physics to settle
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        
        // 7. Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // 8. Re-enable controller
        invectorController.enabled = wasControllerEnabled;
        
        // 9. Re-enable input
        if (invectorInput != null)
            invectorInput.enabled = wasInputEnabled;
        
        // 10. Restore movement capability
        SetCanMove(true);
        
        Debug.Log("✅ Safe teleport completed successfully!");
    }
    
    /// <summary>
    /// Get status of Invector components (untuk debugging)
    /// </summary>
    public void PrintInvectorStatus()
    {
        Debug.Log("=== Invector Components Status ===");
        
        if (invectorController != null)
            Debug.Log($"vThirdPersonController: {(invectorController.enabled ? "ENABLED" : "DISABLED")}");
        else
            Debug.Log("vThirdPersonController: NOT FOUND");
            
        if (invectorInput != null)
            Debug.Log($"vThirdPersonInput: {(invectorInput.enabled ? "ENABLED" : "DISABLED")}");
        else
            Debug.Log("vThirdPersonInput: NOT FOUND");
        
        Debug.Log($"Can Move: {canMove}");
        Debug.Log($"Game Started: {isGameStarted}");
        Debug.Log($"Is Paused: {isPaused}");
        
        Debug.Log("===================================");
    }
    
    /// <summary>
    /// Get the Invector controller for direct access if needed
    /// </summary>
    public vThirdPersonController GetInvectorController()
    {
        return invectorController;
    }
    
    /// <summary>
    /// Get the Invector input for direct access if needed
    /// </summary>
    public vThirdPersonInput GetInvectorInput()
    {
        return invectorInput;
    }
    
    /// <summary>
    /// Get the Invector camera for direct access if needed
    /// </summary>
    public vThirdPersonCamera GetInvectorCamera()
    {
        return invectorCamera;
    }
    
    // Camera switching methods (if needed for additional features)
    
    /// <summary>
    /// Switch camera mode (can be extended for multiple camera types)
    /// </summary>
    public void SwitchCameraMode()
    {
        // Implementation can be added here if multiple camera modes are needed
        // For now, Invector camera handles its own input
    }
    
    // Debug methods
    
    void OnDrawGizmosSelected()
    {
        if (invectorController != null)
        {
            Gizmos.color = canMove ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.5f);
            
            if (!canMove)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position + Vector3.up * 2, Vector3.one * 0.3f);
            }
        }
    }
}