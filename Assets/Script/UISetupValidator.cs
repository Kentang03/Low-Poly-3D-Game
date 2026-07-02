using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to validate UI setup for the new Settings Panel with 2-column layout
/// </summary>
public class UISetupValidator : MonoBehaviour
{
    [Header("UI References to Validate")]
    public MainMenuManager mainMenuManager;
    
    [Header("Validation Results")]
    public bool mainMenuSetupValid = false;
    public bool settingsNavigationValid = false;
    public bool settingsContentValid = false;
    public bool audioSettingsValid = false;
    public bool controlsSettingsValid = false;
    
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string instructions = @"
SETTINGS PANEL VALIDATION (2-Column Layout):

1. LEFT NAVIGATION PANEL:
   - Audio Settings Button
   - Controls Button  
   - Back Button

2. RIGHT CONTENT AREA:
   - Audio Settings Panel (with sliders)
   - Controls Settings Panel (with controls)

3. BEHAVIOR:
   - Clicking Audio/Controls shows different panels on right
   - Only one content panel visible at a time
   - Back button returns to main menu

Run 'Validate Settings Setup' to check configuration.
";
    
    [ContextMenu("Validate Settings Setup")]
    public void ValidateUISetup()
    {
        Debug.Log("=== SETTINGS PANEL VALIDATION (2-Column Layout) ===");
        
        if (mainMenuManager == null)
        {
            mainMenuManager = FindObjectOfType<MainMenuManager>();
        }
        
        if (mainMenuManager == null)
        {
            Debug.LogError("❌ MainMenuManager not found!");
            return;
        }
        
        ValidateMainMenuPanel();
        ValidateSettingsNavigation();
        ValidateSettingsContent();
        ValidateAudioSettings();
        ValidateControlsSettings();
        
        PrintValidationResults();
    }
    
    void ValidateMainMenuPanel()
    {
        mainMenuSetupValid = false;
        
        if (mainMenuManager.mainMenuPanel != null)
        {
            bool hasPlay = mainMenuManager.playButton != null;
            bool hasSettings = mainMenuManager.settingsButton != null;
            bool hasCredits = mainMenuManager.creditsButton != null;
            bool hasExit = mainMenuManager.exitButton != null;
            
            if (hasPlay && hasSettings && hasCredits && hasExit)
            {
                mainMenuSetupValid = true;
                Debug.Log("✅ Main Menu Panel: All required buttons found");
            }
            else
            {
                Debug.LogWarning("⚠️ Main Menu Panel: Missing buttons");
            }
        }
        else
        {
            Debug.LogError("❌ Main Menu Panel not assigned!");
        }
    }
    
    void ValidateSettingsNavigation()
    {
        settingsNavigationValid = false;
        
        bool hasSettingsPanel = mainMenuManager.settingsPanel != null;
        bool hasAudioButton = mainMenuManager.audioSettingsButton != null;
        bool hasControlsButton = mainMenuManager.controlsButton != null;
        bool hasBackButton = mainMenuManager.backFromSettingsButton != null;
        
        if (hasSettingsPanel && hasAudioButton && hasControlsButton && hasBackButton)
        {
            settingsNavigationValid = true;
            Debug.Log("✅ Settings Navigation: All navigation buttons found");
        }
        else
        {
            Debug.LogWarning("⚠️ Settings Navigation: Missing components:");
            if (!hasSettingsPanel) Debug.LogWarning("  - Missing Settings Panel");
            if (!hasAudioButton) Debug.LogWarning("  - Missing Audio Settings Button");
            if (!hasControlsButton) Debug.LogWarning("  - Missing Controls Button");
            if (!hasBackButton) Debug.LogWarning("  - Missing Back Button");
        }
    }
    
    void ValidateSettingsContent()
    {
        settingsContentValid = false;
        
        bool hasAudioPanel = mainMenuManager.audioSettingsPanel != null;
        bool hasControlsPanel = mainMenuManager.controlsSettingsPanel != null;
        
        if (hasAudioPanel && hasControlsPanel)
        {
            settingsContentValid = true;
            Debug.Log("✅ Settings Content: Both content panels found");
        }
        else
        {
            Debug.LogWarning("⚠️ Settings Content: Missing content panels:");
            if (!hasAudioPanel) Debug.LogWarning("  - Missing Audio Settings Panel");
            if (!hasControlsPanel) Debug.LogWarning("  - Missing Controls Settings Panel");
        }
    }
    
    void ValidateAudioSettings()
    {
        audioSettingsValid = false;
        
        if (mainMenuManager.audioSettingsPanel != null)
        {
            var audioManager = mainMenuManager.audioSettingsPanel.GetComponent<AudioSettingsManager>();
            if (audioManager != null)
            {
                audioSettingsValid = true;
                Debug.Log("✅ Audio Settings: AudioSettingsManager component found");
            }
            else
            {
                Debug.LogWarning("⚠️ Audio Settings: AudioSettingsManager component missing on Audio Settings Panel");
            }
        }
    }
    
    void ValidateControlsSettings()
    {
        controlsSettingsValid = false;
        
        if (mainMenuManager.controlsSettingsPanel != null)
        {
            var controlsManager = mainMenuManager.controlsSettingsPanel.GetComponent<ControlsSettingsManager>();
            if (controlsManager != null)
            {
                controlsSettingsValid = true;
                Debug.Log("✅ Controls Settings: ControlsSettingsManager component found");
            }
            else
            {
                Debug.LogWarning("⚠️ Controls Settings: ControlsSettingsManager component missing on Controls Settings Panel");
            }
        }
    }
    
    void PrintValidationResults()
    {
        Debug.Log("\n=== VALIDATION RESULTS ===");
        Debug.Log($"Main Menu Setup: {(mainMenuSetupValid ? "✅" : "❌")}");
        Debug.Log($"Settings Navigation: {(settingsNavigationValid ? "✅" : "❌")}");
        Debug.Log($"Settings Content: {(settingsContentValid ? "✅" : "❌")}");
        Debug.Log($"Audio Settings: {(audioSettingsValid ? "✅" : "❌")}");
        Debug.Log($"Controls Settings: {(controlsSettingsValid ? "✅" : "❌")}");
        
        bool allValid = mainMenuSetupValid && settingsNavigationValid && settingsContentValid && 
                       audioSettingsValid && controlsSettingsValid;
        
        if (allValid)
        {
            Debug.Log("\n🎉 SETTINGS PANEL SETUP COMPLETE! 2-column layout ready.");
        }
        else
        {
            Debug.Log("\n⚠️ Settings panel setup needs attention. Check warnings above.");
        }
    }
    
    [ContextMenu("Show Setup Instructions")]
    public void ShowSetupInstructions()
    {
        Debug.Log("=== SETUP INSTRUCTIONS ===");
        Debug.Log(instructions);
    }
}