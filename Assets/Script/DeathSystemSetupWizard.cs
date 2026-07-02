using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Setup Wizard untuk Death & Checkpoint System
/// Drag ke GameObject manapun untuk setup otomatis seluruh sistem
/// </summary>
public class DeathSystemSetupWizard : MonoBehaviour
{
    [Header("🚀 Setup Wizard - Death & Checkpoint System")]
    [Space(10)]
    
    [Header("Step 1: Player Setup")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private bool autoFindPlayer = true;
    
    [Header("Step 2: Checkpoint Areas")]
    [SerializeField] private List<CheckpointArea> checkpointAreas = new List<CheckpointArea>();
    [SerializeField] private bool createExampleAreas = false;
    [SerializeField] private float areaSpacing = 20f;
    
    [Header("Step 3: System Configuration")]
    [SerializeField] private float checkpointActivationDistance = 5f;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private bool enableAutoRespawn = true;
    [SerializeField] private bool enableDeathUI = true;
    
    [Header("Step 4: Testing Setup")]
    [SerializeField] private bool includeTestingTools = true;
    [SerializeField] private bool enableKeyboardTesting = true;
    
    [Header("📋 Setup Status")]
    [SerializeField] private bool playerSystemReady = false;
    [SerializeField] private bool checkpointManagerReady = false;
    [SerializeField] private bool deathManagerReady = false;
    [SerializeField] private bool areasSetup = false;
    [SerializeField] private bool testingReady = false;
    
    [System.Serializable]
    public class CheckpointArea
    {
        public string areaName = "Area 1";
        public int areaIndex = 1;
        public Vector3 position = Vector3.zero;
        public Vector3 triggerSize = new Vector3(10, 5, 10);
        public bool useCustomSpawnPoint = false;
        public Vector3 customSpawnPosition = Vector3.zero;
        
        [HideInInspector]
        public GameObject areaObject;
    }
    
    void Start()
    {
        // Auto-setup jika belum ready
        if (!IsSystemComplete())
        {
            Debug.Log("🚀 DeathSystemSetupWizard: Auto-setup starting...");
            SetupCompleteSystem();
        }
    }
    
    [ContextMenu("🚀 Setup Complete System")]
    public void SetupCompleteSystem()
    {
        Debug.Log("=== Death System Setup Wizard Starting ===");
        
        // Step 1: Player Setup
        SetupPlayerSystem();
        
        // Step 2: Manager Setup  
        SetupManagers();
        
        // Step 3: Checkpoint Areas
        SetupCheckpointAreas();
        
        // Step 4: Testing Tools
        if (includeTestingTools)
        {
            SetupTestingTools();
        }
        
        // Step 5: Validation
        ValidateSetup();
        
        Debug.Log("=== Death System Setup Complete! ===");
        PrintSetupSummary();
    }
    
    void SetupPlayerSystem()
    {
        Debug.Log("Step 1: Setting up Player System...");
        
        // Find player if auto-find enabled
        if (autoFindPlayer || playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
            
            if (playerObject == null)
            {
                // Try alternative methods
                InvectorControllerAdapter invector = FindObjectOfType<InvectorControllerAdapter>();
                if (invector != null) playerObject = invector.gameObject;
                
                CharacterController controller = FindObjectOfType<CharacterController>();
                if (controller != null && playerObject == null) playerObject = controller.gameObject;
            }
        }
        
        if (playerObject == null)
        {
            Debug.LogError("❌ Player object not found! Please assign manually.");
            return;
        }
        
        Debug.Log($"✅ Player found: {playerObject.name}");
        
        // Ensure player tag
        if (!playerObject.CompareTag("Player"))
        {
            playerObject.tag = "Player";
            Debug.Log("✅ Set player tag to 'Player'");
        }
        
        // Add PlayerSystemSetup if not present
        PlayerSystemSetup setup = playerObject.GetComponent<PlayerSystemSetup>();
        if (setup == null)
        {
            setup = playerObject.AddComponent<PlayerSystemSetup>();
            Debug.Log("✅ Added PlayerSystemSetup component");
        }
        
        // Run the player setup
        setup.SetupPlayerSystems();
        
        playerSystemReady = true;
        Debug.Log("✅ Player system setup complete");
    }
    
    void SetupManagers()
    {
        Debug.Log("Step 2: Setting up Managers...");
        
        // Setup CheckpointManager
        CheckpointManager checkpointManager = CheckpointManager.Instance;
        if (checkpointManager != null)
        {
            // Configure settings
            var distanceField = typeof(CheckpointManager).GetField("checkpointActivationDistance", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (distanceField != null)
                distanceField.SetValue(checkpointManager, checkpointActivationDistance);
            
            checkpointManagerReady = true;
            Debug.Log("✅ CheckpointManager configured");
        }
        
        // Setup DeathManager
        DeathManager deathManager = DeathManager.Instance;
        if (deathManager != null)
        {
            // Configure settings using reflection
            var autoRespawnField = typeof(DeathManager).GetField("autoRespawn", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (autoRespawnField != null)
                autoRespawnField.SetValue(deathManager, enableAutoRespawn);
            
            var respawnDelayField = typeof(DeathManager).GetField("autoRespawnDelay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (respawnDelayField != null)
                respawnDelayField.SetValue(deathManager, respawnDelay);
            
            deathManagerReady = true;
            Debug.Log("✅ DeathManager configured");
        }
    }
    
    void SetupCheckpointAreas()
    {
        Debug.Log("Step 3: Setting up Checkpoint Areas...");
        
        if (createExampleAreas && checkpointAreas.Count == 0)
        {
            CreateExampleAreas();
        }
        
        if (checkpointAreas.Count == 0)
        {
            Debug.LogWarning("⚠️ No checkpoint areas defined. Add areas manually or enable 'Create Example Areas'");
            return;
        }
        
        // Create parent object
        GameObject checkpointsParent = GameObject.Find("CheckpointAreas");
        if (checkpointsParent == null)
        {
            checkpointsParent = new GameObject("CheckpointAreas");
            Debug.Log("✅ Created CheckpointAreas parent object");
        }
        
        // Create each checkpoint area
        for (int i = 0; i < checkpointAreas.Count; i++)
        {
            CreateCheckpointArea(checkpointAreas[i], checkpointsParent.transform);
        }
        
        // Register checkpoints with manager
        CheckpointManager.Instance?.AddCheckpoint(checkpointsParent.transform);
        
        areasSetup = true;
        Debug.Log($"✅ Created {checkpointAreas.Count} checkpoint areas");
    }
    
    void CreateExampleAreas()
    {
        Debug.Log("Creating example checkpoint areas...");
        
        Vector3 playerPos = playerObject != null ? playerObject.transform.position : Vector3.zero;
        
        checkpointAreas.Clear();
        
        for (int i = 1; i <= 3; i++)
        {
            CheckpointArea area = new CheckpointArea();
            area.areaName = $"Area {i}";
            area.areaIndex = i;
            area.position = playerPos + Vector3.right * (i * areaSpacing);
            area.triggerSize = new Vector3(15, 8, 15);
            
            checkpointAreas.Add(area);
        }
        
        Debug.Log("✅ Created 3 example areas");
    }
    
    void CreateCheckpointArea(CheckpointArea area, Transform parent)
    {
        // Create area GameObject
        GameObject areaGO = new GameObject($"Area_{area.areaIndex}_{area.areaName.Replace(" ", "_")}");
        areaGO.transform.SetParent(parent);
        areaGO.transform.position = area.position;
        
        // Add AreaCheckpointTrigger
        AreaCheckpointTrigger trigger = areaGO.AddComponent<AreaCheckpointTrigger>();
        
        // Configure trigger using reflection
        var areaNameField = typeof(AreaCheckpointTrigger).GetField("areaName", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var areaIndexField = typeof(AreaCheckpointTrigger).GetField("areaIndex", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var debugField = typeof(AreaCheckpointTrigger).GetField("showDebugMessages", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (areaNameField != null) areaNameField.SetValue(trigger, area.areaName);
        if (areaIndexField != null) areaIndexField.SetValue(trigger, area.areaIndex);
        if (debugField != null) debugField.SetValue(trigger, true);
        
        // Add and configure collider
        BoxCollider col = areaGO.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = area.triggerSize;
        
        // Setup custom spawn point if needed
        if (area.useCustomSpawnPoint)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{area.areaIndex}");
            spawnPoint.transform.SetParent(areaGO.transform);
            spawnPoint.transform.position = area.customSpawnPosition;
            
            var spawnField = typeof(AreaCheckpointTrigger).GetField("checkpointPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (spawnField != null) spawnField.SetValue(trigger, spawnPoint.transform);
        }
        
        area.areaObject = areaGO;
        
        Debug.Log($"✅ Created checkpoint area: {area.areaName} at {area.position}");
    }
    
    void SetupTestingTools()
    {
        Debug.Log("Step 4: Setting up Testing Tools...");
        
        // Find or create testing object
        GameObject testingGO = GameObject.Find("DeathSystemTesting");
        if (testingGO == null)
        {
            testingGO = new GameObject("DeathSystemTesting");
        }
        
        // Add TestDeathSystem
        TestDeathSystem testSystem = testingGO.GetComponent<TestDeathSystem>();
        if (testSystem == null)
        {
            testSystem = testingGO.AddComponent<TestDeathSystem>();
            
            // Configure using reflection
            var keyboardField = typeof(TestDeathSystem).GetField("enableKeyboardTesting", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (keyboardField != null) keyboardField.SetValue(testSystem, enableKeyboardTesting);
            
            Debug.Log("✅ Added TestDeathSystem component");
        }
        
        testingReady = true;
        Debug.Log("✅ Testing tools setup complete");
    }
    
    void ValidateSetup()
    {
        Debug.Log("Step 5: Validating Setup...");
        
        bool allValid = true;
        
        // Check all systems
        if (!playerSystemReady)
        {
            Debug.LogError("❌ Player system not ready");
            allValid = false;
        }
        
        if (!checkpointManagerReady)
        {
            Debug.LogError("❌ CheckpointManager not ready");
            allValid = false;
        }
        
        if (!deathManagerReady)
        {
            Debug.LogError("❌ DeathManager not ready");
            allValid = false;
        }
        
        if (!areasSetup && checkpointAreas.Count > 0)
        {
            Debug.LogError("❌ Checkpoint areas not setup properly");
            allValid = false;
        }
        
        if (allValid)
        {
            Debug.Log("🎉 All systems validated successfully!");
        }
        else
        {
            Debug.LogError("❌ Setup validation failed. Check errors above.");
        }
    }
    
    void PrintSetupSummary()
    {
        Debug.Log("=== SETUP SUMMARY ===");
        Debug.Log($"✅ Player System: {(playerSystemReady ? "Ready" : "Failed")}");
        Debug.Log($"✅ CheckpointManager: {(checkpointManagerReady ? "Ready" : "Failed")}");
        Debug.Log($"✅ DeathManager: {(deathManagerReady ? "Ready" : "Failed")}");
        Debug.Log($"✅ Checkpoint Areas: {checkpointAreas.Count} areas {(areasSetup ? "created" : "failed")}");
        Debug.Log($"✅ Testing Tools: {(testingReady ? "Ready" : "Not included")}");
        
        if (IsSystemComplete())
        {
            Debug.Log("🚀 SYSTEM READY FOR USE! 🚀");
            Debug.Log("Test with keyboard: K=Kill, R=Rock, H=Damage, N=NextCheckpoint, I=Info");
        }
        else
        {
            Debug.Log("⚠️ System setup incomplete. Check errors above.");
        }
        
        Debug.Log("======================");
    }
    
    bool IsSystemComplete()
    {
        return playerSystemReady && checkpointManagerReady && deathManagerReady;
    }
    
    [ContextMenu("🔧 Reset All Settings")]
    public void ResetAllSettings()
    {
        playerSystemReady = false;
        checkpointManagerReady = false;
        deathManagerReady = false;
        areasSetup = false;
        testingReady = false;
        
        checkpointAreas.Clear();
        
        Debug.Log("All settings reset. Run setup again to reconfigure.");
    }
    
    [ContextMenu("📋 Quick Test Sequence")]
    public void RunQuickTest()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Quick test only available during Play Mode");
            return;
        }
        
        StartCoroutine(QuickTestCoroutine());
    }
    
    System.Collections.IEnumerator QuickTestCoroutine()
    {
        Debug.Log("🧪 Running Quick Test Sequence...");
        
        // Test 1: System Info
        TestDeathSystem testSystem = FindObjectOfType<TestDeathSystem>();
        if (testSystem != null)
        {
            testSystem.PrintSystemInfo();
            yield return new WaitForSeconds(2f);
            
            // Test 2: Checkpoint Progression
            Debug.Log("Testing checkpoint progression...");
            testSystem.TestNextCheckpoint();
            yield return new WaitForSeconds(2f);
            
            // Test 3: Player Death
            Debug.Log("Testing player death...");
            testSystem.TestKillPlayer();
            yield return new WaitForSeconds(5f); // Wait for respawn
            
            // Test 4: Rock Spawn
            Debug.Log("Testing rock spawn...");
            testSystem.TestSpawnRock();
            
            Debug.Log("🎉 Quick test sequence complete!");
        }
        else
        {
            Debug.LogError("TestDeathSystem not found for quick test");
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw checkpoint areas
        if (checkpointAreas != null)
        {
            for (int i = 0; i < checkpointAreas.Count; i++)
            {
                CheckpointArea area = checkpointAreas[i];
                
                // Draw area box
                Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan
                Gizmos.DrawCube(area.position, area.triggerSize);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(area.position, area.triggerSize);
                
                // Draw spawn point
                Vector3 spawnPos = area.useCustomSpawnPoint ? area.customSpawnPosition : area.position;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnPos, 1f);
                
                // Draw connection line
                if (area.useCustomSpawnPoint)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(area.position, area.customSpawnPosition);
                }
            }
        }
        
        // Draw player connection
        if (playerObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerObject.transform.position, 2f);
        }
    }
}