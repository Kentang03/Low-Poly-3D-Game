using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script untuk setup quest system dengan cepat
/// Drag script ini ke GameObject dan klik tombol setup
/// </summary>
public class QuickQuestSetup : MonoBehaviour
{
    [Header("Quick Setup Configuration")]
    [Tooltip("Nama quest yang akan dibuat")]
    public string questName = "Collect Sacred Items";
    
    [Tooltip("Deskripsi quest")]
    public string questDescription = "Find and collect the three sacred items in the correct order to complete your mission.";
    
    [Header("Dynamic Description Settings")]
    [Tooltip("Gunakan dynamic description per item")]
    public bool useDynamicDescription = true;
    
    [Tooltip("Description ketika quest baru dimulai")]
    public string initialDescription = "Prepare for your quest. The first item will appear soon.";
    
    [Tooltip("Description ketika quest selesai")]  
    public string completedDescription = "Quest completed! You have collected all the sacred items!";
    
    [Header("Quest Items Setup")]
    [Tooltip("Prefab untuk item quest (bisa sama semua atau berbeda)")]
    public GameObject questItemPrefab;
    
    [Tooltip("Posisi spawn untuk setiap item quest")]
    public Transform[] spawnPositions = new Transform[3];
    
    [Tooltip("Nama-nama item quest")]
    public string[] itemNames = { "Bakul Nasi", "Mask", "Sacred Key" };
    
    [Tooltip("Deskripsi setiap item quest")]
    public string[] itemDescriptions = { 
        "A traditional rice basket with mystical properties", 
        "An ancient ceremonial mask",
        "A sacred key that unlocks the final chamber"
    };
    
    [Tooltip("Dynamic quest description untuk setiap item (apa yang harus dilakukan player)")]
    [TextArea(2, 3)]
    public string[] questDescriptionsPerItem = {
        "Find and collect the Bakul Nasi. Look for it near the traditional house.",
        "Now find the Mask. Search around the ceremonial area.", 
        "Finally, locate the Sacred Key. Check the temple chamber."
    };
    
    [Header("Audio Setup")]
    public AudioClip questStartSound;
    public AudioClip itemActivatedSound;
    public AudioClip itemCollectedSound;
    public AudioClip questCompleteSound;
    
    [Header("Events Setup")]
    public GameObject questEventHandler;
    
    [Header("UI Setup")]
    public bool createQuestUI = true;
    public Canvas targetCanvas;
    
    [ContextMenu("Setup Complete Quest System")]
    public void SetupCompleteQuestSystem()
    {
        Debug.Log("🔧 Starting Quick Quest Setup...");
        
        // 1. Setup Quest System
        SetupQuestSystemComponent();
        
        // 2. Setup UI
        if (createQuestUI)
        {
            SetupQuestUI();
        }
        
        // 3. Setup Event Handler
        SetupEventHandler();
        
        // 4. Validate setup
        ValidateSetup();
        
        Debug.Log("✅ Quick Quest Setup completed!");
    }
    
    void SetupQuestSystemComponent()
    {
        Debug.Log("📝 Setting up QuestSystem component...");
        
        // Create atau find QuestSystem
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem == null)
        {
            GameObject questGO = new GameObject("QuestSystem");
            questSystem = questGO.AddComponent<QuestSystem>();
        }
        
        // Setup basic info
        questSystem.questName = questName;
        questSystem.questDescription = questDescription;
        
        // Setup dynamic description
        questSystem.useDynamicDescription = useDynamicDescription;
        questSystem.initialQuestDescription = initialDescription;
        questSystem.completedQuestDescription = completedDescription;
        
        // Setup audio
        AudioSource audioSource = questSystem.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = questSystem.gameObject.AddComponent<AudioSource>();
        
        questSystem.questStartSound = questStartSound;
        questSystem.itemActivatedSound = itemActivatedSound;
        questSystem.itemCollectedSound = itemCollectedSound;
        questSystem.questCompleteSound = questCompleteSound;
        
        // Clear existing items
        questSystem.questItems.Clear();
        
        // Setup quest items
        int itemCount = Mathf.Min(spawnPositions.Length, itemNames.Length, itemDescriptions.Length);
        if (useDynamicDescription)
        {
            itemCount = Mathf.Min(itemCount, questDescriptionsPerItem.Length);
        }
        
        for (int i = 0; i < itemCount; i++)
        {
            if (spawnPositions[i] == null)
            {
                Debug.LogWarning($"Spawn position {i} is null! Skipping item {i}");
                continue;
            }
            
            QuestItem questItem = new QuestItem();
            questItem.itemName = itemNames[i];
            questItem.itemDescription = itemDescriptions[i];
            questItem.itemPrefab = questItemPrefab;
            questItem.spawnPosition = spawnPositions[i];
            
            // Set dynamic quest description jika tersedia
            if (useDynamicDescription && i < questDescriptionsPerItem.Length)
            {
                questItem.questDescriptionForThisItem = questDescriptionsPerItem[i];
            }
            
            questSystem.questItems.Add(questItem);
            
            Debug.Log($"✅ Added quest item: {questItem.itemName}");
            if (useDynamicDescription)
            {
                Debug.Log($"   Quest Description: {questItem.questDescriptionForThisItem}");
            }
        }
        
