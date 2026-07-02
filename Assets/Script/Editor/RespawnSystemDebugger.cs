using UnityEngine;
using UnityEditor;

/// <summary>
/// Debug tool untuk mendiagnosis dan memperbaiki masalah respawn system
/// </summary>
[CustomEditor(typeof(PlayerHealthSystem))]
public class RespawnSystemDebugger : Editor
{
    private PlayerHealthSystem healthSystem;
    
    void OnEnable()
    {
        healthSystem = (PlayerHealthSystem)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        EditorGUILayout.LabelField("Respawn System Debugger", EditorStyles.boldLabel);
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to use debugging tools", MessageType.Info);
            return;
        }
        
        GUILayout.Space(5);
        
        // Current Status
        EditorGUILayout.LabelField("Current Status:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Health: {healthSystem.CurrentHealth}/{healthSystem.MaxHealth}");
        EditorGUILayout.LabelField($"Is Dead: {healthSystem.IsDead}");
        EditorGUILayout.LabelField($"Position: {healthSystem.transform.position}");
        
        if (CheckpointManager.Instance != null)
        {
            EditorGUILayout.LabelField($"Current Checkpoint: {CheckpointManager.Instance.GetCurrentCheckpointName()}");
            EditorGUILayout.LabelField($"Checkpoint Position: {CheckpointManager.Instance.GetCurrentCheckpointPosition()}");
        }
        else
        {
            EditorGUILayout.LabelField("Checkpoint Manager: Not Found", EditorStyles.miniLabel);
        }
        
        GUILayout.Space(10);
        
        // Debug Actions
        EditorGUILayout.LabelField("Debug Actions:", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Respawn", GUILayout.Height(25)))
        {
            healthSystem.TestRespawn();
        }
        
        if (GUILayout.Button("Kill Player", GUILayout.Height(25)))
        {
            healthSystem.InstantKill();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Validate Position", GUILayout.Height(25)))
        {
            ValidatePlayerPosition();
        }
        
        if (GUILayout.Button("Print Checkpoint Info", GUILayout.Height(25)))
        {
            if (CheckpointManager.Instance != null)
                CheckpointManager.Instance.PrintCheckpointInfo();
            else
                Debug.LogError("CheckpointManager not found!");
        }
        GUILayout.EndHorizontal();
        
        // Adapter Status
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Invector Adapter Status:", EditorStyles.boldLabel);
        
        InvectorControllerAdapter adapter = healthSystem.GetComponent<InvectorControllerAdapter>();
        if (adapter != null)
        {
            EditorGUILayout.LabelField("Adapter: Found ✅");
            
            if (GUILayout.Button("Validate Adapter Position", GUILayout.Height(25)))
            {
                adapter.ValidatePlayerPosition();
            }
            
            if (GUILayout.Button("Force Enable Components", GUILayout.Height(25)))
            {
                adapter.ForceEnableInvectorComponents();
            }
        }
        else
        {
            EditorGUILayout.LabelField("Adapter: Not Found ❌", EditorStyles.miniLabel);
        }
        
        // Invector Controller Status
        var invectorController = healthSystem.GetComponent<Invector.vCharacterController.vThirdPersonController>();
        if (invectorController != null)
        {
            EditorGUILayout.LabelField("vThirdPersonController: Found ✅");
            EditorGUILayout.LabelField($"Controller Enabled: {invectorController.enabled}");
            EditorGUILayout.LabelField($"Lock Movement: {invectorController.lockMovement}");
            EditorGUILayout.LabelField($"Lock Rotation: {invectorController.lockRotation}");
            
            var invectorInput = healthSystem.GetComponent<Invector.vCharacterController.vThirdPersonInput>();
            if (invectorInput != null)
            {
                EditorGUILayout.LabelField($"Input Enabled: {invectorInput.enabled}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("vThirdPersonController: Not Found ❌", EditorStyles.miniLabel);
        }
        
        // Help Box
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Debugging Tips:\n" +
            "1. Use 'Test Respawn' to test teleportation without dying\n" +
            "2. Use 'Validate Position' to check for position conflicts\n" +
            "3. Check console for detailed debug messages\n" +
            "4. If respawn fails, check Invector controller states",
            MessageType.Info);
    }
    
    void ValidatePlayerPosition()
    {
        Debug.Log("=== RESPAWN SYSTEM VALIDATION ===");
        
        // Check player position
        Vector3 playerPos = healthSystem.transform.position;
        Debug.Log($"Player Position: {playerPos}");
        
        // Check checkpoint position
        if (CheckpointManager.Instance != null)
        {
            Vector3 checkpointPos = CheckpointManager.Instance.GetCurrentCheckpointPosition();
            Debug.Log($"Checkpoint Position: {checkpointPos}");
            
            float distance = Vector3.Distance(playerPos, checkpointPos);
            Debug.Log($"Distance from Checkpoint: {distance:F2}m");
        }
        
        // Check Invector controller state
        var invectorController = healthSystem.GetComponent<Invector.vCharacterController.vThirdPersonController>();
        if (invectorController != null)
        {
            Debug.Log($"Controller Enabled: {invectorController.enabled}");
            Debug.Log($"Controller Input: {invectorController.input}");
            Debug.Log($"Controller Move Direction: {invectorController.moveDirection}");
            Debug.Log($"Lock Movement: {invectorController.lockMovement}");
            Debug.Log($"Lock Rotation: {invectorController.lockRotation}");
            
            // Check animator
            Animator animator = invectorController.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log($"Animator Root Position: {animator.rootPosition}");
                Debug.Log($"Transform Position: {invectorController.transform.position}");
                
                Vector3 posDiff = animator.rootPosition - invectorController.transform.position;
                if (posDiff.magnitude > 0.1f)
                {
                    Debug.LogWarning($"⚠️ Position mismatch detected! Difference: {posDiff}");
                }
            }
        }
        
        // Check rigidbody
        Rigidbody rb = healthSystem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log($"Rigidbody Velocity: {rb.linearVelocity}");
            Debug.Log($"Rigidbody Is Kinematic: {rb.isKinematic}");
            Debug.Log($"Rigidbody Constraints: {rb.constraints}");
        }
        
        Debug.Log("=== VALIDATION COMPLETE ===");
    }
}