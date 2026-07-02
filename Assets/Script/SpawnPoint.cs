using UnityEngine;

/// <summary>
/// Marks a position as a player spawn point
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    public string spawnPointName = "Default Spawn";
    public bool isDefaultSpawn = true;
    
    [Header("Spawn Behavior")]
    public bool resetPlayerRotation = true;
    public bool resetPlayerVelocity = true;
    
    [Header("Visual Settings")]
    public bool showGizmo = true;
    public Color gizmoColor = Color.green;
    public float gizmoSize = 1f;
    
    void Start()
    {
        // Set tag if not already set
        if (!gameObject.CompareTag("SpawnPoint"))
        {
            gameObject.tag = "SpawnPoint";
        }
        
        // Register as default spawn if needed
        if (isDefaultSpawn)
        {
            PlayerSpawnManager spawnManager = PlayerSpawnManager.Instance;
            if (spawnManager != null && spawnManager.defaultSpawnPoint == null)
            {
                spawnManager.SetSpawnPoint(this.transform);
            }
        }
    }
    
    /// <summary>
    /// Spawn player at this spawn point
    /// </summary>
    public void SpawnPlayerHere()
    {
        PlayerSpawnManager spawnManager = PlayerSpawnManager.Instance;
        if (spawnManager != null)
        {
            spawnManager.SetSpawnPoint(this.transform);
            spawnManager.SpawnPlayer();
        }
        else
        {
            Debug.LogWarning("No PlayerSpawnManager found in scene!");
        }
    }
    
    /// <summary>
    /// Get spawn position
    /// </summary>
    /// <returns>World position of this spawn point</returns>
    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }
    
    /// <summary>
    /// Get spawn rotation
    /// </summary>
    /// <returns>World rotation of this spawn point</returns>
    public Quaternion GetSpawnRotation()
    {
        return transform.rotation;
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        // Draw spawn point visualization
        Gizmos.color = gizmoColor;
        
        // Draw base circle
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        
        // Draw direction arrow
        Vector3 forward = transform.forward * (gizmoSize * 1.5f);
        Vector3 arrowHead = transform.position + forward;
        
        Gizmos.DrawLine(transform.position, arrowHead);
        
        // Draw arrow head
        Vector3 arrowLeft = arrowHead + transform.right * (-gizmoSize * 0.3f) + transform.forward * (-gizmoSize * 0.3f);
        Vector3 arrowRight = arrowHead + transform.right * (gizmoSize * 0.3f) + transform.forward * (-gizmoSize * 0.3f);
        
        Gizmos.DrawLine(arrowHead, arrowLeft);
        Gizmos.DrawLine(arrowHead, arrowRight);
        
        // Draw upward indicator
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * gizmoSize);
    }
    
    void OnDrawGizmosSelected()
    {
        // Enhanced visualization when selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmoSize * 1.2f);
        
        // Draw ground indicator
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, gizmoSize * 0.5f);
    }
}