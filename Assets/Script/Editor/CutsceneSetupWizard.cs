using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class CutsceneSetupWizard : EditorWindow
{
    private string cutsceneName = "NewCutscene";
    private Camera mainCamera;
    private MonoBehaviour playerController;
    private bool createFadeTransition = true;
    private bool createCutsceneTrigger = true;
    
    [MenuItem("Tools/Cutscene/Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<CutsceneSetupWizard>("Cutscene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cutscene System Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        cutsceneName = EditorGUILayout.TextField("Cutscene Name", cutsceneName);
        mainCamera = (Camera)EditorGUILayout.ObjectField("Main Camera", mainCamera, typeof(Camera), true);
        playerController = (MonoBehaviour)EditorGUILayout.ObjectField("Player Controller", playerController, typeof(MonoBehaviour), true);
        
        EditorGUILayout.Space();
        
        createFadeTransition = EditorGUILayout.Toggle("Create Fade Transition", createFadeTransition);
        createCutsceneTrigger = EditorGUILayout.Toggle("Create Cutscene Trigger", createCutsceneTrigger);
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Create Cutscene System"))
        {
            CreateCutsceneSystem();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This wizard will create:\n" +
            "• CutsceneManager GameObject\n" +
            "• FadeTransition Canvas (if enabled)\n" +
            "• Timeline and Cutscene Camera\n" +
            "• Cutscene Trigger (if enabled)", 
            MessageType.Info);
    }

    private void CreateCutsceneSystem()
    {
        // Create CutsceneManager
        GameObject managerGO = new GameObject("CutsceneManager");
        CutsceneManager manager = managerGO.AddComponent<CutsceneManager>();
        manager.mainCamera = mainCamera;
        manager.playerController = playerController;

        // Create FadeTransition
        if (createFadeTransition)
        {
            GameObject fadeGO = new GameObject("FadeTransition");
            FadeTransition fade = fadeGO.AddComponent<FadeTransition>();
            manager.fadeTransition = fade;
        }

        // Create Timeline and Cutscene Camera
        GameObject timelineGO = new GameObject($"Timeline_{cutsceneName}");
        PlayableDirector director = timelineGO.AddComponent<PlayableDirector>();
        
        // Create Timeline Asset
        string timelinePath = $"Assets/Timeline_{cutsceneName}.playable";
        TimelineAsset timeline = TimelineAsset.CreateInstance<TimelineAsset>();
        AssetDatabase.CreateAsset(timeline, timelinePath);
        director.playableAsset = timeline;

        // Create Cutscene Camera
        GameObject cameraGO = new GameObject($"CutsceneCamera_{cutsceneName}");
        cameraGO.transform.SetParent(timelineGO.transform);
        Camera cutsceneCam = cameraGO.AddComponent<Camera>();
        CutsceneCamera cutsceneCamScript = cameraGO.AddComponent<CutsceneCamera>();
        
        // Setup camera
        cutsceneCam.enabled = false;
        cameraGO.SetActive(false);

        // Add cutscene to manager
        var cutsceneData = new CutsceneManager.CutsceneData();
        cutsceneData.cutsceneName = cutsceneName;
        cutsceneData.timeline = director;
        cutsceneData.cutsceneCamera = cutsceneCam;
        manager.cutscenes.Add(cutsceneData);

        // Create Cutscene Trigger
        if (createCutsceneTrigger)
        {
            GameObject triggerGO = new GameObject($"CutsceneTrigger_{cutsceneName}");
            BoxCollider triggerCollider = triggerGO.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector3(2, 3, 2);
            
            CutsceneTrigger trigger = triggerGO.AddComponent<CutsceneTrigger>();
            trigger.cutsceneName = cutsceneName;
        }

        EditorUtility.SetDirty(managerGO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Cutscene system '{cutsceneName}' created successfully!");
        
        // Select the manager in hierarchy
        Selection.activeGameObject = managerGO;
    }

    [MenuItem("Tools/Cutscene/Create Simple Cutscene")]
    public static void CreateSimpleCutscene()
    {
        ShowWindow();
    }
}