using UnityEngine;
using UnityEngine.Events;

public class ItemCollectionManager : MonoBehaviour
{
    [Header("Collection Settings")]
    public int targetItemCount = 3;
    public string itemName = "Crystal";
    
    [Header("Current Progress")]
    public int currentItemCount = 0;
    public bool isTaskComplete = false;
    
    [Header("UI References")]
    public ItemCollectionHUD collectionHUD;
    
    [Header("Events")]
    public UnityEvent OnItemCollected;
    public UnityEvent OnTaskCompleted;
    
    [Header("Audio")]
    public AudioClip taskCompleteSound;
    
    private static ItemCollectionManager instance;
    public static ItemCollectionManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ItemCollectionManager>();
            return instance;
        }
    }
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        // Initialize HUD
        if (collectionHUD == null)
            collectionHUD = FindObjectOfType<ItemCollectionHUD>();
            
        UpdateHUD();
    }
    
    public void CollectItem(string collectedItemName, int value = 1)
    {
        // Only collect items that match our target item
        if (collectedItemName != itemName || isTaskComplete)
            return;
            
        currentItemCount += value;
        currentItemCount = Mathf.Min(currentItemCount, targetItemCount); // Cap at target
        
        Debug.Log($"Items collected: {currentItemCount}/{targetItemCount}");
        
        // Update HUD
        UpdateHUD();
        
        // Trigger item collected event
        OnItemCollected?.Invoke();
        
        // Check if task is complete
        if (currentItemCount >= targetItemCount && !isTaskComplete)
        {
            CompleteTask();
        }
    }
    
    void CompleteTask()
    {
        isTaskComplete = true;
        
        Debug.Log("Task Complete! All items collected!");
        
        // Play completion sound
        if (taskCompleteSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(taskCompleteSound);
        }
        
        // Update HUD to show completion
        UpdateHUD();
        
        // Trigger completion event
        OnTaskCompleted?.Invoke();
        
        // Trigger something (customize this part)
        TriggerCompletionEvent();
    }
    
    void TriggerCompletionEvent()
    {
        // Customize what happens when task is complete
        // Examples:
        // - Open a door
        // - Spawn new items/enemies
        // - Trigger cutscene
        // - Unlock new area
        // - Show victory screen
        
        Debug.Log("Something awesome happened! Task completed!");
        
        // Example: Show completion message for 3 seconds
        if (collectionHUD != null)
        {
            collectionHUD.ShowCompletionMessage("All crystals collected! Well done!", 3f);
        }
    }
    
    void UpdateHUD()
    {
        if (collectionHUD != null)
        {
            collectionHUD.UpdateItemCount(currentItemCount, targetItemCount, itemName);
            collectionHUD.SetTaskComplete(isTaskComplete);
        }
    }
    
    // Public methods for external use
    public bool IsTaskComplete()
    {
        return isTaskComplete;
    }
    
    public float GetProgress()
    {
        return (float)currentItemCount / targetItemCount;
    }
    
    public void ResetCollection()
    {
        currentItemCount = 0;
        isTaskComplete = false;
        UpdateHUD();
        Debug.Log("Collection progress reset!");
    }
    
    // Method untuk testing di inspector
    [ContextMenu("Add Test Item")]
    public void AddTestItem()
    {
        CollectItem(itemName, 1);
    }
    
    [ContextMenu("Reset Collection")]
    public void ResetCollectionDebug()
    {
        ResetCollection();
    }
}