using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager untuk mengontrol dan mengkonfigurasi sistem Kancil Guide
/// </summary>
public class KancilGuideManager : MonoBehaviour
{
    [Header("Kancil Guide Reference")]
    [SerializeField] private KancilGuide kancilGuide;
    
    [Header("Preset Checkpoint Routes")]
    [SerializeField] private List<CheckpointRoute> presetRoutes = new List<CheckpointRoute>();
    
    [Header("UI References")]
    [SerializeField] private Button startGuideButton;
    [SerializeField] private Button stopGuideButton;
    [SerializeField] private Dropdown routeSelector;
    [SerializeField] private Text statusText;
    [SerializeField] private Text checkpointInfoText;
    
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugUI = true;
    [SerializeField] private Canvas debugCanvas;
    
    // Private variables
    private int currentRouteIndex = -1;
    private bool isGuiding = false;
    
    [System.Serializable]
    public class CheckpointRoute
    {
        public string routeName;
        public string description;
        public List<KancilCheckpoint> checkpoints;
        public bool loopRoute = true;
        
        public CheckpointRoute(string name, string desc = "")
        {
            routeName = name;
            description = desc;
            checkpoints = new List<KancilCheckpoint>();
        }
    }
    
    private void Start()
    {
        InitializeManager();
        SetupUI();
    }
    
    private void Update()
    {
        UpdateUI();
    }
    
    private void InitializeManager()
    {
        // Find KancilGuide if not assigned
        if (kancilGuide == null)
        {
            kancilGuide = FindObjectOfType<KancilGuide>();
        }
        
        if (kancilGuide == null)
        {
            Debug.LogWarning("KancilGuideManager: No KancilGuide found in scene!");
            return;
        }
        
        // Subscribe to events
        kancilGuide.OnCheckpointReached += OnCheckpointReached;
        kancilGuide.OnStateChanged += OnKancilStateChanged;
        
        // Setup debug UI
        if (enableDebugUI && debugCanvas != null)
        {
            debugCanvas.gameObject.SetActive(true);
        }
        else if (debugCanvas != null)
        {
            debugCanvas.gameObject.SetActive(false);
        }
        
        Debug.Log("KancilGuideManager initialized");
    }
    
    private void SetupUI()
    {
        // Setup buttons
        if (startGuideButton != null)
        {
            startGuideButton.onClick.AddListener(StartGuiding);
        }
        
        if (stopGuideButton != null)
        {
            stopGuideButton.onClick.AddListener(StopGuiding);
        }
        
        // Setup route selector
        if (routeSelector != null)
        {
            routeSelector.ClearOptions();
            List<string> routeNames = new List<string>();
            
            for (int i = 0; i < presetRoutes.Count; i++)
            {
                routeNames.Add($"{i + 1}. {presetRoutes[i].routeName}");
            }
            
            if (routeNames.Count == 0)
            {
                routeNames.Add("No routes available");
            }
            
            routeSelector.AddOptions(routeNames);
            routeSelector.onValueChanged.AddListener(OnRouteSelected);
        }
    }
    
    private void UpdateUI()
    {
        if (kancilGuide == null) return;
        
        // Update status text
        if (statusText != null)
        {
            string status = $"Status: {kancilGuide.GetCurrentState()}";
            status += $"\nPlayer Nearby: {(kancilGuide.IsPlayerNearby() ? "Yes" : "No")}";
            statusText.text = status;
        }
        
        // Update checkpoint info
        if (checkpointInfoText != null)
        {
            int currentIndex = kancilGuide.GetCurrentCheckpointIndex();
            int totalCheckpoints = kancilGuide.GetTotalCheckpoints();
            
            string info = $"Checkpoint: {currentIndex + 1}/{totalCheckpoints}";
            if (currentRouteIndex >= 0 && currentRouteIndex < presetRoutes.Count)
            {
                info += $"\nRoute: {presetRoutes[currentRouteIndex].routeName}";
            }
            checkpointInfoText.text = info;
        }
        
        // Update button states
        if (startGuideButton != null)
        {
            startGuideButton.interactable = !isGuiding && presetRoutes.Count > 0;
        }
        
        if (stopGuideButton != null)
        {
            stopGuideButton.interactable = isGuiding;
        }
    }
    
    // Public methods untuk UI
    public void StartGuiding()
    {
        if (kancilGuide == null || presetRoutes.Count == 0) return;
        
        int routeIndex = routeSelector != null ? routeSelector.value : 0;
        StartGuideWithRoute(routeIndex);
    }
    
    public void StopGuiding()
    {
        if (kancilGuide == null) return;
        
        kancilGuide.StopGuiding();
        isGuiding = false;
        currentRouteIndex = -1;
        
        Debug.Log("Kancil guiding stopped");
    }
    
