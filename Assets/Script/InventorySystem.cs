using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int quantity = 1;
    public bool canBePlaced = false; // Boolean untuk item yang bisa ditaruh
    public GameObject itemPrefab; // Prefab untuk spawn item saat ditempatkan
    public string itemID; // Unique ID untuk item
    
    public InventoryItem(string name, string description, Sprite icon, bool placeable = false, GameObject prefab = null)
    {
        itemName = name;
        itemDescription = description;
        itemIcon = icon;
        canBePlaced = placeable;
        itemPrefab = prefab;
        itemID = Guid.NewGuid().ToString();
    }
}

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxInventorySize = 20;
    
    [Header("UI References")]
    public InventoryUI inventoryUI;
    
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private static InventorySystem instance;
    
    public static InventorySystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<InventorySystem>();
            return instance;
        }
    }
    
    // Events
    public System.Action<InventoryItem> OnItemAdded;
    public System.Action<InventoryItem> OnItemRemoved;
    public System.Action OnInventoryChanged;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (inventoryUI == null)
            inventoryUI = FindObjectOfType<InventoryUI>();
    }
    
    public bool AddItem(string itemName, string description, Sprite icon, bool canBePlaced = false, GameObject prefab = null)
    {
        // Cek apakah inventory penuh
        if (inventoryItems.Count >= maxInventorySize)
        {
            Debug.Log("Inventory penuh!");
            return false;
        }
        
        // Untuk placeable items, selalu cek apakah item dengan data yang sama sudah ada
        if (canBePlaced)
        {
            InventoryItem existingPlaceableItem = inventoryItems.Find(item => 
                item.itemName == itemName && 
                item.canBePlaced && 
                item.itemPrefab == prefab);
                
            if (existingPlaceableItem != null)
            {
                existingPlaceableItem.quantity++;
                // Update UI
                UpdateInventoryUI();
                
                // Trigger events dengan existing item
                OnItemAdded?.Invoke(existingPlaceableItem);
                OnInventoryChanged?.Invoke();
                
                Debug.Log($"Added {itemName} to existing stack! Quantity: {existingPlaceableItem.quantity}");
                return true;
            }
        }
        else
        {
            // Cek apakah item sudah ada (untuk non-placeable stackable items)
            InventoryItem existingItem = inventoryItems.Find(item => item.itemName == itemName && !item.canBePlaced);
            if (existingItem != null)
            {
                existingItem.quantity++;
                // Update UI
                UpdateInventoryUI();
                
                // Trigger events
                OnItemAdded?.Invoke(existingItem);
                OnInventoryChanged?.Invoke();
                
                Debug.Log($"Added {itemName} to existing stack! Quantity: {existingItem.quantity}");
                return true;
            }
        }
        
        // Tambah item baru jika tidak ada yang cocok
        InventoryItem newItem = new InventoryItem(itemName, description, icon, canBePlaced, prefab);
        inventoryItems.Add(newItem);
        
        // Update UI
        UpdateInventoryUI();
        
        // Trigger events
        OnItemAdded?.Invoke(newItem);
        OnInventoryChanged?.Invoke();
        
        Debug.Log($"Item {itemName} ditambahkan ke inventory sebagai item baru!");
        return true;
    }
    
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                inventoryItems.Remove(item);
                OnItemRemoved?.Invoke(item);
            }
            
            UpdateInventoryUI();
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public bool RemoveItemByID(string itemID)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemID == itemID);
        if (item != null)
        {
            inventoryItems.Remove(item);
            OnItemRemoved?.Invoke(item);
            UpdateInventoryUI();
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public InventoryItem GetItem(string itemName)
    {
        return inventoryItems.Find(item => item.itemName == itemName);
    }
    
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventoryItems);
    }
    
    public List<InventoryItem> GetPlaceableItems()
    {
        return inventoryItems.FindAll(item => item.canBePlaced);
    }
    
    public bool HasItem(string itemName)
    {
        return inventoryItems.Exists(item => item.itemName == itemName);
    }
    
    public int GetItemCount(string itemName)
    {
        InventoryItem item = GetItem(itemName);
        return item != null ? item.quantity : 0;
    }
    
    void UpdateInventoryUI()
    {
        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventoryDisplay(inventoryItems);
        }
    }
    
    public void ClearInventory()
    {
        inventoryItems.Clear();
        UpdateInventoryUI();
        OnInventoryChanged?.Invoke();
    }
    
    // Debug methods
    [ContextMenu("Debug Print Inventory")]
    public void DebugPrintInventory()
    {
        Debug.Log("=== INVENTORY ===");
        foreach (var item in inventoryItems)
        {
            Debug.Log($"{item.itemName} x{item.quantity} (Placeable: {item.canBePlaced})");
        }
    }
}