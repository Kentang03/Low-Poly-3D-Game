using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    
    [Header("References")]
    public MainMenuManager mainMenuManager;
    public GameManager gameManager;
    public AudioManager audioManager;
    
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
        // Get references jika belum di-assign
        if (gameManager == null)
            gameManager = GameManager.Instance;
            
        if (audioManager == null)
            audioManager = AudioManager.Instance;
            
        if (mainMenuManager == null)
            mainMenuManager = FindObjectOfType<MainMenuManager>();
        
        // Pastikan pause menu tidak aktif di awal
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
    }
    
    void SetupButtonListeners()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenPauseSettings);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(ClosePauseSettings);
    }
    
    public void PauseGame()
    {
        if (isPaused || gameManager == null || !gameManager.isGameStarted) return;
        
        isPaused = true;
        
        // Pause game time
        Time.timeScale = 0f;
        
        // Show pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
            
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Freeze character movement
        var playerController = FindObjectOfType<CharacterController>();
        if (playerController != null)
            playerController.SetCanMove(false);
        
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
        
        // Unfreeze character movement
        var playerController = FindObjectOfType<CharacterController>();
        if (playerController != null)
            playerController.SetCanMove(true);
        
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
        
        // Resume time before restarting scene
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
        
        // Restart scene from beginning
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
        Debug.Log("Restarting scene and returning to main menu...");
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
}