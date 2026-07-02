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
                return;
            }
        }
        
        // Notify old collection manager (for backward compatibility)
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
        
        Debug.Log($"Collected {itemName} and added to inventory!");
    }
}