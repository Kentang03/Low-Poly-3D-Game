using UnityEngine;
using UnityEditor;
using Invector.vCharacterController;

/// <summary>
/// Setup wizard untuk membuat spawn system dengan cepat
/// </summary>
public class SpawnSystemSetupWizard : EditorWindow
{
    [MenuItem("Tools/Kiro/Spawn System Setup Wizard")]
    public static void ShowWindow()
    {
        SpawnSystemSetupWizard window = GetWindow<SpawnSystemSetupWizard>();
        window.titleContent = new GUIContent("Spawn System Setup");
        window.minSize = new Vector2(400, 600);
        window.Show();
    }
    
    private Vector3 spawnPosition = Vector3.zero;
    private Vector3 spawnRotation = Vector3.zero;
    private string spawnPointName = "Player Spawn Point";
    private bool createSpawnManager = true;
    private bool createSceneInitializer = true;
    private bool setupExistingPlayer = true;
    
    void OnGUI()
    {
        GUILayout.Label("Player Spawn System Setup Wizard", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This wizard will help you set up the player spawn system for your scene. " +
            "Make sure you have a player with vThirdPersonController in your scene first.", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        // Spawn Point Settings
        GUILayout.Label("Spawn Point Settings", EditorStyles.boldLabel);
        spawnPointName = EditorGUILayout.TextField("Spawn Point Name", spawnPointName);
        spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", spawnPosition);
        spawnRotation = EditorGUILayout.Vector3Field("Spawn Rotation", spawnRotation);
        
        EditorGUILayout.Space();
        
        // Setup Options
        GUILayout.Label("Setup Options", EditorStyles.boldLabel);
        createSpawnManager = EditorGUILayout.Toggle("Create Spawn Manager", createSpawnManager);
        createSceneInitializer = EditorGUILayout.Toggle("Create Scene Initializer", createSceneInitializer);
        setupExistingPlayer = EditorGUILayout.Toggle("Setup Existing Player", setupExistingPlayer);
        
        EditorGUILayout.Space();
        
        // Current Scene Analysis
        GUILayout.Label("Current Scene Analysis", EditorStyles.boldLabel);
        AnalyzeCurrentScene();
        
        EditorGUILayout.Space();
        
        // Action Buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Auto Setup Complete System", GUILayout.Height(30)))
        {
            SetupCompleteSpawnSystem();
        }
        
        if (GUILayout.Button("Create Spawn Point Only", GUILayout.Height(30)))
        {
            CreateSpawnPointOnly();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Validate Current Setup"))
        {
            ValidateCurrentSetup();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "After setup, test the spawn system by playing the scene. " +
            "The player should spawn at the spawn point and be able to move immediately.", 
            MessageType.Info);
    }
    
    void AnalyzeCurrentScene()
    {
        // Check for existing components
        PlayerSpawnManager spawnManager = FindObjectOfType<PlayerSpawnManager>();
        GameplaySceneInitializer sceneInit = FindObjectOfType<GameplaySceneInitializer>();
        vThirdPersonController player = FindObjectOfType<vThirdPersonController>();
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        
        EditorGUILayout.LabelField("Existing Components:");
        
        string spawnManagerStatus = spawnManager != null ? "✅ Found" : "❌ Missing";
        string sceneInitStatus = sceneInit != null ? "✅ Found" : "❌ Missing";
        string playerStatus = player != null ? "✅ Found" : "❌ Missing";
        string spawnPointStatus = spawnPoint != null ? "✅ Found" : "❌ Missing";
        
        EditorGUILayout.LabelField($"• Spawn Manager: {spawnManagerStatus}");
        EditorGUILayout.LabelField($"• Scene Initializer: {sceneInitStatus}");
        EditorGUILayout.LabelField($"• Player Controller: {playerStatus}");
        EditorGUILayout.LabelField($"• Spawn Point: {spawnPointStatus}");
        
        if (player != null)
        {
            EditorGUILayout.LabelField($"• Player Position: {player.transform.position}");
            
            if (GUILayout.Button("Use Current Player Position as Spawn"))
            {
                spawnPosition = player.transform.position;
                spawnRotation = player.transform.eulerAngles;
            }
        }
    }
    
    void SetupCompleteSpawnSystem()
    {
        Debug.Log("=== SETTING UP COMPLETE SPAWN SYSTEM ===");
        
        // 1. Create Spawn Point
        GameObject spawnPoint = CreateSpawnPoint();
        
        // 2. Create Spawn Manager
        GameObject spawnManager = null;
        if (createSpawnManager)
        {
            spawnManager = CreateSpawnManager(spawnPoint);
        }
        
        // 3. Create Scene Initializer
        if (createSceneInitializer)
        {
            CreateSceneInitializer();
        }
        
        // 4. Setup existing player
        if (setupExistingPlayer)
        {
            SetupExistingPlayer();
        }
        
        // 5. Configure GameManager
        ConfigureGameManager(spawnManager?.GetComponent<PlayerSpawnManager>());
        
        Debug.Log("✅ Complete spawn system setup finished!");
        EditorUtility.DisplayDialog("Setup Complete", 
            "Spawn system has been set up successfully!\n\n" +
            "Test by playing the scene - the player should spawn at the spawn point and be able to move immediately.", 
            "OK");
    }
    
    GameObject CreateSpawnPoint()
    {
        GameObject spawnPoint = new GameObject(spawnPointName);
        spawnPoint.transform.position = spawnPosition;
        spawnPoint.transform.eulerAngles = spawnRotation;
        spawnPoint.tag = "SpawnPoint";
        
        SpawnPoint spawnComponent = spawnPoint.AddComponent<SpawnPoint>();
        spawnComponent.spawnPointName = spawnPointName;
        spawnComponent.isDefaultSpawn = true;
        
        Selection.activeGameObject = spawnPoint;
        
        Debug.Log($"✅ Created spawn point: {spawnPointName}");
        return spawnPoint;
    }
    
    GameObject CreateSpawnManager(GameObject spawnPoint)
    {
        // Check if already exists
        PlayerSpawnManager existing = FindObjectOfType<PlayerSpawnManager>();
        if (existing != null)
        {
            Debug.Log("⚠️ PlayerSpawnManager already exists, updating reference");
            if (spawnPoint != null)
                existing.defaultSpawnPoint = spawnPoint.transform;
            return existing.gameObject;
        }
        
        GameObject spawnManager = new GameObject("Player Spawn Manager");
        PlayerSpawnManager spawnComponent = spawnManager.AddComponent<PlayerSpawnManager>();
        
        if (spawnPoint != null)
            spawnComponent.defaultSpawnPoint = spawnPoint.transform;
        
        // Try to find player components
        vThirdPersonController player = FindObjectOfType<vThirdPersonController>();
        if (player != null)
        {
            spawnComponent.playerController = player;
            spawnComponent.playerInput = player.GetComponent<vThirdPersonInput>();
        }
        
        Debug.Log("✅ Created PlayerSpawnManager");
        return spawnManager;
    }
    
    void CreateSceneInitializer()
    {
        // Check if already exists
        GameplaySceneInitializer existing = FindObjectOfType<GameplaySceneInitializer>();
        if (existing != null)
        {
            Debug.Log("⚠️ GameplaySceneInitializer already exists");
            return;
        }
        
        GameObject initializer = new GameObject("Gameplay Scene Initializer");
        GameplaySceneInitializer initComponent = initializer.AddComponent<GameplaySceneInitializer>();
        
        initComponent.autoInitializeOnStart = true;
        initComponent.initializationDelay = 0.2f;
        initComponent.ensurePlayerCanMove = true;
        
        Debug.Log("✅ Created GameplaySceneInitializer");
    }
    
    void SetupExistingPlayer()
    {
        vThirdPersonController player = FindObjectOfType<vThirdPersonController>();
        if (player == null)
        {
            Debug.LogWarning("⚠️ No vThirdPersonController found in scene");
            return;
        }
        
        // Ensure player has required components
        vThirdPersonInput input = player.GetComponent<vThirdPersonInput>();
        if (input == null)
        {
            Debug.LogWarning("⚠️ Player missing vThirdPersonInput component");
        }
        
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("⚠️ Player missing Rigidbody component");
        }
        else
        {
            // Set proper rigidbody constraints
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Ensure player has Player tag
        if (!player.CompareTag("Player"))
        {
            player.tag = "Player";
        }
        
        Debug.Log("✅ Existing player setup completed");
    }
    
    void ConfigureGameManager(PlayerSpawnManager spawnManager)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogWarning("⚠️ No GameManager found in scene");
            return;
        }
        
        if (spawnManager != null)
        {
            gameManager.playerSpawnManager = spawnManager;
        }
        
        // Update player references
        vThirdPersonController player = FindObjectOfType<vThirdPersonController>();
        if (player != null)
        {
            gameManager.vThirdPersonController = player;
            gameManager.vThirdPersonInput = player.GetComponent<vThirdPersonInput>();
        }
        
        Debug.Log("✅ GameManager configured with spawn system");
    }
    
