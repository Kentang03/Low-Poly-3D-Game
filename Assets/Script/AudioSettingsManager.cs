using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages audio settings UI in the settings panel
/// </summary>
public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Volume Labels (Optional)")]
    public TextMeshProUGUI masterVolumeLabel;
    public TextMeshProUGUI musicVolumeLabel;
    public TextMeshProUGUI sfxVolumeLabel;
    
    [Header("Audio Manager Reference")]
    public AudioManager audioManager;
    
    [Header("Default Values")]
    public float defaultMasterVolume = 1.0f;
    public float defaultMusicVolume = 0.7f;
    public float defaultSFXVolume = 0.8f;
    
    void Start()
    {
        InitializeAudioSettings();
        SetupSliderListeners();
        LoadAudioSettings();
    }
    
    void InitializeAudioSettings()
    {
        // Get audio manager reference
        if (audioManager == null)
            audioManager = AudioManager.Instance;
    }
    
    void SetupSliderListeners()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    void LoadAudioSettings()
    {
        // Load saved settings or use defaults
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        
        // Set slider values
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
        
        // Apply to audio manager
        ApplyAudioSettings();
        
        // Update labels
        UpdateVolumeLabels();
    }
    
    public void OnMasterVolumeChanged(float value)
    {
        // Save setting
        PlayerPrefs.SetFloat("MasterVolume", value);
        
        // Apply to audio manager
        if (audioManager != null)
            audioManager.SetMasterVolume(value);
        
        // Update label
        UpdateMasterVolumeLabel(value);
        
        Debug.Log($"Master Volume changed to: {value:F2}");
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        // Save setting
        PlayerPrefs.SetFloat("MusicVolume", value);
        
        // Apply to audio manager
        if (audioManager != null)
            audioManager.SetMusicVolume(value);
        
        // Update label
        UpdateMusicVolumeLabel(value);
        
        Debug.Log($"Music Volume changed to: {value:F2}");
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        // Save setting
        PlayerPrefs.SetFloat("SFXVolume", value);
        
        // Apply to audio manager
        if (audioManager != null)
            audioManager.SetSFXVolume(value);
        
        // Update label
        UpdateSFXVolumeLabel(value);
        
        Debug.Log($"SFX Volume changed to: {value:F2}");
    }
    
    void ApplyAudioSettings()
    {
        if (audioManager == null) return;
        
        if (masterVolumeSlider != null)
            audioManager.SetMasterVolume(masterVolumeSlider.value);
            
        if (musicVolumeSlider != null)
            audioManager.SetMusicVolume(musicVolumeSlider.value);
            
        if (sfxVolumeSlider != null)
            audioManager.SetSFXVolume(sfxVolumeSlider.value);
    }
    
    void UpdateVolumeLabels()
    {
        if (masterVolumeSlider != null)
            UpdateMasterVolumeLabel(masterVolumeSlider.value);
            
        if (musicVolumeSlider != null)
            UpdateMusicVolumeLabel(musicVolumeSlider.value);
            
        if (sfxVolumeSlider != null)
            UpdateSFXVolumeLabel(sfxVolumeSlider.value);
    }
    
    void UpdateMasterVolumeLabel(float value)
    {
        if (masterVolumeLabel != null)
            masterVolumeLabel.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
    
    void UpdateMusicVolumeLabel(float value)
    {
        if (musicVolumeLabel != null)
            musicVolumeLabel.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
    
    void UpdateSFXVolumeLabel(float value)
    {
        if (sfxVolumeLabel != null)
            sfxVolumeLabel.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
    
    [ContextMenu("Reset to Defaults")]
    public void ResetToDefaults()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = defaultMasterVolume;
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = defaultMusicVolume;
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = defaultSFXVolume;
        
        Debug.Log("Audio settings reset to defaults");
    }
}