using UnityEngine;
using UnityEngine.UI;

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
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else if (gameManager != null && gameManager.isGameStarted && !gameManager.isPaused)
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
        if (isPaused) return;
        
        isPaused = true;
        
        // Pause game time
        Time.timeScale = 0f;
        
        // Show pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
            
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Notify game manager
        if (gameManager != null)
            gameManager.PauseGame();
            
        // Play pause sound
        if (audioManager != null)
            audioManager.PlayButtonClick();
        
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (!isPaused) return;
        
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
        
        // Notify game manager
        if (gameManager != null)
            gameManager.ResumeGame();
            
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
        
        // Resume time before going to main menu
        Time.timeScale = 1f;
        
        isPaused = false;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);
        
        // Return to main menu
        if (mainMenuManager != null)
        {
            mainMenuManager.ReturnToMainMenu();
        }
        else if (gameManager != null)
        {
            gameManager.RestartGame();
        }
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