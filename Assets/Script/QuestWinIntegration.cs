using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mengelola integrasi antara QuestSystem, GameTimer, dan WinPanelManager
/// untuk menampilkan win panel dengan timer ketika quest selesai
/// </summary>
public class QuestWinIntegration : MonoBehaviour
{
    [Header("Integration Settings")]
    [Tooltip("Auto-setup integration saat Start()")]
    public bool autoSetupIntegration = true;
    
    [Tooltip("Delay sebelum menampilkan win panel (untuk cutscene, dll)")]
    public float winPanelDelay = 1.0f;
    
    [Header("Quest Completion Events")]
    [Tooltip("Event yang dipicu ketika quest selesai (sebelum win panel)")]
    public UnityEvent OnQuestCompletedBeforeWin;
    
    [Tooltip("Event yang dipicu setelah win panel ditampilkan")]
    public UnityEvent OnWinPanelShown;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    // References
    private QuestSystem questSystem;
    private GameTimer gameTimer;
    private WinPanelManager winPanelManager;
    
    // Static instance
    private static QuestWinIntegration instance;
    public static QuestWinIntegration Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<QuestWinIntegration>();
            return instance;
        }
    }
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        if (autoSetupIntegration)
        {
            SetupIntegration();
        }
    }
    
    /// <summary>
    /// Setup integration antara quest system dan win panel
    /// </summary>
    [ContextMenu("Setup Quest-Win Integration")]
    public void SetupIntegration()
    {
        if (enableDebugLogs)
            Debug.Log("QuestWinIntegration: Setting up integration...");
        
        // Find system references
        FindSystemReferences();
        
        // Subscribe to quest completion
        if (questSystem != null)
        {
            questSystem.OnQuestCompleted.AddListener(OnQuestCompleted);
            
            if (enableDebugLogs)
                Debug.Log("✅ QuestWinIntegration: Subscribed to QuestSystem.OnQuestCompleted");
        }
        else
        {
            Debug.LogWarning("⚠️ QuestWinIntegration: QuestSystem tidak ditemukan!");
        }
        
        // Validate other systems
        if (gameTimer == null)
        {
            Debug.LogWarning("⚠️ QuestWinIntegration: GameTimer tidak ditemukan - timer tidak akan ditampilkan di win panel");
        }
        
        if (winPanelManager == null)
        {
            Debug.LogWarning("⚠️ QuestWinIntegration: WinPanelManager tidak ditemukan - win panel tidak akan muncul");
        }
        
        if (enableDebugLogs)
            Debug.Log("🎯 QuestWinIntegration: Setup complete!");
    }
    
    void FindSystemReferences()
    {
        // Find QuestSystem
        if (questSystem == null)
        {
            questSystem = QuestSystem.Instance;
            if (questSystem == null)
                questSystem = FindObjectOfType<QuestSystem>();
        }
        
        // Find GameTimer
        if (gameTimer == null)
        {
            gameTimer = GameTimer.Instance;
            if (gameTimer == null)
                gameTimer = FindObjectOfType<GameTimer>();
        }
        
        // Find WinPanelManager
        if (winPanelManager == null)
        {
            winPanelManager = WinPanelManager.Instance;
            if (winPanelManager == null)
                winPanelManager = FindObjectOfType<WinPanelManager>();
        }
    }
    
    /// <summary>
    /// Handler untuk quest completion
    /// </summary>
    void OnQuestCompleted()
    {
        if (enableDebugLogs)
            Debug.Log("🎉 QuestWinIntegration: Quest completed! Preparing win panel...");
        
        // Stop game timer jika ada
        if (gameTimer != null && gameTimer.IsTimerRunning)
        {
            gameTimer.CompleteGame();
            
            if (enableDebugLogs)
                Debug.Log($"⏰ QuestWinIntegration: Game timer stopped at {gameTimer.GetFormattedTime()}");
        }
        
        // Trigger pre-win events
        OnQuestCompletedBeforeWin?.Invoke();
        
        // Show win panel dengan delay
        if (winPanelDelay > 0)
        {
            Invoke(nameof(ShowWinPanelDelayed), winPanelDelay);
        }
        else
        {
            ShowWinPanelDelayed();
        }
    }
    
    void ShowWinPanelDelayed()
    {
        if (winPanelManager != null)
        {
            winPanelManager.ShowWinPanel();
            OnWinPanelShown?.Invoke();
            
            if (enableDebugLogs)
                Debug.Log("🏆 QuestWinIntegration: Win panel displayed!");
        }
        else
        {
            Debug.LogError("❌ QuestWinIntegration: WinPanelManager tidak tersedia untuk menampilkan win panel!");
        }
    }
    
    /// <summary>
    /// Manual trigger untuk testing
    /// </summary>
    [ContextMenu("Test Quest Complete")]
    public void TestQuestComplete()
    {
        if (enableDebugLogs)
            Debug.Log("🧪 QuestWinIntegration: Testing quest completion...");
        
        OnQuestCompleted();
    }
    
    /// <summary>
    /// Get statistics untuk win panel
    /// </summary>
    public QuestCompletionData GetQuestCompletionData()
    {
        QuestCompletionData data = new QuestCompletionData();
        
        // Quest data
        if (questSystem != null)
        {
            data.questName = questSystem.questName;
            data.totalItems = questSystem.questItems.Count;
            data.collectedItems = questSystem.questItems.Count; // Semua item jika quest complete
            data.questDescription = questSystem.GetCurrentQuestDescription();
        }
        
        // Timer data
        if (gameTimer != null)
        {
            data.completionTime = gameTimer.CurrentTime;
            data.formattedTime = gameTimer.GetFormattedTime();
            data.timeForStats = gameTimer.GetTimeForStats();
        }
        
        return data;
    }
    
    /// <summary>
    /// Cleanup subscriptions
    /// </summary>
    void OnDestroy()
    {
        if (questSystem != null)
        {
            questSystem.OnQuestCompleted.RemoveListener(OnQuestCompleted);
        }
    }
    
    // Validation methods untuk setup wizard
    public bool ValidateIntegration()
    {
        FindSystemReferences();
        
        bool isValid = true;
        
        if (questSystem == null)
        {
            Debug.LogError("❌ QuestSystem tidak ditemukan!");
            isValid = false;
        }
        
        if (winPanelManager == null)
        {
            Debug.LogError("❌ WinPanelManager tidak ditemukan!");
            isValid = false;
        }
        
        if (gameTimer == null)
        {
            Debug.LogWarning("⚠️ GameTimer tidak ditemukan - timer tidak akan ditampilkan");
        }
        
        return isValid;
    }
}

/// <summary>
/// Data struktur untuk quest completion statistics
/// </summary>
[System.Serializable]
public class QuestCompletionData
{
    public string questName = "Quest";
    public int totalItems = 0;
    public int collectedItems = 0;
    public string questDescription = "";
    
    public float completionTime = 0f;
    public string formattedTime = "00:00";
    public string timeForStats = "0s";
}