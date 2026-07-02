using UnityEngine;

public class StoneTargetHelper : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private StoneSpawner stoneSpawner;
    [SerializeField] private bool autoAim = true;
    [SerializeField] private bool showTargetLine = true;
    [SerializeField] private Color targetLineColor = Color.green;
    
    [Header("UI Controls")]
    [SerializeField] private KeyCode aimKey = KeyCode.T;
    [SerializeField] private KeyCode spawnKey = KeyCode.Space;
    
    void Start()
    {
        if (stoneSpawner == null)
            stoneSpawner = GetComponent<StoneSpawner>();
    }

    void Update()
    {
        if (autoAim && target != null && stoneSpawner != null)
        {
            stoneSpawner.AimAtTarget(target);
        }
        
        // Manual aim dengan tombol T
        if (Input.GetKeyDown(aimKey) && target != null && stoneSpawner != null)
        {
            stoneSpawner.AimAtTarget(target);
        }
        
        // Manual spawn dengan tombol Space
        if (Input.GetKeyDown(spawnKey) && stoneSpawner != null)
        {
            stoneSpawner.SpawnStone();
        }
    }

    void OnDrawGizmos()
    {
        if (!showTargetLine || target == null) return;
        
        Gizmos.color = targetLineColor;
        Gizmos.DrawLine(transform.position, target.position);
        
        // Gambar target
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, 1f);
        
        // Gambar jarak
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 midPoint = (transform.position + target.position) / 2f;
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(midPoint, $"Distance: {distance:F1}m");
        #endif
    }

    // Public methods untuk UI atau script lain
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (autoAim && stoneSpawner != null)
            stoneSpawner.AimAtTarget(target);
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void ToggleAutoAim()
    {
        autoAim = !autoAim;
    }
}