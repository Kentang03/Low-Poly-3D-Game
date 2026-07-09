using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneSystemInstaller : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnAwake = true;
    public bool findMainCameraAutomatically = true;
    public bool findPlayerControllerAutomatically = true;
    
    [Header("Manual References")]
    public Camera mainCamera;
    public MonoBehaviour playerController;
    
    [Header("Fade Settings")]
    public Color fadeColor = Color.black;
    public float defaultFadeDuration = 1f;
    
    private void Awake()
    {
        if (setupOnAwake)
        {
            SetupCutsceneSystem();
        }
    }

    [ContextMenu("Setup Cutscene System")]
    public void SetupCutsceneSystem()
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

        // Auto-find main camera
        if (findMainCameraAutomatically && mainCamera == null)
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam.CompareTag("MainCamera") || cam.name.Contains("Main"))
                {
                    mainCamera = cam;
                    break;
                }
            }
            
            // Fallback to first active camera
            if (mainCamera == null && cameras.Length > 0)
            {
                mainCamera = cameras[0];
            }
        }

        // Auto-find player controller
        if (findPlayerControllerAutomatically && playerController == null)
        {
            // Try to find various player controller scripts
            var controllers = FindObjectsOfType<MonoBehaviour>();
            foreach (var ctrl in controllers)
            {
                string typeName = ctrl.GetType().Name;
                if (typeName.Contains("ThirdPerson") || 
                    typeName.Contains("PlayerController") || 
                    typeName.Contains("PlayerMovement") ||
                    typeName == "vThirdPersonController")
                {
                    playerController = ctrl;
                    break;
                }
            }
            
            // Fallback - look for objects with "Player" tag
            if (playerController == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    // Try common controller components
                    var characterController = player.GetComponent<CharacterController>();
                    if (characterController != null)
                    {
                        // Find a MonoBehaviour that likely controls movement
                        var monoBehaviours = player.GetComponents<MonoBehaviour>();
                        foreach (var mb in monoBehaviours)
                        {
                            string typeName = mb.GetType().Name;
                            if (typeName.Contains("Controller") || 
                                typeName.Contains("Movement") || 
                                typeName.Contains("Player"))
                            {
                                playerController = mb;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Assign references
        manager.mainCamera = mainCamera;
        manager.playerController = playerController;
        manager.fadeTransition = fade;

        // Configure fade transition
        fade.fadeColor = fadeColor;
        fade.defaultFadeDuration = defaultFadeDuration;

        Debug.Log("Cutscene system setup completed!");
        
        // Log what was found/assigned
        Debug.Log($"Main Camera: {(mainCamera ? mainCamera.name : "None")}");
        Debug.Log($"Player Controller: {(playerController ? playerController.name : "None")}");
    }

    [ContextMenu("Create Sample Cutscene")]
    public void CreateSampleCutscene()
    {
        SetupCutsceneSystem();
        
        CutsceneManager manager = CutsceneManager.Instance;
        if (manager == null)
        {
            Debug.LogError("Failed to create CutsceneManager!");
            return;
        }

        // Create sample cutscene
        GameObject cutsceneGO = new GameObject("SampleCutscene");
        
        // Add Timeline
        var director = cutsceneGO.AddComponent<UnityEngine.Playables.PlayableDirector>();
        
        // Create cutscene camera
        GameObject cameraGO = new GameObject("SampleCutsceneCamera");
        cameraGO.transform.SetParent(cutsceneGO.transform);
        
        // Position camera in front of main camera
        if (mainCamera != null)
        {
            cameraGO.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 5f;
            cameraGO.transform.LookAt(mainCamera.transform.position);
        }
        
        Camera cutsceneCam = cameraGO.AddComponent<Camera>();
        CutsceneCamera cutsceneCamScript = cameraGO.AddComponent<CutsceneCamera>();
        
        // Setup camera
        cutsceneCam.enabled = false;
        cameraGO.SetActive(false);

        // Add to manager
        var cutsceneData = new CutsceneManager.CutsceneData
        {
            cutsceneName = "SampleCutscene",
            timeline = director,
            cutsceneCamera = cutsceneCam,
            disablePlayerControl = true,
            fadeInDuration = 1f,
            fadeOutDuration = 1f
        };
        
        manager.cutscenes.Add(cutsceneData);

        // Create trigger
        GameObject triggerGO = new GameObject("SampleCutsceneTrigger");
        BoxCollider triggerCollider = triggerGO.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(3, 3, 3);
        
        CutsceneTrigger trigger = triggerGO.AddComponent<CutsceneTrigger>();
        trigger.cutsceneName = "SampleCutscene";
        trigger.triggerKey = KeyCode.E;
        trigger.promptText = "Press E to start cutscene";

        // Position trigger near main camera
        if (mainCamera != null)
        {
            triggerGO.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 3f;
        }

        Debug.Log("Sample cutscene created! Walk to the trigger and press E to test.");
        
        // Select the cutscene in hierarchy (only in editor)
        #if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = cutsceneGO;
        #endif
    }

    private void OnValidate()
    {
        // Validate references in editor
        if (mainCamera == null && findMainCameraAutomatically)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                mainCamera = cam;
            }
        }
    }

    [System.Serializable]
    public class CutscenePreset
    {
        public string name;
        public float fadeInDuration = 1f;
        public float fadeOutDuration = 1f;
        public Color fadeColor = Color.black;
        public bool disablePlayerControl = true;
    }

    [Header("Presets")]
    public CutscenePreset[] presets = new CutscenePreset[]
    {
        new CutscenePreset { name = "Quick", fadeInDuration = 0.5f, fadeOutDuration = 0.5f },
        new CutscenePreset { name = "Normal", fadeInDuration = 1f, fadeOutDuration = 1f },
        new CutscenePreset { name = "Slow", fadeInDuration = 2f, fadeOutDuration = 2f },
        new CutscenePreset { name = "WhiteFade", fadeInDuration = 1f, fadeOutDuration = 1f, fadeColor = Color.white }
    };

    public CutscenePreset GetPreset(string presetName)
    {
        foreach (var preset in presets)
        {
            if (preset.name == presetName)
                return preset;
        }
        return presets[0]; // Return first as default
    }
}