using UnityEngine;

[System.Serializable]
public class ItemSetupData
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public bool canBePlaced = false;
    public GameObject itemPrefab;
}

public class InventorySystemSetup : MonoBehaviour
{
    [Header("System Setup")]
    public bool autoSetup = true;
    
    [Header("Sample Items")]
    public ItemSetupData[] sampleItems;
    
    [Header("UI Setup")]
    public Canvas uiCanvas;
    public GameObject inventoryPanelPrefab;
    public GameObject collectedItemsContainerPrefab;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupInventorySystem();
        }
    }
    
    [ContextMenu("Setup Inventory System")]
    public void SetupInventorySystem()
    {
        Debug.Log("Setting up Inventory System...");
        
        // Ensure InventorySystem exists
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            GameObject inventoryGO = new GameObject("InventorySystem");
            inventorySystem = inventoryGO.AddComponent<InventorySystem>();
        }
        
        // Setup UI if canvas exists
        if (uiCanvas != null)
        {
            SetupUI();
        }
        
        Debug.Log("Inventory System setup completed!");
    }
    
    void SetupUI()
    {
        // Create InventoryUI if it doesn't exist
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
        {
            GameObject uiGO = new GameObject("InventoryUI");
            uiGO.transform.SetParent(uiCanvas.transform);
            inventoryUI = uiGO.AddComponent<InventoryUI>();
        }
        
        // Create inventory panel if prefab exists
        if (inventoryPanelPrefab != null && inventoryUI.inventoryPanel == null)
        {
            GameObject panel = Instantiate(inventoryPanelPrefab, uiCanvas.transform);
            inventoryUI.inventoryPanel = panel.transform;
        }
        
        // Create collected items container if prefab exists
        if (collectedItemsContainerPrefab != null && inventoryUI.collectedItemsContainer == null)
        {
            GameObject container = Instantiate(collectedItemsContainerPrefab, uiCanvas.transform);
            inventoryUI.collectedItemsContainer = container.transform;
        }
    }
    
    [ContextMenu("Add Sample Items to Inventory")]
    public void AddSampleItemsToInventory()
    {
        InventorySystem inventorySystem = InventorySystem.Instance;
        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem tidak ditemukan!");
            return;
        }
        
        foreach (ItemSetupData itemData in sampleItems)
        {
            inventorySystem.AddItem(
                itemData.itemName,
                itemData.itemDescription,
                itemData.itemIcon,
                itemData.canBePlaced,
                itemData.itemPrefab
            );
        }
        
        Debug.Log($"Added {sampleItems.Length} sample items to inventory!");
    }
    
    [ContextMenu("Create Item Receiver")]
    public void CreateItemReceiver()
    {
        GameObject receiverGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        receiverGO.name = "ItemReceiver";
        receiverGO.transform.position = Vector3.forward * 3f;
        
        // Add ItemReceiver component
        ItemReceiver receiver = receiverGO.AddComponent<ItemReceiver>();
        
        // Add ExampleTriggerSystem
        ExampleTriggerSystem trigger = receiverGO.AddComponent<ExampleTriggerSystem>();
        
        // Setup collider as trigger
        Collider col = receiverGO.GetComponent<Collider>();
        col.isTrigger = true;
        
        Debug.Log("Created ItemReceiver with ExampleTriggerSystem!");
    }
    
    [ContextMenu("Create Collectible Item")]
    public void CreateCollectibleItem()
    {
        GameObject collectibleGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        collectibleGO.name = "CollectibleItem";
        collectibleGO.transform.position = Vector3.forward * 2f;
        
        // Add CollectibleItem component
        CollectibleItem collectible = collectibleGO.AddComponent<CollectibleItem>();
        
        // Setup as trigger
        Collider col = collectibleGO.GetComponent<Collider>();
        col.isTrigger = true;
        
        Debug.Log("Created CollectibleItem!");
    }
}