using UnityEngine;

/// <summary>
/// Contoh implementasi quest dengan dynamic description
/// Script ini menunjukkan cara menggunakan quest system dengan description yang berubah per item
/// </summary>
public class DynamicQuestExample : MonoBehaviour
{
    [Header("Quest Configuration")]
    [Tooltip("Quest system yang akan dikonfigurasi")]
    public QuestSystem questSystem;
    
    [Header("Dynamic Descriptions")]
    [TextArea(2, 4)]
    public string initialDescription = "Welcome! Your quest is about to begin. Prepare yourself for the challenge ahead.";
    
    [TextArea(2, 4)]
    public string completedDescription = "Congratulations! You have successfully completed the quest and collected all items!";
    
    [Header("Item Quest Descriptions")]
    [TextArea(2, 4)]
    public string bakulNasiQuestDescription = "Find and collect the Bakul Nasi. Look for it near the traditional Indonesian house. The rice basket holds ancient power.";
    
    [TextArea(2, 4)]
    public string maskQuestDescription = "Now search for the ceremonial Mask. It should be hidden around the temple area. This mask was used in ancient rituals.";
    
    [TextArea(2, 4)]
    public string sacredKeyQuestDescription = "Finally, locate the Sacred Key. Check the final chamber or altar. This key unlocks the ultimate treasure.";
    
    [Header("Audio Feedback")]
    public AudioClip itemFoundSound;
    public AudioClip descriptionUpdateSound;
    
    void Start()
    {
        if (questSystem == null)
            questSystem = FindObjectOfType<QuestSystem>();
            
        if (questSystem != null)
        {
            SetupDynamicQuest();
            
            // Subscribe ke events untuk audio feedback
            questSystem.OnQuestItemActivated.AddListener(OnItemActivated);
            questSystem.OnQuestItemCollected.AddListener(OnItemCollected);
        }
    }
    
    void OnDestroy()
    {
        if (questSystem != null)
        {
            questSystem.OnQuestItemActivated.RemoveListener(OnItemActivated);
            questSystem.OnQuestItemCollected.RemoveListener(OnItemCollected);
        }
    }
    
    [ContextMenu("Setup Dynamic Quest")]
    public void SetupDynamicQuest()
    {
        if (questSystem == null) return;
        
        // Enable dynamic description
        questSystem.useDynamicDescription = true;
        questSystem.initialQuestDescription = initialDescription;
        questSystem.completedQuestDescription = completedDescription;
        
        // Setup individual item descriptions
        if (questSystem.questItems.Count >= 1)
        {
            questSystem.questItems[0].questDescriptionForThisItem = bakulNasiQuestDescription;
            Debug.Log("✅ Set Bakul Nasi quest description");
        }
        
        if (questSystem.questItems.Count >= 2)
        {
            questSystem.questItems[1].questDescriptionForThisItem = maskQuestDescription;
            Debug.Log("✅ Set Mask quest description");
        }
        
        if (questSystem.questItems.Count >= 3)
        {
            questSystem.questItems[2].questDescriptionForThisItem = sacredKeyQuestDescription;
            Debug.Log("✅ Set Sacred Key quest description");
        }
        
        Debug.Log("🎯 Dynamic Quest Configuration Complete!");
    }
    
    void OnItemActivated()
    {
        // Play sound ketika description berubah
        if (descriptionUpdateSound != null)
        {
            AudioSource.PlayClipAtPoint(descriptionUpdateSound, Camera.main.transform.position, 0.5f);
        }
        
        // Log current description
        if (questSystem != null)
        {
            Debug.Log($"📝 Quest Description Updated: {questSystem.GetCurrentQuestDescription()}");
        }
    }
    
    void OnItemCollected()
    {
        // Play sound ketika item dikumpulkan
        if (itemFoundSound != null)
        {
            AudioSource.PlayClipAtPoint(itemFoundSound, Camera.main.transform.position);
        }
        
        // Show progress message dengan description baru
        if (questSystem != null && !questSystem.IsQuestCompleted())
        {
            QuestItem nextItem = questSystem.GetCurrentQuestItem();
            if (nextItem != null)
            {
                ShowProgressMessage($"Item collected! Next: {nextItem.itemName}");
            }
        }
        else if (questSystem != null && questSystem.IsQuestCompleted())
        {
            ShowProgressMessage("All items collected! Quest completed!");
        }
    }
    
