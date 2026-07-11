using UnityEngine;
using System.Collections;

/// <summary>
/// Script testing untuk sistem kematian dan respawn
/// Gunakan untuk testing dan debugging sistem
/// </summary>
public class TestDeathSystem : MonoBehaviour
{
    [Header("Testing References")]
    [SerializeField] private PlayerHealthSystem playerHealth;
    [SerializeField] private CheckpointManager checkpointManager;
    [SerializeField] private DeathManager deathManager;
    [SerializeField] private GameObject rockPrefab;
    
    [Header("Test Settings")]
    [SerializeField] private bool enableKeyboardTesting = true;
    [SerializeField] private KeyCode killPlayerKey = KeyCode.K;
    [SerializeField] private KeyCode spawnRockKey = KeyCode.R;
    [SerializeField] private KeyCode nextCheckpointKey = KeyCode.N;
    [SerializeField] private KeyCode printInfoKey = KeyCode.I;
    [SerializeField] private KeyCode damagePlayerKey = KeyCode.H;
    [SerializeField] private KeyCode invectorStatusKey = KeyCode.V;
    [SerializeField] private KeyCode forceEnableInvectorKey = KeyCode.F;
    
    [Header("Rock Spawning")]
    [SerializeField] private float rockSpawnHeight = 5f;
    [SerializeField] private float rockThrowForce = 10f;
    [SerializeField] private Vector3 rockSpawnOffset = Vector3.forward * 2f;
    
    void Start()
    {
        InitializeReferences();
        PrintControls();
    }
    
    void InitializeReferences()
    {
        // Find references if not assigned
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealthSystem>();
            
        if (checkpointManager == null)
            checkpointManager = CheckpointManager.Instance;
            
        if (deathManager == null)
            deathManager = DeathManager.Instance;
            
        // Get rock prefab from StoneSpawner if available
        if (rockPrefab == null)
        {
            StoneSpawner stoneSpawner = FindObjectOfType<StoneSpawner>();
            if (stoneSpawner != null)
            {
                // Use reflection or make stonePrefab public to access it
                Debug.Log("Found StoneSpawner, but cannot access prefab (private field)");
            }
        }
    }
    
    void Update()
    {
        if (enableKeyboardTesting)
        {
            HandleKeyboardInput();
        }
    }
    
    void HandleKeyboardInput()
    {
        // Kill player
        if (Input.GetKeyDown(killPlayerKey))
        {
            TestKillPlayer();
        }
        
        // Spawn rock
        if (Input.GetKeyDown(spawnRockKey))
        {
            TestSpawnRock();
        }
        
        // Next checkpoint
        if (Input.GetKeyDown(nextCheckpointKey))
        {
            TestNextCheckpoint();
        }
        
        // Print info
        if (Input.GetKeyDown(printInfoKey))
        {
            PrintSystemInfo();
        }
        
        // Damage player
        if (Input.GetKeyDown(damagePlayerKey))
        {
            TestDamagePlayer();
        }
        
        // Check Invector status
        if (Input.GetKeyDown(invectorStatusKey))
        {
            TestInvectorStatus();
        }
        
        // Force enable Invector
        if (Input.GetKeyDown(forceEnableInvectorKey))
        {
            ForceEnableInvector();
        }
    }
    
    void PrintControls()
    {
        Debug.Log("=== Death System Test Controls ===");
        Debug.Log($"[{killPlayerKey}] - Kill Player (Instant Death)");
        Debug.Log($"[{spawnRockKey}] - Spawn Rock Above Player");
        Debug.Log($"[{nextCheckpointKey}] - Advance to Next Checkpoint");
        Debug.Log($"[{damagePlayerKey}] - Damage Player (50 HP)");
        Debug.Log($"[{printInfoKey}] - Print System Information");
        Debug.Log($"[{invectorStatusKey}] - Check Invector Components Status");
        Debug.Log($"[{forceEnableInvectorKey}] - Force Enable Invector Components");
        Debug.Log("=====================================");
    }
    