    public void StartGuideWithRoute(int routeIndex)
    {
        if (kancilGuide == null || routeIndex < 0 || routeIndex >= presetRoutes.Count)
        {
            Debug.LogError($"Invalid route index: {routeIndex}");
            return;
        }
        
        CheckpointRoute selectedRoute = presetRoutes[routeIndex];
        
        if (selectedRoute.checkpoints.Count == 0)
        {
            Debug.LogError($"Route '{selectedRoute.routeName}' has no checkpoints!");
            return;
        }
        
        // Set checkpoints to kancil
        kancilGuide.SetCheckpoints(selectedRoute.checkpoints);
        
        // Start guiding
        kancilGuide.StartGuiding();
        
        isGuiding = true;
        currentRouteIndex = routeIndex;
        
        Debug.Log($"Started guiding with route: {selectedRoute.routeName} ({selectedRoute.checkpoints.Count} checkpoints)");
    }
    
    public void OnRouteSelected(int index)
    {
        if (!isGuiding && index >= 0 && index < presetRoutes.Count)
        {
            Debug.Log($"Selected route: {presetRoutes[index].routeName}");
        }
    }
    
    // Event handlers
    private void OnCheckpointReached(int checkpointIndex)
    {
        Debug.Log($"KancilGuideManager: Checkpoint {checkpointIndex + 1} reached");
        
        if (currentRouteIndex >= 0 && currentRouteIndex < presetRoutes.Count)
        {
            CheckpointRoute currentRoute = presetRoutes[currentRouteIndex];
            if (checkpointIndex >= 0 && checkpointIndex < currentRoute.checkpoints.Count)
            {
                KancilCheckpoint checkpoint = currentRoute.checkpoints[checkpointIndex];
                checkpoint.TriggerCheckpointAction();
            }
        }
    }
    
    private void OnKancilStateChanged(KancilGuide.KancilState newState)
    {
        Debug.Log($"KancilGuideManager: Kancil state changed to {newState}");
        
        // Handle specific state changes if needed
        switch (newState)
        {
            case KancilGuide.KancilState.Idle:
                if (isGuiding)
                {
                    // Route completed
                    Debug.Log("Guide route completed!");
                }
                break;
        }
    }
    
    // Route management methods
    public void AddRoute(CheckpointRoute route)
    {
        presetRoutes.Add(route);
        SetupUI(); // Refresh UI
    }
    
    public void RemoveRoute(int index)
    {
        if (index >= 0 && index < presetRoutes.Count)
        {
            presetRoutes.RemoveAt(index);
            SetupUI(); // Refresh UI
        }
    }
    
    public CheckpointRoute CreateRoute(string routeName, List<KancilCheckpoint> checkpoints, bool loop = true)
    {
        CheckpointRoute newRoute = new CheckpointRoute(routeName);
        newRoute.checkpoints = new List<KancilCheckpoint>(checkpoints);
        newRoute.loopRoute = loop;
        
        AddRoute(newRoute);
        return newRoute;
    }
    
    // Utility methods
    public void CreateQuickRoute(Transform[] positions, string routeName = "Quick Route")
    {
        List<KancilCheckpoint> checkpoints = new List<KancilCheckpoint>();
        
        for (int i = 0; i < positions.Length; i++)
        {
            KancilCheckpoint checkpoint = KancilCheckpoint.CreateCheckpoint(
                positions[i].position, 
                $"{routeName}_Checkpoint_{i + 1}"
            );
            checkpoints.Add(checkpoint);
        }
        
        CreateRoute(routeName, checkpoints);
        Debug.Log($"Created quick route '{routeName}' with {checkpoints.Count} checkpoints");
    }
    
    public void CreateRouteFromGameObjects(GameObject[] checkpointObjects, string routeName = "Generated Route")
    {
        List<KancilCheckpoint> checkpoints = new List<KancilCheckpoint>();
        
        foreach (GameObject obj in checkpointObjects)
        {
            KancilCheckpoint checkpoint = obj.GetComponent<KancilCheckpoint>();
            if (checkpoint == null)
            {
                checkpoint = obj.AddComponent<KancilCheckpoint>();
            }
            checkpoints.Add(checkpoint);
        }
        
        CreateRoute(routeName, checkpoints);
        Debug.Log($"Created route '{routeName}' from {checkpoints.Count} GameObjects");
    }
    
    // Getters
    public bool IsGuiding() => isGuiding;
    public int GetCurrentRouteIndex() => currentRouteIndex;
    public CheckpointRoute GetCurrentRoute() => currentRouteIndex >= 0 ? presetRoutes[currentRouteIndex] : null;
    public List<CheckpointRoute> GetAllRoutes() => presetRoutes;
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (kancilGuide != null)
        {
            kancilGuide.OnCheckpointReached -= OnCheckpointReached;
            kancilGuide.OnStateChanged -= OnKancilStateChanged;
        }
    }
}