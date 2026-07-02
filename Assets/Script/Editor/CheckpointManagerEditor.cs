using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Custom editor untuk CheckpointManager - memudahkan setup dan management checkpoints
/// </summary>
[CustomEditor(typeof(CheckpointManager))]
public class CheckpointManagerEditor : Editor
{
    private CheckpointManager checkpointManager;
    private bool showCheckpointList = true;
    private bool showDebugInfo = true;
    private bool showQuickActions = true;
    
    void OnEnable()
    {
        checkpointManager = (CheckpointManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        // Quick Actions Section
        if (showQuickActions = EditorGUILayout.Foldout(showQuickActions, "Quick Actions"))
        {
            EditorGUILayout.BeginVertical("box");
            DrawQuickActions();
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();
        
        // Checkpoint List Section
        if (showCheckpointList = EditorGUILayout.Foldout(showCheckpointList, "Checkpoint List"))
        {
            EditorGUILayout.BeginVertical("box");
            DrawCheckpointList();
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();
        
        // Debug Info Section
        if (showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, "Debug Information"))
        {
            EditorGUILayout.BeginVertical("box");
            DrawDebugInfo();
            EditorGUILayout.EndVertical();
        }
        
        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(checkpointManager);
        }
    }
    
    void DrawQuickActions()
    {
        EditorGUILayout.LabelField("Setup Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Auto-Detect Checkpoints"))
        {
            AutoDetectCheckpoints();
        }
        
        if (GUILayout.Button("Create Test Checkpoints"))
        {
            CreateTestCheckpoints();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Validate Setup"))
        {
            ValidateCheckpointSetup();
        }
        
        if (GUILayout.Button("Print Info"))
        {
            checkpointManager.PrintCheckpointInfo();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Runtime Actions", EditorStyles.boldLabel);
        
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reset to First"))
            {
                checkpointManager.ResetToFirstCheckpoint();
            }
            
            if (GUILayout.Button("Next Checkpoint"))
            {
                int current = checkpointManager.GetCurrentCheckpointIndex();
                checkpointManager.SetCurrentCheckpoint(current + 1);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("Runtime actions available during Play Mode", MessageType.Info);
        }
    }
    
    void DrawCheckpointList()
    {
        EditorGUILayout.LabelField("Registered Checkpoints", EditorStyles.boldLabel);
        
        if (checkpointManager == null)
        {
            EditorGUILayout.HelpBox("CheckpointManager reference is null", MessageType.Error);
            return;
        }
        
        // Access private field using reflection for display
        var field = typeof(CheckpointManager).GetField("areaCheckpoints", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            List<Transform> checkpoints = (List<Transform>)field.GetValue(checkpointManager);
            
            if (checkpoints == null || checkpoints.Count == 0)
            {
                EditorGUILayout.HelpBox("No checkpoints registered. Use 'Auto-Detect Checkpoints' or add manually.", MessageType.Warning);
                return;
            }
            
            for (int i = 0; i < checkpoints.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Checkpoint info
                string status = "";
                if (Application.isPlaying)
                {
                    int currentIndex = checkpointManager.GetCurrentCheckpointIndex();
                    status = i == currentIndex ? " [CURRENT]" : "";
                }
                
                EditorGUILayout.LabelField($"{i}: {(checkpoints[i] ? checkpoints[i].name : "NULL")}{status}");
                
                // Actions
                GUI.enabled = checkpoints[i] != null;
                
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeGameObject = checkpoints[i].gameObject;
                    EditorGUIUtility.PingObject(checkpoints[i].gameObject);
                }
                
                if (Application.isPlaying && GUILayout.Button("Set Current", GUILayout.Width(70)))
                {
                    checkpointManager.SetCurrentCheckpoint(i);
                }
                
                GUI.enabled = true;
                
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    RemoveCheckpoint(i);
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Cannot access checkpoint list (reflection failed)", MessageType.Error);
        }
    }
    
    void DrawDebugInfo()
    {
        EditorGUILayout.LabelField("System Status", EditorStyles.boldLabel);
        
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Current Checkpoint:", checkpointManager.GetCurrentCheckpointName());
            EditorGUILayout.LabelField("Current Index:", checkpointManager.GetCurrentCheckpointIndex().ToString());
            EditorGUILayout.LabelField("Total Checkpoints:", checkpointManager.GetCheckpointCount().ToString());
            EditorGUILayout.LabelField("Current Position:", checkpointManager.GetCurrentCheckpointPosition().ToString());
        }
        else
        {
            EditorGUILayout.HelpBox("Debug info available during Play Mode", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        // Scene References
        EditorGUILayout.LabelField("Scene References", EditorStyles.boldLabel);
        
        GameObject player = GameObject.FindWithTag("Player");
        EditorGUILayout.LabelField("Player Found:", player != null ? player.name : "None");
        
        AreaCheckpointTrigger[] areaTriggers = FindObjectsOfType<AreaCheckpointTrigger>();
        EditorGUILayout.LabelField("Area Triggers:", areaTriggers.Length.ToString());
        
        KancilCheckpoint[] kancilCheckpoints = FindObjectsOfType<KancilCheckpoint>();
        EditorGUILayout.LabelField("Kancil Checkpoints:", kancilCheckpoints.Length.ToString());
    }
    
    void AutoDetectCheckpoints()
    {
        Debug.Log("Auto-detecting checkpoints...");
        
        // Trigger the auto-detection
        var method = typeof(CheckpointManager).GetMethod("AutoDetectCheckpoints", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {
            method.Invoke(checkpointManager, null);
            EditorUtility.SetDirty(checkpointManager);
            Debug.Log("Auto-detection completed!");
        }
        else
        {
            Debug.LogError("Auto-detection method not found!");
        }
    }
    
    void CreateTestCheckpoints()
    {
        Debug.Log("Creating test checkpoints...");
        
        GameObject checkpointsParent = GameObject.Find("Checkpoints");
        if (checkpointsParent == null)
        {
            checkpointsParent = new GameObject("Checkpoints");
        }
        
        for (int i = 1; i <= 3; i++)
        {
            GameObject checkpoint = new GameObject($"TestCheckpoint_{i}");
            checkpoint.transform.SetParent(checkpointsParent.transform);
            checkpoint.transform.position = Vector3.right * (i * 10); // Space them out
            
            AreaCheckpointTrigger trigger = checkpoint.AddComponent<AreaCheckpointTrigger>();
            
            // Setup trigger properties using reflection or public methods
            var areaNameField = typeof(AreaCheckpointTrigger).GetField("areaName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var areaIndexField = typeof(AreaCheckpointTrigger).GetField("areaIndex", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (areaNameField != null) areaNameField.SetValue(trigger, $"Test Area {i}");
            if (areaIndexField != null) areaIndexField.SetValue(trigger, i);
            
            // Add collider
            BoxCollider col = checkpoint.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = Vector3.one * 5f;
            
            Debug.Log($"Created test checkpoint: Test Area {i}");
        }
        
        // Auto-detect the new checkpoints
        AutoDetectCheckpoints();
    }
    
    void ValidateCheckpointSetup()
    {
        Debug.Log("=== Validating Checkpoint Setup ===");
        
        bool isValid = true;
        
        // Check CheckpointManager
        if (checkpointManager == null)
        {
            Debug.LogError("❌ CheckpointManager is null!");
            isValid = false;
        }
        else
        {
            Debug.Log("✅ CheckpointManager found");
        }
        
        // Check Player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("⚠️ No GameObject tagged 'Player' found");
        }
        else
        {
            Debug.Log($"✅ Player found: {player.name}");
            
            PlayerHealthSystem health = player.GetComponent<PlayerHealthSystem>();
            if (health == null)
            {
                Debug.LogError("❌ PlayerHealthSystem not found on player!");
                isValid = false;
            }
            else
            {
                Debug.Log("✅ PlayerHealthSystem found");
            }
        }
        
        // Check DeathManager
        DeathManager deathManager = FindObjectOfType<DeathManager>();
        if (deathManager == null)
        {
            Debug.LogError("❌ DeathManager not found in scene!");
            isValid = false;
        }
        else
        {
            Debug.Log("✅ DeathManager found");
        }
        
        // Check Area Triggers
        AreaCheckpointTrigger[] triggers = FindObjectsOfType<AreaCheckpointTrigger>();
        if (triggers.Length == 0)
        {
            Debug.LogWarning("⚠️ No AreaCheckpointTrigger found in scene");
        }
        else
        {
            Debug.Log($"✅ Found {triggers.Length} AreaCheckpointTrigger(s)");
            
            // Validate each trigger
            for (int i = 0; i < triggers.Length; i++)
            {
                Collider col = triggers[i].GetComponent<Collider>();
                if (col == null || !col.isTrigger)
                {
                    Debug.LogError($"❌ AreaCheckpointTrigger '{triggers[i].name}' missing trigger collider!");
                    isValid = false;
                }
            }
        }
        
        // Check Checkpoints
        int checkpointCount = checkpointManager.GetCheckpointCount();
        if (checkpointCount == 0)
        {
            Debug.LogWarning("⚠️ No checkpoints registered in CheckpointManager");
        }
        else
        {
            Debug.Log($"✅ {checkpointCount} checkpoint(s) registered");
        }
        
        if (isValid)
        {
            Debug.Log("🎉 Checkpoint setup validation PASSED!");
        }
        else
        {
            Debug.LogError("❌ Checkpoint setup validation FAILED! Check errors above.");
        }
        
        Debug.Log("=== Validation Complete ===");
    }
    
    void RemoveCheckpoint(int index)
    {
        Debug.Log($"Removing checkpoint at index {index}");
        
        // Access private field to remove checkpoint
        var field = typeof(CheckpointManager).GetField("areaCheckpoints", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            List<Transform> checkpoints = (List<Transform>)field.GetValue(checkpointManager);
            if (checkpoints != null && index >= 0 && index < checkpoints.Count)
            {
                checkpoints.RemoveAt(index);
                EditorUtility.SetDirty(checkpointManager);
                Debug.Log("Checkpoint removed");
            }
        }
    }
}