using UnityEngine;
using UnityEngine.UI;
using Invector.vCharacterController;

/// <summary>
/// Debug UI for monitoring and testing Invector integration
/// Shows current states, allows manual state changes, and provides testing buttons
/// </summary>
public class InvectorDebugUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject debugPanel;
    public Text statusText;
    public Text velocityText;
    public Text inputText;
    public Text gameStateText;
    
    [Header("Toggle")]
    public KeyCode toggleKey = KeyCode.F1;
    public bool showOnStart = false;
    
    [Header("References")]
    public InvectorGameStateController gameStateController;
    public InvectorControllerAdapter controllerAdapter;
    public vThirdPersonController invectorController;
    public CustomInvectorInput invectorInput;
    public GameManager gameManager;
    
    private bool isVisible = false;
    
    void Start()
    {
        InitializeComponents();
        SetPanelVisibility(showOnStart);
        
        if (debugPanel != null && statusText == null)
            CreateDebugUI();
    }
    
    void InitializeComponents()
    {
        if (gameStateController == null)
            gameStateController = FindObjectOfType<InvectorGameStateController>();
            
        if (controllerAdapter == null)
            controllerAdapter = FindObjectOfType<InvectorControllerAdapter>();
            
        if (invectorController == null)
            invectorController = FindObjectOfType<vThirdPersonController>();
            
        if (invectorInput == null)
            invectorInput = FindObjectOfType<CustomInvectorInput>();
            
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }
    
    void Update()
    {
        // Toggle debug panel
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
        
        // Update debug info if panel is visible
        if (isVisible)
        {
            UpdateDebugInfo();
        }
    }
    
    void CreateDebugUI()
    {
        if (debugPanel == null) return;
        
        // Create basic debug UI if not already set up
        var canvas = debugPanel.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = debugPanel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
        }
        
        // Add CanvasScaler for responsive UI
        var scaler = debugPanel.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = debugPanel.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
    }
    
    public void TogglePanel()
    {
        SetPanelVisibility(!isVisible);
    }
    
    public void SetPanelVisibility(bool visible)
    {
        isVisible = visible;
        if (debugPanel != null)
            debugPanel.SetActive(visible);
    }
    
    void UpdateDebugInfo()
    {
        UpdateStatusText();
        UpdateVelocityText();
        UpdateInputText();
        UpdateGameStateText();
    }
    
    void UpdateStatusText()
    {
        if (statusText == null) return;
        
        var status = "";
        
        // Controller status
        if (invectorController != null)
        {
            status += $"Grounded: {invectorController.isGrounded}\n";
            status += $"Jumping: {invectorController.isJumping}\n";
            status += $"Sprinting: {invectorController.isSprinting}\n";
            status += $"Strafing: {invectorController.isStrafing}\n";
        }
        
        // Input status
        if (invectorInput != null)
        {
            status += $"Input Enabled: {invectorInput.enabled}\n";
            if (invectorInput is CustomInvectorInput customInput)
            {
                status += $"Game Input Enabled: {customInput.IsInputEnabled()}\n";
            }
        }
        
        // Adapter status
        if (controllerAdapter != null)
        {
            status += $"Can Move: {controllerAdapter.canMove}\n";
        }
        
        statusText.text = status;
    }
    
    void UpdateVelocityText()
    {
        if (velocityText == null || invectorController == null) return;
        
        var rb = invectorController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            var velocity = rb.linearVelocity;
            var speed = velocity.magnitude;
            
            velocityText.text = $"Velocity: {velocity:F2}\n" +
                               $"Speed: {speed:F2}\n" +
                               $"Horizontal Speed: {invectorController.horizontalSpeed:F2}\n" +
                               $"Vertical Speed: {invectorController.verticalSpeed:F2}";
        }
    }
    
    void UpdateInputText()
    {
        if (inputText == null || invectorController == null) return;
        
        inputText.text = $"Input: {invectorController.input}\n" +
                        $"Input Smooth: {invectorController.inputSmooth}\n" +
                        $"Move Direction: {invectorController.moveDirection}\n" +
                        $"Input Magnitude: {invectorController.inputMagnitude:F2}";
    }
    
    void UpdateGameStateText()
    {
        if (gameStateText == null) return;
        
        var stateInfo = "";
        
        if (gameStateController != null)
        {
            stateInfo += $"Game State: {gameStateController.currentState}\n";
            stateInfo += $"Can Receive Input: {gameStateController.CanReceiveInput()}\n";
        }
        
        if (gameManager != null)
        {
            stateInfo += $"Game Started: {gameManager.isGameStarted}\n";
            stateInfo += $"Game Paused: {gameManager.isPaused}\n";
        }
        
        stateInfo += $"Time Scale: {Time.timeScale}\n";
        stateInfo += $"Cursor Locked: {Cursor.lockState}\n";
        stateInfo += $"Cursor Visible: {Cursor.visible}";
        
        gameStateText.text = stateInfo;
    }
    
    // Public methods for UI buttons
    
    public void TestStartGameplay()
    {
        if (gameStateController != null)
            gameStateController.StartGameplay();
        else
            Debug.LogWarning("GameStateController not found!");
    }
    
    public void TestReturnToMenu()
    {
        if (gameStateController != null)
            gameStateController.ReturnToMenu();
        else
            Debug.LogWarning("GameStateController not found!");
    }
    
    public void TestPauseGame()
    {
        if (gameStateController != null)
            gameStateController.PauseGame();
        else
            Debug.LogWarning("GameStateController not found!");
    }
    
    public void TestResumeGame()
    {
        if (gameStateController != null)
            gameStateController.ResumeGame();
        else
            Debug.LogWarning("GameStateController not found!");
    }
    
    public void TestToggleInput()
    {
        if (invectorInput != null)
            invectorInput.enabled = !invectorInput.enabled;
    }
    
    public void TestResetPosition()
    {
        if (invectorController != null)
        {
            invectorController.transform.position = Vector3.zero;
            var rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
    
    public void TestJump()
    {
        if (invectorController != null && invectorController.isGrounded)
        {
            invectorController.Jump();
        }
    }
    
    public void TestSprint()
    {
        if (invectorController != null)
        {
            invectorController.Sprint(!invectorController.isSprinting);
        }
    }
    
    public void TestStrafe()
    {
        if (invectorController != null)
        {
            invectorController.Strafe();
        }
    }
    
    // Utility methods
    
    void OnGUI()
    {
        if (!isVisible) return;
        
        // Simple fallback GUI if UI Text components are not set up
        if (statusText == null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Box("Invector Debug Info");
            
            if (GUILayout.Button("Start Gameplay"))
                TestStartGameplay();
            if (GUILayout.Button("Return to Menu"))
                TestReturnToMenu();
            if (GUILayout.Button("Pause Game"))
                TestPauseGame();
            if (GUILayout.Button("Resume Game"))
                TestResumeGame();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Toggle Input"))
                TestToggleInput();
            if (GUILayout.Button("Reset Position"))
                TestResetPosition();
            if (GUILayout.Button("Jump"))
                TestJump();
            if (GUILayout.Button("Toggle Sprint"))
                TestSprint();
            if (GUILayout.Button("Toggle Strafe"))
                TestStrafe();
            
            GUILayout.EndArea();
        }
    }
}