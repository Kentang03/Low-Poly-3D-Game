using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager untuk sistem checkpoint respawn player
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private List<Transform> areaCheckpoints = new List<Transform>();
    [SerializeField] private Transform defaultSpawnPoint;
    [SerializeField] private int currentCheckpointIndex = 0;
    [SerializeField] private bool autoDetectCheckpoints = true;
    
    [Header("Area Detection")]
    [SerializeField] private float checkpointActivationDistance = 5f;
    [SerializeField] private Transform player;
    
    [Header("Visual Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color checkpointColor = Color.green;
    [SerializeField] private Color activeCheckpointColor = Color.yellow;
    [SerializeField] private float gizmoSize = 1f;
    
    // Singleton pattern
    private static CheckpointManager instance;
    public static CheckpointManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CheckpointManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("CheckpointManager");
                    instance = go.AddComponent<CheckpointManager>();
                }
            }
            return instance;
        }
    }
    
    // Events
    public System.Action<int, string> OnCheckpointActivated;
    public System.Action<Vector3> OnPlayerRespawned;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeCheckpoints();
        FindPlayerReference();
    }
    
    void Update()
    {
        if (player != null)
        {
            CheckPlayerNearCheckpoints();
        }
    }
    
    void InitializeCheckpoints()
    {
        if (autoDetectCheckpoints && areaCheckpoints.Count == 0)
        {
            AutoDetectCheckpoints();
        }
        
        // Set default spawn point jika belum ada
        if (defaultSpawnPoint == null && areaCheckpoints.Count > 0)
        {
            defaultSpawnPoint = areaCheckpoints[0];
        }
        
        Debug.Log($"CheckpointManager initialized with {areaCheckpoints.Count} checkpoints");
    }
    
    void FindPlayerReference()
    {
        if (player == null)
        {
            // Cari player dengan tag "Player" atau component player
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.transform;
            }
            else
            {
                // Fallback: cari berdasarkan component
                InvectorControllerAdapter invector = FindObjectOfType<InvectorControllerAdapter>();
                if (invector != null)
                    player = invector.transform;
                else
                {
                    CharacterController controller = FindObjectOfType<CharacterController>();
                    if (controller != null)
                        player = controller.transform;
                }
            }
        }
    }
    
    /// <summary>
    /// Auto-detect checkpoints dari scene
    /// </summary>
    void AutoDetectCheckpoints()
    {
        // Cari semua KancilCheckpoint di scene
        KancilCheckpoint[] kancilCheckpoints = FindObjectsOfType<KancilCheckpoint>();
        
        foreach (var checkpoint in kancilCheckpoints)
        {
            if (!areaCheckpoints.Contains(checkpoint.transform))
            {
                areaCheckpoints.Add(checkpoint.transform);
            }
        }
        
        // Cari transform dengan tag "Checkpoint" jika ada
        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var checkpointObj in checkpointObjects)
        {
            if (!areaCheckpoints.Contains(checkpointObj.transform))
            {
                areaCheckpoints.Add(checkpointObj.transform);
            }
        }
        
        // Sort berdasarkan nama untuk urutan yang konsisten
        areaCheckpoints.Sort((a, b) => a.name.CompareTo(b.name));
        
        Debug.Log($"Auto-detected {areaCheckpoints.Count} checkpoints");
    }
    
    /// <summary>
    /// Cek jarak player dengan checkpoint untuk aktivasi otomatis
    /// </summary>
    void CheckPlayerNearCheckpoints()
    {
        for (int i = 0; i < areaCheckpoints.Count; i++)
        {
            if (i > currentCheckpointIndex) // Hanya cek checkpoint yang lebih maju
            {
                float distance = Vector3.Distance(player.position, areaCheckpoints[i].position);
                
                if (distance <= checkpointActivationDistance)
                {
                    SetCurrentCheckpoint(i);
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Set checkpoint aktif berdasarkan index
    /// </summary>
    /// <param name="checkpointIndex">Index checkpoint (0-based)</param>
    public void SetCurrentCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex < 0 || checkpointIndex >= areaCheckpoints.Count) return;
        
        // Hanya update jika checkpoint lebih maju (tidak mundur)
        if (checkpointIndex > currentCheckpointIndex)
        {
            currentCheckpointIndex = checkpointIndex;
            string checkpointName = areaCheckpoints[currentCheckpointIndex].name;
            
            Debug.Log($"Checkpoint activated: {checkpointName} (Index: {currentCheckpointIndex})");
            
            OnCheckpointActivated?.Invoke(currentCheckpointIndex, checkpointName);
        }
    }
    
    /// <summary>
    /// Set checkpoint berdasarkan nama
    /// </summary>
    /// <param name="checkpointName">Nama checkpoint</param>
    public void SetCurrentCheckpoint(string checkpointName)
    {
        for (int i = 0; i < areaCheckpoints.Count; i++)
        {
            if (areaCheckpoints[i].name.Contains(checkpointName))
            {
                SetCurrentCheckpoint(i);
                return;
            }
        }
        
        Debug.LogWarning($"Checkpoint '{checkpointName}' not found!");
    }
    
    /// <summary>
    /// Set checkpoint berdasarkan Transform
    /// </summary>
    /// <param name="checkpointTransform">Transform checkpoint</param>
    public void SetCurrentCheckpoint(Transform checkpointTransform)
    {
        int index = areaCheckpoints.IndexOf(checkpointTransform);
        if (index >= 0)
        {
            SetCurrentCheckpoint(index);
        }
        else
        {
            Debug.LogWarning($"Checkpoint {checkpointTransform.name} not registered in manager!");
        }
    }
    
    /// <summary>
    /// Dapatkan posisi checkpoint saat ini
    /// </summary>
    /// <returns>Posisi spawn</returns>
    public Vector3 GetCurrentCheckpointPosition()
    {
        if (areaCheckpoints.Count == 0 || currentCheckpointIndex >= areaCheckpoints.Count || 
            areaCheckpoints[currentCheckpointIndex] == null)
        {
            if (defaultSpawnPoint != null)
                return defaultSpawnPoint.position;
            else
                return Vector3.zero;
        }
        
        Transform currentCheckpoint = areaCheckpoints[currentCheckpointIndex];
        
        // Check if checkpoint has AreaCheckpointTrigger with custom spawn position
        AreaCheckpointTrigger trigger = currentCheckpoint.GetComponent<AreaCheckpointTrigger>();
        if (trigger != null)
        {
            // Use reflection to get checkpointPosition
            var field = typeof(AreaCheckpointTrigger).GetField("checkpointPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                Transform checkpointPos = (Transform)field.GetValue(trigger);
                if (checkpointPos != null)
                {
                    return checkpointPos.position;
                }
            }
        }
        
        // Fallback to checkpoint transform position
        return currentCheckpoint.position;
    }
    
    /// <summary>
    /// Dapatkan rotasi checkpoint saat ini
    /// </summary>
    /// <returns>Rotasi spawn</returns>
    public Quaternion GetCurrentCheckpointRotation()
    {
        if (areaCheckpoints.Count == 0 || currentCheckpointIndex >= areaCheckpoints.Count || 
            areaCheckpoints[currentCheckpointIndex] == null)
        {
            if (defaultSpawnPoint != null)
                return defaultSpawnPoint.rotation;
            else
                return Quaternion.identity;
        }
        
        Transform currentCheckpoint = areaCheckpoints[currentCheckpointIndex];
        
        // Check if checkpoint has AreaCheckpointTrigger with custom spawn rotation
        AreaCheckpointTrigger trigger = currentCheckpoint.GetComponent<AreaCheckpointTrigger>();
        if (trigger != null)
        {
            // Use reflection to get checkpointPosition
            var field = typeof(AreaCheckpointTrigger).GetField("checkpointPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                Transform checkpointPos = (Transform)field.GetValue(trigger);
                if (checkpointPos != null)
                {
                    return checkpointPos.rotation;
                }
            }
        }
        
        // Fallback to checkpoint transform rotation
        return currentCheckpoint.rotation;
    }
    
    /// <summary>
    /// Dapatkan nama checkpoint saat ini
    /// </summary>
    /// <returns>Nama checkpoint</returns>
    public string GetCurrentCheckpointName()
    {
        if (areaCheckpoints.Count == 0 || currentCheckpointIndex >= areaCheckpoints.Count)
        {
            if (defaultSpawnPoint != null)
                return defaultSpawnPoint.name;
            else
                return "Default Spawn";
        }
        
        return areaCheckpoints[currentCheckpointIndex].name;
    }
    
    /// <summary>
    /// Tambah checkpoint secara manual
    /// </summary>
    /// <param name="newCheckpoint">Transform checkpoint baru</param>
    public void AddCheckpoint(Transform newCheckpoint)
    {
        if (!areaCheckpoints.Contains(newCheckpoint))
        {
            areaCheckpoints.Add(newCheckpoint);
            Debug.Log($"Added checkpoint: {newCheckpoint.name}");
        }
    }
    
    /// <summary>
    /// Add checkpoint at specific index (untuk area-based system)
    /// </summary>
    /// <param name="newCheckpoint">Transform checkpoint</param>
    /// <param name="index">Index dimana checkpoint akan diinsert</param>
    public void AddCheckpointAtIndex(Transform newCheckpoint, int index)
    {
        // Pastikan list cukup besar
        while (areaCheckpoints.Count <= index)
        {
            areaCheckpoints.Add(null);
        }
        
        // Set checkpoint di index yang tepat
        areaCheckpoints[index] = newCheckpoint;
        Debug.Log($"Added checkpoint at index {index}: {newCheckpoint.name}");
    }
    
    /// <summary>
    /// Register area checkpoint dengan proper index mapping
    /// </summary>
    /// <param name="checkpointTransform">Checkpoint transform</param>
    /// <param name="areaIndex">Area index (1-based)</param>
    public void RegisterAreaCheckpoint(Transform checkpointTransform, int areaIndex)
    {
        int listIndex = areaIndex - 1; // Convert to 0-based
        
        if (listIndex < 0)
        {
            Debug.LogError($"Invalid area index: {areaIndex}. Must be 1 or greater.");
            return;
        }
        
        // Add checkpoint at correct index
        AddCheckpointAtIndex(checkpointTransform, listIndex);
        
        Debug.Log($"Registered area checkpoint: {checkpointTransform.name} at area index {areaIndex} (list index {listIndex})");
    }
    
    /// <summary>
    /// Reset checkpoint ke awal
    /// </summary>
    public void ResetToFirstCheckpoint()
    {
        currentCheckpointIndex = 0;
        Debug.Log("Reset to first checkpoint");
    }
    
    /// <summary>
    /// Get checkpoint info untuk debugging
    /// </summary>
    public void PrintCheckpointInfo()
    {
        Debug.Log($"=== Checkpoint Manager Info ===");
        Debug.Log($"Total Checkpoints: {areaCheckpoints.Count}");
        Debug.Log($"Current Checkpoint: {currentCheckpointIndex} - {GetCurrentCheckpointName()}");
        Debug.Log($"Position: {GetCurrentCheckpointPosition()}");
        
        for (int i = 0; i < areaCheckpoints.Count; i++)
        {
            if (areaCheckpoints[i] != null)
            {
                string status = i == currentCheckpointIndex ? " [CURRENT]" : "";
                Debug.Log($"  {i}: {areaCheckpoints[i].name} at {areaCheckpoints[i].position}{status}");
            }
            else
            {
                Debug.Log($"  {i}: NULL CHECKPOINT");
            }
        }
    }
    
    /// <summary>
    /// Debug method to verify checkpoint position mapping
    /// </summary>
    public void VerifyCheckpointPositions()
    {
        Debug.Log("=== Checkpoint Position Verification ===");
        
        for (int i = 0; i < areaCheckpoints.Count; i++)
        {
            if (areaCheckpoints[i] != null)
            {
                Vector3 checkpointPos = areaCheckpoints[i].position;
                Debug.Log($"Checkpoint {i} ({areaCheckpoints[i].name}): {checkpointPos}");
                
                // Check if there's an AreaCheckpointTrigger component
                AreaCheckpointTrigger trigger = areaCheckpoints[i].GetComponent<AreaCheckpointTrigger>();
                if (trigger != null)
                {
                    // Get checkpoint position from trigger
                    var checkpointPosField = typeof(AreaCheckpointTrigger).GetField("checkpointPosition", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (checkpointPosField != null)
                    {
                        Transform checkpointPosTransform = (Transform)checkpointPosField.GetValue(trigger);
                        Vector3 actualSpawnPos = checkpointPosTransform != null ? checkpointPosTransform.position : checkpointPos;
                        
                        if (Vector3.Distance(checkpointPos, actualSpawnPos) > 0.1f)
                        {
                            Debug.LogWarning($"  ⚠️  Spawn position mismatch! Trigger: {checkpointPos}, Spawn: {actualSpawnPos}");
                        }
                        else
                        {
                            Debug.Log($"  ✅  Positions match: {actualSpawnPos}");
                        }
                    }
                }
            }
        }
        
        Debug.Log("=== Verification Complete ===");
    }
    
    /// <summary>
    /// Get current checkpoint index for testing
    /// </summary>
    /// <returns>Current checkpoint index</returns>
    public int GetCurrentCheckpointIndex()
    {
        return currentCheckpointIndex;
    }
    
    /// <summary>
    /// Get total checkpoint count
    /// </summary>
    /// <returns>Total checkpoint count</returns>
    public int GetCheckpointCount()
    {
        return areaCheckpoints.Count;
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        for (int i = 0; i < areaCheckpoints.Count; i++)
        {
            if (areaCheckpoints[i] == null) continue;
            
            // Set color berdasarkan status
            Gizmos.color = i == currentCheckpointIndex ? activeCheckpointColor : checkpointColor;
            
            // Draw checkpoint sphere
            Gizmos.DrawWireSphere(areaCheckpoints[i].position, gizmoSize);
            
            // Draw activation range
            Gizmos.color = new Color(checkpointColor.r, checkpointColor.g, checkpointColor.b, 0.3f);
            Gizmos.DrawWireSphere(areaCheckpoints[i].position, checkpointActivationDistance);
        }
        
        // Draw default spawn point
        if (defaultSpawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(defaultSpawnPoint.position, Vector3.one * 0.5f);
        }
    }
}