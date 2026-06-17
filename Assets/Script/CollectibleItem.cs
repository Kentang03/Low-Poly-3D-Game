using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Crystal";
    public int itemValue = 1;
    public bool isCollected = false;
    
    [Header("Visual Effects")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public GameObject collectEffect;
    
    [Header("Audio")]
    public AudioClip collectSound;
    
    private Vector3 startPosition;
    private AudioSource audioSource;
    
    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        
        // Add AudioSource jika belum ada
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Update()
    {
        if (!isCollected)
        {
            // Rotate item
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            
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
        
        // Play collect sound
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // Spawn collect effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, transform.rotation);
        }
        
        // Notify collection manager
        ItemCollectionManager manager = FindObjectOfType<ItemCollectionManager>();
        if (manager != null)
        {
            manager.CollectItem(itemName, itemValue);
        }
        
        // Hide the item (don't destroy immediately to let sound play)
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        
        // Destroy after sound finishes
        Destroy(gameObject, collectSound != null ? collectSound.length : 0.1f);
        
        Debug.Log($"Collected {itemName}!");
    }
}