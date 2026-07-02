using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script untuk NPC Kancil sebagai penunjuk arah menggunakan pathfinding dengan checkpoint system
/// </summary>
public class KancilGuide : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float waitTimeAtCheckpoint = 2f;
    [SerializeField] private float playerDetectionRange = 5f;
    [SerializeField] private float maxDistanceFromPlayer = 15f;
    
    [Header("NavMesh Terrain Settings")]
    [SerializeField] private float navMeshRadius = 0.5f;
    [SerializeField] private float navMeshHeight = 1f;
    [SerializeField] private float baseOffset = 0f;
    [SerializeField] private bool enableAutobraking = true;
    
    [Header("Slope Speed Control")]
    [SerializeField] private bool enableSlopeCompensation = true;
    [SerializeField] private float slopeSpeedMultiplier = 0.7f;
    [SerializeField] private float slopeDetectionDistance = 1.5f;
    [SerializeField] private LayerMask terrainLayer = -1;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkAnimationTrigger = "Walk";
    [SerializeField] private string idleAnimationTrigger = "Idle";
    
    [Header("Checkpoint System")]
    [SerializeField] private List<KancilCheckpoint> checkpoints = new List<KancilCheckpoint>();
    [SerializeField] private bool loopCheckpoints = true;
    [SerializeField] private bool waitForPlayer = true;
    
    [Header("Player Reference")]
    [SerializeField] private Transform player;
    
    // Private variables
    private NavMeshAgent navAgent;
    private int currentCheckpointIndex = 0;
    private bool isWaiting = false;
    private bool isMoving = false;
    private bool playerNearby = false;
    private KancilState currentState = KancilState.Idle;
    
    // Slope detection variables
    private float currentSlopeAngle = 0f;
    private Vector3 terrainNormal = Vector3.up;
    private float originalSpeed;
    
    // Events
    public System.Action<int> OnCheckpointReached;
    public System.Action<KancilState> OnStateChanged;
    
    public enum KancilState
    {
        Idle,
        MovingToCheckpoint,
        WaitingAtCheckpoint,
        WaitingForPlayer,
        ReturningToPlayer
    }
    
    private void Start()
    {
        InitializeKancil();
    }
    
    private void Update()
    {
        CheckPlayerDistance();
        
        // Apply slope compensation if enabled
        if (enableSlopeCompensation)
        {
            ApplySlopeSpeedCompensation();
        }
        
        UpdateStateMachine();
    }
    
    private void InitializeKancil()
    {
        // Setup NavMeshAgent
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }
        
        // Configure optimal NavMeshAgent settings for terrain
        ConfigureNavMeshAgent();
        
        // Setup Animator jika belum di-assign
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // Cari player jika belum di-assign
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Set initial state
        ChangeState(KancilState.Idle);
        
        Debug.Log($"Kancil Guide initialized with {checkpoints.Count} checkpoints");
    }
    
    /// <summary>
    /// Configure NavMeshAgent settings for optimal terrain movement
    /// </summary>
    private void ConfigureNavMeshAgent()
    {
        // Basic movement settings
        navAgent.speed = moveSpeed;
        navAgent.acceleration = acceleration;   // Bisa di-adjust dari Inspector
        navAgent.angularSpeed = rotationSpeed;
        navAgent.stoppingDistance = 0.1f;       // Lebih kecil untuk precision yang lebih baik
        
        // Terrain-specific settings
        navAgent.baseOffset = baseOffset;       // Bisa di-adjust dari Inspector
        navAgent.height = navMeshHeight;        // Bisa di-adjust dari Inspector
        navAgent.radius = navMeshRadius;        // Bisa di-adjust dari Inspector
        
        // Obstacle avoidance settings for smooth movement
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        navAgent.avoidancePriority = 50;        // Medium priority (0-99, lower = higher priority)
        
        // Auto-braking settings untuk pergerakan yang lebih smooth
        navAgent.autoBraking = enableAutobraking;  // Bisa di-adjust dari Inspector
        
        // Path settings untuk terrain navigation
        navAgent.autoTraverseOffMeshLink = true;   // Handle bridges, jumps otomatis
        navAgent.autoRepath = true;                // Auto recalculate path jika terblokir
        
        // Store original speed for slope compensation
        originalSpeed = moveSpeed;
        
        Debug.Log("NavMeshAgent configured for optimal terrain movement");
    }
    
    /// <summary>
    /// Apply speed compensation based on terrain slope
    /// </summary>
    private void ApplySlopeSpeedCompensation()
    {
        if (navAgent == null || !navAgent.hasPath) return;
        
        // Detect current slope angle
        DetectTerrainSlope();
        
        // Apply speed adjustment based on slope
        float adjustedSpeed = CalculateAdjustedSpeed();
        
        // Only update if there's a significant change to avoid jittering
        if (Mathf.Abs(navAgent.speed - adjustedSpeed) > 0.1f)
        {
            navAgent.speed = adjustedSpeed;
        }
    }
    
    /// <summary>
    /// Detect terrain slope under Kancil
    /// </summary>
    private void DetectTerrainSlope()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, slopeDetectionDistance, terrainLayer))
        {
            terrainNormal = hit.normal;
            currentSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        }
        else
        {
            // Default to flat ground if no hit
            terrainNormal = Vector3.up;
            currentSlopeAngle = 0f;
        }
    }
    
    /// <summary>
    /// Calculate adjusted speed based on slope angle
    /// </summary>
    private float CalculateAdjustedSpeed()
    {
        if (currentSlopeAngle <= 5f) // Flat ground tolerance
        {
            return originalSpeed;
        }
        
        // Calculate speed reduction for slopes
        // Steeper slopes = more speed reduction
        float slopeFactor = Mathf.Lerp(1f, slopeSpeedMultiplier, currentSlopeAngle / 45f);
        
        return originalSpeed * slopeFactor;
    }
    
    private void UpdateStateMachine()
    {
        switch (currentState)
        {
            case KancilState.Idle:
                HandleIdleState();
                break;
            case KancilState.MovingToCheckpoint:
                HandleMovingState();
                break;
            case KancilState.WaitingAtCheckpoint:
                HandleWaitingAtCheckpointState();
                break;
            case KancilState.WaitingForPlayer:
                HandleWaitingForPlayerState();
                break;
            case KancilState.ReturningToPlayer:
                HandleReturningToPlayerState();
                break;
        }
    }
    
    private void HandleIdleState()
    {
        if (checkpoints.Count > 0 && playerNearby)
        {
            MoveToNextCheckpoint();
        }
    }
    
    private void HandleMovingState()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            // Reached checkpoint
            ReachedCheckpoint();
        }
        
        // Check if player is too far
        if (waitForPlayer && !playerNearby && Vector3.Distance(transform.position, player.position) > maxDistanceFromPlayer)
        {
            ChangeState(KancilState.WaitingForPlayer);
        }
    }
    
    private void HandleWaitingAtCheckpointState()
    {
        // This state is handled by coroutine
    }
    
    private void HandleWaitingForPlayerState()
    {
        // Wait for player to come closer
        if (playerNearby)
        {
            // Return to moving or continue to next checkpoint
            if (currentCheckpointIndex < checkpoints.Count)
            {
                MoveToCurrentCheckpoint();
            }
            else
            {
                ChangeState(KancilState.Idle);
            }
        }
    }
    
    private void HandleReturningToPlayerState()
    {
        if (playerNearby)
        {
            ChangeState(KancilState.Idle);
        }
        else
        {
            // Move towards player
            navAgent.SetDestination(player.position);
        }
    }
    
    private void CheckPlayerDistance()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool wasNearby = playerNearby;
        playerNearby = distanceToPlayer <= playerDetectionRange;
        
        // Player detection changed
        if (playerNearby != wasNearby)
        {
            if (playerNearby)
            {
                Debug.Log("Player detected nearby");
            }
            else
            {
                Debug.Log("Player moved away");
            }
        }
    }
    
    public void MoveToNextCheckpoint()
    {
        if (checkpoints.Count == 0) return;
        
        // Move to next checkpoint in sequence
        if (currentCheckpointIndex >= checkpoints.Count)
        {
            if (loopCheckpoints)
            {
                currentCheckpointIndex = 0;
            }
            else
            {
                ChangeState(KancilState.Idle);
                return;
            }
        }
        
        MoveToCurrentCheckpoint();
    }
    
    private void MoveToCurrentCheckpoint()
    {
        if (currentCheckpointIndex >= checkpoints.Count) return;
        
        KancilCheckpoint targetCheckpoint = checkpoints[currentCheckpointIndex];
        navAgent.SetDestination(targetCheckpoint.transform.position);
        
        ChangeState(KancilState.MovingToCheckpoint);
        
        Debug.Log($"Moving to checkpoint {currentCheckpointIndex}: {targetCheckpoint.name}");
    }
    
    private void ReachedCheckpoint()
    {
        KancilCheckpoint checkpoint = checkpoints[currentCheckpointIndex];
        
        Debug.Log($"Reached checkpoint {currentCheckpointIndex}: {checkpoint.name}");
        
        // Trigger checkpoint event
        OnCheckpointReached?.Invoke(currentCheckpointIndex);
        checkpoint.OnKancilReached?.Invoke();
        
        // Face the checkpoint direction if specified
        if (checkpoint.HasLookDirection())
        {
            Vector3 lookDirection = checkpoint.GetLookDirection();
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        
        // Wait at checkpoint
        ChangeState(KancilState.WaitingAtCheckpoint);
        StartCoroutine(WaitAtCheckpoint(checkpoint.waitTime > 0 ? checkpoint.waitTime : waitTimeAtCheckpoint));
    }
    
    private IEnumerator WaitAtCheckpoint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        // Move to next checkpoint
        currentCheckpointIndex++;
        
        if (waitForPlayer && !playerNearby)
        {
            ChangeState(KancilState.WaitingForPlayer);
        }
        else
        {
            MoveToNextCheckpoint();
        }
    }
    
    private void ChangeState(KancilState newState)
    {
        if (currentState == newState) return;
        
        KancilState previousState = currentState;
        currentState = newState;
        
        // Handle state transitions
        switch (newState)
        {
            case KancilState.Idle:
                SetAnimationState(false);
                navAgent.ResetPath();
                break;
            case KancilState.MovingToCheckpoint:
            case KancilState.ReturningToPlayer:
                SetAnimationState(true);
                break;
            case KancilState.WaitingAtCheckpoint:
            case KancilState.WaitingForPlayer:
                SetAnimationState(false);
                break;
        }
        
        OnStateChanged?.Invoke(newState);
        Debug.Log($"Kancil state changed: {previousState} -> {newState}");
    }
    
    private void SetAnimationState(bool isWalking)
    {
        if (animator == null) return;
        
        if (isWalking)
        {
            if (!string.IsNullOrEmpty(walkAnimationTrigger))
            {
                animator.SetTrigger(walkAnimationTrigger);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(idleAnimationTrigger))
            {
                animator.SetTrigger(idleAnimationTrigger);
            }
        }
    }
    
    // Public methods untuk mengontrol kancil
    public void StartGuiding()
    {
        if (checkpoints.Count > 0)
        {
            currentCheckpointIndex = 0;
            MoveToNextCheckpoint();
        }
    }
    
    public void StopGuiding()
    {
        navAgent.ResetPath();
        ChangeState(KancilState.Idle);
    }
    
    public void SetCheckpoints(List<KancilCheckpoint> newCheckpoints)
    {
        checkpoints = newCheckpoints;
        currentCheckpointIndex = 0;
    }
    
    public void AddCheckpoint(KancilCheckpoint checkpoint)
    {
        checkpoints.Add(checkpoint);
    }
    
    public void RemoveCheckpoint(KancilCheckpoint checkpoint)
    {
        checkpoints.Remove(checkpoint);
    }
    
    public void JumpToCheckpoint(int index)
    {
        if (index >= 0 && index < checkpoints.Count)
        {
            currentCheckpointIndex = index;
            MoveToCurrentCheckpoint();
        }
    }
    
    // Getters
    public KancilState GetCurrentState() => currentState;
    public int GetCurrentCheckpointIndex() => currentCheckpointIndex;
    public int GetTotalCheckpoints() => checkpoints.Count;
    public bool IsPlayerNearby() => playerNearby;
    
    /// <summary>
    /// Update NavMeshAgent settings during runtime if needed
    /// </summary>
    public void UpdateNavMeshSettings(float newSpeed = -1f, float newAcceleration = -1f)
    {
        if (navAgent == null) return;
        
        if (newSpeed > 0)
        {
            moveSpeed = newSpeed;
            navAgent.speed = moveSpeed;
        }
        
        if (newAcceleration > 0)
        {
            navAgent.acceleration = newAcceleration;
        }
        
        Debug.Log($"NavMeshAgent settings updated - Speed: {navAgent.speed}, Acceleration: {navAgent.acceleration}");
    }
    
    /// <summary>
    /// Reset NavMeshAgent to optimal terrain settings
    /// </summary>
    public void ResetNavMeshSettings()
    {
        ConfigureNavMeshAgent();
    }
    
    /// <summary>
    /// Get current slope information for debugging
    /// </summary>
    public float GetCurrentSlopeAngle() => currentSlopeAngle;
    
    /// <summary>
    /// Toggle slope compensation on/off during runtime
    /// </summary>
    public void SetSlopeCompensation(bool enabled)
    {
        enableSlopeCompensation = enabled;
        if (!enabled)
        {
            navAgent.speed = originalSpeed; // Reset to original speed
        }
    }
    
    /// <summary>
    /// Adjust slope speed multiplier during runtime
    /// </summary>
    public void SetSlopeSpeedMultiplier(float multiplier)
    {
        slopeSpeedMultiplier = Mathf.Clamp01(multiplier);
    }
    
    // Gizmos untuk debugging
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        
        // Draw max distance from player
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistanceFromPlayer);
        
        // Draw checkpoint connections
        if (checkpoints.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < checkpoints.Count - 1; i++)
            {
                if (checkpoints[i] != null && checkpoints[i + 1] != null)
                {
                    Gizmos.DrawLine(checkpoints[i].transform.position, checkpoints[i + 1].transform.position);
                }
            }
            
            // Draw loop connection if enabled
            if (loopCheckpoints && checkpoints.Count > 0 && checkpoints[0] != null && checkpoints[checkpoints.Count - 1] != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(checkpoints[checkpoints.Count - 1].transform.position, checkpoints[0].transform.position);
            }
        }
        
        // Draw current path
        if (navAgent != null && navAgent.hasPath)
        {
            Gizmos.color = Color.cyan;
            Vector3[] pathCorners = navAgent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
        }
        
        // Draw slope detection info
        if (enableSlopeCompensation)
        {
            // Draw terrain normal
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, terrainNormal * 2f);
            
            // Draw slope angle info
            Gizmos.color = currentSlopeAngle > 15f ? Color.red : Color.green;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2f, Vector3.one * 0.2f);
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, 
                $"Slope: {currentSlopeAngle:F1}°\nSpeed: {(navAgent != null ? navAgent.speed : 0):F1}");
            #endif
        }
    }
}