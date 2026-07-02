using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor utility untuk membuat prefab Kancil Guide
/// </summary>
public class KancilPrefabCreator : EditorWindow
{
    private GameObject kancilModel;
    private RuntimeAnimatorController animatorController;
    private string prefabName = "KancilGuide";
    private string prefabPath = "Assets/Prefabs/";
    
    [MenuItem("Tools/Kancil Guide/Prefab Creator")]
    public static void ShowWindow()
    {
        KancilPrefabCreator window = GetWindow<KancilPrefabCreator>("Kancil Prefab Creator");
        window.minSize = new Vector2(350, 400);
        window.Show();
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Kancil Guide Prefab Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);
        kancilModel = (GameObject)EditorGUILayout.ObjectField("Kancil Model", kancilModel, typeof(GameObject), false);
        animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(RuntimeAnimatorController), false);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Prefab Settings", EditorStyles.boldLabel);
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
        prefabPath = EditorGUILayout.TextField("Prefab Path", prefabPath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Auto-Find Assets"))
        {
            AutoFindAssets();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Prefab", GUILayout.Height(30)))
        {
            CreateKancilPrefab();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This will create a complete Kancil Guide prefab with all necessary components configured.", MessageType.Info);
        
        if (GUILayout.Button("Create Sample Scene"))
        {
            CreateSampleScene();
        }
    }
    
    private void AutoFindAssets()
    {
        // Find kancil model
        if (kancilModel == null)
        {
            string[] modelGuids = AssetDatabase.FindAssets("CharacterModel t:GameObject", new[] { "Assets/3D MOdel/Character" });
            
            foreach (string guid in modelGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (model != null && (model.name.ToLower().Contains("character") || model.name.ToLower().Contains("kancil")))
                {
                    kancilModel = model;
                    Debug.Log($"Auto-found kancil model: {model.name}");
                    break;
                }
            }
        }
        
        // Find animator controller
        if (animatorController == null)
        {
            string[] controllerGuids = AssetDatabase.FindAssets("t:AnimatorController", new[] { "Assets/3D MOdel/Character/Animation" });
            
            if (controllerGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(controllerGuids[0]);
                animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
                
                if (animatorController != null)
                {
                    Debug.Log($"Auto-found animator controller: {animatorController.name}");
                }
            }
        }
        
        Repaint();
    }
    
    private void CreateKancilPrefab()
    {
        // Validate inputs
        if (string.IsNullOrEmpty(prefabName))
        {
            EditorUtility.DisplayDialog("Error", "Prefab name cannot be empty!", "OK");
            return;
        }
        
        // Create prefab directory if it doesn't exist
        if (!Directory.Exists(prefabPath))
        {
            Directory.CreateDirectory(prefabPath);
        }
        
        // Create main GameObject
        GameObject kancilObj = new GameObject(prefabName);
        
        // Add model if available
        if (kancilModel != null)
        {
            GameObject modelInstance = PrefabUtility.InstantiatePrefab(kancilModel) as GameObject;
            if (modelInstance != null)
            {
                modelInstance.transform.SetParent(kancilObj.transform);
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = Quaternion.identity;
                modelInstance.transform.localScale = Vector3.one;
                modelInstance.name = "Model";
            }
        }
        else
        {
            // Create placeholder
            CreatePlaceholderModel(kancilObj);
        }
        
        // Add NavMeshAgent
        UnityEngine.AI.NavMeshAgent navAgent = kancilObj.AddComponent<UnityEngine.AI.NavMeshAgent>();
        ConfigureNavMeshAgent(navAgent);
        
        // Add Animator
        Animator animator = kancilObj.AddComponent<Animator>();
        if (animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }
        
        // Add KancilGuide
        KancilGuide kancilGuide = kancilObj.AddComponent<KancilGuide>();
        
        // Add KancilAnimationController
        KancilAnimationController animController = kancilObj.AddComponent<KancilAnimationController>();
        
        // Add KancilIntegrationHelper
        KancilIntegrationHelper integrationHelper = kancilObj.AddComponent<KancilIntegrationHelper>();
        
        // Create prefab
        string fullPrefabPath = Path.Combine(prefabPath, prefabName + ".prefab");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(kancilObj, fullPrefabPath);
        
        // Clean up scene object
        DestroyImmediate(kancilObj);
        
        // Select the created prefab
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
        
        Debug.Log($"Kancil prefab created: {fullPrefabPath}");
        EditorUtility.DisplayDialog("Success", $"Kancil prefab created successfully!\n\nPath: {fullPrefabPath}", "OK");
    }
    
    private void CreatePlaceholderModel(GameObject parent)
    {
        // Create body (cube)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(parent.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.6f, 1f, 1.2f);
        
        // Create head (sphere)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(parent.transform);
        head.transform.localPosition = new Vector3(0f, 0.8f, 0.3f);
        head.transform.localScale = new Vector3(0.4f, 0.4f, 0.6f);
        
        // Create legs
        for (int i = 0; i < 4; i++)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leg.name = $"Leg_{i + 1}";
            leg.transform.SetParent(parent.transform);
            
            float x = (i % 2 == 0) ? -0.2f : 0.2f;
            float z = (i < 2) ? 0.3f : -0.3f;
            
            leg.transform.localPosition = new Vector3(x, -0.7f, z);
            leg.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
        }
        
        // Apply kancil-like material
        Material kancilMat = new Material(Shader.Find("Standard"));
        kancilMat.color = new Color(0.8f, 0.6f, 0.4f, 1f); // Brown color
        kancilMat.name = "KancilPlaceholderMaterial";
        
        // Apply material to all renderers
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = kancilMat;
        }
        
        Debug.Log("Created placeholder kancil model");
    }
    
    private void ConfigureNavMeshAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        agent.speed = 3f;
        agent.acceleration = 8f;
        agent.angularSpeed = 180f;
        agent.stoppingDistance = 0.5f;
        agent.radius = 0.5f;
        agent.height = 1f;
        agent.baseOffset = 0f;
    }
    
    private void CreateSampleScene()
    {
        // Create new scene
        UnityEngine.SceneManagement.Scene newScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, 
            UnityEditor.SceneManagement.NewSceneMode.Single
        );
        
        // Create ground plane
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(10f, 1f, 10f);
        
        Material groundMat = new Material(Shader.Find("Standard"));
        groundMat.color = new Color(0.5f, 0.7f, 0.3f, 1f);
        ground.GetComponent<Renderer>().material = groundMat;
        
        // Bake NavMesh for the ground
        // Note: NavMeshSurface requires AI Navigation package to be installed
        // If you have AI Navigation package installed, uncomment the following lines:
        /*
        UnityEngine.AI.NavMeshSurface navSurface = ground.AddComponent<UnityEngine.AI.NavMeshSurface>();
        navSurface.BuildNavMesh();
        */
        
        // Alternative: Use legacy NavMesh baking
        // The NavMesh can be baked manually through Window > AI > Navigation
        
        // Create player (cube with Player tag)
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(-5f, 1f, 0f);
        player.transform.localScale = new Vector3(1f, 2f, 1f);
        
        Material playerMat = new Material(Shader.Find("Standard"));
        playerMat.color = Color.blue;
        player.GetComponent<Renderer>().material = playerMat;
        
        // Add CharacterController to player
        player.AddComponent<CharacterController>();
        
        // Instantiate Kancil prefab if it exists
        string prefabPath = Path.Combine(this.prefabPath, prefabName + ".prefab");
        GameObject kancilPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (kancilPrefab != null)
        {
            GameObject kancilInstance = PrefabUtility.InstantiatePrefab(kancilPrefab) as GameObject;
            kancilInstance.transform.position = new Vector3(0f, 0.5f, 0f);
        }
        else
        {
            Debug.LogWarning("Kancil prefab not found. Create the prefab first!");
        }
        
        // Create sample checkpoints
        CreateSampleCheckpoints();
        
        // Create KancilGuideManager
        GameObject manager = new GameObject("KancilGuideManager");
        manager.AddComponent<KancilGuideManager>();
        
        // Create Debug UI
        GameObject debugUI = new GameObject("KancilDebugUI");
        debugUI.AddComponent<KancilDebugUI>();
        
        // Save scene
        string scenePath = "Assets/Scenes/KancilGuideSample.unity";
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"Sample scene created: {scenePath}");
        EditorUtility.DisplayDialog("Success", $"Sample scene created!\n\nPath: {scenePath}", "OK");
    }
    
    private void CreateSampleCheckpoints()
    {
        GameObject checkpointParent = new GameObject("Sample_Checkpoints");
        
        Vector3[] positions = {
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 0f, 3f),
            new Vector3(0f, 0f, 3f),
            new Vector3(-3f, 0f, 3f),
            new Vector3(-3f, 0f, 0f)
        };
        
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject checkpointObj = new GameObject($"Checkpoint_{i + 1:D2}");
            checkpointObj.transform.position = positions[i];
            checkpointObj.transform.SetParent(checkpointParent.transform);
            
            KancilCheckpoint checkpoint = checkpointObj.AddComponent<KancilCheckpoint>();
            checkpoint.CheckpointName = $"Sample Checkpoint {i + 1}";
            checkpoint.waitTime = 2f;
            
            // Visual marker
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "Marker";
            marker.transform.SetParent(checkpointObj.transform);
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
            
            Material markerMat = new Material(Shader.Find("Standard"));
            markerMat.color = Color.green;
            marker.GetComponent<Renderer>().material = markerMat;
            
            // Remove collider to avoid interference
            DestroyImmediate(marker.GetComponent<Collider>());
        }
        
        Debug.Log($"Created {positions.Length} sample checkpoints");
    }
}