    void CreateSpawnPointOnly()
    {
        CreateSpawnPoint();
        
        EditorUtility.DisplayDialog("Spawn Point Created", 
            $"Spawn point '{spawnPointName}' has been created at position {spawnPosition}.\n\n" +
            "You can now manually set up other components or run the complete setup later.", 
            "OK");
    }
    
    void ValidateCurrentSetup()
    {
        Debug.Log("=== SPAWN SYSTEM VALIDATION ===");
        
        bool valid = true;
        
        // Check spawn point
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            Debug.Log("✅ Spawn Point found: " + spawnPoint.name);
        }
        else
        {
            Debug.LogError("❌ No spawn point found with 'SpawnPoint' tag");
            valid = false;
        }
        
        // Check spawn manager
        PlayerSpawnManager spawnManager = FindObjectOfType<PlayerSpawnManager>();
        if (spawnManager != null)
        {
            Debug.Log("✅ PlayerSpawnManager found");
            if (spawnManager.defaultSpawnPoint == null)
            {
                Debug.LogWarning("⚠️ SpawnManager has no default spawn point assigned");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No PlayerSpawnManager found");
        }
        
        // Check player controller
        vThirdPersonController player = FindObjectOfType<vThirdPersonController>();
        if (player != null)
        {
            Debug.Log("✅ Player controller found");
            
            vThirdPersonInput input = player.GetComponent<vThirdPersonInput>();
            if (input != null)
            {
                Debug.Log("✅ Player input found");
            }
            else
            {
                Debug.LogError("❌ Player missing vThirdPersonInput component");
                valid = false;
            }
        }
        else
        {
            Debug.LogError("❌ No vThirdPersonController found in scene");
            valid = false;
        }
        
        // Check game manager
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            Debug.Log("✅ GameManager found");
        }
        else
        {
            Debug.LogWarning("⚠️ No GameManager found");
        }
        
        if (valid)
        {
            Debug.Log("🎉 SPAWN SYSTEM VALIDATION PASSED!");
        }
        else
        {
            Debug.Log("❌ SPAWN SYSTEM VALIDATION FAILED - Check errors above");
        }
    }
}