using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemCollectionHUD : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI itemCountText;
    public TextMeshProUGUI taskStatusText;
    public TextMeshProUGUI completionMessageText;
    public Image progressBar;
    public GameObject hudPanel;
    
    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color completeColor = Color.green;
    public float textAnimationDuration = 0.5f;
    
    [Header("Messages")]
    public string collectingMessage = "Collect {0}: {1}/{2}";
    public string completedMessage = "Task Complete!";
    
    private bool isTaskComplete = false;
    private Coroutine messageCoroutine;
    
    void Start()
    {
        // Initialize HUD
        if (hudPanel != null)
            hudPanel.SetActive(true);
            
        if (completionMessageText != null)
            completionMessageText.gameObject.SetActive(false);
    }
    
    public void UpdateItemCount(int current, int target, string itemName)
    {
        // Update main counter text
        if (itemCountText != null)
        {
            if (current >= target)
            {
                // Show completion in main text
                itemCountText.text = $"{itemName} Collection: COMPLETE!";
                itemCountText.color = completeColor;
            }
            else
            {
                // Show progress
                string displayText = string.Format(collectingMessage, itemName, current, target);
                itemCountText.text = displayText;
                itemCountText.color = normalColor;
            }
            
            // Animate text color
            StartCoroutine(AnimateTextColor(itemCountText));
        }
        
        // Update progress bar
        if (progressBar != null)
        {
            float progress = (float)current / target;
            progressBar.fillAmount = progress;
            
            // Change progress bar color when complete
            if (current >= target)
            {
                progressBar.color = completeColor;
            }
        }
    }
    
    public void SetTaskComplete(bool complete)
    {
        isTaskComplete = complete;
        // TaskStatusText tidak digunakan dalam versi simplified ini
    }
    
    public void ShowCompletionMessage(string message, float duration)
    {
        if (completionMessageText != null)
        {
            if (messageCoroutine != null)
                StopCoroutine(messageCoroutine);
                
            messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, duration));
        }
    }
    
    IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        completionMessageText.text = message;
        completionMessageText.gameObject.SetActive(true);
        
        // Fade in
        CanvasGroup canvasGroup = completionMessageText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = completionMessageText.gameObject.AddComponent<CanvasGroup>();
        
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.3f));
        
        // Wait
        yield return new WaitForSeconds(duration);
        
        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.3f));
        
        completionMessageText.gameObject.SetActive(false);
    }
    
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    IEnumerator AnimateTextColor(TextMeshProUGUI textComponent)
    {
        Color originalColor = textComponent.color;
        Color highlightColor = completeColor;
        
        // Animate to highlight color
        float elapsedTime = 0f;
        while (elapsedTime < textAnimationDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (textAnimationDuration / 2);
            textComponent.color = Color.Lerp(originalColor, highlightColor, progress);
            yield return null;
        }
        
        // Animate back to original color (unless task is complete)
        if (!isTaskComplete)
        {
            elapsedTime = 0f;
            while (elapsedTime < textAnimationDuration / 2)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (textAnimationDuration / 2);
                textComponent.color = Color.Lerp(highlightColor, originalColor, progress);
                yield return null;
            }
            
            textComponent.color = originalColor;
        }
    }
    
    public void SetHUDVisibility(bool visible)
    {
        if (hudPanel != null)
            hudPanel.SetActive(visible);
    }
    
    // Method untuk testing
    [ContextMenu("Test Completion Message")]
    public void TestCompletionMessage()
    {
        ShowCompletionMessage("Test completion message!", 2f);
    }
}