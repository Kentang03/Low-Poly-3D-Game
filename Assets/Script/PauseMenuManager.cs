using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Invector.vCharacterController; // Add Invector namespace

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;
    
    [Header("Settings Panel")]
    public GameObject pauseSettingsPanel;
    public Button backFromSettingsButton;
    
    [Header("Player Controller References")]
    public vThirdPersonController vThirdPersonController;  // Invector controller
    public vThirdPersonInput vThirdPersonInput;            // Invector input
    public InvectorControllerAdapter invectorAdapter;      // Optional adapter (if still needed)
    [Header("Manager References")]
    public GameManager gameManager;
    public AudioManager audioManager;
    public SceneTransitionManager sceneTransitionManager;
    
    private bool isPaused = false;
    
    void Start()
    {
        InitializePauseMenu();
        SetupButtonListeners();
    }
    
    void Update()
    {
        // Handle pause input (only when game is started)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameManager == null || !gameManager.isGameStarted)
                return;
                
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    void InitializePauseMenu()
    {
        // Get manager references
        if (gameManager == null)
            gameManager = GameManager.Instance;
            
        if (audioManager == null)
            audioManager = AudioManager.Instance;
            
        if (sceneTransitionManager == null)
            sceneTransitionManager = SceneTransitionManager.Instance;
        
        // Get Invector controller references
        if (vThirdPersonController == null)
            vThirdPersonController = FindObjectOfType<vThirdPersonController>();
            
        if (vThirdPersonInput == null)
            vThirdPersonInput = FindObjectOfType<vThirdPersonInput>();
            
        // Optional adapter for backward compatibility
        if (invectorAdapter == null)
            invectorAdapter = FindObjectOfType<InvectorControllerAdapter>();
        
        // Pastikan pause menu tidak aktif di awal
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
    }
    
    void SetupButtonListeners()
    {
        // Setup click listeners and hover sounds
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            AddHoverSound(resumeButton);
        }
            
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenPauseSettings);
            AddHoverSound(settingsButton);
        }
            
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            AddHoverSound(mainMenuButton);
        }
            
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            AddHoverSound(quitButton);
        }
            
        if (backFromSettingsButton != null)
        {
            backFromSettingsButton.onClick.AddListener(ClosePauseSettings);
            AddHoverSound(backFromSettingsButton);
        }
    }
    
    /// <summary>
    /// Add hover sound to a button using EventTrigger
    /// </summary>
    /// <param name="button">Button to add hover sound to</param>
    private void AddHoverSound(Button button)
    {
        if (button == null) return;
        
        // Get or add EventTrigger component
        var eventTrigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Create hover entry
        var hoverEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { PlayHoverSound(); });
        
        eventTrigger.triggers.Add(hoverEntry);
    }
    
    public void PauseGame()
    {
        if (isPaused || gameManager == null || !gameManager.isGameStarted) return;
        
        isPaused = true;
                
        // Show pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
            
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Freeze character movement using Invector controller
        FreezePlayerMovement(true);
        
        // Notify game manager
        gameManager.SetPauseState(true);
            
        // Play pause sound
        if (audioManager != null)
            audioManager.PlayButtonClick();
        
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (!isPaused || gameManager == null) return;
        
        isPaused = false;
        
        // Resume game time
        Time.timeScale = 1f;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
            
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Unfreeze character movement using Invector controller
        FreezePlayerMovement(false);
        
        // Notify game manager
        gameManager.SetPauseState(false);
            
        // Play resume sound
        if (audioManager != null)
            audioManager.PlayButtonClick();
        
        Debug.Log("Game Resumed");
    }
    
    public void OpenPauseSettings()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();
            
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(true);
    }
    
    public void ClosePauseSettings()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
            
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }
    
    public void GoToMainMenu()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();
        
        // Resume time before switching scene
        Time.timeScale = 1f;
        
        isPaused = false;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
        
        // Show cursor untuk main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Use scene transition manager to return to main menu
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.ReturnToMainMenu();
        }
        else
        {
            // Fallback: restart current scene (for backward compatibility)
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        Debug.Log("Returning to main menu...");
    }
    
    public void QuitGame()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Public method untuk mengecek status pause
    public bool IsPaused()
    {
        return isPaused;
    }
    
    /// <summary>
    /// Freeze or unfreeze player movement using Invector controller
    /// </summary>
    /// <param name="freeze">True to freeze movement, false to unfreeze</param>
    private void FreezePlayerMovement(bool freeze)
    {
        // Primary method: Use Invector vThirdPersonController and vThirdPersonInput
        if (vThirdPersonController != null && vThirdPersonInput != null)
        {
            // Disable/enable input component
            vThirdPersonInput.enabled = !freeze;
            
            if (freeze)
            {
                // Stop current movement
                vThirdPersonController.input = Vector3.zero;
                vThirdPersonController.isSprinting = false;
            }
            
            Debug.Log($"Invector Controller movement {(freeze ? "frozen" : "unfrozen")}");
            return;
        }
        
        // Fallback method: Use InvectorControllerAdapter if available
        if (invectorAdapter != null)
        {
            if (freeze)
                invectorAdapter.SetCanMove(false);
            else
                invectorAdapter.SetCanMove(true);
                
            Debug.Log($"Invector Adapter movement {(freeze ? "frozen" : "unfrozen")}");
            return;
        }
        
        // Legacy fallback: Direct component control (not recommended)
        var legacyController = FindObjectOfType<CharacterController>();
        if (legacyController != null)
        {
            legacyController.enabled = !freeze;
            Debug.Log($"Legacy Controller movement {(freeze ? "frozen" : "unfrozen")}");
            return;
        }
        
        Debug.LogWarning("No suitable player controller found for movement control!");
    }
    
    /// <summary>
    /// Play hover sound when button is hovered
    /// </summary>
    void PlayHoverSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonHover();
        }
    }
}