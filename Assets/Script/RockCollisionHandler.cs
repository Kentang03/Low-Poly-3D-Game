using UnityEngine;

/// <summary>
/// Handler untuk collision rock dengan player - menyebabkan instant death
/// </summary>
public class RockCollisionHandler : MonoBehaviour
{
    [Header("Rock Settings")]
    [SerializeField] private float damageAmount = 100f; // Instant kill
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private float destroyDelay = 0.1f;
    
    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float soundVolume = 1f;
    
    [Header("Physics")]
    [SerializeField] private float minimumVelocity = 1f; // Minimum velocity untuk damage
    [SerializeField] private bool debugCollisions = true;
    
    private AudioSource audioSource;
    private Rigidbody rockRigidbody;
    private bool hasHitPlayer = false;
    
    void Start()
    {
        InitializeRock();
    }
    
    void InitializeRock()
    {
        // Get components
        rockRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        // Add AudioSource if not present
        if (audioSource == null && hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Ensure Rigidbody exists
        if (rockRigidbody == null)
        {
            rockRigidbody = gameObject.AddComponent<Rigidbody>();
            rockRigidbody.mass = 1f;
            rockRigidbody.linearDamping = 0.1f;
        }
        
        // Ensure Collider exists
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        
        // Make sure it's not a trigger for physical collision
        col.isTrigger = false;
        
        if (debugCollisions)
        {
            Debug.Log($"Rock {gameObject.name} initialized with collision detection");
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Fallback untuk trigger collision
        Collision fakeCollision = new Collision();
        HandleTriggerCollision(other);
    }
    
    void HandleCollision(Collision collision)
    {
        if (hasHitPlayer) return; // Prevent multiple hits
        
        GameObject hitObject = collision.gameObject;
        
        if (debugCollisions)
        {
            Debug.Log($"Rock collision with: {hitObject.name} (Tag: {hitObject.tag})");
        }
        
        // Check if hit player
        if (IsPlayer(hitObject))
        {
            float velocity = rockRigidbody != null ? rockRigidbody.linearVelocity.magnitude : 0f;
            
            if (velocity >= minimumVelocity)
            {
                DamagePlayer(hitObject, collision.contacts[0].point);
            }
            else if (debugCollisions)
            {
                Debug.Log($"Rock velocity too low for damage: {velocity} < {minimumVelocity}");
            }
        }
    }
    
    void HandleTriggerCollision(Collider other)
    {
        if (hasHitPlayer) return;
        
        GameObject hitObject = other.gameObject;
        
        if (debugCollisions)
        {
            Debug.Log($"Rock trigger with: {hitObject.name} (Tag: {hitObject.tag})");
        }
        
        if (IsPlayer(hitObject))
        {
            DamagePlayer(hitObject, transform.position);
        }
    }
    
    bool IsPlayer(GameObject obj)
    {
        // Check multiple ways to identify player
        return obj.CompareTag("Player") || 
               obj.GetComponent<PlayerHealthSystem>() != null ||
               obj.GetComponent<InvectorControllerAdapter>() != null ||
               obj.GetComponent<CharacterController>() != null ||
               obj.name.ToLower().Contains("player");
    }
    
    void DamagePlayer(GameObject player, Vector3 hitPoint)
    {
        hasHitPlayer = true;
        
        if (debugCollisions)
        {
            Debug.Log($"Rock hit player: {player.name} at position {hitPoint}");
        }
        
        // Get player health system
        PlayerHealthSystem healthSystem = player.GetComponent<PlayerHealthSystem>();
        
        if (healthSystem == null)
        {
            // Try to find health system on parent or child objects
            healthSystem = player.GetComponentInParent<PlayerHealthSystem>();
            if (healthSystem == null)
                healthSystem = player.GetComponentInChildren<PlayerHealthSystem>();
        }
        
        if (healthSystem != null)
        {
            // Instant kill player
            healthSystem.InstantKill(gameObject);
            Debug.Log($"Player killed by rock: {gameObject.name}");
        }
        else
        {
            Debug.LogError($"PlayerHealthSystem not found on {player.name}! Cannot damage player.");
        }
        
        // Play effects
        PlayHitEffects(hitPoint);
        
        // Destroy rock after hit
        if (destroyOnHit)
        {
            DestroyRock();
        }
    }
    
    void PlayHitEffects(Vector3 hitPosition)
    {
        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.clip = hitSound;
            audioSource.volume = soundVolume;
            audioSource.Play();
        }
        
        // Spawn hit effect
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, hitPosition, Quaternion.identity);
            
            // Auto-destroy effect after 5 seconds
            Destroy(effect, 5f);
        }
    }
    
    void DestroyRock()
    {
        if (debugCollisions)
        {
            Debug.Log($"Destroying rock: {gameObject.name}");
        }
        
        // Disable collider to prevent further collisions
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        // Stop physics
        if (rockRigidbody != null)
        {
            rockRigidbody.isKinematic = true;
        }
        
        // Destroy after delay (to let sound finish)
        Destroy(gameObject, destroyDelay);
    }
    
    /// <summary>
    /// Set damage amount (untuk customization)
    /// </summary>
    /// <param name="damage">Damage amount</param>
    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }
    
    /// <summary>
    /// Set minimum velocity untuk damage
    /// </summary>
    /// <param name="minVel">Minimum velocity</param>
    public void SetMinimumVelocity(float minVel)
    {
        minimumVelocity = minVel;
    }
    
    void OnDrawGizmos()
    {
        if (debugCollisions)
        {
            // Draw collision sphere
            Gizmos.color = hasHitPlayer ? Color.red : Color.yellow;
            
            Collider col = GetComponent<Collider>();
            if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawWireSphere(transform.position, sphereCol.radius);
            }
            else if (col != null)
            {
                Gizmos.DrawWireCube(transform.position, col.bounds.size);
            }
        }
    }
}