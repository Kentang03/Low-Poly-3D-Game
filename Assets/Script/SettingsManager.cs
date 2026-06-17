using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Graphics Settings")]
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    
    [Header("Controls Settings")]
    public Slider mouseSensitivitySlider;
    public Toggle invertMouseToggle;
    
    [Header("Character Reference")]
    public CharacterController playerController;
    
    private Resolution[] resolutions;
    
    void Start()
    {
        InitializeSettings();
        LoadSettings();
    }
    
    void InitializeSettings()
    {
        // Initialize resolution dropdown
        InitializeResolutions();
        
        // Initialize quality dropdown
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }
        
        // Setup slider listeners
        SetupSliderListeners();
        
        // Setup toggle listeners
        SetupToggleListeners();
        
        // Setup dropdown listeners
        SetupDropdownListeners();
    }
    
    void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;
        
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
    }
    
    void SetupSliderListeners()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }
    
    void SetupToggleListeners()
    {
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            
        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(SetVSync);
            
        if (invertMouseToggle != null)
            invertMouseToggle.onValueChanged.AddListener(SetInvertMouse);
    }
    
    void SetupDropdownListeners()
    {
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(SetQuality);
            
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    
    // Audio Settings
    public void SetMasterVolume(float volume)
    {
        // Clamp volume to prevent Log10(0) error
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            Debug.Log($"Set Master Volume: {volume} -> {Mathf.Log10(volume) * 20}dB, Success: {success}");
        }
        else
        {
            Debug.LogWarning("AudioMixer is null!");
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        // Clamp volume to prevent Log10(0) error
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            Debug.Log($"Set Music Volume: {volume} -> {Mathf.Log10(volume) * 20}dB, Success: {success}");
        }
        else
        {
            Debug.LogWarning("AudioMixer is null!");
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        // Clamp volume to prevent Log10(0) error
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        
        if (audioMixer != null)
        {
            bool success = audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            Debug.Log($"Set SFX Volume: {volume} -> {Mathf.Log10(volume) * 20}dB, Success: {success}");
        }
        else
        {
            Debug.LogWarning("AudioMixer is null!");
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    // Graphics Settings
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVSync ? 1 : 0);
    }
    
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        }
    }
    
    // Controls Settings
    public void SetMouseSensitivity(float sensitivity)
    {
        if (playerController != null)
            playerController.mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }
    
    public void SetInvertMouse(bool invert)
    {
        // Implementasi invert mouse bisa ditambahkan ke CharacterController
        PlayerPrefs.SetInt("InvertMouse", invert ? 1 : 0);
    }
    
    void LoadSettings()
    {
        // Load Audio Settings
        if (masterVolumeSlider != null)
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
            masterVolumeSlider.value = masterVolume;
            SetMasterVolume(masterVolume);
        }
        
        if (musicVolumeSlider != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            musicVolumeSlider.value = musicVolume;
            SetMusicVolume(musicVolume);
        }
        
        if (sfxVolumeSlider != null)
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            sfxVolumeSlider.value = sfxVolume;
            SetSFXVolume(sfxVolume);
        }
        
        // Load Graphics Settings
        if (qualityDropdown != null)
        {
            int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
            qualityDropdown.value = qualityLevel;
            SetQuality(qualityLevel);
        }
        
        if (fullscreenToggle != null)
        {
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
            fullscreenToggle.isOn = fullscreen;
            SetFullscreen(fullscreen);
        }
        
        if (vsyncToggle != null)
        {
            bool vsync = PlayerPrefs.GetInt("VSync", QualitySettings.vSyncCount) == 1;
            vsyncToggle.isOn = vsync;
            SetVSync(vsync);
        }
        
        if (resolutionDropdown != null)
        {
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutionDropdown.value);
            resolutionDropdown.value = resolutionIndex;
            SetResolution(resolutionIndex);
        }
        
        // Load Controls Settings
        if (mouseSensitivitySlider != null)
        {
            float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
            mouseSensitivitySlider.value = sensitivity;
            SetMouseSensitivity(sensitivity);
        }
        
        if (invertMouseToggle != null)
        {
            bool invert = PlayerPrefs.GetInt("InvertMouse", 0) == 1;
            invertMouseToggle.isOn = invert;
            SetInvertMouse(invert);
        }
    }
    
    public void ResetToDefaults()
    {
        // Reset semua setting ke default
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = 0.75f;
            SetMasterVolume(0.75f);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 0.75f;
            SetMusicVolume(0.75f);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 0.75f;
            SetSFXVolume(0.75f);
        }
        
        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = 2f;
            SetMouseSensitivity(2f);
        }
        
        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            SetQuality(QualitySettings.GetQualityLevel());
        }
        
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = true;
            SetFullscreen(true);
        }
        
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = true;
            SetVSync(true);
        }
    }
}