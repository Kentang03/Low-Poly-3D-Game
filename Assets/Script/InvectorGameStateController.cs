using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Centralized controller for managing Invector character states based on game events
/// This handles all character state changes for menus, pausing, cutscenes, etc.
/// </summary>
public class InvectorGameStateController : MonoBehaviour
{
    [Header("State Management")]
    public GameState currentState = GameState.Menu;
    
    [Header("Component References")]
    public vThirdPersonController invectorController;
    public CustomInvectorInput invectorInput;
    public vThirdPersonCamera invectorCamera;
    public InvectorControllerAdapter controllerAdapter;
    
    [Header("State Settings")]
    public bool freezePositionInMenu = true;
    public bool disableCameraInMenu = false;
    public bool resetVelocityOnStateChange = true;
    
    public enum GameState
    {
        Menu,
        Gameplay, 
        Paused,
        Cutscene,
        Dialogue,
        Loading
    }
    
    void Start()
    {
        InitializeComponents();
        SetGameState(GameState.Menu);
    }
    
    void InitializeComponents()
    {
        if (invectorController == null)
            invectorController = GetComponent<vThirdPersonController>();
            
        if (invectorInput == null)
            invectorInput = GetComponent<CustomInvectorInput>();
            
        if (invectorCamera == null)
            invectorCamera = FindObjectOfType<vThirdPersonCamera>();
            
        if (controllerAdapter == null)
            controllerAdapter = GetComponent<InvectorControllerAdapter>();
    }
    
    /// <summary>
    /// Set the game state and update all relevant systems
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (currentState == newState) return;
        
        GameState previousState = currentState;
        currentState = newState;
        
        ApplyStateChanges(previousState, newState);
        
        // Notify other systems
        BroadcastStateChange(newState);
        
        Debug.Log($"Game state changed: {previousState} -> {newState}");
    }
    
    void ApplyStateChanges(GameState from, GameState to)
    {
        switch (to)
        {
            case GameState.Menu:
                SetMenuState();
                break;
                
            case GameState.Gameplay:
                SetGameplayState();
                break;
                
            case GameState.Paused:
                SetPausedState();
                break;
                
            case GameState.Cutscene:
                SetCutsceneState();
                break;
                
            case GameState.Dialogue:
                SetDialogueState();
                break;
                
            case GameState.Loading:
                SetLoadingState();
                break;
        }
    }
    
    void SetMenuState()
    {
        // Disable input
        if (invectorInput != null)
            invectorInput.enabled = false;
            
        // Reset movement
        ResetMovement();
        
        // Freeze physics if needed
        if (freezePositionInMenu)
            FreezePhysics(true);
        
        // Handle camera
        if (disableCameraInMenu && invectorCamera != null)
            invectorCamera.enabled = false;
            
        // Set cursor
        SetCursorState(true);
        
        // Notify adapter
        if (controllerAdapter != null)
            controllerAdapter.SetCanMove(false);
    }
    
    void SetGameplayState()
    {
        // Enable input
        if (invectorInput != null)
            invectorInput.enabled = true;
            
        // Unfreeze physics
        FreezePhysics(false);
        
        // Enable camera
        if (invectorCamera != null)
            invectorCamera.enabled = true;
            
        // Set cursor
        SetCursorState(false);
        
        // Notify adapter
        if (controllerAdapter != null)
            controllerAdapter.SetCanMove(true);
    }
    
    void SetPausedState()
    {
        // Disable input but keep physics enabled for smooth resume
        if (invectorInput != null)
            invectorInput.enabled = false;
            
        // Reset movement
        ResetMovement();
        
        // Keep camera enabled but stop input
        // Set cursor
        SetCursorState(true);
        
        // Notify adapter
        if (controllerAdapter != null)
            controllerAdapter.SetCanMove(false);
    }
    
    void SetCutsceneState()
    {
        // Disable input
        if (invectorInput != null)
            invectorInput.enabled = false;
            
        // Reset movement
        ResetMovement();
        
        // Keep physics enabled for cutscene animations
        // Camera might be controlled by cutscene system
        
        // Notify adapter
        if (controllerAdapter != null)
            controllerAdapter.SetCanMove(false);
    }
    
    void SetDialogueState()
    {
        // Similar to pause but might allow limited movement
        if (invectorInput != null)
            invectorInput.enabled = false;
            
        ResetMovement();
        
        // Set cursor
        SetCursorState(true);
        
        // Notify adapter
        if (controllerAdapter != null)
            controllerAdapter.SetCanMove(false);
    }
    
    void SetLoadingState()
    {
        // Disable everything
        if (invectorInput != null)
            invectorInput.enabled = false;
            
        ResetMovement();
        FreezePhysics(true);
        
        if (invectorCamera != null)
            invectorCamera.enabled = false;
            
        // Notify adapter
        if (controllerAdapter != null)
            controllerAdapter.SetCanMove(false);
    }
    
    void ResetMovement()
    {
        if (!resetVelocityOnStateChange || invectorController == null) return;
        
        // Clear input
        invectorController.input = Vector3.zero;
        invectorController.inputSmooth = Vector3.zero;
        invectorController.moveDirection = Vector3.zero;
        
        // Reset rigidbody velocity
        Rigidbody rb = invectorController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    void FreezePhysics(bool freeze)
    {
        if (invectorController == null) return;
        
        Rigidbody rb = invectorController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (freeze)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
    
    void SetCursorState(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void BroadcastStateChange(GameState newState)
    {
        // Send message to all interested components
        SendMessage("OnGameStateChanged", newState, SendMessageOptions.DontRequireReceiver);
        
        // Notify specific managers
        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            switch (newState)
            {
                case GameState.Gameplay:
                    gameManager.isGameStarted = true;
                    gameManager.isPaused = false;
                    break;
                case GameState.Paused:
                    gameManager.isPaused = true;
                    break;
                case GameState.Menu:
                    gameManager.isGameStarted = false;
                    gameManager.isPaused = false;
                    break;
            }
        }
    }
    
    // Public methods for external systems
    
    public void StartGameplay()
    {
        SetGameState(GameState.Gameplay);
    }
    
    public void ReturnToMenu()
    {
        SetGameState(GameState.Menu);
    }
    
    public void PauseGame()
    {
        SetGameState(GameState.Paused);
    }
    
    public void ResumeGame()
    {
        SetGameState(GameState.Gameplay);
    }
    
    public void StartCutscene()
    {
        SetGameState(GameState.Cutscene);
    }
    
    public void EndCutscene()
    {
        SetGameState(GameState.Gameplay);
    }
    
    public void StartDialogue()
    {
        SetGameState(GameState.Dialogue);
    }
    
    public void EndDialogue()
    {
        SetGameState(GameState.Gameplay);
    }
    
    public void StartLoading()
    {
        SetGameState(GameState.Loading);
    }
    
    public bool IsInGameplay()
    {
        return currentState == GameState.Gameplay;
    }
    
    public bool IsInMenu()
    {
        return currentState == GameState.Menu;
    }
    
    public bool IsPaused()
    {
        return currentState == GameState.Paused;
    }
    
    public bool CanReceiveInput()
    {
        return currentState == GameState.Gameplay;
    }
}