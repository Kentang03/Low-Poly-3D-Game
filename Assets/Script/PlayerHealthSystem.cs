using UnityEngine;
using System.Collections;

/// <summary>
/// Sistem kesehatan player dengan mekanisme kematian dan respawn
/// </summary>
public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 1f;
    
    [Header("Death Settings")]
    [SerializeField] private bool isDead = false;
    [SerializeField] private float respawnDelay = 2f;
    
    [Header("References")]
    [SerializeField] private InvectorControllerAdapter invectorAdapter;
    
    // Events
    public System.Action<int> OnHealthChanged;
    public System.Action OnPlayerDied;
    public System.Action OnPlayerRespawned;
    public System.Action OnDamageTaken;
    
    // Properties
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public bool IsInvulnerable => isInvulnerable;
    
    void Start()
    {
        InitializeHealth();
        FindControllerReferences();
    }
    
    void InitializeHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvulnerable = false;
    }
    
    void FindControllerReferences()
    {
        // Auto-find Invector adapter
        if (invectorAdapter == null)
            invectorAdapter = GetComponent<InvectorControllerAdapter>();
        
        if (invectorAdapter == null)
        {
            Debug.LogWarning("InvectorControllerAdapter not found on player! Player movement control may not work properly.");
        }
        else
        {
            Debug.Log("✅ Found InvectorControllerAdapter for player movement control");
        }
    }
    
    /// <summary>
    /// Berikan damage kepada player
    /// </summary>
    /// <param name="damage">Jumlah damage</param>
    /// <param name="damageSource">Sumber damage (opsional)</param>
    public void TakeDamage(int damage, GameObject damageSource = null)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Trigger events
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke();
        
        // Check if player should die
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Start invulnerability period after taking damage
            StartInvulnerability();
        }
    }
    
    /// <summary>
    /// Instant kill player (untuk rock collision)
    /// </summary>
    public void InstantKill(GameObject killerSource = null)
    {
        if (isDead) return;
        
        Debug.Log($"Player killed instantly by {(killerSource ? killerSource.name : "unknown source")}");
        
        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth);
        Die();
    }
    
    /// <summary>
    /// Proses kematian player
    /// </summary>
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player died!");
        
        // Disable player movement
        DisablePlayerMovement();
        
        // Trigger death event
        OnPlayerDied?.Invoke();
        
        // Start respawn process
        StartCoroutine(RespawnProcess());
    }
    
    /// <summary>
    /// Disable player movement saat mati
    /// </summary>
    void DisablePlayerMovement()
    {
        if (invectorAdapter != null)
        {
            invectorAdapter.SetCanMove(false);
            invectorAdapter.FreezeCharacter();
            Debug.Log("🔒 Player movement disabled via InvectorControllerAdapter");
        }
        else
        {
            // Direct Invector control sebagai fallback
            DisableInvectorInput();
        }
    }
    
    /// <summary>
    /// Enable player movement saat respawn
    /// </summary>
    void EnablePlayerMovement()
    {
        if (invectorAdapter != null)
        {
            invectorAdapter.SetCanMove(true);
            invectorAdapter.UnfreezeCharacter();
            Debug.Log("🔓 Player movement enabled via InvectorControllerAdapter");
        }
        else
        {
            // Direct Invector control sebagai fallback
            EnableInvectorInput();
        }
    }
    
    /// <summary>
    /// Disable Invector input components
    /// </summary>
    void DisableInvectorInput()
    {
        // Disable vThirdPersonInput
        var invectorInput = GetComponent<Invector.vCharacterController.vThirdPersonInput>();
        if (invectorInput != null)
        {
            invectorInput.enabled = false;
            Debug.Log("Disabled vThirdPersonInput");
        }
        
        // Disable vThirdPersonController if found
        var invectorController = GetComponent<Invector.vCharacterController.vThirdPersonController>();
        if (invectorController != null)
        {
            invectorController.enabled = false;
            Debug.Log("Disabled vThirdPersonController");
        }
    }
    
    /// <summary>
    /// Enable Invector input components
    /// </summary>
    void EnableInvectorInput()
    {
        // Enable vThirdPersonController first
        var invectorController = GetComponent<Invector.vCharacterController.vThirdPersonController>();
        if (invectorController != null)
        {
            invectorController.enabled = true;
            Debug.Log("Enabled vThirdPersonController");
        }
        
        // Enable vThirdPersonInput
        var invectorInput = GetComponent<Invector.vCharacterController.vThirdPersonInput>();
        if (invectorInput != null)
        {
            invectorInput.enabled = true;
            Debug.Log("Enabled vThirdPersonInput");
        }
    }
    
    /// <summary>
    /// Proses respawn dengan delay
    /// </summary>
    IEnumerator RespawnProcess()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }
    
    /// <summary>
    /// Respawn player di checkpoint terakhir
    /// </summary>
    public void Respawn()
    {
        // Get respawn position from CheckpointManager
        Vector3 respawnPosition = CheckpointManager.Instance.GetCurrentCheckpointPosition();
        Quaternion respawnRotation = CheckpointManager.Instance.GetCurrentCheckpointRotation();
        
        // Use proper Invector teleportation method
        if (invectorAdapter != null)
        {
            // Use adapter's safe teleportation
            invectorAdapter.TeleportCharacter(respawnPosition, respawnRotation);
        }
        else
        {
            // Fallback to manual teleportation
            TeleportInvectorPlayer(respawnPosition, respawnRotation);
        }
        
        // Reset health and state
        currentHealth = maxHealth;
        isDead = false;
        
        // Enable movement
        EnablePlayerMovement();
        
        // Force enable Invector components (important for vThirdPersonInput)
        if (invectorAdapter != null)
        {
            invectorAdapter.ForceEnableInvectorComponents();
        }
        
        // Start brief invulnerability
        StartInvulnerability();
        
        Debug.Log($"Player respawned at checkpoint: {CheckpointManager.Instance.GetCurrentCheckpointName()}");
        Debug.Log($"Respawn position: {respawnPosition}");
        
        // Trigger events
        OnHealthChanged?.Invoke(currentHealth);
        OnPlayerRespawned?.Invoke();
        
        // Validate respawn position after a short delay
        StartCoroutine(ValidateRespawnPosition(respawnPosition));
    }
    
    /// <summary>
    /// Validate that respawn position is maintained after teleportation
    /// </summary>
    System.Collections.IEnumerator ValidateRespawnPosition(Vector3 expectedPosition)
    {
        // Wait a moment for all systems to stabilize
        yield return new WaitForSeconds(0.5f);
        
        Vector3 currentPosition = transform.position;
        float distance = Vector3.Distance(currentPosition, expectedPosition);
        
        if (distance > 1f) // Allow small variations due to ground snapping
        {
            Debug.LogWarning($"⚠️ RESPAWN POSITION MISMATCH!");
            Debug.LogWarning($"Expected: {expectedPosition}");
            Debug.LogWarning($"Actual: {currentPosition}");
            Debug.LogWarning($"Distance: {distance:F2}m");
            
            // Try emergency correction
            if (invectorAdapter != null)
            {
                Debug.Log("Attempting emergency position correction...");
                invectorAdapter.TeleportCharacter(expectedPosition, CheckpointManager.Instance.GetCurrentCheckpointRotation());
            }
        }
        else
        {
            Debug.Log($"✅ Respawn position validated - Player at correct location (distance: {distance:F2}m)");
        }
    }
    
    /// <summary>
    /// Teleport Invector player dengan cara yang benar (tidak akan di-override)
    /// </summary>
    /// <param name="targetPosition">Target position</param>
    /// <param name="targetRotation">Target rotation</param>
    void TeleportInvectorPlayer(Vector3 targetPosition, Quaternion targetRotation)
    {
        // Get Invector controller
        var invectorController = GetComponent<Invector.vCharacterController.vThirdPersonController>();
        if (invectorController == null && invectorAdapter != null)
            invectorController = invectorAdapter.GetInvectorController();
        
        if (invectorController != null)
        {
            // Disable controller to prevent conflicts
            bool wasEnabled = invectorController.enabled;
            invectorController.enabled = false;
            
            // Reset physics
            Rigidbody rb = invectorController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // Temporarily disable physics
            }
            
            // Set position and rotation - CRITICAL: Do this while controller is disabled
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            
            // Reset Invector internal states that could cause position override
            invectorController.input = Vector3.zero;
            invectorController.inputSmooth = Vector3.zero;
            invectorController.moveDirection = Vector3.zero;
            invectorController.verticalVelocity = 0f;
            
            // Force animator to match new position (prevents root motion override)
            Animator animator = invectorController.GetComponent<Animator>();
            if (animator != null && animator.isHuman)
            {
                // Reset animator to prevent root motion conflicts
                animator.Rebind();
                animator.Update(0f); // Force update with current transform
            }
            
            // Wait a frame then re-enable controller and physics
            StartCoroutine(ReEnableControllerAfterTeleport(invectorController, rb, wasEnabled));
            
            Debug.Log($"🎯 Invector player teleported to: {targetPosition}");
        }
        else
        {
            // Fallback untuk non-Invector controller
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            Debug.LogWarning("Using fallback teleportation - Invector controller not found");
        }
    }
    
    /// <summary>
    /// Re-enable controller after teleport to prevent position conflicts
    /// </summary>
    System.Collections.IEnumerator ReEnableControllerAfterTeleport(Invector.vCharacterController.vThirdPersonController controller, Rigidbody rb, bool wasEnabled)
    {
        // Wait two frames untuk memastikan transform sudah di-set
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        // Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Re-enable controller
        if (controller != null)
        {
            controller.enabled = wasEnabled;
        }
        
        Debug.Log("✅ Invector controller re-enabled after teleport");
    }
    
    /// <summary>
    /// Start temporary invulnerability
    /// </summary>
    void StartInvulnerability()
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }
    
    /// <summary>
    /// Invulnerability coroutine
    /// </summary>
    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
    
    /// <summary>
    /// Heal player
    /// </summary>
    /// <param name="healAmount">Amount to heal</param>
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"Player healed for {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// Set max health (untuk power-ups)
    /// </summary>
    /// <param name="newMaxHealth">New maximum health</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Reset health system
    /// </summary>
    public void ResetHealthSystem()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvulnerable = false;
        
        EnablePlayerMovement();
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Test respawn tanpa mati (untuk debugging)
    /// </summary>
    [ContextMenu("Test Respawn Position")]
    public void TestRespawn()
    {
        Debug.Log("=== TESTING RESPAWN SYSTEM ===");
        
        Vector3 beforePosition = transform.position;
        Vector3 respawnPos = CheckpointManager.Instance.GetCurrentCheckpointPosition();
        
        Debug.Log($"Current Position: {beforePosition}");
        Debug.Log($"Target Respawn Position: {respawnPos}");
        
        // Force respawn
        if (invectorAdapter != null)
        {
            invectorAdapter.TeleportCharacter(respawnPos, CheckpointManager.Instance.GetCurrentCheckpointRotation());
        }
        else
        {
            TeleportInvectorPlayer(respawnPos, CheckpointManager.Instance.GetCurrentCheckpointRotation());
        }
        
        // Validate immediately
        StartCoroutine(TestRespawnValidation(respawnPos));
    }
    
    /// <summary>
    /// Validation coroutine untuk test respawn
    /// </summary>
    System.Collections.IEnumerator TestRespawnValidation(Vector3 expectedPos)
    {
        yield return new WaitForSeconds(1f);
        
        Vector3 actualPos = transform.position;
        float distance = Vector3.Distance(actualPos, expectedPos);
        
        Debug.Log($"=== RESPAWN TEST RESULT ===");
        Debug.Log($"Expected Position: {expectedPos}");
        Debug.Log($"Actual Position: {actualPos}");
        Debug.Log($"Distance Difference: {distance:F2}m");
        
        if (distance < 1f)
        {
            Debug.Log("✅ RESPAWN TEST PASSED!");
        }
        else
        {
            Debug.LogError("❌ RESPAWN TEST FAILED - Position mismatch detected");
            
            if (invectorAdapter != null)
            {
                invectorAdapter.ValidatePlayerPosition();
            }
        }
    }
}