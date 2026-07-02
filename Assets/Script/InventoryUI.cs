using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform inventoryPanel;
    public GameObject inventorySlotPrefab;
    public GameObject collectedItemDisplayPrefab; // Prefab untuk menampilkan item di pojok kanan atas
    public Transform collectedItemsContainer; // Container di pojok kanan atas
    public Button toggleInventoryButton;
    
    [Header("Inventory Display Settings")]
    public int maxDisplayedCollectedItems = 5; // Maksimal item yang ditampilkan di pojok kanan atas
    public float displayDuration = 3f; // Durasi tampilan item baru
    public bool pauseGameWhenOpen = false; // Apakah game pause saat inventory dibuka
    
    private List<GameObject> inventorySlots = new List<GameObject>();
    private List<GameObject> collectedItemDisplays = new List<GameObject>();
    private bool isInventoryOpen = false;
    
    void Start()
    {
        // Setup inventory toggle
        if (toggleInventoryButton != null)
        {
            toggleInventoryButton.onClick.AddListener(ToggleInventory);
        }
        
        // Hide inventory by default
        if (inventoryPanel != null)
        {
            inventoryPanel.gameObject.SetActive(false);
        }
        
        // Subscribe to inventory events
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnItemAdded += OnItemAdded;
        }
    }
    
    void Update()
    {
        // Toggle inventory dengan tombol Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }
    
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryPanel != null)
        {
            inventoryPanel.gameObject.SetActive(isInventoryOpen);
        }
        
        // Pause/unpause game jika diperlukan
        if (pauseGameWhenOpen)
        {
            Time.timeScale = isInventoryOpen ? 0f : 1f;
        }
    }
    
    public void UpdateInventoryDisplay(List<InventoryItem> items)
    {
        // Clear existing slots
        foreach (GameObject slot in inventorySlots)
        {
            if (slot != null)
                Destroy(slot);
        }
        inventorySlots.Clear();
        
        // Create new slots
        foreach (InventoryItem item in items)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot != null)
            {
                slot.SetItem(item);
            }
            
            inventorySlots.Add(slotObj);
        }
    }
    
    private void OnItemAdded(InventoryItem item)
    {
        // Tampilkan item baru di pojok kanan atas
        ShowCollectedItemNotification(item);
    }
    
    public void ShowCollectedItemNotification(InventoryItem item)
    {
        if (collectedItemDisplayPrefab == null || collectedItemsContainer == null)
            return;
        
        // Buat display baru
        GameObject displayObj = Instantiate(collectedItemDisplayPrefab, collectedItemsContainer);
        CollectedItemDisplay display = displayObj.GetComponent<CollectedItemDisplay>();
        
        if (display != null)
        {
            display.SetItem(item, displayDuration);
        }
        
        collectedItemDisplays.Add(displayObj);
        
        // Remove oldest displays if too many
        while (collectedItemDisplays.Count > maxDisplayedCollectedItems)
        {
            GameObject oldest = collectedItemDisplays[0];
            collectedItemDisplays.RemoveAt(0);
            if (oldest != null)
                Destroy(oldest);
        }
        
        // Clean up null references
        collectedItemDisplays.RemoveAll(item => item == null);
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnItemAdded -= OnItemAdded;
        }
        
        // Resume game jika sebelumnya di-pause oleh inventory
        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }
}