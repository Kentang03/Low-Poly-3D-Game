using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Wizard untuk setup otomatis sistem Kancil Guide
/// </summary>
public class KancilSetupWizard : MonoBehaviour
{
    [Header("Setup Configuration")]
    [SerializeField] private GameObject kancilModelPrefab;
    [SerializeField] private RuntimeAnimatorController kancilAnimatorController;
    [SerializeField] private Transform playerTransform;
    
    [Header("Checkpoint Setup")]
    [SerializeField] private bool createSampleCheckpoints = true;
    [SerializeField] private int numberOfCheckpoints = 5;
    [SerializeField] private float checkpointSpacing = 10f;
    [SerializeField] private CheckpointPattern checkpointPattern = CheckpointPattern.Circle;
    
    [Header("NavMesh Settings")]
    [SerializeField] private bool validateNavMesh = true;
    [SerializeField] private float navMeshSampleDistance = 2f;
    
    public enum CheckpointPattern
    {
        Line,
        Circle,
        Square,
        Random
    }
    
    /// <summary>
    /// Setup lengkap sistem Kancil Guide
    /// </summary>
    [ContextMenu("Setup Complete Kancil System")]
    public void SetupCompleteSystem()
    {
        Debug.Log("Starting Kancil Guide setup...");
        
        // 1. Create atau configure kancil GameObject
        GameObject kancilObj = SetupKancilGameObject();
        if (kancilObj == null)
        {
            Debug.LogError("Failed to setup Kancil GameObject");
            return;
        }
        
        // 2. Setup components
        SetupKancilComponents(kancilObj);
        
        // 3. Create checkpoints
        if (createSampleCheckpoints)
        {
            CreateCheckpoints(kancilObj);
        }
        
        // 4. Setup manager
        SetupManager(kancilObj);
        
        Debug.Log("Kancil Guide setup completed!");
    }
    
    private GameObject SetupKancilGameObject()
    {
        GameObject kancilObj = null;
        
        // Cek apakah sudah ada kancil di scene
        KancilGuide existingKancil = FindObjectOfType<KancilGuide>();
        if (existingKancil != null)
        {
            kancilObj = existingKancil.gameObject;
            Debug.Log("Using existing KancilGuide in scene");
        }
        else if (kancilModelPrefab != null)
        {
            // Instantiate dari prefab
            kancilObj = Instantiate(kancilModelPrefab);
            kancilObj.name = "Kancil_Guide";
            Debug.Log("Created Kancil from prefab");
        }
        else
        {
            // Buat GameObject baru
            kancilObj = new GameObject("Kancil_Guide");
            
            // Coba cari model kancil di folder Character
            GameObject modelObj = FindKancilModel();
            if (modelObj != null)
            {
                GameObject modelInstance = Instantiate(modelObj, kancilObj.transform);
                modelInstance.name = "Model";
                Debug.Log("Added Kancil model from Character folder");
            }
            else
            {
                // Buat placeholder cube
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.transform.SetParent(kancilObj.transform);
                placeholder.transform.localPosition = Vector3.zero;
                placeholder.transform.localScale = new Vector3(0.5f, 1f, 1f);
                
                // Beri warna khusus
                Renderer renderer = placeholder.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.color = new Color(0.8f, 0.6f, 0.4f); // Warna coklat untuk kancil
                    renderer.material = mat;
                }
                
                Debug.LogWarning("Created placeholder model for Kancil");
            }
        }
        
