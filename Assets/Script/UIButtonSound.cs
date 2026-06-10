using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Sound Settings")]
    public bool playHoverSound = true;
    public bool playClickSound = true;
    
    [Header("Custom Sounds (Optional)")]
    public AudioClip customHoverSound;
    public AudioClip customClickSound;
    
    private Button button;
    private AudioManager audioManager;
    
    void Start()
    {
        button = GetComponent<Button>();
        audioManager = AudioManager.Instance;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHoverSound && button.interactable)
        {
            if (customHoverSound != null && audioManager != null)
            {
                audioManager.PlayUISound(customHoverSound);
            }
            else if (audioManager != null)
            {
                audioManager.PlayButtonHover();
            }
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playClickSound && button.interactable)
        {
            if (customClickSound != null && audioManager != null)
            {
                audioManager.PlayUISound(customClickSound);
            }
            else if (audioManager != null)
            {
                audioManager.PlayButtonClick();
            }
        }
    }
    
    // Method untuk memanggil sound secara manual
    public void PlayHoverSound()
    {
        OnPointerEnter(null);
    }
    
    public void PlayClickSound()
    {
        OnPointerClick(null);
    }
}