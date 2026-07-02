using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug UI untuk monitoring dan testing sistem Kancil Guide
/// </summary>
public class KancilDebugUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas debugCanvas;
    [SerializeField] private Text statusText;
    [SerializeField] private Text checkpointText;
    [SerializeField] private Text playerInfoText;
    [SerializeField] private Text performanceText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Toggle loopToggle;
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button nextCheckpointButton;
    [SerializeField] private Button resetButton;
    
    [Header("Settings")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private bool enablePerformanceMonitoring = true;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;
    
    // References
    private KancilGuide kancilGuide;
    private KancilGuideManager guideManager;
    
    // Performance monitoring
    private float lastUpdateTime;
    private int frameCount;
    private float avgFrameTime;
    
    // State tracking
    private KancilGuide.KancilState lastState;
    private int lastCheckpointIndex;
    private bool lastPlayerNearby;
    
    private void Start()
    {
        InitializeDebugUI();
        SetupUI();
        
        // Find references
        kancilGuide = FindObjectOfType<KancilGuide>();
        guideManager = FindObjectOfType<KancilGuideManager>();
        
        if (kancilGuide != null)
        {
            kancilGuide.OnStateChanged += OnKancilStateChanged;
            kancilGuide.OnCheckpointReached += OnCheckpointReached;
        }
    }
    
    private void Update()
    {
        HandleInput();
        
        if (showDebugUI && Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateDebugInfo();
            lastUpdateTime = Time.time;
        }
        
        UpdatePerformanceStats();
    }
    
    private void InitializeDebugUI()
    {
        if (debugCanvas != null)
        {
            debugCanvas.gameObject.SetActive(showDebugUI);
        }
        
        // Auto-create UI jika tidak ada
        if (debugCanvas == null && showDebugUI)
        {
            CreateDebugUI();
        }
    }
    
    private void CreateDebugUI()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("KancilDebugCanvas");
        debugCanvas = canvasObj.AddComponent<Canvas>();
        debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        debugCanvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create panel
        GameObject panelObj = new GameObject("DebugPanel");
        panelObj.transform.SetParent(debugCanvas.transform, false);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.7f);
        panelRect.anchorMax = new Vector2(0.3f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        // Create text elements
        CreateDebugTextElements(panelObj);
        CreateDebugControls(panelObj);
        
        Debug.Log("Auto-created Kancil Debug UI");
    }
    
    private void CreateDebugTextElements(GameObject parent)
    {
        // Status text
        statusText = CreateTextElement(parent, "StatusText", "Status: Initializing...", 
            new Vector2(10, -10), new Vector2(-20, 60));
        
        // Checkpoint text
        checkpointText = CreateTextElement(parent, "CheckpointText", "Checkpoint: 0/0",
            new Vector2(10, -70), new Vector2(-20, 40));
        
        // Player info text
        playerInfoText = CreateTextElement(parent, "PlayerInfoText", "Player: Not Found",
            new Vector2(10, -110), new Vector2(-20, 40));
        
        // Performance text
        performanceText = CreateTextElement(parent, "PerformanceText", "FPS: --",
            new Vector2(10, -150), new Vector2(-20, 40));
    }
    
    private Text CreateTextElement(GameObject parent, string name, string text, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.anchoredPosition = anchoredPos;
        textRect.sizeDelta = sizeDelta;
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.fontSize = 14;
        textComp.color = Color.white;
        
        return textComp;
    }
    
    private void CreateDebugControls(GameObject parent)
    {
        // Start button
        startButton = CreateButton(parent, "StartButton", "Start", new Vector2(10, -200), new Vector2(80, 30));
        startButton.onClick.AddListener(StartGuiding);
        
        // Stop button
        stopButton = CreateButton(parent, "StopButton", "Stop", new Vector2(100, -200), new Vector2(80, 30));
        stopButton.onClick.AddListener(StopGuiding);
        
        // Next checkpoint button
        nextCheckpointButton = CreateButton(parent, "NextButton", "Next CP", new Vector2(190, -200), new Vector2(80, 30));
        nextCheckpointButton.onClick.AddListener(NextCheckpoint);
        
        // Reset button
        resetButton = CreateButton(parent, "ResetButton", "Reset", new Vector2(10, -240), new Vector2(80, 30));
        resetButton.onClick.AddListener(ResetKancil);
        
        // Speed slider
        GameObject sliderObj = new GameObject("SpeedSlider");
        sliderObj.transform.SetParent(parent.transform, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0, 1);
        sliderRect.anchorMax = new Vector2(1, 1);
        sliderRect.anchoredPosition = new Vector2(10, -280);
        sliderRect.sizeDelta = new Vector2(-20, 20);
        
        speedSlider = sliderObj.AddComponent<Slider>();
        speedSlider.minValue = 0.5f;
        speedSlider.maxValue = 10f;
        speedSlider.value = 3f;
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        
        // Loop toggle
        GameObject toggleObj = new GameObject("LoopToggle");
        toggleObj.transform.SetParent(parent.transform, false);
        
        RectTransform toggleRect = toggleObj.AddComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(0, 1);
        toggleRect.anchorMax = new Vector2(1, 1);
        toggleRect.anchoredPosition = new Vector2(10, -310);
        toggleRect.sizeDelta = new Vector2(-20, 20);
        
        loopToggle = toggleObj.AddComponent<Toggle>();
        loopToggle.isOn = true;
        loopToggle.onValueChanged.AddListener(OnLoopChanged);
        
        // Add toggle label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleObj.transform, false);
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "Loop Route";
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 12;
        labelText.color = Color.white;
        
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(20, 0);
        labelRect.offsetMax = Vector2.zero;
    }
    
    private Button CreateButton(GameObject parent, string name, string text, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 1);
        buttonRect.anchorMax = new Vector2(0, 1);
        buttonRect.anchoredPosition = anchoredPos;
        buttonRect.sizeDelta = sizeDelta;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.5f, 0.8f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 12;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private void SetupUI()
    {
        // Setup initial UI state
        if (speedSlider != null && kancilGuide != null)
        {
            UnityEngine.AI.NavMeshAgent agent = kancilGuide.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                speedSlider.value = agent.speed;
            }
        }
    }
    
    private void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleDebugUI();
        }
        
        // Keyboard shortcuts
        if (showDebugUI)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    StartGuiding();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    StopGuiding();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    NextCheckpoint();
                }
            }
        }
    }
    
    private void UpdateDebugInfo()
    {
        if (kancilGuide == null) return;
        
        // Update status
        if (statusText != null)
        {
            KancilGuide.KancilState currentState = kancilGuide.GetCurrentState();
            string status = $"State: {currentState}";
            
            if (currentState != lastState)
            {
                status += " ← CHANGED";
                lastState = currentState;
            }
            
            statusText.text = status;
        }
        
        // Update checkpoint info
        if (checkpointText != null)
        {
            int currentIndex = kancilGuide.GetCurrentCheckpointIndex();
            int totalCheckpoints = kancilGuide.GetTotalCheckpoints();
            
            string checkpointInfo = $"Checkpoint: {currentIndex + 1}/{totalCheckpoints}";
            
            if (currentIndex != lastCheckpointIndex)
            {
                checkpointInfo += " ← NEW";
                lastCheckpointIndex = currentIndex;
            }
            
            checkpointText.text = checkpointInfo;
        }
        
        // Update player info
        if (playerInfoText != null)
        {
            bool playerNearby = kancilGuide.IsPlayerNearby();
            string playerInfo = $"Player Nearby: {(playerNearby ? "Yes" : "No")}";
            
            if (playerNearby != lastPlayerNearby)
            {
                playerInfo += " ← CHANGED";
                lastPlayerNearby = playerNearby;
            }
            
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && kancilGuide.transform != null)
            {
                float distance = Vector3.Distance(player.transform.position, kancilGuide.transform.position);
                playerInfo += $"\nDistance: {distance:F1}m";
            }
            
            playerInfoText.text = playerInfo;
        }
        
        // Update performance
        if (performanceText != null && enablePerformanceMonitoring)
        {
            string perfInfo = $"FPS: {(1f / avgFrameTime):F0}";
            perfInfo += $"\nFrame Time: {(avgFrameTime * 1000):F1}ms";
            performanceText.text = perfInfo;
        }
        
        // Update button states
        UpdateButtonStates();
    }
    
    private void UpdateButtonStates()
    {
        if (kancilGuide == null) return;
        
        bool isGuiding = kancilGuide.GetCurrentState() != KancilGuide.KancilState.Idle;
        
        if (startButton != null)
        {
            startButton.interactable = !isGuiding && kancilGuide.GetTotalCheckpoints() > 0;
        }
        
        if (stopButton != null)
        {
            stopButton.interactable = isGuiding;
        }
        
        if (nextCheckpointButton != null)
        {
            nextCheckpointButton.interactable = kancilGuide.GetTotalCheckpoints() > 0;
        }
    }
    
    private void UpdatePerformanceStats()
    {
        frameCount++;
        
        if (frameCount % 10 == 0) // Update every 10 frames
        {
            avgFrameTime = Time.deltaTime;
        }
    }
    
    // UI Event Handlers
    public void ToggleDebugUI()
    {
        showDebugUI = !showDebugUI;
        if (debugCanvas != null)
        {
            debugCanvas.gameObject.SetActive(showDebugUI);
        }
    }
    
    public void StartGuiding()
    {
        if (kancilGuide != null)
        {
            kancilGuide.StartGuiding();
            Debug.Log("Debug UI: Started guiding");
        }
    }
    
    public void StopGuiding()
    {
        if (kancilGuide != null)
        {
            kancilGuide.StopGuiding();
            Debug.Log("Debug UI: Stopped guiding");
        }
    }
    
    public void NextCheckpoint()
    {
        if (kancilGuide != null)
        {
            int nextIndex = kancilGuide.GetCurrentCheckpointIndex() + 1;
            if (nextIndex < kancilGuide.GetTotalCheckpoints())
            {
                kancilGuide.JumpToCheckpoint(nextIndex);
                Debug.Log($"Debug UI: Jumped to checkpoint {nextIndex + 1}");
            }
        }
    }
    
    public void ResetKancil()
    {
        if (kancilGuide != null)
        {
            kancilGuide.StopGuiding();
            kancilGuide.JumpToCheckpoint(0);
            Debug.Log("Debug UI: Reset kancil to first checkpoint");
        }
    }
    
    public void OnSpeedChanged(float newSpeed)
    {
        if (kancilGuide != null)
        {
            UnityEngine.AI.NavMeshAgent agent = kancilGuide.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = newSpeed;
                Debug.Log($"Debug UI: Changed speed to {newSpeed:F1}");
            }
        }
    }
    
    public void OnLoopChanged(bool loop)
    {
        // This would need to be implemented in KancilGuide
        Debug.Log($"Debug UI: Loop setting changed to {loop}");
    }
    
    // Event Handlers
    private void OnKancilStateChanged(KancilGuide.KancilState newState)
    {
        Debug.Log($"Debug UI: Kancil state changed to {newState}");
    }
    
    private void OnCheckpointReached(int checkpointIndex)
    {
        Debug.Log($"Debug UI: Checkpoint {checkpointIndex + 1} reached");
    }
    
    private void OnDestroy()
    {
        // Clean up events
        if (kancilGuide != null)
        {
            kancilGuide.OnStateChanged -= OnKancilStateChanged;
            kancilGuide.OnCheckpointReached -= OnCheckpointReached;
        }
    }
    
    // Public API untuk external control
    public void SetKancilGuide(KancilGuide guide)
    {
        if (kancilGuide != null)
        {
            kancilGuide.OnStateChanged -= OnKancilStateChanged;
            kancilGuide.OnCheckpointReached -= OnCheckpointReached;
        }
        
        kancilGuide = guide;
        
        if (kancilGuide != null)
        {
            kancilGuide.OnStateChanged += OnKancilStateChanged;
            kancilGuide.OnCheckpointReached += OnCheckpointReached;
        }
    }
    
    public bool IsDebugUIVisible() => showDebugUI;
    
    public void SetPerformanceMonitoring(bool enabled)
    {
        enablePerformanceMonitoring = enabled;
    }
}