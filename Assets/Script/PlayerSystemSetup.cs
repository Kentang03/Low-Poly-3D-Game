using UnityEngine;

/// <summary>
/// Script helper untuk setup otomatis sistem player health, checkpoint, dan death manager
/// Drag ke player GameObject untuk auto-setup
/// </summary>
public class PlayerSystemSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool debugSetup = true;
    
    [Header("System References")]
    [SerializeField] private GameObject checkpointManagerPrefab;
    [SerializeField] private GameObject deathManagerPrefab;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupPlayerSystems();
        }
    }
    
    [ContextMenu("Setup Player Systems")]
    public void SetupPlayerSystems()
    {
        if (debugSetup)
            Debug.Log("Setting up Player Systems...");
        
        // 1. Setup PlayerHealthSystem
        SetupHealthSystem();
        
        // 2. Setup CheckpointManager
        SetupCheckpointManager();
        
        // 3. Setup DeathManager
        SetupDeathManager();
        
        // 4. Setup Player Tag
        SetupPlayerTag();
        
        // 5. Validate setup
        ValidateSetup();
        
        if (debugSetup)
            Debug.Log("Player Systems setup complete!");
    }
    
    void SetupHealthSystem()
    {
        PlayerHealthSystem healthSystem = GetComponent<PlayerHealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<PlayerHealthSystem>();
            if (debugSetup)
                Debug.Log("✅ Added PlayerHealthSystem component");
        }
        
        // Auto-assign Invector controller references
        InvectorControllerAdapter invectorAdapter = GetComponent<InvectorControllerAdapter>();
        
        if (invectorAdapter != null)
        {
            if (debugSetup)
                Debug.Log("✅ Found InvectorControllerAdapter - Player movement system ready");
        }
        else
        {
            // Check for direct Invector components
            var invectorController = GetComponent<Invector.vCharacterController.vThirdPersonController>();
            var invectorInput = GetComponent<Invector.vCharacterController.vThirdPersonInput>();
            
            if (invectorController != null && invectorInput != null)
            {
                if (debugSetup)
                    Debug.Log("✅ Found direct Invector components (vThirdPersonController + vThirdPersonInput)");
            }
            else
            {
                Debug.LogWarning("⚠️ No Invector controller system found! Please ensure player has Invector Third Person Controller components.");
            }
        }
    }
    
    void SetupCheckpointManager()
    {
        CheckpointManager existingManager = FindObjectOfType<CheckpointManager>();
        if (existingManager == null)
        {
            GameObject managerGO;
            
            if (checkpointManagerPrefab != null)
            {
                managerGO = Instantiate(checkpointManagerPrefab);
            }
            else
            {
                managerGO = new GameObject("CheckpointManager");
                managerGO.AddComponent<CheckpointManager>();
            }
            
            if (debugSetup)
                Debug.Log("Created CheckpointManager");
        }
        else
        {
            if (debugSetup)
                Debug.Log("CheckpointManager already exists");
        }
    }
    
    void SetupDeathManager()
    {
        DeathManager existingDeathManager = FindObjectOfType<DeathManager>();
        if (existingDeathManager == null)
        {
            GameObject deathManagerGO;
            
            if (deathManagerPrefab != null)
            {
                deathManagerGO = Instantiate(deathManagerPrefab);
            }
            else
            {
                deathManagerGO = new GameObject("DeathManager");
                deathManagerGO.AddComponent<DeathManager>();
            }
            
            if (debugSetup)
                Debug.Log("Created DeathManager");
        }
        else
        {
            if (debugSetup)
                Debug.Log("DeathManager already exists");
        }
    }
    
    void SetupPlayerTag()
    {
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            if (debugSetup)
                Debug.Log("Set player tag to 'Player'");
        }
    }
    
    void ValidateSetup()
    {
        bool isValid = true;
        
        // Check PlayerHealthSystem
        if (GetComponent<PlayerHealthSystem>() == null)
        {
            Debug.LogError("PlayerHealthSystem not found!");
            isValid = false;
        }
        
        // Check CheckpointManager
        if (FindObjectOfType<CheckpointManager>() == null)
        {
            Debug.LogError("CheckpointManager not found in scene!");
            isValid = false;
        }
        
        // Check DeathManager
        if (FindObjectOfType<DeathManager>() == null)
        {
            Debug.LogError("DeathManager not found in scene!");
            isValid = false;
        }
        
        // Check controller
        InvectorControllerAdapter invectorAdapter = GetComponent<InvectorControllerAdapter>();
        var invectorController = GetComponent<Invector.vCharacterController.vThirdPersonController>();
        var invectorInput = GetComponent<Invector.vCharacterController.vThirdPersonInput>();
        
        if (invectorAdapter != null)
        {
            if (debugSetup)
                Debug.Log("✅ InvectorControllerAdapter found - Optimal setup");
        }
        else if (invectorController != null && invectorInput != null)
        {
            if (debugSetup)
                Debug.Log("✅ Direct Invector components found - Basic setup");
        }
        else
        {
            Debug.LogError("❌ No Invector controller system found! Player movement will not work.");
            isValid = false;
        }
        
        // Check player tag
        if (!gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("GameObject is not tagged as 'Player'");
        }
        
        if (isValid && debugSetup)
        {
            Debug.Log("✓ All player systems validated successfully!");
        }
    }
    
    [ContextMenu("Test Player Death")]
    public void TestPlayerDeath()
    {
        PlayerHealthSystem healthSystem = GetComponent<PlayerHealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.InstantKill(gameObject);
            Debug.Log("Triggered test player death");
        }
        else
        {
            Debug.LogError("PlayerHealthSystem not found for testing!");
        }
    }
    
    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        PlayerHealthSystem healthSystem = GetComponent<PlayerHealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(50, gameObject);
            Debug.Log("Triggered test damage (50 HP)");
        }
        else
        {
            Debug.LogError("PlayerHealthSystem not found for testing!");
        }
    }
    
    [ContextMenu("Force Respawn")]
    public void ForceRespawn()
    {
        PlayerHealthSystem healthSystem = GetComponent<PlayerHealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.Respawn();
            Debug.Log("Forced respawn");
        }
        else
        {
            Debug.LogError("PlayerHealthSystem not found for respawn!");
        }
    }
    
    [ContextMenu("Print Checkpoint Info")]
    public void PrintCheckpointInfo()
    {
        CheckpointManager checkpointManager = CheckpointManager.Instance;
        if (checkpointManager != null)
        {
            checkpointManager.PrintCheckpointInfo();
        }
        else
        {
            Debug.LogError("CheckpointManager not found!");
        }
    }
}