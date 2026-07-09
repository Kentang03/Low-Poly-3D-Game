using UnityEngine;

/// <summary>
/// Setup wizard untuk mengintegrasikan Quest System dengan Win Panel dan Timer
/// </summary>
public class QuestWinSetupWizard : MonoBehaviour
{
    [Header("🎯 Quest Win Integration Setup")]
    [Space(10)]
    
    [Header("System Status")]
    [SerializeField] private bool questSystemReady = false;
    [SerializeField] private bool gameTimerReady = false;
    [SerializeField] private bool winPanelReady = false;
    [SerializeField] private bool integrationReady = false;
    
    [Header("Setup Options")]
    [Tooltip("Auto-create missing systems jika tidak ditemukan")]
    public bool autoCreateMissingSystems = true;
    
    [Tooltip("Auto-configure integration setelah setup")]
    public bool autoConfigureIntegration = true;
    
    [Header("Prefab References (Optional)")]
    public GameObject questSystemPrefab;
    public GameObject gameTimerPrefab;
    public GameObject winPanelPrefab;
    public GameObject questWinIntegrationPrefab;
    
    void Start()
    {
        ValidateAllSystems();
    }
    
    /// <summary>
    /// Complete setup untuk semua sistem
    /// </summary>
    [ContextMenu("🚀 Complete Quest Win Setup")]
    public void CompleteQuestWinSetup()
    {
        Debug.Log("=== Quest Win Integration Setup Starting ===");
        
        SetupQuestSystem();
        SetupGameTimer();
        SetupWinPanel();
        SetupQuestWinIntegration();
        
        if (autoConfigureIntegration)
        {
            ConfigureIntegration();
        }
        
        ValidateAllSystems();
        
        Debug.Log("=== Quest Win Integration Setup Complete! ===");
        
        if (questSystemReady && winPanelReady && integrationReady)
        {
            Debug.Log("🎉 Success! Quest completion akan menampilkan win panel dengan timer!");
        }
        else
        {
            Debug.LogWarning("⚠️ Setup tidak sempurna. Check console untuk details.");
        }
    }
    
    /// <summary>
    /// Setup QuestSystem
    /// </summary>
    [ContextMenu("📋 Setup Quest System")]
    public void SetupQuestSystem()
    {
        Debug.Log("Setting up Quest System...");
        
        QuestSystem existingQuest = FindObjectOfType<QuestSystem>();
        
        if (existingQuest == null && autoCreateMissingSystems)
        {
            GameObject questGO;
            
            if (questSystemPrefab != null)
            {
                questGO = Instantiate(questSystemPrefab);
                Debug.Log("✅ QuestSystem created from prefab");
            }
            else
            {
                questGO = new GameObject("QuestSystem");
                QuestSystem questSystem = questGO.AddComponent<QuestSystem>();
                
                // Default settings
                questSystem.questName = "Collect Sacred Items";
                questSystem.questDescription = "Collect all the sacred items to complete the quest";
                questSystem.useDynamicDescription = true;
                questSystem.initialQuestDescription = "Prepare to collect the sacred items";
                questSystem.completedQuestDescription = "Congratulations! All sacred items collected!";
                
                Debug.Log("✅ QuestSystem created manually");
            }
        }
        else if (existingQuest != null)
        {
            Debug.Log("✅ QuestSystem already exists");
        }
        else
        {
            Debug.LogWarning("⚠️ QuestSystem not found and autoCreateMissingSystems is disabled");
        }
        
        questSystemReady = FindObjectOfType<QuestSystem>() != null;
    }
    
    /// <summary>
    /// Setup GameTimer
    /// </summary>
    [ContextMenu("⏰ Setup Game Timer")]
    public void SetupGameTimer()
    {
        Debug.Log("Setting up Game Timer...");
        
        GameTimer existingTimer = FindObjectOfType<GameTimer>();
        
        if (existingTimer == null && autoCreateMissingSystems)
        {
            GameObject timerGO;
            
            if (gameTimerPrefab != null)
            {
                timerGO = Instantiate(gameTimerPrefab);
                Debug.Log("✅ GameTimer created from prefab");
            }
            else
            {
                timerGO = new GameObject("GameTimer");
                GameTimer timer = timerGO.AddComponent<GameTimer>();
                
                // Default settings
                timer.startTimerOnStart = true;
                timer.showMilliseconds = false;
                
                Debug.Log("✅ GameTimer created manually");
            }
        }
        else if (existingTimer != null)
        {
            Debug.Log("✅ GameTimer already exists");
        }
        else
        {
            Debug.LogWarning("⚠️ GameTimer not found and autoCreateMissingSystems is disabled");
        }
        
        gameTimerReady = FindObjectOfType<GameTimer>() != null;
    }
    
    /// <summary>
    /// Setup WinPanelManager
    /// </summary>
    [ContextMenu("🏆 Setup Win Panel")]
    public void SetupWinPanel()
    {
        Debug.Log("Setting up Win Panel...");
        
        WinPanelManager existingWinPanel = FindObjectOfType<WinPanelManager>();
        
        if (existingWinPanel == null && autoCreateMissingSystems)
        {
            if (winPanelPrefab != null)
            {
                GameObject winPanelGO = Instantiate(winPanelPrefab);
                Debug.Log("✅ WinPanelManager created from prefab");
            }
            else
            {
                Debug.LogWarning("⚠️ WinPanel prefab not assigned - manual creation needed for UI components");
            }
        }
        else if (existingWinPanel != null)
        {
            Debug.Log("✅ WinPanelManager already exists");
        }
        else
        {
            Debug.LogWarning("⚠️ WinPanelManager not found and autoCreateMissingSystems is disabled");
        }
        
        winPanelReady = FindObjectOfType<WinPanelManager>() != null;
    }
    
