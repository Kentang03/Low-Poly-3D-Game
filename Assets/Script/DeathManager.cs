using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manager untuk mengatur proses kematian dan respawn player
/// Menangani UI, effects, dan koordinasi antara sistem
/// </summary>
public class DeathManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Text deathMessageText;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Text checkpointInfoText;
    
    [Header("Death Effects")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathSoundVolume = 1f;
    
    [Header("Respawn Settings")]
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private float autoRespawnDelay = 3f;
    [SerializeField] private bool showRespawnButton = true;
    
    [Header("Camera Effects")]
    [SerializeField] private bool enableCameraShake = true;
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float shakeDuration = 0.5f;
    
    [Header("References")]
    [SerializeField] private PlayerHealthSystem playerHealth;
    [SerializeField] private CheckpointManager checkpointManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource audioSource;
    
    // Singleton pattern
    private static DeathManager instance;
    public static DeathManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DeathManager>();
            }
            return instance;
        }
    }
    
    // State tracking
    private bool isPlayerDead = false;
    private Coroutine autoRespawnCoroutine;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeDeathManager();
        SetupEventListeners();
    }
    
    void InitializeDeathManager()
    {
        // Find references if not assigned
        FindRequiredReferences();
        
        // Setup UI
        SetupUI();
        
        // Setup audio
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    
    void FindRequiredReferences()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealthSystem>();
            
        if (checkpointManager == null)
            checkpointManager = CheckpointManager.Instance;
            
        if (gameManager == null)
            gameManager = GameManager.Instance;
    }
    
    void SetupUI()
    {
        // Hide death panel initially
        if (deathPanel != null)
            deathPanel.SetActive(false);
            
        // Setup respawn button
        if (respawnButton != null)
        {
            respawnButton.onClick.RemoveAllListeners();
            respawnButton.onClick.AddListener(ManualRespawn);
            respawnButton.gameObject.SetActive(showRespawnButton);
        }
        
        // Set default death message
        if (deathMessageText != null)
            deathMessageText.text = "You Died!";
    }
    
    void SetupEventListeners()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied += HandlePlayerDeath;
            playerHealth.OnPlayerRespawned += HandlePlayerRespawned;
        }
    }
    
    void OnDestroy()
    {
        // Cleanup event listeners
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied -= HandlePlayerDeath;
            playerHealth.OnPlayerRespawned -= HandlePlayerRespawned;
        }
    }
    
    /// <summary>
    /// Handle player death event
    /// </summary>
    void HandlePlayerDeath()
    {
        if (isPlayerDead) return;
        
        isPlayerDead = true;
        Debug.Log("DeathManager: Player death detected");
        
        // Play death effects
        PlayDeathEffects();
        
        // Show death UI
        ShowDeathUI();
        
        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Start auto respawn if enabled
        if (autoRespawn)
        {
            autoRespawnCoroutine = StartCoroutine(AutoRespawnCountdown());
        }
    }
    
    /// <summary>
    /// Handle player respawn event
    /// </summary>
    void HandlePlayerRespawned()
    {
        isPlayerDead = false;
        Debug.Log("DeathManager: Player respawned");
        
        // Hide death UI
        HideDeathUI();
        
        // Lock cursor back for gameplay
        if (gameManager != null && gameManager.isGameStarted)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Cancel auto respawn coroutine if running
        if (autoRespawnCoroutine != null)
        {
            StopCoroutine(autoRespawnCoroutine);
            autoRespawnCoroutine = null;
        }
    }
    
    /// <summary>
    /// Play visual and audio effects when player dies
    /// </summary>
    void PlayDeathEffects()
    {
        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.volume = deathSoundVolume;
            audioSource.Play();
        }
        
        // Spawn death effect at player position
        if (deathEffect != null && playerHealth != null)
        {
            GameObject effect = Instantiate(deathEffect, playerHealth.transform.position, Quaternion.identity);
            Destroy(effect, 5f); // Auto cleanup
        }
        
        // Camera shake effect
        if (enableCameraShake)
        {
            StartCoroutine(CameraShakeEffect());
        }
    }
    
    /// <summary>
    /// Camera shake effect coroutine
    /// </summary>
    IEnumerator CameraShakeEffect()
    {
        Transform cameraTransform = Camera.main?.transform;
        if (cameraTransform == null) yield break;
        
        Vector3 originalPosition = cameraTransform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            cameraTransform.localPosition = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        cameraTransform.localPosition = originalPosition;
    }
    
    /// <summary>
    /// Show death UI panel
    /// </summary>
    void ShowDeathUI()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            
            // Update checkpoint info
            if (checkpointInfoText != null && checkpointManager != null)
            {
                string checkpointName = checkpointManager.GetCurrentCheckpointName();
                checkpointInfoText.text = $"Respawn at: {checkpointName}";
            }
        }
    }
    
    /// <summary>
    /// Hide death UI panel
    /// </summary>
    void HideDeathUI()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }
    
    /// <summary>
    /// Auto respawn countdown coroutine
    /// </summary>
    IEnumerator AutoRespawnCountdown()
    {
        float countdown = autoRespawnDelay;
        
        while (countdown > 0 && isPlayerDead)
        {
            // Update UI with countdown
            if (deathMessageText != null)
            {
                deathMessageText.text = $"Respawning in {countdown:F0}...";
            }
            
            countdown -= Time.deltaTime;
            yield return null;
        }
        
        // Auto respawn if still dead
        if (isPlayerDead)
        {
            ManualRespawn();
        }
    }
    
    /// <summary>
    /// Manual respawn triggered by button or external call
    /// </summary>
    public void ManualRespawn()
    {
        if (!isPlayerDead) return;
        
        Debug.Log("DeathManager: Manual respawn triggered");
        
        // Respawn player through health system
        if (playerHealth != null)
        {
            playerHealth.Respawn();
        }
    }
    
    /// <summary>
    /// Set custom death message
    /// </summary>
    /// <param name="message">Death message to display</param>
    public void SetDeathMessage(string message)
    {
        if (deathMessageText != null)
            deathMessageText.text = message;
    }
    
    /// <summary>
    /// Trigger player death from external source
    /// </summary>
    /// <param name="cause">Cause of death (optional)</param>
    public void TriggerPlayerDeath(string cause = "Unknown")
    {
        if (playerHealth != null)
        {
            SetDeathMessage($"You died from {cause}!");
            playerHealth.InstantKill();
        }
    }
    
    /// <summary>
    /// Check if player is currently dead
    /// </summary>
    /// <returns>True if player is dead</returns>
    public bool IsPlayerDead()
    {
        return isPlayerDead;
    }
    
    /// <summary>
    /// Force hide death UI (untuk testing)
    /// </summary>
    public void ForceHideDeathUI()
    {
        HideDeathUI();
        isPlayerDead = false;
        
        if (autoRespawnCoroutine != null)
        {
            StopCoroutine(autoRespawnCoroutine);
            autoRespawnCoroutine = null;
        }
    }
}