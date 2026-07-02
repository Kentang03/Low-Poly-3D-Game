using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper script untuk integrasi sistem Kancil Guide dengan game yang sudah ada
/// </summary>
public class KancilIntegrationHelper : MonoBehaviour
{
    [Header("Game Integration")]
    [SerializeField] private bool autoFindGameComponents = true;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Transform playerController;
    
    [Header("Scene Integration")]
    [SerializeField] private bool autoSetupOnSceneLoad = true;
    [SerializeField] private float setupDelay = 1f;
    
    [Header("Quest Integration")]
    [SerializeField] private bool enableQuestIntegration = false;
    [SerializeField] private string[] questTriggerNames;
    
    // References
    private KancilGuide kancilGuide;
    private KancilGuideManager guideManager;
    private GameManager gameManagerScript;
    
    private void Start()
    {
        if (autoSetupOnSceneLoad)
        {
            StartCoroutine(DelayedSetup());
        }
    }
    
    private IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(setupDelay);
        
        SetupIntegration();
    }
    
    public void SetupIntegration()
    {
        Debug.Log("Setting up Kancil Guide integration...");
        
        // Find components
        FindGameComponents();
        
        // Setup player reference
        SetupPlayerReference();
        
        // Setup quest integration
        if (enableQuestIntegration)
        {
            SetupQuestIntegration();
        }
        
        Debug.Log("Kancil Guide integration setup completed");
    }
    
    private void FindGameComponents()
    {
        if (!autoFindGameComponents) return;
        
        // Find KancilGuide
        if (kancilGuide == null)
        {
            kancilGuide = FindObjectOfType<KancilGuide>();
        }
        
        // Find KancilGuideManager
        if (guideManager == null)
        {
            guideManager = FindObjectOfType<KancilGuideManager>();
        }
        
        // Find GameManager
        if (gameManager == null)
        {
            GameObject[] managers = GameObject.FindGameObjectsWithTag("GameController");
            if (managers.Length > 0)
            {
                gameManager = managers[0];
            }
            else
            {
                // Cari berdasarkan nama
                gameManager = GameObject.Find("GameManager");
                if (gameManager == null)
                {
                    gameManager = GameObject.Find("Game Manager");
                }
            }
        }
        
        if (gameManager != null)
        {
            gameManagerScript = gameManager.GetComponent<GameManager>();
        }
        
        // Find Player Controller
        if (playerController == null)
        {
            // Cari Invector controller
            var invectorController = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
            if (invectorController != null)
            {
                playerController = invectorController.transform;
                Debug.Log("Found Invector player controller");
            }
            else
            {
                // Cari player dengan tag
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    playerController = player.transform;
                    Debug.Log("Found player with Player tag");
                }
            }
        }
    }
    
    private void SetupPlayerReference()
    {
        if (kancilGuide != null && playerController != null)
        {
            // Use reflection to set private player field
            var field = typeof(KancilGuide).GetField("player", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(kancilGuide, playerController);
                Debug.Log($"Set player reference to: {playerController.name}");
            }
        }
    }
    
    private void SetupQuestIntegration()
    {
        if (kancilGuide == null) return;
        
        // Subscribe to checkpoint events untuk quest triggers
        kancilGuide.OnCheckpointReached += OnCheckpointReachedForQuest;
        
        Debug.Log("Quest integration setup completed");
    }
    
    private void OnCheckpointReachedForQuest(int checkpointIndex)
    {
        if (questTriggerNames == null || checkpointIndex >= questTriggerNames.Length) return;
        
        string triggerName = questTriggerNames[checkpointIndex];
        if (string.IsNullOrEmpty(triggerName)) return;
        
        // Trigger quest event
        TriggerQuestEvent(triggerName);
    }
    
    private void TriggerQuestEvent(string eventName)
    {
        Debug.Log($"Triggering quest event: {eventName}");
        
        // Integrasi dengan sistem quest yang ada
        // Contoh implementasi untuk berbagai sistem quest:
        
        // 1. Untuk sistem quest custom
        if (gameManagerScript != null)
        {
            // Coba call method dengan reflection
            var method = gameManagerScript.GetType().GetMethod("TriggerQuestEvent");
            if (method != null)
            {
                method.Invoke(gameManagerScript, new object[] { eventName });
                return;
            }
        }
        
        // 2. Untuk Unity Events - mencari komponen yang mengimplementasikan UnityEvent
        var eventComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in eventComponents)
        {
            // Cari field atau property yang bertipe UnityEvent
            var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(UnityEngine.Events.UnityEvent))
                {
                    var unityEvent = field.GetValue(component) as UnityEngine.Events.UnityEvent;
                    unityEvent?.Invoke();
                    break;
                }
            }
        }
        
        // 3. Untuk custom event system
        SendMessage("OnQuestTrigger", eventName, SendMessageOptions.DontRequireReceiver);
        
        // 4. Broadcast ke semua GameObjects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            obj.SendMessage("OnKancilQuestTrigger", eventName, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    // Public API untuk external control
    public void StartGuideToLocation(string locationName)
    {
        if (guideManager == null) return;
        
        // Find route by name
        var routes = guideManager.GetAllRoutes();
        for (int i = 0; i < routes.Count; i++)
        {
            if (routes[i].routeName.ToLower().Contains(locationName.ToLower()))
            {
                guideManager.StartGuideWithRoute(i);
                Debug.Log($"Started guide to: {locationName}");
                return;
            }
        }
        
        Debug.LogWarning($"Route to '{locationName}' not found");
    }
    
    public void StopGuide()
    {
        if (guideManager != null)
        {
            guideManager.StopGuiding();
        }
    }
    
    public bool IsKancilGuiding()
    {
        return guideManager != null && guideManager.IsGuiding();
    }
    
    public void CreateQuickRouteToPosition(Vector3 targetPosition, string routeName = "Quick Route")
    {
        if (kancilGuide == null) return;
        
        // Create simple 3-point route
        Vector3 startPos = kancilGuide.transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, targetPosition, 0.5f);
        
        List<KancilCheckpoint> checkpoints = new List<KancilCheckpoint>();
        
        // Mid checkpoint
        KancilCheckpoint midCheckpoint = KancilCheckpoint.CreateCheckpoint(midPos, "Midpoint");
        checkpoints.Add(midCheckpoint);
        
        // Target checkpoint
        KancilCheckpoint targetCheckpoint = KancilCheckpoint.CreateCheckpoint(targetPosition, "Destination");
        checkpoints.Add(targetCheckpoint);
        
        // Set route
        kancilGuide.SetCheckpoints(checkpoints);
        kancilGuide.StartGuiding();
        
        Debug.Log($"Created quick route to {targetPosition}");
    }
    
    // Integration dengan Invector controller
    public void IntegrateWithInvectorController()
    {
        var invectorController = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
        if (invectorController == null) return;
        
        // Set player reference
        playerController = invectorController.transform;
        SetupPlayerReference();
        
        // Subscribe to Invector events if available
        Debug.Log("Integrated with Invector controller");
    }
    
    // Integration dengan sistem audio
    public void SetupAudioIntegration()
    {
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null) return;
        
        if (kancilGuide != null)
        {
            kancilGuide.OnCheckpointReached += (index) =>
            {
                // Play checkpoint sound
                // audioManager.PlaySound("checkpoint_reached");
            };
            
            kancilGuide.OnStateChanged += (state) =>
            {
                switch (state)
                {
                    case KancilGuide.KancilState.MovingToCheckpoint:
                        // audioManager.PlaySound("kancil_start_moving");
                        break;
                    case KancilGuide.KancilState.WaitingForPlayer:
                        // audioManager.PlaySound("kancil_waiting");
                        break;
                }
            };
        }
        
        Debug.Log("Audio integration setup completed");
    }
    
    // Helper methods untuk debugging
    [ContextMenu("Test Integration")]
    public void TestIntegration()
    {
        Debug.Log("=== Kancil Integration Test ===");
        
        Debug.Log($"KancilGuide found: {kancilGuide != null}");
        Debug.Log($"GuideManager found: {guideManager != null}");
        Debug.Log($"Player Controller found: {playerController != null}");
        Debug.Log($"GameManager found: {gameManagerScript != null}");
        
        if (kancilGuide != null)
        {
            Debug.Log($"Total checkpoints: {kancilGuide.GetTotalCheckpoints()}");
            Debug.Log($"Current state: {kancilGuide.GetCurrentState()}");
            Debug.Log($"Player nearby: {kancilGuide.IsPlayerNearby()}");
        }
        
        Debug.Log("=== End Integration Test ===");
    }
    
    [ContextMenu("Quick Setup Test")]
    public void QuickSetupTest()
    {
        if (kancilGuide == null)
        {
            Debug.LogError("No KancilGuide found! Please run setup first.");
            return;
        }
        
        Vector3 testPosition = kancilGuide.transform.position + Vector3.forward * 10f;
        CreateQuickRouteToPosition(testPosition, "Test Route");
    }
    
    private void OnDestroy()
    {
        // Cleanup events
        if (kancilGuide != null)
        {
            kancilGuide.OnCheckpointReached -= OnCheckpointReachedForQuest;
        }
    }
    
    // Static utility methods
    public static KancilIntegrationHelper CreateIntegrationHelper()
    {
        GameObject helperObj = new GameObject("KancilIntegrationHelper");
        return helperObj.AddComponent<KancilIntegrationHelper>();
    }
    
    public static void SetupKancilForExistingScene()
    {
        KancilIntegrationHelper helper = FindObjectOfType<KancilIntegrationHelper>();
        if (helper == null)
        {
            helper = CreateIntegrationHelper();
        }
        
        helper.SetupIntegration();
    }
}