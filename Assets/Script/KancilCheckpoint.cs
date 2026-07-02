using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checkpoint untuk sistem pathfinding Kancil Guide
/// </summary>
public class KancilCheckpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private string checkpointName = "Checkpoint";
    [SerializeField] public float waitTime = 2f;
    [SerializeField] private bool hasLookDirection = false;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Vector3 lookDirection = Vector3.forward;
    
    [Header("Visual Settings")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoSize = 0.5f;
    [SerializeField] private bool showLookDirection = true;
    
    [Header("Checkpoint Actions")]
    [SerializeField] private bool triggerCustomAction = false;
    [SerializeField] private string customActionMessage = "";
    
    // Events
    public System.Action OnKancilReached;
    public System.Action OnKancilLeft;
    
    // Properties
    public string CheckpointName 
    { 
        get => string.IsNullOrEmpty(checkpointName) ? gameObject.name : checkpointName;
        set => checkpointName = value;
    }
    
    private void Start()
    {
        // Set default name if empty
        if (string.IsNullOrEmpty(checkpointName))
        {
            checkpointName = $"Checkpoint_{transform.GetSiblingIndex()}";
        }
    }
    
    /// <summary>
    /// Cek apakah checkpoint ini memiliki arah pandang khusus
    /// </summary>
    public bool HasLookDirection()
    {
        return hasLookDirection;
    }
    
    /// <summary>
    /// Dapatkan arah pandang untuk checkpoint ini
    /// </summary>
    public Vector3 GetLookDirection()
    {
        if (lookTarget != null)
        {
            return (lookTarget.position - transform.position).normalized;
        }
        return lookDirection.normalized;
    }
    
    /// <summary>
    /// Set target untuk arah pandang
    /// </summary>
    public void SetLookTarget(Transform target)
    {
        lookTarget = target;
        hasLookDirection = target != null;
    }
    
    /// <summary>
    /// Set arah pandang manual
    /// </summary>
    public void SetLookDirection(Vector3 direction)
    {
        lookDirection = direction.normalized;
        hasLookDirection = true;
        lookTarget = null;
    }
    
    /// <summary>
    /// Disable arah pandang khusus
    /// </summary>
    public void DisableLookDirection()
    {
        hasLookDirection = false;
        lookTarget = null;
    }
    
    /// <summary>
    /// Trigger aksi khusus ketika kancil sampai di checkpoint
    /// </summary>
    public void TriggerCheckpointAction()
    {
        if (triggerCustomAction)
        {
            Debug.Log($"Checkpoint Action: {customActionMessage}");
            
            // Di sini bisa ditambahkan aksi khusus seperti:
            // - Menampilkan UI hint
            // - Memutar suara
            // - Mengaktifkan objek tertentu
            // - Dan lain-lain
        }
        
        OnKancilReached?.Invoke();
    }
    
    /// <summary>
    /// Called when kancil leaves this checkpoint
    /// </summary>
    public void OnKancilLeaveCheckpoint()
    {
        OnKancilLeft?.Invoke();
    }
    
    // Static helper methods untuk membuat checkpoint
    /// <summary>
    /// Buat checkpoint baru di posisi tertentu
    /// </summary>
    public static KancilCheckpoint CreateCheckpoint(Vector3 position, string name = "")
    {
        GameObject checkpointObj = new GameObject(string.IsNullOrEmpty(name) ? "KancilCheckpoint" : name);
        checkpointObj.transform.position = position;
        
        KancilCheckpoint checkpoint = checkpointObj.AddComponent<KancilCheckpoint>();
        checkpoint.checkpointName = name;
        
        return checkpoint;
    }
    
    /// <summary>
    /// Buat checkpoint dengan arah pandang
    /// </summary>
    public static KancilCheckpoint CreateCheckpointWithLookDirection(Vector3 position, Vector3 lookDir, string name = "")
    {
        KancilCheckpoint checkpoint = CreateCheckpoint(position, name);
        checkpoint.SetLookDirection(lookDir);
        return checkpoint;
    }
    
    /// <summary>
    /// Buat checkpoint yang mengarah ke target
    /// </summary>
    public static KancilCheckpoint CreateCheckpointWithTarget(Vector3 position, Transform target, string name = "")
    {
        KancilCheckpoint checkpoint = CreateCheckpoint(position, name);
        checkpoint.SetLookTarget(target);
        return checkpoint;
    }
    
    // Gizmos untuk visualisasi di Scene View
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        // Draw checkpoint position
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        
        // Draw checkpoint name
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * (gizmoSize + 0.5f), CheckpointName);
        #endif
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        
        // Draw checkpoint position (solid when selected)
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize * 0.8f);
        
        // Draw look direction
        if (showLookDirection && hasLookDirection)
        {
            Vector3 lookDir = GetLookDirection();
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, lookDir * 2f);
            
            // Draw arrow head
            Vector3 arrowTip = transform.position + lookDir * 2f;
            Vector3 arrowRight = Quaternion.AngleAxis(135, Vector3.up) * lookDir * 0.5f;
            Vector3 arrowLeft = Quaternion.AngleAxis(-135, Vector3.up) * lookDir * 0.5f;
            
            Gizmos.DrawLine(arrowTip, arrowTip + arrowRight);
            Gizmos.DrawLine(arrowTip, arrowTip + arrowLeft);
        }
        
        // Draw connection to look target
        if (lookTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, lookTarget.position);
        }
    }
}