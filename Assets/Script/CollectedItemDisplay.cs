using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CollectedItemDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public float slideDistance = 50f;
    
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        // Set initial state
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
    
    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
    }
    
    public void SetItem(InventoryItem item, float displayDuration)
    {
        if (item == null)
            return;
        
        // Set item data
        if (itemIcon != null)
            itemIcon.sprite = item.itemIcon;
        
        if (itemNameText != null)
            itemNameText.text = item.itemName;
        
        // Start display sequence
        StartCoroutine(DisplaySequence(displayDuration));
    }
    
    private IEnumerator DisplaySequence(float displayDuration)
    {
        // Fade in with slide animation
        yield return StartCoroutine(FadeIn());
        
        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out with slide animation
        yield return StartCoroutine(FadeOut());
        
        // Destroy object
        Destroy(gameObject);
    }
    
    private IEnumerator FadeIn()
    {
        if (canvasGroup == null)
            yield break;
        
        // Set starting position (slide from right)
        Vector3 startPos = originalPosition + Vector3.right * slideDistance;
        rectTransform.anchoredPosition = startPos;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time for UI
            float t = elapsedTime / fadeInDuration;
            
            // Smooth fade and slide
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, originalPosition, t);
            
            yield return null;
        }
        
        // Ensure final values
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;
    }
    
    private IEnumerator FadeOut()
    {
        if (canvasGroup == null)
            yield break;
        
        Vector3 endPos = originalPosition + Vector3.right * slideDistance;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / fadeOutDuration;
            
            // Smooth fade and slide
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            rectTransform.anchoredPosition = Vector3.Lerp(originalPosition, endPos, t);
            
            yield return null;
        }
        
        // Ensure final values
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = endPos;
    }
}