using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class QuestSystemSetup : MonoBehaviour
{
    [Header("Setup Guide")]
    [TextArea(5, 10)]
    public string setupInstructions = 
        "QUEST SYSTEM SETUP GUIDE:\n\n" +
        "1. Drag QuestSystem prefab ke scene\n" +
        "2. Setup UI Canvas dengan QuestUI component\n" +
        "3. Assign quest items dengan prefab dan spawn positions\n" +
        "4. Configure quest events sesuai kebutuhan\n" +
        "5. Test quest progression di Play Mode";
    
    [Header("Auto Setup")]
    public bool autoSetupUI = true;
    public bool autoSetupAudio = true;
    public bool createExampleQuest = true;
    
    [Header("Quest Configuration")]
    public string questName = "Collect Sacred Items";
    public string questDescription = "Find and collect the three sacred items in the correct order";
    
    [Header("Example Quest Items")]
    public GameObject item1Prefab;
    public GameObject item2Prefab;
    public GameObject item3Prefab;
    
    [Header("Spawn Positions")]
    public Transform item1SpawnPosition;
    public Transform item2SpawnPosition;
    public Transform item3SpawnPosition;
    
    [ContextMenu("Setup Quest System")]
    public void SetupQuestSystem()
    {
        Debug.Log("Setting up Quest System...");
        
        // 1. Setup QuestSystem GameObject
        SetupQuestSystemComponent();
        
        // 2. Setup UI if enabled
        if (autoSetupUI)
        {
            SetupQuestUI();
        }
        
        // 3. Create example quest if enabled
        if (createExampleQuest)
        {
            CreateExampleQuest();
        }
        
        // 4. Setup audio if enabled
        if (autoSetupAudio)
        {
            SetupAudioSources();
        }
        
        Debug.Log("Quest System setup completed!");
    }
    
    void SetupQuestSystemComponent()
    {
        // Find atau create QuestSystem
        QuestSystem existingQuestSystem = FindObjectOfType<QuestSystem>();
        if (existingQuestSystem == null)
        {
            GameObject questSystemGO = new GameObject("QuestSystem");
            existingQuestSystem = questSystemGO.AddComponent<QuestSystem>();
            Debug.Log("Created new QuestSystem GameObject");
        }
        
        // Configure basic settings
        existingQuestSystem.questName = questName;
        existingQuestSystem.questDescription = questDescription;
    }
    
    void SetupQuestUI()
    {
        // Find Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found! Creating new Canvas for Quest UI");
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create Quest UI Panel
        GameObject questUIPanel = CreateQuestUIPanel(canvas.transform);
        
        // Add QuestUI component
        QuestUI questUI = questUIPanel.AddComponent<QuestUI>();
        
        Debug.Log("Quest UI setup completed");
    }
    
    GameObject CreateQuestUIPanel(Transform parent)
    {
        // Main Quest Panel
        GameObject questPanel = new GameObject("QuestPanel");
        questPanel.transform.SetParent(parent, false);
        
        RectTransform questPanelRect = questPanel.AddComponent<RectTransform>();
        questPanelRect.anchorMin = new Vector2(0, 0.7f);
        questPanelRect.anchorMax = new Vector2(0.4f, 1);
        questPanelRect.offsetMin = Vector2.zero;
        questPanelRect.offsetMax = Vector2.zero;
        
        Image questPanelImage = questPanel.AddComponent<Image>();
        questPanelImage.color = new Color(0, 0, 0, 0.7f);
        
        // Quest Title
        GameObject titleObj = new GameObject("QuestTitle");
        titleObj.transform.SetParent(questPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, -5);
        titleRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Quest Title";
        titleText.fontSize = 18;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        
        // Quest Description
        GameObject descObj = new GameObject("QuestDescription");
        descObj.transform.SetParent(questPanel.transform, false);
        
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0.6f);
        descRect.anchorMax = new Vector2(1, 0.8f);
        descRect.offsetMin = new Vector2(10, 0);
        descRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Quest Description";
        descText.fontSize = 12;
        descText.color = Color.white;
        descText.alignment = TextAlignmentOptions.TopLeft;
        
        // Quest Status
        GameObject statusObj = new GameObject("QuestStatus");
        statusObj.transform.SetParent(questPanel.transform, false);
        
        RectTransform statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0.4f);
        statusRect.anchorMax = new Vector2(1, 0.6f);
        statusRect.offsetMin = new Vector2(10, 0);
        statusRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI statusText = statusObj.AddComponent<TextMeshProUGUI>();
        statusText.text = "Status: Ready";
        statusText.fontSize = 14;
        statusText.color = Color.yellow;
        statusText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // Quest Progress Slider
        GameObject progressObj = new GameObject("QuestProgress");
        progressObj.transform.SetParent(questPanel.transform, false);
        
        RectTransform progressRect = progressObj.AddComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0, 0.2f);
        progressRect.anchorMax = new Vector2(1, 0.4f);
        progressRect.offsetMin = new Vector2(10, 10);
        progressRect.offsetMax = new Vector2(-10, -10);
        
        Slider progressSlider = progressObj.AddComponent<Slider>();
        progressSlider.minValue = 0;
        progressSlider.maxValue = 1;
        progressSlider.value = 0;
        
        // Slider Background
        GameObject sliderBG = new GameObject("Background");
        sliderBG.transform.SetParent(progressObj.transform, false);
        RectTransform bgRect = sliderBG.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = sliderBG.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        progressSlider.targetGraphic = bgImage;
        
        // Slider Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(progressObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        progressSlider.fillRect = fillAreaRect;
        
        // Slider Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        progressSlider.fillRect = fillRect;
        
        // Progress Text
        GameObject progressTextObj = new GameObject("ProgressText");
        progressTextObj.transform.SetParent(questPanel.transform, false);
        
        RectTransform progressTextRect = progressTextObj.AddComponent<RectTransform>();
        progressTextRect.anchorMin = new Vector2(0, 0);
        progressTextRect.anchorMax = new Vector2(1, 0.2f);
        progressTextRect.offsetMin = new Vector2(10, 0);
        progressTextRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI progressText = progressTextObj.AddComponent<TextMeshProUGUI>();
        progressText.text = "0/3";
        progressText.fontSize = 12;
        progressText.color = Color.white;
        progressText.alignment = TextAlignmentOptions.Center;
        
        // Setup QuestUI component references
        QuestUI questUI = questPanel.GetComponent<QuestUI>();
        if (questUI != null)
        {
            questUI.questPanel = questPanel;
            questUI.questTitleText = titleText;
            questUI.questDescriptionText = descText;
            questUI.questStatusText = statusText;
            questUI.questProgressText = progressText;
            questUI.questProgressSlider = progressSlider;
            questUI.questProgressFill = fillImage;
        }
        
        return questPanel;
    }
    
    void CreateExampleQuest()
    {
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem == null) return;
        
        // Clear existing items
        questSystem.questItems.Clear();
        
        // Create quest items
        if (item1Prefab != null && item1SpawnPosition != null)
        {
            QuestItem item1 = new QuestItem();
            item1.itemName = "Bakul Nasi";
            item1.itemDescription = "A traditional rice basket with mystical properties";
            item1.questDescriptionForThisItem = "Find and collect the Bakul Nasi. Look for it near the traditional house.";
            item1.itemPrefab = item1Prefab;
            item1.spawnPosition = item1SpawnPosition;
            questSystem.questItems.Add(item1);
        }
        
        if (item2Prefab != null && item2SpawnPosition != null)
        {
            QuestItem item2 = new QuestItem();
            item2.itemName = "Mask";
            item2.itemDescription = "An ancient ceremonial mask";
            item2.questDescriptionForThisItem = "Now find the Mask. Search around the ceremonial area.";
            item2.itemPrefab = item2Prefab;
            item2.spawnPosition = item2SpawnPosition;
            questSystem.questItems.Add(item2);
        }
        
        if (item3Prefab != null && item3SpawnPosition != null)
        {
            QuestItem item3 = new QuestItem();
            item3.itemName = "Sacred Key";
            item3.itemDescription = "A sacred key that unlocks the final chamber";
            item3.questDescriptionForThisItem = "Finally, locate the Sacred Key. Check the temple chamber.";
            item3.itemPrefab = item3Prefab;
            item3.spawnPosition = item3SpawnPosition;
            questSystem.questItems.Add(item3);
        }
        
        Debug.Log($"Created example quest with {questSystem.questItems.Count} items");
    }
    
    void SetupAudioSources()
    {
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem == null) return;
        
        // Add AudioSource if not exists
        AudioSource audioSource = questSystem.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = questSystem.gameObject.AddComponent<AudioSource>();
        }
        
        // Configure audio source
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;
        
        Debug.Log("Audio setup completed for QuestSystem");
    }
    
    [ContextMenu("Clear Quest System")]
    public void ClearQuestSystem()
    {
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem != null)
        {
            if (Application.isPlaying)
            {
                questSystem.ResetQuest();
            }
            else
            {
                DestroyImmediate(questSystem.gameObject);
            }
        }
        
        QuestUI questUI = FindObjectOfType<QuestUI>();
        if (questUI != null)
        {
            DestroyImmediate(questUI.gameObject);
        }
        
        Debug.Log("Quest System cleared!");
    }
    
    [ContextMenu("Validate Quest Setup")]
    public void ValidateQuestSetup()
    {
        bool isValid = true;
        
        QuestSystem questSystem = FindObjectOfType<QuestSystem>();
        if (questSystem == null)
        {
            Debug.LogError("❌ QuestSystem not found in scene!");
            isValid = false;
        }
        else
        {
            Debug.Log("✅ QuestSystem found");
            
            if (questSystem.questItems.Count == 0)
            {
                Debug.LogWarning("⚠️ No quest items configured");
            }
            else
            {
                Debug.Log($"✅ {questSystem.questItems.Count} quest items configured");
            }
        }
        
        QuestUI questUI = FindObjectOfType<QuestUI>();
        if (questUI == null)
        {
            Debug.LogWarning("⚠️ QuestUI not found - UI will not be displayed");
        }
        else
        {
            Debug.Log("✅ QuestUI found");
        }
        
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogError("❌ InventorySystem not found - quest items won't be detected!");
            isValid = false;
        }
        else
        {
            Debug.Log("✅ InventorySystem found");
        }
        
        if (isValid)
        {
            Debug.Log("🎉 Quest System validation passed!");
        }
        else
        {
            Debug.LogError("💥 Quest System validation failed - check errors above");
        }
    }
}