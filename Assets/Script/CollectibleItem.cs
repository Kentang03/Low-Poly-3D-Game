using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Crystal";
    public string itemDescription = "A mysterious crystal";
    public Sprite itemIcon; // Icon untuk inventory
    public bool canBePlaced = false; // Apakah item bisa ditempatkan
    public GameObject itemPrefab; // Prefab untuk placement system
    public int itemValue = 1;
    public bool isCollected = false;
    
    [Header("Visual Effects")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public GameObject collectEffect;
    
    [Header("Audio")]
    public AudioClip collectSound;
    public string collectSoundResourcePath = "Audio/CollectSound"; // Path untuk Resources.Load
    
    private Vector3 startPosition;
    public AudioSource audioSource;
    
    // Static AudioSource untuk backup jika local AudioSource tidak bekerja
    private static AudioSource globalAudioSource;
    
    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        
        // Add AudioSource jika belum ada
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource settings
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.spatialBlend = 0f; // 2D sound (not affected by distance)
        }
        
        // Load audio from Resources jika collectSound null (untuk instantiated objects)
        if (collectSound == null && !string.IsNullOrEmpty(collectSoundResourcePath))
        {
            collectSound = Resources.Load<AudioClip>(collectSoundResourcePath);
            if (collectSound != null)
            {
                Debug.Log($"Loaded collect sound from Resources: {collectSound.name}");
            }
            else
            {
                Debug.LogWarning($"Could not load collect sound from path: {collectSoundResourcePath}");
            }
        }
        
        // Create global AudioSource jika belum ada
        if (globalAudioSource == null)
        {
            GameObject audioGO = new GameObject("GlobalCollectAudioSource");
            globalAudioSource = audioGO.AddComponent<AudioSource>();
            globalAudioSource.playOnAwake = false;
            globalAudioSource.loop = false;
            globalAudioSource.volume = 1f;
            globalAudioSource.pitch = 1f;
            globalAudioSource.spatialBlend = 0f;
            DontDestroyOnLoad(audioGO);
            Debug.Log("Created global AudioSource for collect sounds");
        }
    }
    
    void Update()
    {
        if (!isCollected)
        {
            // Rotate item on Y axis only
            float yRotation = transform.eulerAngles.y + (rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            
            // Bob up and down
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player collected the item
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectItem();
        }
    }
    
    void CollectItem()
    {
        isCollected = true;
        
        // Hide the item visually first
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        
        // Spawn collect effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, transform.rotation);
        }
        
        // Add to inventory system
        if (InventorySystem.Instance != null)
        {
            bool addedToInventory = InventorySystem.Instance.AddItem(
                itemName,
                itemDescription,
                itemIcon,
                canBePlaced,
                itemPrefab
            );
            
            if (!addedToInventory)
            {
                Debug.Log("Inventory penuh! Item tidak dapat diambil.");
                isCollected = false;
                // Re-enable visuals if inventory full
                GetComponent<Renderer>().enabled = true;
                GetComponent<Collider>().enabled = true;
                return;
            }
        }
        
        // Notify old collection manager (for backward compatibility)
        ItemCollectionManager manager = FindObjectOfType<ItemCollectionManager>();
        if (manager != null)
        {
            manager.CollectItem(itemName, itemValue);
        }
        
        // Play collect sound and handle destruction
        if (collectSound != null)
        {
            bool soundPlayed = false;
            
            // Method 1: Try AudioManager (recommended)
            if (AudioManager.Instance != null)
            {
                AudioManager.PlaySound(collectSound);
                soundPlayed = true;
                Debug.Log($"Playing collect sound via AudioManager: {collectSound.name}");
            }
            
            // Method 2: Try local AudioSource
            if (!soundPlayed && audioSource != null)
            {
                // Ensure AudioSource settings are correct
                audioSource.volume = 1f;
                audioSource.pitch = 1f;
                audioSource.spatialBlend = 0f; // Make it 2D sound
                audioSource.priority = 0; // Highest priority
                audioSource.mute = false;
                audioSource.enabled = true;
                
                // Try PlayOneShot first
                audioSource.PlayOneShot(collectSound);
                soundPlayed = true;
                
                Debug.Log($"Playing collect sound via local AudioSource: {collectSound.name}");
            }
            
            // Method 3: Try global AudioSource as backup
            if (!soundPlayed && globalAudioSource != null)
            {
                globalAudioSource.PlayOneShot(collectSound);
                Debug.Log("Playing collect sound via Global AudioSource");
                soundPlayed = true;
            }
            
            // Method 4: Try Camera AudioSource as last resort
            if (!soundPlayed)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    AudioSource cameraAudio = mainCamera.GetComponent<AudioSource>();
                    if (cameraAudio == null)
                        cameraAudio = mainCamera.gameObject.AddComponent<AudioSource>();
                    
                    cameraAudio.PlayOneShot(collectSound);
                    Debug.Log("Playing collect sound via Camera AudioSource");
                    soundPlayed = true;
                }
            }
            
            if (soundPlayed)
            {
                // Use coroutine to wait for sound to finish
                StartCoroutine(DestroyAfterSound());
            }
            else
            {
                Debug.LogError("Failed to play collect sound with all methods!");
                Destroy(gameObject, 0.1f);
            }
        }
        else
        {
            Debug.LogWarning("Collect sound is null! Trying to use AudioManager default sound...");
            
            // Try using AudioManager's default sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCollectSound();
                Debug.Log("Playing default collect sound via AudioManager");
                StartCoroutine(DestroyAfterSound());
            }
            else
            {
                Debug.LogError("No collect sound available and AudioManager not found!");
                Destroy(gameObject, 0.1f);
            }
        }
        
        Debug.Log($"Collected {itemName} and added to inventory!");
    }
    
    System.Collections.IEnumerator DestroyAfterSound()
    {
        // Wait for the sound to finish playing
        yield return new WaitForSeconds(collectSound.length + 0.4f);
        
        // Make sure the sound actually finished
        while (audioSource != null && audioSource.isPlaying)
        {
            yield return null;
        }
        
        // Now destroy the object
        Destroy(gameObject);
    }
}