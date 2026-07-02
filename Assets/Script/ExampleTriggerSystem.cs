using UnityEngine;

public class ExampleTriggerSystem : MonoBehaviour
{
    [Header("Trigger Example")]
    public string requiredItemName = "Key"; // Item yang dibutuhkan untuk trigger
    public GameObject targetObject; // Object yang akan diaktifkan/dinonaktifkan
    public bool activateObject = true; // True untuk activate, false untuk deactivate
    
    [Header("Door Example")]
    public Animator doorAnimator; // Untuk animasi pintu
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    
    [Header("Audio")]
    public AudioClip successSound;
    public AudioClip failSound;
    
    private AudioSource audioSource;
    private ItemReceiver itemReceiver;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        itemReceiver = GetComponent<ItemReceiver>();
        if (itemReceiver != null)
        {
            // Subscribe to item received event
            itemReceiver.OnItemReceived.AddListener(OnItemReceived);
        }
    }
    
    void OnItemReceived(string itemName)
    {
        Debug.Log($"Item received: {itemName}");
        
        if (itemName.Equals(requiredItemName, System.StringComparison.OrdinalIgnoreCase))
        {
            TriggerSuccess();
        }
        else
        {
            TriggerFail();
        }
    }
    
    void TriggerSuccess()
    {
        Debug.Log($"Trigger activated! Required item '{requiredItemName}' was received.");
        
        // Play success sound
        if (successSound != null && audioSource != null)
            audioSource.PlayOneShot(successSound);
        
        // Activate/deactivate target object
        if (targetObject != null)
        {
            targetObject.SetActive(activateObject);
            Debug.Log($"Target object {targetObject.name} set to {activateObject}");
        }
        
        // Animate door
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTrigger);
            Debug.Log("Door opened!");
        }
        
        // Add your custom trigger logic here
        // Examples:
        // - Spawn enemies
        // - Change lighting
        // - Start cutscene
        // - Unlock new area
        // - Show UI message
        // - Change music
        
        // Example: Show message
        ShowMessage($"You used {requiredItemName}! Something happened!");
    }
    
    void TriggerFail()
    {
        Debug.Log("Wrong item used for trigger!");
        
        // Play fail sound
        if (failSound != null && audioSource != null)
            audioSource.PlayOneShot(failSound);
        
        ShowMessage("This item doesn't work here...");
    }
    
    void ShowMessage(string message)
    {
        // Simple debug message - you can replace this with proper UI
        Debug.Log($"MESSAGE: {message}");
        
        // TODO: Implement proper UI message system
        // Example: FindObjectOfType<UIMessageSystem>()?.ShowMessage(message);
    }
    
    // Example method untuk reset trigger
    public void ResetTrigger()
    {
        if (targetObject != null)
            targetObject.SetActive(!activateObject);
        
        if (doorAnimator != null)
            doorAnimator.SetTrigger(closeTrigger);
        
        Debug.Log("Trigger reset!");
    }
    
    // Example method untuk custom action
    public void CustomAction()
    {
        targetObject.SetActive(false);
    }
}