using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ItemReceiver : MonoBehaviour
{
    [Header("Receiver Settings")]
    public string[] acceptedItemNames; // Item apa saja yang bisa diterima
    public bool acceptAnyItem = false; // Terima semua item
    public bool consumeItem = true; // Apakah item akan hilang setelah ditempatkan
    public bool oneTimeUse = false; // Hanya bisa digunakan sekali
    public float interactionRange = 3f; // Jarak untuk interaksi
    
    [Header("Visual Feedback")]
    public GameObject highlightObject; // Object untuk highlight saat item compatible
    public GameObject interactionPrompt; // UI prompt "Press E to interact"
    public Material highlightMaterial;
    public Color highlightColor = Color.green;
    public Color noItemColor = Color.red;
    
    [Header("Audio")]
    public AudioClip receiveSound;
    public AudioClip rejectSound;
    public AudioClip noItemSound;
    
    [Header("Events")]
    public UnityEvent<string> OnItemReceived; // Event saat item diterima
    public UnityEvent OnItemRejected; // Event saat item ditolak
    
    private bool hasBeenUsed = false;
    private bool playerNearby = false;
    private GameObject player;
    private AudioSource audioSource;
    private Renderer originalRenderer;
    private Material originalMaterial;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Store original material for highlighting
        originalRenderer = GetComponent<Renderer>();
        if (originalRenderer != null)
            originalMaterial = originalRenderer.material;
        
        // Hide highlight and prompt by default
        if (highlightObject != null)
            highlightObject.SetActive(false);
            
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        CheckPlayerProximity();
        HandleInteractionInput();
    }
    
    void CheckPlayerProximity()
    {
        if (player == null)
            return;
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool wasNearby = playerNearby;
        playerNearby = distance <= interactionRange;
        
        // Show/hide interaction elements based on proximity
        if (playerNearby != wasNearby)
        {
            UpdateVisualCues();
        }
    }
    
    void UpdateVisualCues()
    {
        if (hasBeenUsed && oneTimeUse)
        {
            // Already used, hide all cues
            SetHighlight(false);
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            return;
        }
        
        if (playerNearby)
        {
            // Check if player has required item
            bool hasRequiredItem = PlayerHasRequiredItem();
            
            // Set highlight color based on item availability
            SetHighlight(true, hasRequiredItem);
            
            // Show interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                
                // Update prompt text if it has TextMeshPro component
                TextMeshProUGUI promptText = interactionPrompt.GetComponentInChildren<TextMeshProUGUI>();
                if (promptText != null)
                {
                    if (hasRequiredItem)
                    {
                        string itemName = GetRequiredItemName();
                        promptText.text = $"Press E to use {itemName}";
                        promptText.color = Color.white;
                    }
                    else
                    {
                        promptText.text = "Required item not found";
                        promptText.color = Color.red;
                    }
                }
            }
        }
        else
        {
            // Player not nearby, hide all cues
            SetHighlight(false);
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
    
    bool PlayerHasRequiredItem()
    {
        if (InventorySystem.Instance == null)
            return false;
        
        if (acceptAnyItem)
        {
            return InventorySystem.Instance.GetAllItems().Count > 0;
        }
        
        foreach (string itemName in acceptedItemNames)
        {
            if (InventorySystem.Instance.HasItem(itemName))
                return true;
        }
        
        return false;
    }
    
    string GetRequiredItemName()
    {
        if (InventorySystem.Instance == null)
            return "item";
        
        if (acceptAnyItem)
        {
            var items = InventorySystem.Instance.GetAllItems();
            return items.Count > 0 ? items[0].itemName : "item";
        }
        
        foreach (string itemName in acceptedItemNames)
        {
            if (InventorySystem.Instance.HasItem(itemName))
                return itemName;
        }
        
        return acceptedItemNames.Length > 0 ? acceptedItemNames[0] : "item";
    }
    
    void HandleInteractionInput()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }
    
    void TryInteract()
    {
        if (hasBeenUsed && oneTimeUse)
        {
            Debug.Log("This receiver has already been used!");
            return;
        }
        
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem not found!");
            return;
        }
        
        // Find required item in inventory
        string foundItemName = null;
        
        if (acceptAnyItem)
        {
            var items = InventorySystem.Instance.GetAllItems();
            if (items.Count > 0)
                foundItemName = items[0].itemName;
        }
        else
        {
            foreach (string itemName in acceptedItemNames)
            {
                if (InventorySystem.Instance.HasItem(itemName))
                {
                    foundItemName = itemName;
                    break;
                }
            }
        }
        
        if (foundItemName != null)
        {
            ReceiveItemSuccess(foundItemName);
                
        }
        else
        {
            // No required item found
            NoItemReaction();
        }
    }
    
    public bool CanReceiveItem(string itemName)
    {
        // Check if already used
        if (oneTimeUse && hasBeenUsed)
            return false;
        
        // Check if accepts any item
        if (acceptAnyItem)
            return true;
        
        // Check if item name is in accepted list
        foreach (string acceptedName in acceptedItemNames)
        {
            if (itemName.Equals(acceptedName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        
        return false;
    }
    
    void ReceiveItemSuccess(string itemName)
    {
        Debug.Log($"ItemReceiver {gameObject.name} received item: {itemName}");
        
        // Mark as used if one-time use
        if (oneTimeUse)
            hasBeenUsed = true;
        
        // Play success sound
        if (receiveSound != null && audioSource != null)
            audioSource.PlayOneShot(receiveSound);
        
        // Trigger event
        OnItemReceived?.Invoke(itemName);
        
        // Update visual cues
        UpdateVisualCues();
        
        Debug.Log($"Successfully used {itemName} on {gameObject.name}!");
    }
    
    void NoItemReaction()
    {
        Debug.Log($"No required item for {gameObject.name}");
        
        // Play no item sound
        if (noItemSound != null && audioSource != null)
            audioSource.PlayOneShot(noItemSound);
        else if (rejectSound != null && audioSource != null)
            audioSource.PlayOneShot(rejectSound);
        
        // Trigger reject event
        OnItemRejected?.Invoke();
        
        // Brief red highlight
        StartCoroutine(BriefErrorHighlight());
    }
    
    System.Collections.IEnumerator BriefErrorHighlight()
    {
        SetHighlight(true, false); // Red highlight
        yield return new WaitForSeconds(0.5f);
        UpdateVisualCues(); // Return to normal state
    }
    
    public void SetHighlight(bool enabled, bool hasRequiredItem = true)
    {
        // Highlight with separate object
        if (highlightObject != null)
        {
            highlightObject.SetActive(enabled);
        }
        // Or highlight with material change
        else if (originalRenderer != null)
        {
            if (enabled)
            {
                Color targetColor = hasRequiredItem ? highlightColor : noItemColor;
                
                if (highlightMaterial != null)
                {
                    originalRenderer.material = highlightMaterial;
                    originalRenderer.material.color = targetColor;
                }
                else
                {
                    Material tempMaterial = new Material(originalMaterial);
                    tempMaterial.color = targetColor;
                    originalRenderer.material = tempMaterial;
                }
            }
            else
            {
                originalRenderer.material = originalMaterial;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw interaction range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    
    // Method to reset receiver (useful for debugging or game reset)
    public void Reset()
    {
        hasBeenUsed = false;
        SetHighlight(false);
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    [ContextMenu("Test Highlight")]
    public void TestHighlight()
    {
        bool currentState = highlightObject != null ? highlightObject.activeInHierarchy : false;
        SetHighlight(!currentState);
    }
    
    [ContextMenu("Test Interaction")]
    public void TestInteraction()
    {
        TryInteract();
    }
}