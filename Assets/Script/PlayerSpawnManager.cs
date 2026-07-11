using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Manages player spawning at designated spawn points when scene loads
/// </summary>
public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform defaultSpawnPoint;
    public bool spawnOnSceneLoad = true;
    public bool enableMovementAfterSpawn = true;
    
    [Header("Player References")]
    public GameObject playerPrefab;
    public vThirdPersonController playerController;
    public vThirdPersonInput playerInput;
    
    [Header("Spawn Configuration")]
    public float spawnDelay = 0.1f;
    public bool resetPlayerRotation = true;
    public bool resetPlayerVelocity = true;
    
    [Header("Debug")]
    public bool showSpawnPointGizmo = true;
    public Color spawnPointColor = Color.green;
    
    private static PlayerSpawnManager instance;
    public static PlayerSpawnManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerSpawnManager>();
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    
    void Start()
    {
        if (spawnOnSceneLoad)
        {
            StartCoroutine(SpawnPlayerCoroutine());
        }
    }
    
    System.Collections.IEnumerator SpawnPlayerCoroutine()
    {
        // Wait a frame to ensure everything is initialized
        yield return new WaitForEndOfFrame();
        
        // Additional delay if specified
        if (spawnDelay > 0)
            yield return new WaitForSeconds(spawnDelay);
        
        SpawnPlayer();
    }
    
    /// <summary>
    /// Spawn player at the designated spawn point
    /// </summary>
    public void SpawnPlayer()
    {
        // Find spawn point if not assigned
        if (defaultSpawnPoint == null)
        {
            GameObject spawnPointGO = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (spawnPointGO != null)
                defaultSpawnPoint = spawnPointGO.transform;
        }
        
        // Find existing player if not assigned
        if (playerController == null)
            playerController = FindObjectOfType<vThirdPersonController>();
        
        if (playerInput == null)
            playerInput = FindObjectOfType<vThirdPersonInput>();
        
        // Spawn player at spawn point
        if (playerController != null && defaultSpawnPoint != null)
        {
            SpawnPlayerAtPoint(defaultSpawnPoint);
        }
        else if (playerPrefab != null && defaultSpawnPoint != null)
        {
            InstantiatePlayerAtPoint(defaultSpawnPoint);
        }
        else
        {
            Debug.LogWarning("PlayerSpawnManager: No player controller or spawn point found!");
            return;
        }
        
        // Ensure player can move after spawning
        if (enableMovementAfterSpawn)
        {
            EnablePlayerMovement();
        }
        
        Debug.Log($"Player spawned at: {(defaultSpawnPoint != null ? defaultSpawnPoint.name : "default position")}");
    }
    
    /// <summary>
    /// Move existing player to spawn point
    /// </summary>
    /// <param name="spawnPoint">Target spawn point</param>
    void SpawnPlayerAtPoint(Transform spawnPoint)
    {
        if (playerController == null) return;
        
        // Disable controller temporarily to prevent conflicts
        bool wasEnabled = playerController.enabled;
        playerController.enabled = false;
        
        // Set position and rotation
        playerController.transform.position = spawnPoint.position;
        
        if (resetPlayerRotation)
            playerController.transform.rotation = spawnPoint.rotation;
        
        // Reset physics if needed
        if (resetPlayerVelocity)
        {
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        
        // Re-enable controller
        playerController.enabled = wasEnabled;
        
        // Reset controller state
        if (playerController != null)
        {
            playerController.input = Vector3.zero;
            playerController.isSprinting = false;
            playerController.isJumping = false;
        }
    }
    
    /// <summary>
    /// Instantiate new player at spawn point
    /// </summary>
    /// <param name="spawnPoint">Target spawn point</param>
    void InstantiatePlayerAtPoint(Transform spawnPoint)
    {
        if (playerPrefab == null) return;
        
        // Instantiate player prefab
        GameObject newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Get components from new player
        playerController = newPlayer.GetComponent<vThirdPersonController>();
        playerInput = newPlayer.GetComponent<vThirdPersonInput>();
        
        // Initialize new player
        if (playerController != null)
            playerController.Init();
        
        Debug.Log("New player instantiated at spawn point");
    }
    
    /// <summary>
    /// Ensure player movement is enabled and not locked
    /// </summary>
    public void EnablePlayerMovement()
    {
        // Enable Invector controller components
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.lockMovement = false;
            playerController.lockRotation = false;
        }
        
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
        
        // Ensure rigidbody is not frozen
        if (playerController != null)
        {
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = false;
            }
        }
        
        // Ensure proper cursor state for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Set normal time scale
        Time.timeScale = 1f;
        
        Debug.Log("Player movement enabled - ready to play!");
    }
    
    /// <summary>
    /// Spawn player at specific spawn point by name
    /// </summary>
    /// <param name="spawnPointName">Name of the spawn point GameObject</param>
    public void SpawnPlayerAtNamedPoint(string spawnPointName)
    {
        GameObject namedSpawnPoint = GameObject.Find(spawnPointName);
        if (namedSpawnPoint != null)
        {
            SpawnPlayerAtPoint(namedSpawnPoint.transform);
            EnablePlayerMovement();
        }
        else
        {
            Debug.LogWarning($"Spawn point '{spawnPointName}' not found!");
        }
    }
    
    /// <summary>
    /// Get the current spawn point position
    /// </summary>
    /// <returns>Spawn point position or Vector3.zero if not found</returns>
    public Vector3 GetSpawnPointPosition()
    {
        return defaultSpawnPoint != null ? defaultSpawnPoint.position : Vector3.zero;
    }
    
    /// <summary>
    /// Set a new default spawn point
    /// </summary>
    /// <param name="newSpawnPoint">New spawn point transform</param>
    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        defaultSpawnPoint = newSpawnPoint;
        Debug.Log($"Spawn point set to: {newSpawnPoint.name}");
    }
    
    void OnDrawGizmos()
    {
        if (!showSpawnPointGizmo || defaultSpawnPoint == null) return;
        
        // Draw spawn point visualization
        Gizmos.color = spawnPointColor;
        Gizmos.DrawWireSphere(defaultSpawnPoint.position, 1f);
        Gizmos.DrawLine(defaultSpawnPoint.position, defaultSpawnPoint.position + defaultSpawnPoint.forward * 2f);
        
        // Draw arrow for direction
        Vector3 arrowHead = defaultSpawnPoint.position + defaultSpawnPoint.forward * 2f;
        Vector3 arrowLeft = arrowHead + defaultSpawnPoint.right * -0.3f + defaultSpawnPoint.forward * -0.3f;
        Vector3 arrowRight = arrowHead + defaultSpawnPoint.right * 0.3f + defaultSpawnPoint.forward * -0.3f;
        
        Gizmos.DrawLine(arrowHead, arrowLeft);
        Gizmos.DrawLine(arrowHead, arrowRight);
    }
}