    /// <summary>
    /// Setup QuestWinIntegration
    /// </summary>
    [ContextMenu("🔗 Setup Quest Win Integration")]
    public void SetupQuestWinIntegration()
    {
        Debug.Log("Setting up Quest Win Integration...");
        
        QuestWinIntegration existingIntegration = FindObjectOfType<QuestWinIntegration>();
        
        if (existingIntegration == null && autoCreateMissingSystems)
        {
            GameObject integrationGO;
            
            if (questWinIntegrationPrefab != null)
            {
                integrationGO = Instantiate(questWinIntegrationPrefab);
                Debug.Log("✅ QuestWinIntegration created from prefab");
            }
            else
            {
                integrationGO = new GameObject("QuestWinIntegration");
                QuestWinIntegration integration = integrationGO.AddComponent<QuestWinIntegration>();
                
                // Default settings
                integration.autoSetupIntegration = true;
                integration.winPanelDelay = 1.0f;
                integration.enableDebugLogs = true;
                
                Debug.Log("✅ QuestWinIntegration created manually");
            }
        }
        else if (existingIntegration != null)
        {
            Debug.Log("✅ QuestWinIntegration already exists");
        }
        else
        {
            Debug.LogWarning("⚠️ QuestWinIntegration not found and autoCreateMissingSystems is disabled");
        }
        
        integrationReady = FindObjectOfType<QuestWinIntegration>() != null;
    }
    
    /// <summary>
    /// Configure integration antar sistem
    /// </summary>
    [ContextMenu("⚙️ Configure Integration")]
    public void ConfigureIntegration()
    {
        Debug.Log("Configuring system integration...");
        
        QuestWinIntegration integration = FindObjectOfType<QuestWinIntegration>();
        
        if (integration != null)
        {
            integration.SetupIntegration();
            Debug.Log("✅ Integration configured");
        }
        else
        {
            Debug.LogError("❌ QuestWinIntegration not found for configuration");
        }
    }
    
    /// <summary>
    /// Validate semua sistem
    /// </summary>
    [ContextMenu("🔍 Validate All Systems")]
    public void ValidateAllSystems()
    {
        Debug.Log("Validating all systems...");
        
        // Check individual systems
        questSystemReady = FindObjectOfType<QuestSystem>() != null;
        gameTimerReady = FindObjectOfType<GameTimer>() != null;
        winPanelReady = FindObjectOfType<WinPanelManager>() != null;
        integrationReady = FindObjectOfType<QuestWinIntegration>() != null;
        
        // Log status
        LogSystemStatus("QuestSystem", questSystemReady);
        LogSystemStatus("GameTimer", gameTimerReady);
        LogSystemStatus("WinPanelManager", winPanelReady);
        LogSystemStatus("QuestWinIntegration", integrationReady);
        
        // Overall validation
        bool allReady = questSystemReady && winPanelReady && integrationReady;
        
        if (allReady)
        {
            Debug.Log("🎉 All systems ready! Quest completion akan trigger win panel dengan timer.");
        }
        else
        {
            Debug.LogWarning("⚠️ Some systems missing. Run 'Complete Quest Win Setup' untuk auto-setup.");
        }
        
        // Validate integration
        if (integrationReady)
        {
            QuestWinIntegration integration = FindObjectOfType<QuestWinIntegration>();
            bool integrationValid = integration.ValidateIntegration();
            
            if (!integrationValid)
            {
                Debug.LogWarning("⚠️ Integration validation failed. Check QuestWinIntegration logs.");
            }
        }
    }
    
    void LogSystemStatus(string systemName, bool isReady)
    {
        if (isReady)
            Debug.Log($"✅ {systemName}: Ready");
        else
            Debug.LogWarning($"❌ {systemName}: Missing");
    }
    
    /// <summary>
    /// Test quest completion flow
    /// </summary>
    [ContextMenu("🧪 Test Quest Completion")]
    public void TestQuestCompletion()
    {
        Debug.Log("Testing quest completion flow...");
        
        QuestWinIntegration integration = FindObjectOfType<QuestWinIntegration>();
        
        if (integration != null)
        {
            integration.TestQuestComplete();
        }
        else
        {
            Debug.LogError("❌ Cannot test - QuestWinIntegration not found!");
        }
    }
    
    /// <summary>
    /// Quick setup untuk development
    /// </summary>
    [ContextMenu("⚡ Quick Development Setup")]
    public void QuickDevelopmentSetup()
    {
        Debug.Log("🚀 Running Quick Development Setup...");
        
        autoCreateMissingSystems = true;
        autoConfigureIntegration = true;
        
        CompleteQuestWinSetup();
        
        // Additional development settings
        QuestWinIntegration integration = FindObjectOfType<QuestWinIntegration>();
        if (integration != null)
        {
            integration.winPanelDelay = 0.5f; // Shorter delay untuk testing
            integration.enableDebugLogs = true; // Enable debug logs
        }
        
        Debug.Log("⚡ Quick setup complete! Ready untuk development dan testing.");
    }
}