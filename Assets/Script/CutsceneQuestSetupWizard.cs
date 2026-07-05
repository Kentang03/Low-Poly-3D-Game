using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneQuestSetupWizard : MonoBehaviour
{
    [Header("Setup Configuration")]
    public bool setupOnAwake = false;
    
    [Header("Cutscene Settings")]
    public string[] cutsceneNames = new string[]
    {
        "QuestIntro",
        "FirstItemFound",
        "SecondItemFound",
        "QuestComplete"
    };
    
    [Header("References (Auto-find if null)")]
    public Camera mainCamera;
    public MonoBehaviour playerController;
    public QuestSystem questSystem;
    
    [Header("Cutscene Positions")]
    public Transform[] cutsceneCameraPositions;
    
    [Header("Quest Integration")]
    public bool createIntroDelay = true;
    public float introDelay = 2f;
    public bool createCompletionCutscene = true;
    public float completionDelay = 1f;

    private void Awake()
    {
        if (setupOnAwake)
        {
            SetupComplete();
        }
    }

    [ContextMenu("Setup Complete Cutscene-Quest System")]
    public void SetupComplete()
    {
        Debug.Log("Starting Complete Cutscene-Quest System Setup...");
        
        // 1. Setup basic cutscene system
        SetupCutsceneSystem();
        
        // 2. Create cutscenes
        CreateCutscenes();
        
        // 3. Setup quest integration
        SetupQuestIntegration();
        
        // 4. Create test manager
        SetupTestManager();
        
        Debug.Log("Complete Cutscene-Quest System Setup finished!");
    }

    private void SetupCutsceneSystem()
    {
        // Find or create CutsceneManager
        CutsceneManager manager = CutsceneManager.Instance;
        if (manager == null)
        {
            GameObject managerGO = new GameObject("CutsceneManager");
            manager = managerGO.AddComponent<CutsceneManager>();
        }

        // Find or create FadeTransition
        FadeTransition fade = FadeTransition.Instance;
        if (fade == null)
        {
            GameObject fadeGO = new GameObject("FadeTransition");
            fade = fadeGO.AddComponent<FadeTransition>();
        }

        // Auto-find references
        FindReferences();

        // Configure manager
        manager.mainCamera = mainCamera;
        manager.playerController = playerController;
        manager.fadeTransition = fade;

        Debug.Log("✓ Cutscene System setup complete");
    }

    private void FindReferences()
    {
        // Find main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                foreach (var cam in cameras)
                {
                    if (cam.CompareTag("MainCamera"))
                    {
                        mainCamera = cam;
                        break;
                    }
                }
            }
        }

        // Find player controller
        if (playerController == null)
        {
            // Try to find vThirdPersonController from Invector
            var thirdPerson = FindObjectOfType<MonoBehaviour>();
            if (thirdPerson != null && thirdPerson.GetType().Name == "vThirdPersonController")
            {
                playerController = thirdPerson;
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    // Try various controller types
                    var controller = player.GetComponent<CharacterController>();
                    if (controller != null)
                    {
                        // Find a MonoBehaviour that controls the CharacterController
                        var controllers = player.GetComponents<MonoBehaviour>();
                        foreach (var ctrl in controllers)
                        {
                            if (ctrl.GetType().Name.Contains("Controller") || 
                                ctrl.GetType().Name.Contains("Player") ||
                                ctrl.GetType().Name.Contains("Movement"))
                            {
                                playerController = ctrl;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Find quest system
        if (questSystem == null)
        {
            questSystem = FindObjectOfType<QuestSystem>();
        }
    }

    private void CreateCutscenes()
    {
        CutsceneManager manager = CutsceneManager.Instance;
        if (manager == null) return;

        for (int i = 0; i < cutsceneNames.Length; i++)
        {
            CreateSingleCutscene(cutsceneNames[i], i);
        }

        Debug.Log($"✓ Created {cutsceneNames.Length} cutscenes");
    }

    private void CreateSingleCutscene(string cutsceneName, int index)
    {
        CutsceneManager manager = CutsceneManager.Instance;
        
        // Create timeline GameObject
        GameObject timelineGO = new GameObject($"Timeline_{cutsceneName}");
        PlayableDirector director = timelineGO.AddComponent<PlayableDirector>();
        
        // Create Timeline Asset
        TimelineAsset timeline = ScriptableObject.CreateInstance<TimelineAsset>();
        string assetPath = $"Assets/Cutscenes/Timeline_{cutsceneName}.playable";
        
        // Ensure directory exists
        if (!System.IO.Directory.Exists("Assets/Cutscenes"))
        {
            System.IO.Directory.CreateDirectory("Assets/Cutscenes");
        }
        
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(timeline, assetPath);
        #endif
        
        director.playableAsset = timeline;

        // Create cutscene camera
        GameObject cameraGO = new GameObject($"Camera_{cutsceneName}");
        cameraGO.transform.SetParent(timelineGO.transform);
        
        // Position camera
        PositionCutsceneCamera(cameraGO.transform, index);
        
        Camera cutsceneCam = cameraGO.AddComponent<Camera>();
        CutsceneCamera cutsceneCamScript = cameraGO.AddComponent<CutsceneCamera>();
        
        // Setup camera
        cutsceneCam.enabled = false;
        cameraGO.SetActive(false);

        // Add to manager
        var cutsceneData = new CutsceneManager.CutsceneData
        {
            cutsceneName = cutsceneName,
            timeline = director,
            cutsceneCamera = cutsceneCam,
            disablePlayerControl = true,
            fadeInDuration = 1f,
            fadeOutDuration = 1f
        };
        
        manager.cutscenes.Add(cutsceneData);
    }

    private void PositionCutsceneCamera(Transform cameraTransform, int index)
    {
        if (cutsceneCameraPositions != null && index < cutsceneCameraPositions.Length && cutsceneCameraPositions[index] != null)
        {
            // Use predefined position
            cameraTransform.position = cutsceneCameraPositions[index].position;
            cameraTransform.rotation = cutsceneCameraPositions[index].rotation;
        }
        else if (mainCamera != null)
        {
            // Default positioning based on main camera
            Vector3 offset = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(1f, 3f),
                Random.Range(3f, 8f)
            );
            
            cameraTransform.position = mainCamera.transform.position + offset;
            cameraTransform.LookAt(mainCamera.transform.position);
        }
        else
        {
            // Fallback positioning
            cameraTransform.position = new Vector3(0, 2, 5);
            cameraTransform.rotation = Quaternion.identity;
        }
    }

    private void SetupQuestIntegration()
    {
        if (questSystem == null)
        {
            Debug.LogWarning("QuestSystem not found, skipping quest integration");
            return;
        }

        // Create or find integration component
        QuestCutsceneIntegration integration = FindObjectOfType<QuestCutsceneIntegration>();
        if (integration == null)
        {
            GameObject integrationGO = new GameObject("QuestCutsceneIntegration");
            integration = integrationGO.AddComponent<QuestCutsceneIntegration>();
        }

        // Configure integration
        integration.questSystem = questSystem;
        integration.cutsceneManager = CutsceneManager.Instance;

        // Setup cutscene triggers
        var cutscenes = new System.Collections.Generic.List<QuestCutsceneIntegration.QuestCutsceneData>();

        // Quest intro cutscene
        if (cutsceneNames.Length > 0)
        {
            cutscenes.Add(new QuestCutsceneIntegration.QuestCutsceneData
            {
                eventType = QuestCutsceneIntegration.QuestEventType.QuestStarted,
                cutsceneName = cutsceneNames[0],
                delay = createIntroDelay ? introDelay : 0f,
                skipIfAlreadyPlayed = true
            });
        }

        // Item collection cutscenes
        for (int i = 1; i < cutsceneNames.Length - 1 && i < questSystem.questItems.Count + 1; i++)
        {
            cutscenes.Add(new QuestCutsceneIntegration.QuestCutsceneData
            {
                eventType = QuestCutsceneIntegration.QuestEventType.ItemCollected,
                cutsceneName = cutsceneNames[i],
                questItemIndex = i - 1, // Map to quest item index
                delay = 0.5f,
                skipIfAlreadyPlayed = true
            });
        }

        // Quest completion cutscene
        if (createCompletionCutscene && cutsceneNames.Length > 1)
        {
            cutscenes.Add(new QuestCutsceneIntegration.QuestCutsceneData
            {
                eventType = QuestCutsceneIntegration.QuestEventType.QuestCompleted,
                cutsceneName = cutsceneNames[cutsceneNames.Length - 1],
                delay = completionDelay,
                skipIfAlreadyPlayed = true
            });
        }

        integration.questCutscenes = cutscenes.ToArray();

        Debug.Log("✓ Quest-Cutscene Integration setup complete");
    }

    private void SetupTestManager()
    {
        // Create or find test manager
        CutsceneTestManager testManager = FindObjectOfType<CutsceneTestManager>();
        if (testManager == null)
        {
            GameObject testGO = new GameObject("CutsceneTestManager");
            testManager = testGO.AddComponent<CutsceneTestManager>();
        }

        Debug.Log("✓ Test Manager setup complete");
    }

    [ContextMenu("Create Example Camera Positions")]
    public void CreateExampleCameraPositions()
    {
        if (mainCamera == null)
        {
            FindReferences();
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found!");
                return;
            }
        }

        GameObject positionsParent = new GameObject("CutsceneCameraPositions");
        cutsceneCameraPositions = new Transform[cutsceneNames.Length];

        for (int i = 0; i < cutsceneNames.Length; i++)
        {
            GameObject posGO = new GameObject($"Position_{cutsceneNames[i]}");
            posGO.transform.SetParent(positionsParent.transform);
            
            // Position around the main camera
            float angle = (360f / cutsceneNames.Length) * i;
            float distance = 8f;
            
            Vector3 position = mainCamera.transform.position + new Vector3(
                Mathf.Sin(angle * Mathf.Deg2Rad) * distance,
                Random.Range(1f, 4f),
                Mathf.Cos(angle * Mathf.Deg2Rad) * distance
            );
            
            posGO.transform.position = position;
            posGO.transform.LookAt(mainCamera.transform.position);
            
            cutsceneCameraPositions[i] = posGO.transform;
            
            // Add visual indicator
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(posGO.transform);
            cube.transform.localScale = Vector3.one * 0.5f;
            cube.GetComponent<Renderer>().material.color = Color.yellow;
            
            // Remove collider
            DestroyImmediate(cube.GetComponent<Collider>());
        }

        Debug.Log($"✓ Created {cutsceneNames.Length} example camera positions");
    }

    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        FindReferences();
        
        Debug.Log("=== Cutscene-Quest System Validation ===");
        
        Debug.Log($"Main Camera: {(mainCamera ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"Player Controller: {(playerController ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"Quest System: {(questSystem ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"Cutscene Manager: {(CutsceneManager.Instance ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"Fade Transition: {(FadeTransition.Instance ? "✓ Found" : "✗ Missing")}");
        
        var integration = FindObjectOfType<QuestCutsceneIntegration>();
        Debug.Log($"Quest Integration: {(integration ? "✓ Found" : "✗ Missing")}");
        
        Debug.Log("=== Validation Complete ===");
    }
}