    [ContextMenu("Test Kill Player")]
    public void TestKillPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.InstantKill(gameObject);
            Debug.Log("✓ Test: Player killed instantly");
        }
        else
        {
            Debug.LogError("✗ Test: PlayerHealthSystem not found!");
        }
    }
    
    [ContextMenu("Test Invector Status")]
    public void TestInvectorStatus()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("✗ Test: Player not found!");
            return;
        }
        
        Debug.Log("=== Invector System Status ===");
        
        // Check InvectorControllerAdapter (Recommended)
        InvectorControllerAdapter adapter = player.GetComponent<InvectorControllerAdapter>();
        if (adapter != null)
        {
            Debug.Log("✅ Using InvectorControllerAdapter (Recommended)");
            adapter.PrintInvectorStatus();
        }
        else
        {
            Debug.Log("⚠️ No InvectorControllerAdapter found - Using direct Invector components");
        }
        
        // Check direct Invector components
        var invectorInput = player.GetComponent<Invector.vCharacterController.vThirdPersonInput>();
        var invectorController = player.GetComponent<Invector.vCharacterController.vThirdPersonController>();
        
        Debug.Log("=== Direct Invector Components ===");
        if (invectorController != null)
        {
            Debug.Log($"✅ vThirdPersonController: {(invectorController.enabled ? "ENABLED" : "DISABLED")}");
            
            var rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"   Rigidbody Constraints: {rb.constraints}");
                Debug.Log($"   Is Grounded: {invectorController.isGrounded}");
            }
        }
        else
        {
            Debug.LogError("❌ vThirdPersonController: NOT FOUND");
        }
        
        if (invectorInput != null)
        {
            Debug.Log($"✅ vThirdPersonInput: {(invectorInput.enabled ? "ENABLED" : "DISABLED")}");
        }
        else
        {
            Debug.LogError("❌ vThirdPersonInput: NOT FOUND");
        }
        
        Debug.Log("===================================");
    }
    
    [ContextMenu("Force Enable Invector")]
    public void ForceEnableInvector()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("✗ Test: Player not found!");
            return;
        }
        
        Debug.Log("🔧 Force enabling Invector components...");
        
        // Try InvectorControllerAdapter first (recommended)
        InvectorControllerAdapter adapter = player.GetComponent<InvectorControllerAdapter>();
        if (adapter != null)
        {
            Debug.Log("✅ Using InvectorControllerAdapter method");
            adapter.ForceEnableInvectorComponents();
        }
        else
        {
            Debug.Log("⚠️ Using direct Invector component method");
            
            // Manual enable direct components
            var invectorController = player.GetComponent<Invector.vCharacterController.vThirdPersonController>();
            var invectorInput = player.GetComponent<Invector.vCharacterController.vThirdPersonInput>();
            
            if (invectorController != null)
            {
                invectorController.enabled = true;
                
                // Unfreeze rigidbody
                var rb = invectorController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }
                
                Debug.Log("✅ Manually enabled vThirdPersonController");
            }
            else
            {
                Debug.LogError("❌ vThirdPersonController not found!");
            }
            
            if (invectorInput != null)
            {
                invectorInput.enabled = true;
                Debug.Log("✅ Manually enabled vThirdPersonInput");
            }
            else
            {
                Debug.LogError("❌ vThirdPersonInput not found!");
            }
        }
        
        Debug.Log("🎮 Try moving with WASD now!");
    }
    
    [ContextMenu("Test Damage Player")]
    public void TestDamagePlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(50, gameObject);
            Debug.Log($"✓ Test: Player damaged for 50 HP. Current health: {playerHealth.CurrentHealth}");
        }
        else
        {
            Debug.LogError("✗ Test: PlayerHealthSystem not found!");
        }
    }
    
    [ContextMenu("Test Spawn Rock")]
    public void TestSpawnRock()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("✗ Test: Player not found!");
            return;
        }
        
        Vector3 spawnPosition = player.transform.position + Vector3.up * rockSpawnHeight + rockSpawnOffset;
        
        GameObject testRock;
        
        if (rockPrefab != null)
        {
            testRock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            // Create basic rock
            testRock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            testRock.transform.position = spawnPosition;
            testRock.name = "TestRock";
            
            // Add physics
            Rigidbody rb = testRock.AddComponent<Rigidbody>();
            rb.mass = 1f;
            
            // Add collision handler
            RockCollisionHandler collision = testRock.AddComponent<RockCollisionHandler>();
            
            // Set material for visibility
            Renderer renderer = testRock.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
        }
        
        // Add initial velocity toward player
        Rigidbody rockRb = testRock.GetComponent<Rigidbody>();
        if (rockRb != null)
        {
            Vector3 directionToPlayer = (player.transform.position - testRock.transform.position).normalized;
            rockRb.linearVelocity = directionToPlayer * rockThrowForce;
        }
        
        // Auto-destroy after 10 seconds
        Destroy(testRock, 10f);
        
        Debug.Log("✓ Test: Rock spawned above player");
    }
    
    [ContextMenu("Test Next Checkpoint")]
    public void TestNextCheckpoint()
    {
        if (checkpointManager == null)
        {
            Debug.LogError("✗ Test: CheckpointManager not found!");
            return;
        }
        
        // Get current checkpoint and advance to next
        int currentIndex = checkpointManager.GetCurrentCheckpointIndex();
        int nextIndex = currentIndex + 1;
        
        // Ensure we don't exceed available checkpoints
        if (nextIndex >= checkpointManager.GetCheckpointCount())
        {
            Debug.Log($"✓ Test: Already at last checkpoint ({currentIndex}). Cannot advance further.");
            return;
        }
        
        checkpointManager.SetCurrentCheckpoint(nextIndex);
        Debug.Log($"✓ Test: Advanced from checkpoint {currentIndex} to {nextIndex}");
    }
    
    [ContextMenu("Test Force Respawn")]
    public void TestForceRespawn()
    {
        if (playerHealth != null)
        {
            playerHealth.Respawn();
            Debug.Log("✓ Test: Forced player respawn");
        }
        else
        {
            Debug.LogError("✗ Test: PlayerHealthSystem not found!");
        }
    }
    
    [ContextMenu("Test Checkpoint Positions")]
    public void TestCheckpointPositions()
    {
        if (checkpointManager == null)
        {
            Debug.LogError("✗ Test: CheckpointManager not found!");
            return;
        }
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("✗ Test: Player not found!");
            return;
        }
        
        Debug.Log("=== Testing Checkpoint Positions ===");
        
        Vector3 currentPos = checkpointManager.GetCurrentCheckpointPosition();
        Quaternion currentRot = checkpointManager.GetCurrentCheckpointRotation();
        string currentName = checkpointManager.GetCurrentCheckpointName();
        
        Debug.Log($"Current Checkpoint: {currentName}");
        Debug.Log($"Expected Spawn Position: {currentPos}");
        Debug.Log($"Expected Spawn Rotation: {currentRot.eulerAngles}");
        Debug.Log($"Player Current Position: {player.transform.position}");
        
        float distance = Vector3.Distance(player.transform.position, currentPos);
        Debug.Log($"Distance from spawn point: {distance:F2} units");
        
        if (distance > 10f)
        {
            Debug.LogWarning("⚠️ Player seems far from expected checkpoint position!");
        }
        else
        {
            Debug.Log("✅ Player position seems correct relative to checkpoint");
        }
        
        Debug.Log("===================================");
    }
    
    [ContextMenu("Print System Information")]
    public void PrintSystemInfo()
    {
        Debug.Log("=== Death System Information ===");
        
        // Player Health Info
        if (playerHealth != null)
        {
            Debug.Log($"Player Health: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
            Debug.Log($"Player Is Dead: {playerHealth.IsDead}");
            Debug.Log($"Player Invulnerable: {playerHealth.IsInvulnerable}");
        }
        else
        {
            Debug.Log("Player Health: NOT FOUND");
        }
        
        // Checkpoint Info
        if (checkpointManager != null)
        {
            checkpointManager.PrintCheckpointInfo();
            checkpointManager.VerifyCheckpointPositions();
        }
        else
        {
            Debug.Log("CheckpointManager: NOT FOUND");
        }
        
        // Death Manager Info
        if (deathManager != null)
        {
            Debug.Log($"Player Currently Dead: {deathManager.IsPlayerDead()}");
        }
        else
        {
            Debug.Log("DeathManager: NOT FOUND");
        }
        
        // Scene Objects Count
        int rockCount = FindObjectsOfType<RockCollisionHandler>().Length;
        int checkpointCount = FindObjectsOfType<AreaCheckpointTrigger>().Length;
        
        Debug.Log($"Active Rocks: {rockCount}");
        Debug.Log($"Area Checkpoint Triggers: {checkpointCount}");
        
        Debug.Log("================================");
    }
    
    [ContextMenu("Test Checkpoint Sequence")]
    public void TestCheckpointSequence()
    {
        StartCoroutine(CheckpointSequenceTest());
    }
    
    IEnumerator CheckpointSequenceTest()
    {
        Debug.Log("Starting checkpoint sequence test...");
        
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"Setting checkpoint {i}");
            checkpointManager?.SetCurrentCheckpoint(i);
            
            yield return new WaitForSeconds(2f);
            
            Debug.Log($"Testing death at checkpoint {i}");
            TestKillPlayer();
            
            yield return new WaitForSeconds(4f); // Wait for respawn
        }
        
        Debug.Log("Checkpoint sequence test completed!");
    }
    
    [ContextMenu("Stress Test - Multiple Rocks")]
    public void StressTestMultipleRocks()
    {
        StartCoroutine(SpawnMultipleRocks());
    }
    
    IEnumerator SpawnMultipleRocks()
    {
        Debug.Log("Starting multiple rock stress test...");
        
        for (int i = 0; i < 10; i++)
        {
            TestSpawnRock();
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("Multiple rock test completed!");
    }
    
    void OnGUI()
    {
        if (!enableKeyboardTesting) return;
        
        // Simple on-screen info
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Death System Test");
        GUILayout.Space(5);
        
        if (playerHealth != null)
        {
            GUILayout.Label($"Health: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
            GUILayout.Label($"Dead: {playerHealth.IsDead}");
        }
        
        if (checkpointManager != null)
        {
            GUILayout.Label($"Checkpoint: {checkpointManager.GetCurrentCheckpointName()}");
        }
        
        GUILayout.Space(5);
        GUILayout.Label($"[{killPlayerKey}] Kill Player");
        GUILayout.Label($"[{spawnRockKey}] Spawn Rock");
        GUILayout.Label($"[{damagePlayerKey}] Damage");
        GUILayout.Label($"[{invectorStatusKey}] Check Invector");
        GUILayout.Label($"[{forceEnableInvectorKey}] Force Enable");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}