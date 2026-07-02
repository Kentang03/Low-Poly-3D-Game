using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Manages controls settings UI in the settings panel
/// </summary>
public class ControlsSettingsManager : MonoBehaviour
{
    [Header("Sensitivity Sliders")]
    public Slider mouseSensitivitySlider;
    public Slider cameraSpeedSlider;
    
    [Header("Sensitivity Labels")]
    public TextMeshProUGUI mouseSensitivityLabel;
    public TextMeshProUGUI cameraSpeedLabel;
    
    [Header("Invert Options")]
    public Toggle invertYAxisToggle;
    public Toggle invertXAxisToggle;
    
    [Header("Control Display")]
    public GameObject controlsDisplayArea;
    public TextMeshProUGUI controlsInstructionsText;
    
    [Header("Default Values")]
    public float defaultMouseSensitivity = 3.0f;
    public float defaultCameraSpeed = 5.0f;
    public bool defaultInvertY = false;
    public bool defaultInvertX = false;
    
    [Header("Controls Instructions")]
    [TextArea(8, 12)]
    public string controlsText = @"CONTROLS:

MOVEMENT:
• W, A, S, D - Move Character
• Mouse - Look Around
• Left Shift - Run
• Space - Jump

INTERACTION:
• E - Interact with Objects
• F - Collect Items

CAMERA:
• Mouse Scroll - Zoom In/Out
• Hold Middle Mouse - Free Look

MENU:
• ESC - Pause Menu
• TAB - Inventory (if available)

SETTINGS:
Adjust mouse sensitivity and camera speed below to customize your experience.";
    
    void Start()
    {
        InitializeControlsSettings();
        SetupControlListeners();
        LoadControlsSettings();
        DisplayControlsInstructions();
    }
    
    void InitializeControlsSettings()
    {
        // Setup UI elements
        if (controlsInstructionsText != null)
            controlsInstructionsText.text = controlsText;
    }
    
    void SetupControlListeners()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            
        if (cameraSpeedSlider != null)
            cameraSpeedSlider.onValueChanged.AddListener(OnCameraSpeedChanged);
            
        if (invertYAxisToggle != null)
            invertYAxisToggle.onValueChanged.AddListener(OnInvertYChanged);
            
        if (invertXAxisToggle != null)
            invertXAxisToggle.onValueChanged.AddListener(OnInvertXChanged);
    }
    
    void LoadControlsSettings()
    {
        // Load saved settings or use defaults
        float mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);
        float cameraSpeed = PlayerPrefs.GetFloat("CameraSpeed", defaultCameraSpeed);
        bool invertY = PlayerPrefs.GetInt("InvertY", defaultInvertY ? 1 : 0) == 1;
        bool invertX = PlayerPrefs.GetInt("InvertX", defaultInvertX ? 1 : 0) == 1;
        
        // Set UI values
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = mouseSensitivity;
            
        if (cameraSpeedSlider != null)
            cameraSpeedSlider.value = cameraSpeed;
            
        if (invertYAxisToggle != null)
            invertYAxisToggle.isOn = invertY;
            
        if (invertXAxisToggle != null)
            invertXAxisToggle.isOn = invertX;
        
        // Apply settings to camera system
        ApplyControlsSettings();
        
        // Update labels
        UpdateControlLabels();
    }
    
    public void OnMouseSensitivityChanged(float value)
    {
        // Save setting
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        
        // Apply to camera system
        ApplyMouseSensitivity(value);
        
        // Update label
        UpdateMouseSensitivityLabel(value);
        
        Debug.Log($"Mouse Sensitivity changed to: {value:F1}");
    }
    
    public void OnCameraSpeedChanged(float value)
    {
        // Save setting
        PlayerPrefs.SetFloat("CameraSpeed", value);
        
        // Apply to camera system
        ApplyCameraSpeed(value);
        
        // Update label
        UpdateCameraSpeedLabel(value);
        
        Debug.Log($"Camera Speed changed to: {value:F1}");
    }
    
    public void OnInvertYChanged(bool inverted)
    {
        // Save setting
        PlayerPrefs.SetInt("InvertY", inverted ? 1 : 0);
        
        // Apply to camera system
        ApplyInvertY(inverted);
        
        Debug.Log($"Invert Y Axis: {inverted}");
    }
    
    public void OnInvertXChanged(bool inverted)
    {
        // Save setting
        PlayerPrefs.SetInt("InvertX", inverted ? 1 : 0);
        
        // Apply to camera system
        ApplyInvertX(inverted);
        
        Debug.Log($"Invert X Axis: {inverted}");
    }
    
    void ApplyControlsSettings()
    {
        if (mouseSensitivitySlider != null)
            ApplyMouseSensitivity(mouseSensitivitySlider.value);
            
        if (cameraSpeedSlider != null)
            ApplyCameraSpeed(cameraSpeedSlider.value);
            
        if (invertYAxisToggle != null)
            ApplyInvertY(invertYAxisToggle.isOn);
            
        if (invertXAxisToggle != null)
            ApplyInvertX(invertXAxisToggle.isOn);
    }
    
    void ApplyMouseSensitivity(float value)
    {
        // Apply to vThirdPersonCamera if available
        var camera = FindObjectOfType<vThirdPersonCamera>();
        if (camera != null)
        {
            camera.xMouseSensitivity = value;
            camera.yMouseSensitivity = value;
        }
    }
    
    void ApplyCameraSpeed(float value)
    {
        // Apply to vThirdPersonCamera if available
        var camera = FindObjectOfType<vThirdPersonCamera>();
        if (camera != null)
        {
            camera.smoothCameraRotation = value;
        }
    }
    
    void ApplyInvertY(bool inverted)
    {
        // Apply Y-axis inversion to camera system
        // This would need to be implemented in the camera controller
        Debug.Log($"Y-Axis inversion applied: {inverted}");
    }
    
    void ApplyInvertX(bool inverted)
    {
        // Apply X-axis inversion to camera system
        // This would need to be implemented in the camera controller
        Debug.Log($"X-Axis inversion applied: {inverted}");
    }
    
    void UpdateControlLabels()
    {
        if (mouseSensitivitySlider != null)
            UpdateMouseSensitivityLabel(mouseSensitivitySlider.value);
            
        if (cameraSpeedSlider != null)
            UpdateCameraSpeedLabel(cameraSpeedSlider.value);
    }
    
    void UpdateMouseSensitivityLabel(float value)
    {
        if (mouseSensitivityLabel != null)
            mouseSensitivityLabel.text = value.ToString("F1");
    }
    
    void UpdateCameraSpeedLabel(float value)
    {
        if (cameraSpeedLabel != null)
            cameraSpeedLabel.text = value.ToString("F1");
    }
    
    void DisplayControlsInstructions()
    {
        if (controlsInstructionsText != null && !string.IsNullOrEmpty(controlsText))
        {
            controlsInstructionsText.text = controlsText;
        }
    }
    
    [ContextMenu("Reset Controls to Defaults")]
    public void ResetControlsToDefaults()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = defaultMouseSensitivity;
            
        if (cameraSpeedSlider != null)
            cameraSpeedSlider.value = defaultCameraSpeed;
            
        if (invertYAxisToggle != null)
            invertYAxisToggle.isOn = defaultInvertY;
            
        if (invertXAxisToggle != null)
            invertXAxisToggle.isOn = defaultInvertX;
        
        Debug.Log("Controls settings reset to defaults");
    }
}