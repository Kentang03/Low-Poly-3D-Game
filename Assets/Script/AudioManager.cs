using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Collect Audio Settings")]
    public AudioClip defaultCollectSound;
    public AudioSource audioSource;
    
    [Header("UI Audio Settings")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    
    [Header("Audio Mixer")]
    public AudioMixerGroup masterMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Setup AudioSource
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
                
            // Configure AudioSource
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.spatialBlend = 0f; // 2D sound
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Collect sound methods
    public void PlayCollectSound(AudioClip customSound = null)
    {
        AudioClip soundToPlay = customSound != null ? customSound : defaultCollectSound;
        
        if (soundToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundToPlay);
            Debug.Log($"AudioManager playing: {soundToPlay.name}");
        }
        else
        {
            Debug.LogWarning("AudioManager: No sound to play or AudioSource missing!");
        }
    }
    
    public static void PlaySound(AudioClip clip)
    {
        if (Instance != null)
        {
            Instance.PlayCollectSound(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found!");
        }
    }
    
    // UI Sound methods (required by other scripts)
    public void PlayButtonClick()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        else if (defaultCollectSound != null && audioSource != null)
        {
            // Fallback to collect sound if button click sound not available
            audioSource.PlayOneShot(defaultCollectSound);
        }
    }
    
    public void PlayButtonHover()
    {
        if (buttonHoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }
    
    public void PlayUISound(AudioClip customSound)
    {
        if (customSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(customSound);
        }
    }
    
    // Audio mixer volume methods (required by settings)
    public void SetMasterVolume(float volume)
    {
        // Convert 0-1 range to decibel range (-80 to 0)
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        
        if (masterMixer != null)
        {
            masterMixer.audioMixer.SetFloat("MasterVolume", dB);
        }
        else
        {
            // Fallback to AudioListener
            AudioListener.volume = volume;
        }
        
        Debug.Log($"Set Master Volume: {volume} ({dB}dB)");
    }
    
    public void SetMusicVolume(float volume)
    {
        // Convert 0-1 range to decibel range (-80 to 0)
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        
        if (musicMixer != null)
        {
            musicMixer.audioMixer.SetFloat("MusicVolume", dB);
        }
        
        Debug.Log($"Set Music Volume: {volume} ({dB}dB)");
    }
    
    public void SetSFXVolume(float volume)
    {
        // Convert 0-1 range to decibel range (-80 to 0)  
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        
        if (sfxMixer != null)
        {
            sfxMixer.audioMixer.SetFloat("SFXVolume", dB);
        }
        
        Debug.Log($"Set SFX Volume: {volume} ({dB}dB)");
    }
}