using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;           // Panel utama dengan background
    public GameObject mainMenuButtonsGroup;    // GameObject yang berisi semua button utama (Play, Settings, Credits, Exit)
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button exitButton;
    
    [Header("Panel References")]
    public GameObject settingsPanel;
    public Button backFromSettingsButton;
    
    [Header("Settings Sub-Panels (Left Side Navigation)")]
    public Button audioSettingsButton;    // Button to show audio settings
    public Button controlsButton;         // Button to show controls
    
    [Header("Settings Content Panels (Right Side Display)")]
    public GameObject audioSettingsPanel;   // Audio settings content panel
    public GameObject controlsSettingsPanel; // Controls content panel
    
    [Header("Other Panels")]
    public GameObject creditsPanel;
    public Button backFromCreditsButton;
    public GameObject controlsPanel;      // Deprecated - use controlsSettingsPanel instead
    public Button backFromControlsButton; // Deprecated
    
    [Header("Scene Management")]
    public string gameSceneName = "GameScene"; // Nama scene gameplay
    public GameObject loadingScreen;
    public Slider loadingBar;
    
    [Header("Audio")]
    public AudioSource buttonClickSound;
    public AudioSource backgroundMusic;
    
    [Header("References")]
    public AudioManager audioManager;
    
    private bool isTransitioning = false;
    
    void Start()
    {
        InitializeMainMenu();
        SetupButtonListeners();
        InitializeReferences();
        
        // Pastikan cursor visible di menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Set framerate untuk menu (tidak perlu terlalu tinggi)
        Application.targetFrameRate = 60;
    }
    
    void InitializeReferences()
    {
        // Get references jika belum di-assign
        if (audioManager == null)
            audioManager = AudioManager.Instance;
        
        // Hide loading screen at start
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
    
    void InitializeMainMenu()
    {
        // Pastikan menu panel aktif (background tetap terlihat)
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        // Pastikan button group aktif di awal
        if (mainMenuButtonsGroup != null)
            mainMenuButtonsGroup.SetActive(true);
            
        // Pastikan semua panel lain tidak aktif
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
            
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        
        // Set time scale normal
        Time.timeScale = 1f;
    }
    
    void SetupButtonListeners()
    {
        // Setup click listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
            AddHoverSound(playButton);
        }
            
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            AddHoverSound(settingsButton);
        }
            
        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OpenCredits);
            AddHoverSound(creditsButton);
        }
            
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            AddHoverSound(exitButton);
        }
            
        if (backFromSettingsButton != null)
        {
            backFromSettingsButton.onClick.AddListener(CloseSettings);
            AddHoverSound(backFromSettingsButton);
        }
            
        // Settings sub-panel navigation buttons
        if (audioSettingsButton != null)
        {
            audioSettingsButton.onClick.AddListener(ShowAudioSettings);
            AddHoverSound(audioSettingsButton);
        }
            
        if (controlsButton != null)
        {
            controlsButton.onClick.AddListener(ShowControlsSettings);
            AddHoverSound(controlsButton);
        }
            
        if (backFromCreditsButton != null)
        {
            backFromCreditsButton.onClick.AddListener(CloseCredits);
            AddHoverSound(backFromCreditsButton);
        }
            
        // Deprecated controls panel support (for backward compatibility)
        if (backFromControlsButton != null)
        {
            backFromControlsButton.onClick.AddListener(CloseControls);
            AddHoverSound(backFromControlsButton);
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
    
    public void StartGame()
    {
        if (isTransitioning) return;
        
        isTransitioning = true;
        
        // Play button click sound
        PlayButtonSound();
        
        // Start loading game scene
        StartCoroutine(LoadGameScene());
    }
    
    IEnumerator LoadGameScene()
    {
        // Show loading screen
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
            
        // Hide main menu buttons (background tetap terlihat)
        if (mainMenuButtonsGroup != null)
            mainMenuButtonsGroup.SetActive(false);
        
        // Wait a frame
        yield return null;
        
        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        
        // Don't allow scene activation until loading is complete
        asyncLoad.allowSceneActivation = false;
        
        // Update loading bar while loading
        while (!asyncLoad.isDone)
        {
            // Loading progress goes from 0 to 0.9
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (loadingBar != null)
                loadingBar.value = progress;
            
            // Scene is ready to activate
            if (asyncLoad.progress >= 0.9f)
            {
                // Optional: wait a moment to show 100% loading
                if (loadingBar != null)
                    loadingBar.value = 1f;
                    
                yield return new WaitForSeconds(0.5f);
                
                // Activate the scene
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
    public void OpenSettings()
    {
        PlayButtonSound();
        
        // Sembunyikan button group (background tetap terlihat)
        if (mainMenuButtonsGroup != null)
            mainMenuButtonsGroup.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
            
        // Show audio settings by default when opening settings
        ShowAudioSettings();
    }
    
    public void CloseSettings()
    {
        PlayButtonSound();
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        // Hide all settings content panels
        HideAllSettingsContentPanels();
            
        // Tampilkan kembali button group (background sudah terlihat)
        if (mainMenuButtonsGroup != null)
            mainMenuButtonsGroup.SetActive(true);
    }
    
    /// <summary>
    /// Show Audio Settings in the right panel
    /// </summary>
    public void ShowAudioSettings()
    {
        PlayButtonSound();
        
        // Hide all content panels first
        HideAllSettingsContentPanels();
        
        // Show audio settings panel
        if (audioSettingsPanel != null)
        {
            audioSettingsPanel.SetActive(true);
        }
        
        // Visual feedback - highlight audio button (optional)
        HighlightSelectedButton(audioSettingsButton);
        
        Debug.Log("Audio Settings panel displayed");
    }
    
    /// <summary>
    /// Show Controls Settings in the right panel
    /// </summary>
    public void ShowControlsSettings()
    {
        PlayButtonSound();
        
        // Hide all content panels first
        HideAllSettingsContentPanels();
        
        // Show controls settings panel
        if (controlsSettingsPanel != null)
        {
            controlsSettingsPanel.SetActive(true);
        }
        
        // Visual feedback - highlight controls button (optional)
        HighlightSelectedButton(controlsButton);
        
        Debug.Log("Controls Settings panel displayed");
    }
    
    /// <summary>
    /// Hide all settings content panels
    /// </summary>
    private void HideAllSettingsContentPanels()
    {
        if (audioSettingsPanel != null)
            audioSettingsPanel.SetActive(false);
            
        if (controlsSettingsPanel != null)
            controlsSettingsPanel.SetActive(false);
    }
    
    /// <summary>
    /// Highlight the selected navigation button (optional visual feedback)
    /// </summary>
    /// <param name="selectedButton">The button to highlight</param>
    private void HighlightSelectedButton(Button selectedButton)
    {
        // Reset all buttons to normal state
        ResetButtonHighlights();
        
        // Highlight the selected button
        if (selectedButton != null)
        {
            var colors = selectedButton.colors;
            colors.normalColor = colors.selectedColor; // Use selected color as normal
            selectedButton.colors = colors;
        }
    }
    
    /// <summary>
    /// Reset all navigation buttons to normal state
    /// </summary>
    private void ResetButtonHighlights()
    {
        if (audioSettingsButton != null)
        {
            var colors = audioSettingsButton.colors;
            colors.normalColor = Color.white; // Default normal color
            audioSettingsButton.colors = colors;
        }
        
        if (controlsButton != null)
        {
            var colors = controlsButton.colors;
            colors.normalColor = Color.white; // Default normal color
            controlsButton.colors = colors;
        }
    }
    
    public void OpenCredits()
    {
        PlayButtonSound();
        
        // Sembunyikan button group (background tetap terlihat)
        if (mainMenuButtonsGroup != null)
            mainMenuButtonsGroup.SetActive(false);
            
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }
    
    public void CloseCredits()
    {
        PlayButtonSound();
        
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
            
        // Tampilkan kembali button group (background sudah terlihat)
        if (mainMenuButtonsGroup != null)
            mainMenuButtonsGroup.SetActive(true);
    }
    
    // Deprecated methods for backward compatibility
    public void OpenControls()
    {
        ShowControlsSettings(); // Redirect to new method
    }
    
    public void CloseControls()
    {
        // Legacy behavior - close entire settings
        CloseSettings();
    }
    
    public void ExitGame()
    {
        PlayButtonSound();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void PlayButtonSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }
        else if (buttonClickSound != null)
        {
            buttonClickSound.Play();
        }
    }
    
    void PlayHoverSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonHover();
        }
    }
}