    void ShowProgressMessage(string message)
    {
        Debug.Log($"💬 Progress: {message}");
        
        // Integrate dengan UI message system jika ada
        // Example: UIMessageSystem.Instance?.ShowMessage(message, 3f);
    }
    
    /// <summary>
    /// Method untuk mengupdate description secara manual
    /// Berguna jika ingin mengubah description berdasarkan kondisi tertentu
    /// </summary>
    /// <param name="itemIndex">Index item yang akan diupdate</param>
    /// <param name="newDescription">Description baru</param>
    public void UpdateItemQuestDescription(int itemIndex, string newDescription)
    {
        if (questSystem == null || itemIndex < 0 || itemIndex >= questSystem.questItems.Count)
        {
            Debug.LogWarning($"Invalid item index: {itemIndex}");
            return;
        }
        
        questSystem.questItems[itemIndex].questDescriptionForThisItem = newDescription;
        
        // Update UI jika item tersebut sedang aktif
        if (questSystem.currentQuestIndex == itemIndex)
        {
            // Trigger UI update
            QuestUI questUI = FindObjectOfType<QuestUI>();
            if (questUI != null)
            {
                questUI.ManualUpdateDisplay();
            }
        }
        
        Debug.Log($"📝 Updated quest description for item {itemIndex}: {newDescription}");
    }
    
    /// <summary>
    /// Method untuk mengupdate description berdasarkan kondisi game
    /// Contoh penggunaan: weather, time of day, player level, etc.
    /// </summary>
    public void UpdateDescriptionBasedOnGameState()
    {
        if (questSystem == null) return;
        
        // Example: Update description berdasarkan waktu
        int currentHour = System.DateTime.Now.Hour;
        
        if (currentHour >= 18 || currentHour <= 6) // Malam
        {
            if (questSystem.questItems.Count > 0)
            {
                UpdateItemQuestDescription(0, 
                    "Find the Bakul Nasi in the darkness. Use your lantern to search near the traditional house. Be careful of the night creatures.");
            }
        }
        else // Siang
        {
            if (questSystem.questItems.Count > 0)
            {
                UpdateItemQuestDescription(0, 
                    "Find the Bakul Nasi in the daylight. Look for it near the traditional Indonesian house. The rice basket should be easier to spot now.");
            }
        }
    }
    
    /// <summary>
    /// Method untuk mengupdate description berdasarkan difficulty level
    /// </summary>
    /// <param name="isHardMode">Apakah menggunakan hard mode</param>
    public void UpdateDescriptionForDifficulty(bool isHardMode)
    {
        if (questSystem == null) return;
        
        if (isHardMode)
        {
            // Hard mode - less specific hints
            UpdateItemQuestDescription(0, "Locate the first sacred item. No more hints will be provided.");
            UpdateItemQuestDescription(1, "Find the second sacred item. Trust your instincts.");
            UpdateItemQuestDescription(2, "Discover the final sacred item. The path is yours to find.");
        }
        else
        {
            // Easy mode - detailed hints
            UpdateItemQuestDescription(0, bakulNasiQuestDescription);
            UpdateItemQuestDescription(1, maskQuestDescription);
            UpdateItemQuestDescription(2, sacredKeyQuestDescription);
        }
        
        Debug.Log($"🎯 Updated quest descriptions for {(isHardMode ? "Hard" : "Easy")} mode");
    }
    
    // Debug methods untuk testing
    [ContextMenu("Test Current Description")]
    public void TestCurrentDescription()
    {
        if (questSystem != null)
        {
            Debug.Log($"Current Quest Description: {questSystem.GetCurrentQuestDescription()}");
        }
    }
    
    [ContextMenu("Update for Night Mode")]
    public void TestNightMode()
    {
        UpdateDescriptionBasedOnGameState();
    }
    
    [ContextMenu("Update for Hard Mode")]
    public void TestHardMode()
    {
        UpdateDescriptionForDifficulty(true);
    }
    
    [ContextMenu("Update for Easy Mode")]
    public void TestEasyMode()
    {
        UpdateDescriptionForDifficulty(false);
    }
}