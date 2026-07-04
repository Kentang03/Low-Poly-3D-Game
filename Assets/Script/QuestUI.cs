using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questPanel;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questStatusText;
    public TextMeshProUGUI questProgressText;
    public Slider questProgressSlider;
    public Image questProgressFill;
    
    [Header("Quest Item Display")]
    public GameObject questItemPanel;
    public Image currentItemIcon;
    public TextMeshProUGUI currentItemNameText;
    public TextMeshProUGUI currentItemDescriptionText;
    
    [Header("Visual Settings")]
    public Color progressColor = Color.green;
    public Color incompleteColor = Color.yellow;
    public Color completedColor = Color.cyan;
    
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private QuestSystem questSystem;
    private Animator panelAnimator;
    
    void Start()
    {
        questSystem = QuestSystem.Instance;
        panelAnimator = questPanel?.GetComponent<Animator>();
        
        if (questSystem != null)
        {
            // Subscribe to quest events
            questSystem.OnQuestStarted.AddListener(OnQuestStarted);
            questSystem.OnQuestItemActivated.AddListener(OnQuestItemActivated);
            questSystem.OnQuestItemCollected.AddListener(OnQuestItemCollected);
            questSystem.OnQuestCompleted.AddListener(OnQuestCompleted);
            questSystem.OnQuestReset.AddListener(OnQuestReset);
        }
        
        // Initialize UI
        UpdateQuestDisplay();
        
    }
    
    void OnDestroy()
    {
        if (questSystem != null)
        {
            questSystem.OnQuestStarted.RemoveListener(OnQuestStarted);
            questSystem.OnQuestItemActivated.RemoveListener(OnQuestItemActivated);
            questSystem.OnQuestItemCollected.RemoveListener(OnQuestItemCollected);
            questSystem.OnQuestCompleted.RemoveListener(OnQuestCompleted);
            questSystem.OnQuestReset.RemoveListener(OnQuestReset);
        }
    }
    
    void OnQuestStarted()
    {
        ShowQuestPanel();
        UpdateQuestDisplay();
        Debug.Log("Quest UI: Quest Started");
    }
    
    void OnQuestItemActivated()
    {
        UpdateQuestDisplay();
        AnimateProgressUpdate();
        Debug.Log("Quest UI: New Item Activated");
    }
    
    void OnQuestItemCollected()
    {
        UpdateQuestDisplay();
        AnimateProgressUpdate();
        Debug.Log("Quest UI: Item Collected");
    }
    
    void OnQuestCompleted()
    {
        UpdateQuestDisplay();
        AnimateQuestCompletion();
        Debug.Log("Quest UI: Quest Completed");
    }
    
    void OnQuestReset()
    {
        UpdateQuestDisplay();
        Debug.Log("Quest UI: Quest Reset");
    }
    
    void UpdateQuestDisplay()
    {
        if (questSystem == null) return;
        
        // Update quest title and description
        if (questTitleText != null)
            questTitleText.text = questSystem.questName;
            
        if (questDescriptionText != null)
        {
            // Gunakan dynamic description dari quest system
            questDescriptionText.text = questSystem.GetCurrentQuestDescription();
        }
        
        // Update quest status
        if (questStatusText != null)
        {
            string status = questSystem.GetQuestStatus();
            questStatusText.text = status;
            
            // Change color based on status
            if (questSystem.IsQuestCompleted())
                questStatusText.color = completedColor;
            else
                questStatusText.color = incompleteColor;
        }
        
        // Update progress
        float progress = questSystem.GetQuestProgress();
        int currentIndex = questSystem.currentQuestIndex;
        int totalItems = questSystem.questItems.Count;
        
        if (questProgressText != null)
            questProgressText.text = $"{currentIndex}/{totalItems}";
            
        if (questProgressSlider != null)
        {
            questProgressSlider.value = progress;
            
            // Change fill color based on completion
            if (questProgressFill != null)
            {
                questProgressFill.color = questSystem.IsQuestCompleted() ? completedColor : progressColor;
            }
        }
        
        // Update current quest item display
        UpdateCurrentItemDisplay();
    }
    
    void UpdateCurrentItemDisplay()
    {
        if (questSystem == null || questItemPanel == null) return;
        
        QuestItem currentItem = questSystem.GetCurrentQuestItem();
        
        if (currentItem != null && !questSystem.IsQuestCompleted())
        {
            questItemPanel.SetActive(true);
            
            if (currentItemIcon != null)
                currentItemIcon.sprite = currentItem.itemIcon;
                
            if (currentItemNameText != null)
                currentItemNameText.text = currentItem.itemName;
                
            if (currentItemDescriptionText != null)
                currentItemDescriptionText.text = currentItem.itemDescription;
        }
        else
        {
            questItemPanel.SetActive(false);
        }
    }
    
    public void ShowQuestPanel()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(true);
            
            if (panelAnimator != null)
                panelAnimator.SetTrigger("Show");
        }
    }
    
    public void HideQuestPanel()
    {
        if (questPanel != null)
        {
            if (panelAnimator != null)
                panelAnimator.SetTrigger("Hide");
            else
                questPanel.SetActive(false);
        }
    }
    
    void AnimateProgressUpdate()
    {
        // Simple scale animation for progress update
        if (questProgressSlider != null)
        {
            StartCoroutine(AnimateScale(questProgressSlider.transform, Vector3.one * 1.1f, animationDuration * 0.5f));
        }
    }
    
    void AnimateQuestCompletion()
    {
        // Celebration animation when quest completes
        if (questPanel != null)
        {
            StartCoroutine(AnimateScale(questPanel.transform, Vector3.one * 1.05f, animationDuration));
        }
    }
    
    System.Collections.IEnumerator AnimateScale(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 originalScale = target.localScale;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            target.localScale = Vector3.Lerp(originalScale, targetScale, animationCurve.Evaluate(progress));
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale back down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            target.localScale = Vector3.Lerp(targetScale, originalScale, animationCurve.Evaluate(progress));
            yield return null;
        }
        
        target.localScale = originalScale;
    }
    
    // Public methods untuk toggle UI
    public void ToggleQuestPanel()
    {
        if (questPanel != null)
        {
            if (questPanel.activeInHierarchy)
                HideQuestPanel();
            else
                ShowQuestPanel();
        }
    }
    
    // Method untuk update manual jika diperlukan
    [ContextMenu("Update Display")]
    public void ManualUpdateDisplay()
    {
        UpdateQuestDisplay();
    }
}