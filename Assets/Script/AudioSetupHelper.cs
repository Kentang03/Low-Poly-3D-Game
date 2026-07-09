using UnityEngine;

[System.Serializable]
public class AudioSetupHelper : MonoBehaviour
{
    [Header("Setup AudioManager")]
    [Tooltip("Drag your collect sound here to setup AudioManager")]
    public AudioClip collectSoundToAssign;
    
    [Header("UI Sounds")]
    public AudioClip buttonClickSoundToAssign;
    public AudioClip buttonHoverSoundToAssign;
    
    [ContextMenu("Setup AudioManager")]
    public void SetupAudioManager()
    {
        // Find or create AudioManager
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        
        if (audioManager == null)
        {
            // Create new AudioManager
            GameObject audioManagerGO = new GameObject("AudioManager");
            audioManager = audioManagerGO.AddComponent<AudioManager>();
            Debug.Log("Created AudioManager GameObject");
        }
        
        // Assign collect sound
        if (collectSoundToAssign != null)
        {
            audioManager.defaultCollectSound = collectSoundToAssign;
            Debug.Log($"Assigned collect sound: {collectSoundToAssign.name} to AudioManager");
        }
        
        // Assign UI sounds
        if (buttonClickSoundToAssign != null)
        {
            audioManager.buttonClickSound = buttonClickSoundToAssign;
            Debug.Log($"Assigned button click sound: {buttonClickSoundToAssign.name} to AudioManager");
        }
        
        if (buttonHoverSoundToAssign != null)
        {
            audioManager.buttonHoverSound = buttonHoverSoundToAssign;
            Debug.Log($"Assigned button hover sound: {buttonHoverSoundToAssign.name} to AudioManager");
        }
        
        Debug.Log("AudioManager setup complete!");
    }
    
    [ContextMenu("Test Collect Audio")]
    public void TestCollectAudio()
    {
        if (AudioManager.Instance != null && collectSoundToAssign != null)
        {
            AudioManager.PlaySound(collectSoundToAssign);
            Debug.Log("Testing collect audio playback...");
        }
        else
        {
            Debug.LogWarning("AudioManager not found or no collect sound assigned!");
        }
    }
    
    [ContextMenu("Test Button Click")]
    public void TestButtonClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
            Debug.Log("Testing button click audio...");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
    
    [ContextMenu("Test Button Hover")]
    public void TestButtonHover()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonHover();
            Debug.Log("Testing button hover audio...");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
}