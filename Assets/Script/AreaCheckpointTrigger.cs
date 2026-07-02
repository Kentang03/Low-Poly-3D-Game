using UnityEngine;

/// <summary>
/// Trigger area untuk mengaktifkan checkpoint otomatis berdasarkan area
/// Letakkan di berbagai area untuk sistem checkpoint otomatis
/// </summary>
public class AreaCheckpointTrigger : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private string areaName = "Area 1";
    [SerializeField] private int areaIndex = 1;
    [SerializeField] private Transform checkpointPosition;
    [SerializeField] private bool oneTimeActivation = true;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool useOnTriggerEnter = true;
    [SerializeField] private bool requirePlayerTag = true;
    [SerializeField] private LayerMask playerLayerMask = -1;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject activationEffect;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private bool showDebugMessages = true;
    
    [Header("Gizmo Settings")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private bool showAreaName = true;
    
    // State
    private bool hasBeenActivated = false;
    private AudioSource audioSource;
    
    // Events
    public System.Action<string> OnAreaEntered;
    public System.Action<int> OnCheckpointActivated;
    
    void Start()
    {
        InitializeTrigger();
        SetupComponents();
    }
    
    void InitializeTrigger()
    {
        // Setup checkpoint position jika belum diatur
        if (checkpointPosition == null)
        {
            checkpointPosition = transform;
        }
        
        // Set default area name berdasarkan GameObject name jika kosong
        if (string.IsNullOrEmpty(areaName))
        {
            areaName = gameObject.name;
        }
        
        // Ensure we have a collider untuk trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;
            boxCol.size = Vector3.one * 5f; // Default size
            
            if (showDebugMessages)
                Debug.Log($"Added BoxCollider trigger to {gameObject.name}");
        }
        else
        {
            col.isTrigger = true;
        }
    }
    
    void SetupComponents()
    {
        // Setup AudioSource untuk sound effects
        if (activationSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!useOnTriggerEnter) return;
        if (oneTimeActivation && hasBeenActivated) return;
        
        if (IsPlayer(other.gameObject))
        {
            ActivateCheckpoint(other.gameObject);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Optional: bisa digunakan untuk activation yang membutuhkan player tinggal di area
        // Currently not used, but available for future features
    }
    
    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            if (showDebugMessages)
                Debug.Log($"Player left area: {areaName}");
        }
    }
    
    bool IsPlayer(GameObject obj)
    {
        // Multiple ways to detect player
        bool isPlayer = false;
        
        if (requirePlayerTag)
        {
            isPlayer = obj.CompareTag("Player");
        }
        else
        {
            // Check layer mask
            int objLayer = obj.layer;
            isPlayer = (playerLayerMask.value & (1 << objLayer)) > 0;
        }
        
        // Additional checks
        if (!isPlayer)
        {
            isPlayer = obj.GetComponent<PlayerHealthSystem>() != null ||
                      obj.GetComponent<InvectorControllerAdapter>() != null ||
                      obj.GetComponent<CharacterController>() != null;
        }
        
        return isPlayer;
    }
    
    void ActivateCheckpoint(GameObject player)
    {
        if (oneTimeActivation && hasBeenActivated) return;
        
        hasBeenActivated = true;
        
        if (showDebugMessages)
            Debug.Log($"Area checkpoint activated: {areaName} (Index: {areaIndex})");
        
        // Register checkpoint dengan CheckpointManager
        CheckpointManager checkpointManager = CheckpointManager.Instance;
        if (checkpointManager != null)
        {
            // Register checkpoint dengan proper transform (checkpointPosition or self)
            Transform spawnTransform = GetCheckpointTransform();
            checkpointManager.RegisterAreaCheckpoint(spawnTransform, areaIndex);
            
            // Set sebagai current checkpoint berdasarkan area index
            checkpointManager.SetCurrentCheckpoint(areaIndex - 1); // Convert to 0-based index
            
            if (showDebugMessages)
            {
                Debug.Log($"Registered checkpoint {areaName} with spawn position: {spawnTransform.position}");
            }
        }
        else
        {
            Debug.LogError("CheckpointManager not found! Cannot activate checkpoint.");
        }
        
        // Play effects
        PlayActivationEffects();
        
        // Trigger events
        OnAreaEntered?.Invoke(areaName);
        OnCheckpointActivated?.Invoke(areaIndex);
    }
    
    void PlayActivationEffects()
    {
        // Play sound
        if (audioSource != null && activationSound != null)
        {
            audioSource.clip = activationSound;
            audioSource.Play();
        }
        
        // Spawn visual effect
        if (activationEffect != null)
        {
            GameObject effect = Instantiate(activationEffect, checkpointPosition.position, checkpointPosition.rotation);
            
            // Auto destroy effect after 5 seconds
            Destroy(effect, 5f);
        }
    }
    
    /// <summary>
    /// Manual activation (untuk testing atau scripted events)
    /// </summary>
    public void ManualActivate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            ActivateCheckpoint(player);
        }
        else
        {
            Debug.LogWarning("Player not found for manual checkpoint activation");
        }
    }
    
    /// <summary>
    /// Reset activation state
    /// </summary>
    public void ResetActivation()
    {
        hasBeenActivated = false;
        if (showDebugMessages)
            Debug.Log($"Reset activation for area: {areaName}");
    }
    
    /// <summary>
    /// Get actual spawn position for this checkpoint
    /// </summary>
    /// <returns>Spawn position</returns>
    public Vector3 GetSpawnPosition()
    {
        return checkpointPosition != null ? checkpointPosition.position : transform.position;
    }
    
    /// <summary>
    /// Get actual spawn rotation for this checkpoint  
    /// </summary>
    /// <returns>Spawn rotation</returns>
    public Quaternion GetSpawnRotation()
    {
        return checkpointPosition != null ? checkpointPosition.rotation : transform.rotation;
    }
    
    /// <summary>
    /// Get checkpoint position transform (for CheckpointManager registration)
    /// </summary>
    /// <returns>Transform yang akan digunakan untuk spawn</returns>
    public Transform GetCheckpointTransform()
    {
        return checkpointPosition != null ? checkpointPosition : transform;
    }
    
    /// <summary>
    /// Get checkpoint info
    /// </summary>
    /// <returns>Area info string</returns>
    public string GetAreaInfo()
    {
        return $"Area: {areaName} (Index: {areaIndex}) - Activated: {hasBeenActivated}";
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            // Set color berdasarkan activation state
            Color gizmoColorToUse = hasBeenActivated ? Color.green : gizmoColor;
            gizmoColorToUse.a = 0.3f;
            
            Gizmos.color = gizmoColorToUse;
            
            // Draw trigger area
            if (col is BoxCollider boxCol)
            {
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(boxCol.center, boxCol.size);
                Gizmos.matrix = oldMatrix;
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
            else
            {
                // Fallback untuk collider lain
                Gizmos.DrawWireCube(transform.position, col.bounds.size);
            }
            
            // Draw checkpoint position
            if (checkpointPosition != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(checkpointPosition.position, 0.5f);
                
                // Draw line dari trigger ke checkpoint
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, checkpointPosition.position);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (showAreaName)
        {
            // Display area name di scene view (membutuhkan custom editor untuk teks yang proper)
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2, Vector3.one * 0.1f);
        }
    }
}