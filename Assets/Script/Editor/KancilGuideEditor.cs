using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Custom Editor untuk memudahkan setup KancilGuide system
/// </summary>
[CustomEditor(typeof(KancilGuide))]
public class KancilGuideEditor : Editor
{
    private KancilGuide kancilGuide;
    private SerializedProperty checkpointsProperty;
    private bool showCheckpoints = true;
    private bool showSettings = true;
    private bool showDebug = true;
    
    private void OnEnable()
    {
        kancilGuide = (KancilGuide)target;
        checkpointsProperty = serializedObject.FindProperty("checkpoints");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Kancil Guide System", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Settings Section
        showSettings = EditorGUILayout.Foldout(showSettings, "Movement & Animation Settings", true);
        if (showSettings)
        {
            EditorGUI.indentLevel++;
            DrawDefaultInspector();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        
        // Checkpoints Section
        showCheckpoints = EditorGUILayout.Foldout(showCheckpoints, $"Checkpoints ({checkpointsProperty.arraySize})", true);
        if (showCheckpoints)
        {
            EditorGUI.indentLevel++;
            DrawCheckpointsSection();
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Quick Setup Buttons
        DrawQuickSetupButtons();
        
        EditorGUILayout.Space();
        
        // Debug Section
        if (Application.isPlaying)
        {
            showDebug = EditorGUILayout.Foldout(showDebug, "Runtime Debug", true);
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                DrawDebugSection();
                EditorGUI.indentLevel--;
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawCheckpointsSection()
    {
        EditorGUILayout.PropertyField(checkpointsProperty, true);
        
        EditorGUILayout.Space();
        
        // Checkpoint management buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Current Position"))
        {
            AddCheckpointAtCurrentPosition();
        }
        
        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear Checkpoints", 
                "Are you sure you want to clear all checkpoints?", "Yes", "No"))
            {
                checkpointsProperty.ClearArray();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Auto-Arrange"))
        {
            AutoArrangeCheckpoints();
        }
        
        if (GUILayout.Button("Validate Route"))
        {
            ValidateCheckpointRoute();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawQuickSetupButtons()
    {
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Create Simple Path"))
        {
            CreateSimplePath();
        }
        
        if (GUILayout.Button("Setup NavMesh Agent"))
        {
            SetupNavMeshAgent();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Find Player"))
        {
            FindPlayerReference();
        }
        
        if (GUILayout.Button("Setup Animator"))
        {
            SetupAnimator();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawDebugSection()
    {
        EditorGUI.BeginDisabledGroup(true);
        
        EditorGUILayout.LabelField($"Current State: {kancilGuide.GetCurrentState()}");
        EditorGUILayout.LabelField($"Current Checkpoint: {kancilGuide.GetCurrentCheckpointIndex() + 1}/{kancilGuide.GetTotalCheckpoints()}");
        EditorGUILayout.LabelField($"Player Nearby: {kancilGuide.IsPlayerNearby()}");
        
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();
        
        // Runtime control buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Start Guiding"))
        {
            kancilGuide.StartGuiding();
        }
        
        if (GUILayout.Button("Stop Guiding"))
        {
            kancilGuide.StopGuiding();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Checkpoint jump controls
        if (kancilGuide.GetTotalCheckpoints() > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Jump to Checkpoint:", EditorStyles.miniBoldLabel);
            
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < kancilGuide.GetTotalCheckpoints() && i < 10; i++)
            {
                if (GUILayout.Button((i + 1).ToString()))
                {
                    kancilGuide.JumpToCheckpoint(i);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    
    private void AddCheckpointAtCurrentPosition()
    {
        GameObject checkpointObj = new GameObject($"Kancil_Checkpoint_{checkpointsProperty.arraySize + 1}");
        checkpointObj.transform.position = kancilGuide.transform.position;
        
        KancilCheckpoint checkpoint = checkpointObj.AddComponent<KancilCheckpoint>();
        checkpoint.CheckpointName = $"Checkpoint {checkpointsProperty.arraySize + 1}";
        
        checkpointsProperty.InsertArrayElementAtIndex(checkpointsProperty.arraySize);
        checkpointsProperty.GetArrayElementAtIndex(checkpointsProperty.arraySize - 1).objectReferenceValue = checkpoint;
        
        Selection.activeGameObject = checkpointObj;
        
        EditorUtility.SetDirty(kancilGuide);
    }
    
    private void AutoArrangeCheckpoints()
    {
        if (checkpointsProperty.arraySize < 2) return;
        
        List<KancilCheckpoint> checkpoints = new List<KancilCheckpoint>();
        
        for (int i = 0; i < checkpointsProperty.arraySize; i++)
        {
            KancilCheckpoint checkpoint = checkpointsProperty.GetArrayElementAtIndex(i).objectReferenceValue as KancilCheckpoint;
            if (checkpoint != null)
            {
                checkpoints.Add(checkpoint);
            }
        }
        
        // Sort checkpoints by distance from kancil (simple arrangement)
        Vector3 startPos = kancilGuide.transform.position;
        checkpoints.Sort((a, b) => 
        {
            float distA = Vector3.Distance(startPos, a.transform.position);
            float distB = Vector3.Distance(startPos, b.transform.position);
            return distA.CompareTo(distB);
        });
        
        // Update array
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = checkpoints[i];
        }
        
        EditorUtility.SetDirty(kancilGuide);
        Debug.Log($"Auto-arranged {checkpoints.Count} checkpoints by distance");
    }
    
    private void ValidateCheckpointRoute()
    {
        List<string> issues = new List<string>();
        
        // Check for null checkpoints
        int nullCount = 0;
        for (int i = 0; i < checkpointsProperty.arraySize; i++)
        {
            if (checkpointsProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                nullCount++;
            }
        }
        
        if (nullCount > 0)
        {
            issues.Add($"{nullCount} null checkpoint references found");
        }
        
        // Check for duplicate checkpoints
        HashSet<KancilCheckpoint> uniqueCheckpoints = new HashSet<KancilCheckpoint>();
        int duplicateCount = 0;
        
        for (int i = 0; i < checkpointsProperty.arraySize; i++)
        {
            KancilCheckpoint checkpoint = checkpointsProperty.GetArrayElementAtIndex(i).objectReferenceValue as KancilCheckpoint;
            if (checkpoint != null)
            {
                if (!uniqueCheckpoints.Add(checkpoint))
                {
                    duplicateCount++;
                }
            }
        }
        
        if (duplicateCount > 0)
        {
            issues.Add($"{duplicateCount} duplicate checkpoints found");
        }
        
        // Display results
        if (issues.Count == 0)
        {
            EditorUtility.DisplayDialog("Route Validation", 
                "✓ No issues found!\nCheckpoint route is valid.", "OK");
        }
        else
        {
            string message = "Issues found:\n\n";
            foreach (string issue in issues)
            {
                message += "• " + issue + "\n";
            }
            
            EditorUtility.DisplayDialog("Route Validation", message, "OK");
        }
    }
    
    private void CreateSimplePath()
    {
        Vector3 startPos = kancilGuide.transform.position;
        
        // Create 3 checkpoints in a simple forward path
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = startPos + kancilGuide.transform.forward * (5f * (i + 1));
            
            GameObject checkpointObj = new GameObject($"Checkpoint_{i + 1}");
            checkpointObj.transform.position = pos;
            
            KancilCheckpoint checkpoint = checkpointObj.AddComponent<KancilCheckpoint>();
            checkpoint.CheckpointName = $"Checkpoint {i + 1}";
            
            checkpointsProperty.InsertArrayElementAtIndex(checkpointsProperty.arraySize);
            checkpointsProperty.GetArrayElementAtIndex(checkpointsProperty.arraySize - 1).objectReferenceValue = checkpoint;
        }
        
        EditorUtility.SetDirty(kancilGuide);
        Debug.Log("Created simple 3-checkpoint path");
    }
    
    private void SetupNavMeshAgent()
    {
        UnityEngine.AI.NavMeshAgent agent = kancilGuide.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            agent = kancilGuide.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            Debug.Log("Added NavMeshAgent component");
        }
        
        // Configure NavMeshAgent for kancil
        agent.speed = 2f;
        agent.acceleration = 8f;
        agent.angularSpeed = 180f;
        agent.stoppingDistance = 0.5f;
        agent.radius = 0.5f;
        agent.height = 1f;
        
        EditorUtility.SetDirty(kancilGuide);
        Debug.Log("Configured NavMeshAgent for kancil movement");
    }
    
    private void FindPlayerReference()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            SerializedProperty playerProperty = serializedObject.FindProperty("player");
            playerProperty.objectReferenceValue = player.transform;
            Debug.Log($"Found and assigned player: {player.name}");
        }
        else
        {
            Debug.LogWarning("No GameObject with 'Player' tag found in scene");
        }
    }
    
    private void SetupAnimator()
    {
        Animator animator = kancilGuide.GetComponent<Animator>();
        if (animator == null)
        {
            animator = kancilGuide.gameObject.AddComponent<Animator>();
            Debug.Log("Added Animator component");
        }
        
        // Try to find kancil animator controller
        string[] controllerGuids = AssetDatabase.FindAssets("t:AnimatorController", new[] { "Assets/3D MOdel/Character/Animation" });
        
        if (controllerGuids.Length > 0)
        {
            string controllerPath = AssetDatabase.GUIDToAssetPath(controllerGuids[0]);
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
            
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;
                Debug.Log($"Assigned animator controller: {controller.name}");
            }
        }
        else
        {
            Debug.LogWarning("No AnimatorController found in Character/Animation folder");
        }
    }
    
    private void OnSceneGUI()
    {
        if (kancilGuide == null) return;
        
        // Draw checkpoint path in scene view
        Handles.color = Color.green;
        
        for (int i = 0; i < checkpointsProperty.arraySize - 1; i++)
        {
            KancilCheckpoint current = checkpointsProperty.GetArrayElementAtIndex(i).objectReferenceValue as KancilCheckpoint;
            KancilCheckpoint next = checkpointsProperty.GetArrayElementAtIndex(i + 1).objectReferenceValue as KancilCheckpoint;
            
            if (current != null && next != null)
            {
                Handles.DrawLine(current.transform.position, next.transform.position);
            }
        }
        
        // Draw detection range
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(kancilGuide.transform.position, Vector3.up, 5f); // playerDetectionRange
    }
}