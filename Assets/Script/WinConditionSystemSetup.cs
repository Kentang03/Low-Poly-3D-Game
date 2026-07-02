using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Setup Wizard untuk Win Condition System dengan Timer dan Win Panel
/// Membantu setup komponen yang dibutuhkan untuk sistem win condition
/// </summary>
public class WinConditionSystemSetup : MonoBehaviour
{
    [Header("Auto Setup Settings")]
    public bool autoSetupOnStart = false;
    
    [Header("System Status")]
    [SerializeField] private bool gameTimerReady = false;
    [SerializeField] private bool gameTimerUIReady = false;
    [SerializeField] private bool winPanelReady = false;
    [SerializeField] private bool itemCollectionReady = false;
    
    [Header("Prefab References (Optional)")]
    public GameObject gameTimerPrefab;
    public GameObject gameTimerUIPrefab;
    public GameObject winPanelPrefab;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCompleteWinConditionSystem();
        }
    }
    
    /// <summary>
    /// Setup complete win condition system
    /// </summary>
    [ContextMenu("🎯 Setup Complete Win Condition System")]
    public void SetupCompleteWinConditionSystem()
    {
        Debug.Log("=== Win Condition System Setup Starting ===");
        
        SetupGameTimer();
        SetupGameTimerUI();
        SetupWinPanel();
        CheckItemCollectionManager();
        
        ValidateSystemSetup();
        PrintSetupSummary();
        
        Debug.Log("=== Win Condition System Setup Complete! ===");
    }
    
    /// <summary>
    /// Setup GameTimer component
    /// </summary>
    [ContextMenu("⏱️ Setup Game Timer")]
    public void SetupGameTimer()
    {
        Debug.Log("Setting up Game Timer...");
        
        GameTimer existingTimer = FindObjectOfType<GameTimer>();
        
        if (existingTimer == null)
        {
            GameObject timerGO = null;
            
            // Try to use prefab first
            if (gameTimerPrefab != null)
            {
                timerGO = Instantiate(gameTimerPrefab);
                Debug.Log("✅ GameTimer created from prefab");
            }
            else
            {
                // Create manually
                timerGO = new GameObject("GameTimer");
                GameTimer timer = timerGO.AddComponent<GameTimer>();
                
                // Default settings
                timer.startTimerOnStart = true;
                timer.showMilliseconds = false;
                
                Debug.Log("✅ GameTimer created manually");
            }
            
            // Make persistent across scenes if needed
            DontDestroyOnLoad(timerGO);
        }
        else
        {
            Debug.Log("✅ GameTimer already exists");
        }
        
        gameTimerReady = true;
    }
    
    /// <summary>
    /// Setup GameTimer UI
    /// </summary>
    [ContextMenu("📱 Setup Game Timer UI")]
    public void SetupGameTimerUI()
    {
        Debug.Log("Setting up Game Timer UI...");
        
        GameTimerUI existingTimerUI = FindObjectOfType<GameTimerUI>();
        
        if (existingTimerUI == null)
        {
            Canvas gameCanvas = FindGameCanvas();
            
            if (gameCanvas != null)
            {
                GameObject timerUIGO = null;
                
                // Try to use prefab first
                if (gameTimerUIPrefab != null)
                {
                    timerUIGO = Instantiate(gameTimerUIPrefab, gameCanvas.transform);
                    Debug.Log("✅ GameTimerUI created from prefab");
                }
                else
                {
                    // Create manually
                    timerUIGO = CreateGameTimerUI(gameCanvas);
                    Debug.Log("✅ GameTimerUI created manually");
                }
            }
            else
            {
                Debug.LogError("❌ No Canvas found! Create a Canvas first.");
                return;
            }
        }
        else
        {
            Debug.Log("✅ GameTimerUI already exists");
        }
        
        gameTimerUIReady = true;
    }
    
    /// <summary>
    /// Setup Win Panel
    /// </summary>
    [ContextMenu("🏆 Setup Win Panel")]
    public void SetupWinPanel()
    {
        Debug.Log("Setting up Win Panel...");
        
        WinPanelManager existingWinPanel = FindObjectOfType<WinPanelManager>();
        
        if (existingWinPanel == null)
        {
            Canvas gameCanvas = FindGameCanvas();
            
            if (gameCanvas != null)
            {
                GameObject winPanelGO = null;
                
                // Try to use prefab first
                if (winPanelPrefab != null)
                {
                    winPanelGO = Instantiate(winPanelPrefab, gameCanvas.transform);
                    Debug.Log("✅ WinPanel created from prefab");
                }
                else
                {
                    // Create manually
                    winPanelGO = CreateWinPanel(gameCanvas);
                    Debug.Log("✅ WinPanel created manually");
                }
            }
            else
            {
                Debug.LogError("❌ No Canvas found! Create a Canvas first.");
                return;
            }
        }
        else
        {
            Debug.Log("✅ WinPanelManager already exists");
        }
        
        winPanelReady = true;
    }
    
    /// <summary>
    /// Check if ItemCollectionManager exists
    /// </summary>
    void CheckItemCollectionManager()
    {
        ItemCollectionManager existing = ItemCollectionManager.Instance;
        
        if (existing != null)
        {
            Debug.Log("✅ ItemCollectionManager found");
            itemCollectionReady = true;
        }
        else
        {
            Debug.LogWarning("⚠️ ItemCollectionManager not found! This is needed for win condition.");
            Debug.LogWarning("  Please add ItemCollectionManager to your scene.");
            itemCollectionReady = false;
        }
    }
    
    /// <summary>
    /// Find or create game canvas
    /// </summary>
    Canvas FindGameCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        
        // Look for main game canvas
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.name.ToLower().Contains("game") || 
                canvas.gameObject.name.ToLower().Contains("main") ||
                canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return canvas;
            }
        }
        
        // If no suitable canvas found, create one
        if (canvases.Length > 0)
        {
            return canvases[0]; // Use first canvas
        }
        else
        {
            return CreateGameCanvas();
        }
    }
    
    /// <summary>
    /// Create game canvas if none exists
    /// </summary>
    Canvas CreateGameCanvas()
    {
        GameObject canvasGO = new GameObject("GameCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ Created new GameCanvas");
        return canvas;
    }
    
    /// <summary>
    /// Create GameTimer UI manually
    /// </summary>
    GameObject CreateGameTimerUI(Canvas parentCanvas)
    {
        // Create timer container
        GameObject timerGO = new GameObject("GameTimerUI");
        timerGO.transform.SetParent(parentCanvas.transform, false);
        
        // Add RectTransform
        RectTransform rectTransform = timerGO.AddComponent<RectTransform>();
        
        // Create timer text
        GameObject textGO = new GameObject("TimerText");
        textGO.transform.SetParent(timerGO.transform, false);
        
        TextMeshProUGUI timerText = textGO.AddComponent<TextMeshProUGUI>();
        timerText.text = "Time: 00:00";
        timerText.fontSize = 24;
        timerText.color = Color.white;
        timerText.alignment = TextAlignmentOptions.Center;
        
        // Setup RectTransform for text
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Add GameTimerUI component
        GameTimerUI timerUI = timerGO.AddComponent<GameTimerUI>();
        timerUI.timerText = timerText;
        timerUI.timerPanel = timerGO;
        
        // Position at top right
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-100f, -50f);
        rectTransform.sizeDelta = new Vector2(200f, 50f);
        
        return timerGO;
    }
    
    /// <summary>
    /// Create Win Panel manually
    /// </summary>
    GameObject CreateWinPanel(Canvas parentCanvas)
    {
        // Create main panel
        GameObject panelGO = new GameObject("WinPanel");
        panelGO.transform.SetParent(parentCanvas.transform, false);
        
        // Add background image
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);
        
        // Setup RectTransform (fullscreen)
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create content container
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(panelGO.transform, false);
        
        RectTransform contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(600f, 400f);
        
        // Add background to content
        Image contentImage = contentGO.AddComponent<Image>();
        contentImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Create UI elements
        TextMeshProUGUI victoryTitle = CreateTextElement(contentGO, "VictoryTitle", "Victory!", 36, new Vector2(0f, 120f));
        TextMeshProUGUI itemsText = CreateTextElement(contentGO, "ItemsText", "Items: 0/0", 20, new Vector2(0f, 60f));
        TextMeshProUGUI timeText = CreateTextElement(contentGO, "TimeText", "Time: 00:00", 20, new Vector2(0f, 30f));
        TextMeshProUGUI congratsText = CreateTextElement(contentGO, "CongratsText", "Congratulations!", 18, new Vector2(0f, -20f));
        
        // Create buttons
        Button playAgainBtn = CreateButton(contentGO, "PlayAgainButton", "Play Again", new Vector2(-80f, -80f));
        Button mainMenuBtn = CreateButton(contentGO, "MainMenuButton", "Main Menu", new Vector2(80f, -80f));
        
        // Add WinPanelManager component
        WinPanelManager winPanelManager = panelGO.AddComponent<WinPanelManager>();
        winPanelManager.winPanel = panelGO;
        winPanelManager.victoryTitle = victoryTitle;
        winPanelManager.itemsCollectedText = itemsText;
        winPanelManager.timeCompletedText = timeText;
        winPanelManager.congratulationsText = congratsText;
        winPanelManager.playAgainButton = playAgainBtn;
        winPanelManager.mainMenuButton = mainMenuBtn;
        
        return panelGO;
    }
    
    TextMeshProUGUI CreateTextElement(GameObject parent, string name, string text, float fontSize, Vector2 position)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI textComp = textGO.AddComponent<TextMeshProUGUI>();
        textComp.text = text;
        textComp.fontSize = fontSize;
        textComp.color = Color.white;
        textComp.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400f, fontSize + 10f);
        
        return textComp;
    }
    
    Button CreateButton(GameObject parent, string name, string text, Vector2 position)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent.transform, false);
        
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Create button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Setup transforms
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(120f, 40f);
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    /// <summary>
    /// Validate system setup
    /// </summary>
    void ValidateSystemSetup()
    {
        Debug.Log("=== Validating Win Condition System ===");
        
        bool allValid = true;
        
        if (gameTimerReady)
            Debug.Log("✅ Game Timer: Ready");
        else
        {
            Debug.LogError("❌ Game Timer: Not Ready");
            allValid = false;
        }
        
        if (gameTimerUIReady)
            Debug.Log("✅ Game Timer UI: Ready");
        else
        {
            Debug.LogError("❌ Game Timer UI: Not Ready");
            allValid = false;
        }
        
        if (winPanelReady)
            Debug.Log("✅ Win Panel: Ready");
        else
        {
            Debug.LogError("❌ Win Panel: Not Ready");
            allValid = false;
        }
        
        if (itemCollectionReady)
            Debug.Log("✅ Item Collection Manager: Ready");
        else
        {
            Debug.LogWarning("⚠️ Item Collection Manager: Missing (Required for win condition)");
            allValid = false;
        }
        
        if (allValid)
        {
            Debug.Log("🎉 Win Condition System validated successfully!");
        }
        else
        {
            Debug.LogWarning("⚠️ Win Condition System setup incomplete. Check errors above.");
        }
    }
    
    /// <summary>
    /// Print setup summary
    /// </summary>
    void PrintSetupSummary()
    {
        Debug.Log("=== Win Condition System Summary ===");
        Debug.Log($"✅ Game Timer: {(gameTimerReady ? "Ready" : "Missing")}");
        Debug.Log($"✅ Timer UI: {(gameTimerUIReady ? "Ready" : "Missing")}");
        Debug.Log($"✅ Win Panel: {(winPanelReady ? "Ready" : "Missing")}");
        Debug.Log($"✅ Item Collection: {(itemCollectionReady ? "Ready" : "Missing")}");
        
        if (IsSystemReady())
        {
            Debug.Log("🚀 SYSTEM READY FOR USE! 🚀");
            Debug.Log("The win condition system is now active and will:");
            Debug.Log("- Track game time from start");
            Debug.Log("- Show timer on screen");
            Debug.Log("- Display win panel when items collected");
            Debug.Log("- Allow restart and return to main menu");
        }
        else
        {
            Debug.Log("⚠️ System setup incomplete. Please fix missing components.");
        }
    }
    
    /// <summary>
    /// Check if entire system is ready
    /// </summary>
    public bool IsSystemReady()
    {
        return gameTimerReady && gameTimerUIReady && winPanelReady && itemCollectionReady;
    }
    
    // Quick test methods
    [ContextMenu("🧪 Test Win Condition")]
    public void TestWinCondition()
    {
        if (ItemCollectionManager.Instance != null)
        {
            // Force complete the collection task
            ItemCollectionManager.Instance.OnTaskCompleted?.Invoke();
            Debug.Log("🧪 Triggered win condition for testing");
        }
        else
        {
            Debug.LogError("❌ Cannot test: ItemCollectionManager not found!");
        }
    }
    
    [ContextMenu("📊 System Status")]
    public void CheckSystemStatus()
    {
        Debug.Log("=== Current System Status ===");
        Debug.Log($"Game Timer: {(FindObjectOfType<GameTimer>() != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Timer UI: {(FindObjectOfType<GameTimerUI>() != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Win Panel: {(FindObjectOfType<WinPanelManager>() != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Item Collection: {(FindObjectOfType<ItemCollectionManager>() != null ? "✅ Found" : "❌ Missing")}");
    }
}