        return kancilObj;
    }
    
    private GameObject FindKancilModel()
    {
        // Cari model di folder Character
        string[] searchPaths = {
            "Assets/3D MOdel/Character/Model",
            "Assets/3D MOdel/Character",
            "Assets/3D MOdel"
        };
        
        foreach (string path in searchPaths)
        {
            GameObject[] models = Resources.LoadAll<GameObject>(path);
            foreach (GameObject model in models)
            {
                if (model.name.ToLower().Contains("character") || 
                    model.name.ToLower().Contains("kancil") ||
                    model.name.ToLower().Contains("deer"))
                {
                    return model;
                }
            }
        }
        
        return null;
    }
    
    private void SetupKancilComponents(GameObject kancilObj)
    {
        // 1. NavMeshAgent
        NavMeshAgent agent = kancilObj.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = kancilObj.AddComponent<NavMeshAgent>();
        }
        
        // Configure NavMeshAgent untuk kancil
        agent.speed = 3f;
        agent.acceleration = 8f;
        agent.angularSpeed = 180f;
        agent.stoppingDistance = 0.5f;
        agent.radius = 0.5f;
        agent.height = 1f;
        agent.baseOffset = 0f;
        
        // 2. KancilGuide
        KancilGuide guide = kancilObj.GetComponent<KancilGuide>();
        if (guide == null)
        {
            guide = kancilObj.AddComponent<KancilGuide>();
        }
        
        // Set player reference
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
        
        if (playerTransform != null)
        {
            // Use reflection atau SerializedObject untuk set private field
            var field = typeof(KancilGuide).GetField("player", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(guide, playerTransform);
            }
        }
        
        // 3. Animator
        Animator animator = kancilObj.GetComponent<Animator>();
        if (animator == null)
        {
            animator = kancilObj.AddComponent<Animator>();
        }
        
        if (kancilAnimatorController != null)
        {
            animator.runtimeAnimatorController = kancilAnimatorController;
        }
        else
        {
            // Cari animator controller otomatis
            RuntimeAnimatorController foundController = FindKancilAnimatorController();
            if (foundController != null)
            {
                animator.runtimeAnimatorController = foundController;
                Debug.Log($"Auto-assigned animator controller: {foundController.name}");
            }
        }
        
        // 4. KancilAnimationController
        KancilAnimationController animController = kancilObj.GetComponent<KancilAnimationController>();
        if (animController == null)
        {
            animController = kancilObj.AddComponent<KancilAnimationController>();
        }
        
        Debug.Log("Kancil components setup completed");
    }
    
    private RuntimeAnimatorController FindKancilAnimatorController()
    {
        // // Cari di folder Animation
        // string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AnimatorController", 
        //     new[] { "Assets/3D MOdel/Character/Animation" });
            
        // foreach (string guid in guids)
        // {
        //     string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        //     RuntimeAnimatorController controller = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            
        //     if (controller != null)
        //     {
        //         return controller;
        //     }
        // }
        
        return null;
    }
    
    private void CreateCheckpoints(GameObject kancilObj)
    {
        Vector3 kancilPos = kancilObj.transform.position;
        List<Vector3> checkpointPositions = GenerateCheckpointPositions(kancilPos);
        
        GameObject checkpointParent = new GameObject("Kancil_Checkpoints");
        List<KancilCheckpoint> checkpoints = new List<KancilCheckpoint>();
        
        for (int i = 0; i < checkpointPositions.Count; i++)
        {
            Vector3 position = checkpointPositions[i];
            
            // Validate position with NavMesh
            if (validateNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                {
                    position = hit.position;
                }
                else
                {
                    Debug.LogWarning($"Checkpoint {i + 1} is not on NavMesh, using original position");
                }
            }
            
            // Create checkpoint
            GameObject checkpointObj = new GameObject($"Checkpoint_{i + 1:D2}");
            checkpointObj.transform.position = position;
            checkpointObj.transform.SetParent(checkpointParent.transform);
            
            KancilCheckpoint checkpoint = checkpointObj.AddComponent<KancilCheckpoint>();
            checkpoint.CheckpointName = $"Checkpoint {i + 1}";
            checkpoint.waitTime = Random.Range(1f, 3f);
            
            checkpoints.Add(checkpoint);
        }
        
        // Assign checkpoints ke KancilGuide
        KancilGuide guide = kancilObj.GetComponent<KancilGuide>();
        if (guide != null)
        {
            guide.SetCheckpoints(checkpoints);
        }
        
        Debug.Log($"Created {checkpoints.Count} checkpoints in {checkpointPattern} pattern");
    }
    
    private List<Vector3> GenerateCheckpointPositions(Vector3 center)
    {
        List<Vector3> positions = new List<Vector3>();
        
        switch (checkpointPattern)
        {
            case CheckpointPattern.Line:
                for (int i = 0; i < numberOfCheckpoints; i++)
                {
                    Vector3 pos = center + Vector3.forward * (checkpointSpacing * (i + 1));
                    positions.Add(pos);
                }
                break;
                
            case CheckpointPattern.Circle:
                float angleStep = 360f / numberOfCheckpoints;
                for (int i = 0; i < numberOfCheckpoints; i++)
                {
                    float angle = angleStep * i * Mathf.Deg2Rad;
                    Vector3 pos = center + new Vector3(
                        Mathf.Cos(angle) * checkpointSpacing,
                        0f,
                        Mathf.Sin(angle) * checkpointSpacing
                    );
                    positions.Add(pos);
                }
                break;
                
            case CheckpointPattern.Square:
                int perSide = Mathf.CeilToInt(numberOfCheckpoints / 4f);
                float halfSize = checkpointSpacing;
                
                // Top side
                for (int i = 0; i < perSide && positions.Count < numberOfCheckpoints; i++)
                {
                    float t = (float)i / (perSide - 1);
                    Vector3 pos = center + new Vector3(Mathf.Lerp(-halfSize, halfSize, t), 0f, halfSize);
                    positions.Add(pos);
                }
                
                // Right side
                for (int i = 1; i < perSide && positions.Count < numberOfCheckpoints; i++)
                {
                    float t = (float)i / (perSide - 1);
                    Vector3 pos = center + new Vector3(halfSize, 0f, Mathf.Lerp(halfSize, -halfSize, t));
                    positions.Add(pos);
                }
                
                // Bottom side
                for (int i = 1; i < perSide && positions.Count < numberOfCheckpoints; i++)
                {
                    float t = (float)i / (perSide - 1);
                    Vector3 pos = center + new Vector3(Mathf.Lerp(halfSize, -halfSize, t), 0f, -halfSize);
                    positions.Add(pos);
                }
                
                // Left side
                for (int i = 1; i < perSide - 1 && positions.Count < numberOfCheckpoints; i++)
                {
                    float t = (float)i / (perSide - 1);
                    Vector3 pos = center + new Vector3(-halfSize, 0f, Mathf.Lerp(-halfSize, halfSize, t));
                    positions.Add(pos);
                }
                break;
                
            case CheckpointPattern.Random:
                for (int i = 0; i < numberOfCheckpoints; i++)
                {
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-checkpointSpacing, checkpointSpacing),
                        0f,
                        Random.Range(-checkpointSpacing, checkpointSpacing)
                    );
                    positions.Add(center + randomOffset);
                }
                break;
        }
        
        return positions;
    }
    
    private void SetupManager(GameObject kancilObj)
    {
        // Cari atau buat KancilGuideManager
        KancilGuideManager manager = FindObjectOfType<KancilGuideManager>();
        
        if (manager == null)
        {
            GameObject managerObj = new GameObject("KancilGuide_Manager");
            manager = managerObj.AddComponent<KancilGuideManager>();
        }
        
        // Assign kancil guide reference
        var field = typeof(KancilGuideManager).GetField("kancilGuide",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            KancilGuide guide = kancilObj.GetComponent<KancilGuide>();
            field.SetValue(manager, guide);
        }
        
        Debug.Log("KancilGuideManager setup completed");
    }
    
    /// <summary>
    /// Validate setup sebelum menjalankan
    /// </summary>
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        Debug.Log("Validating Kancil Guide setup...");
        
        List<string> issues = new List<string>();
        List<string> warnings = new List<string>();
        
        // Check NavMesh
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            issues.Add("No NavMesh found near current position");
        }
        
        // Check player
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                warnings.Add("No Player GameObject found with 'Player' tag");
            }
        }
        
        // Check kancil model
        if (kancilModelPrefab == null && FindKancilModel() == null)
        {
            warnings.Add("No Kancil model found, will use placeholder");
        }
        
        // Check animator
        if (kancilAnimatorController == null && FindKancilAnimatorController() == null)
        {
            warnings.Add("No Animator Controller found for Kancil");
        }
        
        // Display results
        string result = "Validation Results:\n\n";
        
        if (issues.Count == 0)
        {
            result += "✓ No critical issues found!\n\n";
        }
        else
        {
            result += "Critical Issues:\n";
            foreach (string issue in issues)
            {
                result += "• " + issue + "\n";
            }
            result += "\n";
        }
        
        if (warnings.Count > 0)
        {
            result += "Warnings:\n";
            foreach (string warning in warnings)
            {
                result += "• " + warning + "\n";
            }
        }
        
        Debug.Log(result);
    }
    
    /// <summary>
    /// Quick setup untuk testing
    /// </summary>
    [ContextMenu("Quick Test Setup")]
    public void QuickTestSetup()
    {
        createSampleCheckpoints = true;
        numberOfCheckpoints = 3;
        checkpointSpacing = 5f;
        checkpointPattern = CheckpointPattern.Line;
        
        SetupCompleteSystem();
        
        // Auto start guiding
        KancilGuide guide = FindObjectOfType<KancilGuide>();
        if (guide != null)
        {
            StartCoroutine(DelayedStart(guide));
        }
    }
    
    private IEnumerator DelayedStart(KancilGuide guide)
    {
        yield return new WaitForSeconds(1f);
        guide.StartGuiding();
        Debug.Log("Auto-started Kancil guiding for testing");
    }
}