        Debug.Log($"📦 Created {questSystem.questItems.Count} quest items");
    }
    
    void SetupQuestUI()
    {
        Debug.Log("🖥️ Setting up Quest UI...");
        
        // Find atau create Canvas
        if (targetCanvas == null)
            targetCanvas = FindObjectOfType<Canvas>();
            
        if (targetCanvas == null)
        {
            Debug.Log("Creating new Canvas for Quest UI...");
            GameObject canvasGO = new GameObject("Canvas");
            targetCanvas = canvasGO.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Add EventSystem if not exists
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        
        // Create Quest UI
        QuestSystemSetup setup = FindObjectOfType<QuestSystemSetup>();
        if (setup == null)
        {
            GameObject setupGO = new GameObject("QuestSystemSetup");
            setup = setupGO.AddComponent<QuestSystemSetup>();
        }
        
        setup.autoSetupUI = true;
        setup.questName = questName;
        setup.questDescription = questDescription;
        setup.SetupQuestSystem();
        
        Debug.Log("✅ Quest UI setup completed");
    }
    
    void SetupEventHandler()
    {
        Debug.Log("🎪 Setting up Event Handler...");
        
        GameObject eventHandler = questEventHandler;
        
        // Create event handler jika tidak ada
        if (eventHandler == null)
        {
            eventHandler = new GameObject("QuestEventHandler");
            eventHandler.AddComponent<QuestEventExamples>();
        }
        
        // Pastikan ada QuestEventExamples component
        QuestEventExamples eventExamples = eventHandler.GetComponent<QuestEventExamples>();
        if (eventExamples == null)
        {
            eventExamples = eventHandler.AddComponent<QuestEventExamples>();
        }
        
        // Setup audio di event handler
        eventExamples.questStartSound = questStartSound;
        eventExamples.itemActivatedSound = itemActivatedSound;
        eventExamples.itemCollectedSound = itemCollectedSound;
        eventExamples.questCompleteSound = questCompleteSound;
        
        // Auto-wire events
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem != null)
        {
            // Clear existing events
            questSystem.OnQuestStarted.RemoveAllListeners();
            questSystem.OnQuestItemActivated.RemoveAllListeners();
            questSystem.OnQuestItemCollected.RemoveAllListeners();
            questSystem.OnQuestCompleted.RemoveAllListeners();
            
            // Wire events
            questSystem.OnQuestStarted.AddListener(eventExamples.OnQuestStarted);
            questSystem.OnQuestItemActivated.AddListener(eventExamples.OnQuestItemActivated);
            questSystem.OnQuestItemCollected.AddListener(eventExamples.OnQuestItemCollected);
            questSystem.OnQuestCompleted.AddListener(eventExamples.OnQuestCompleted);
            
            Debug.Log("✅ Quest events wired to event handler");
        }
        
        Debug.Log("✅ Event Handler setup completed");
    }
    
    void ValidateSetup()
    {
        Debug.Log("🔍 Validating quest setup...");
        
        bool isValid = true;
        
        // Check QuestSystem
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem == null)
        {
            Debug.LogError("❌ QuestSystem not found!");
            isValid = false;
        }
        else if (questSystem.questItems.Count == 0)
        {
            Debug.LogError("❌ No quest items in QuestSystem!");
            isValid = false;
        }
        
        // Check InventorySystem
        if (FindObjectOfType<InventorySystem>() == null)
        {
            Debug.LogWarning("⚠️ InventorySystem not found - quest progression may not work!");
        }
        
        // Check QuestUI
        if (createQuestUI && FindObjectOfType<QuestUI>() == null)
        {
            Debug.LogWarning("⚠️ QuestUI not found - no UI will be displayed!");
        }
        
        // Check CollectibleItem prefab
        if (questItemPrefab != null)
        {
            CollectibleItem collectible = questItemPrefab.GetComponent<CollectibleItem>();
            if (collectible == null)
            {
                Debug.LogWarning("⚠️ Quest item prefab doesn't have CollectibleItem component!");
            }
        }
        
        if (isValid)
        {
            Debug.Log("🎉 Quest setup validation passed!");
            ShowSetupSummary();
        }
        else
        {
            Debug.LogError("💥 Quest setup validation failed! Check errors above.");
        }
    }
    
    void ShowSetupSummary()
    {
        Debug.Log("📋 QUEST SETUP SUMMARY:");
        Debug.Log($"   Quest Name: {questName}");
        
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem != null)
        {
            Debug.Log($"   Quest Items: {questSystem.questItems.Count}");
            for (int i = 0; i < questSystem.questItems.Count; i++)
            {
                Debug.Log($"     {i + 1}. {questSystem.questItems[i].itemName}");
            }
        }
        
        Debug.Log("   Components Created:");
        Debug.Log($"     ✅ QuestSystem");
        if (createQuestUI) Debug.Log($"     ✅ QuestUI");
        Debug.Log($"     ✅ QuestEventHandler");
        
        Debug.Log("\n🚀 Your quest system is ready to use!");
        Debug.Log("💡 TIP: Enter Play Mode and test with QuestSystem context menu options");
    }
    
    [ContextMenu("Create Example Spawn Points")]
    public void CreateExampleSpawnPoints()
    {
        for (int i = 0; i < 3; i++)
        {
            if (spawnPositions[i] == null)
            {
                GameObject spawnPoint = new GameObject($"QuestItem{i + 1}_SpawnPoint");
                spawnPoint.transform.position = new Vector3(i * 5f, 1f, 0f);
                spawnPositions[i] = spawnPoint.transform;
                
                Debug.Log($"Created spawn point {i + 1} at position {spawnPoint.transform.position}");
            }
        }
    }
    
    [ContextMenu("Reset Quest Setup")]
    public void ResetQuestSetup()
    {
        // Destroy created objects
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem != null)
            DestroyImmediate(questSystem.gameObject);
            
        QuestUI questUI = FindObjectOfType<QuestUI>();
        if (questUI != null)
            DestroyImmediate(questUI.gameObject);
            
        QuestEventExamples eventHandler = FindObjectOfType<QuestEventExamples>();
        if (eventHandler != null)
            DestroyImmediate(eventHandler.gameObject);
        
        Debug.Log("🔄 Quest setup has been reset");
    }
}