using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Custom Invector Input that integrates with game state management
/// This class extends vThirdPersonInput to respect game pausing and menu states
/// </summary>
public class CustomInvectorInput : vThirdPersonInput
{
    [Header("Game State Integration")]
    public GameManager gameManager;
    public InvectorControllerAdapter controllerAdapter;
    
    private bool gameStateEnabled = true;
    
    protected override void Start()
    {
        // Initialize game references first
        if (gameManager == null)
            gameManager = GameManager.Instance;
            
        if (controllerAdapter == null)
            controllerAdapter = GetComponent<InvectorControllerAdapter>();
            
        // Call base initialization
        base.Start();
    }
    
    protected override void Update()
    {
        // Check game state before processing input
        UpdateGameState();
        
        // Only process input if game state allows it
        if (!gameStateEnabled)
            return;
            
        // Call base update for normal input processing
        base.Update();
    }
    
    protected override void FixedUpdate()
    {
        // Only process physics if game state allows it
        if (!gameStateEnabled)
            return;
            
        // Call base fixed update
        base.FixedUpdate();
    }
    
    void UpdateGameState()
    {
        // Determine if input should be enabled based on game state
        bool shouldBeEnabled = true;
        
        if (gameManager != null)
        {
            // Disable input if game hasn't started or is paused
            shouldBeEnabled = gameManager.isGameStarted && !gameManager.isPaused;
        }
        
        if (controllerAdapter != null)
        {
            // Also check adapter's can move state
            shouldBeEnabled = shouldBeEnabled && controllerAdapter.canMove;
        }
        
        gameStateEnabled = shouldBeEnabled;
    }
    
    protected override void InputHandle()
    {
        // Only handle input if game state allows it
        if (!gameStateEnabled)
        {
            // Clear all input when disabled
            ClearInput();
            return;
        }
        
        base.InputHandle();
    }
    
    /// <summary>
    /// Clear all input values when movement is disabled
    /// </summary>
    void ClearInput()
    {
        if (cc != null)
        {
            cc.input = Vector3.zero;
            cc.inputSmooth = Vector3.zero;
            cc.moveDirection = Vector3.zero;
        }
    }
    
    public override void MoveInput()
    {
        // Only process move input if enabled
        if (!gameStateEnabled)
        {
            if (cc != null)
            {
                cc.input.x = 0;
                cc.input.z = 0;
            }
            return;
        }
        
        base.MoveInput();
    }
    
    protected override void CameraInput()
    {
        // Only process camera input if enabled
        if (!gameStateEnabled)
            return;
            
        base.CameraInput();
    }
    
    protected override void SprintInput()
    {
        // Only process sprint input if enabled
        if (!gameStateEnabled)
            return;
            
        base.SprintInput();
    }
    
    protected override void StrafeInput()
    {
        // Only process strafe input if enabled
        if (!gameStateEnabled)
            return;
            
        base.StrafeInput();
    }
    
    protected override void JumpInput()
    {
        // Only process jump input if enabled
        if (!gameStateEnabled)
            return;
            
        base.JumpInput();
    }
    
    /// <summary>
    /// Force enable/disable input (can be called by other systems)
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        gameStateEnabled = enabled;
        
        if (!enabled)
            ClearInput();
    }
    
    /// <summary>
    /// Check if input is currently enabled
    /// </summary>
    public bool IsInputEnabled()
    {
        return gameStateEnabled;
    }
}