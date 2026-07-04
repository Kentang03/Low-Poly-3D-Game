using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class QuestItem
{
    [Header("Quest Item Settings")]
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public GameObject itemPrefab; // Prefab item yang akan muncul di scene
    public Transform spawnPosition; // Posisi spawn item
    public bool isActive = false; // Apakah item ini aktif/bisa diambil
    public bool isCollected = false; // Apakah item ini sudah diambil
    
    [Header("Dynamic Quest Description")]
    [Tooltip("Deskripsi quest yang ditampilkan ketika item ini menjadi aktif")]
    [TextArea(2, 4)]
    public string questDescriptionForThisItem = "Find and collect this item";
    
    [Header("Quest Events")]
    public UnityEvent OnItemActivated; // Event ketika item menjadi aktif
    public UnityEvent OnItemCollected; // Event ketika item diambil
    
    private GameObject spawnedItemInstance;
    
    public void ActivateItem()
    {
        if (isActive || isCollected) return;
        
        isActive = true;
        
        // Spawn item di scene jika ada prefab dan posisi spawn
        if (itemPrefab != null && spawnPosition != null)
        {
            spawnedItemInstance = GameObject.Instantiate(itemPrefab, spawnPosition.position, spawnPosition.rotation);
            
            // Set up collectible item component
            CollectibleItem collectible = spawnedItemInstance.GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                collectible.itemName = itemName;
                collectible.itemDescription = itemDescription;
                collectible.itemIcon = itemIcon;
            }
        }
        
        Debug.Log($"Quest Item '{itemName}' telah aktif dan muncul di scene!");
        OnItemActivated?.Invoke();
    }
    
    public void CollectItem()
    {
        if (!isActive || isCollected) return;
        
        isCollected = true;
        
        // Destroy spawned instance jika ada
        if (spawnedItemInstance != null)
        {
            GameObject.Destroy(spawnedItemInstance);
        }
        
        Debug.Log($"Quest Item '{itemName}' telah dikumpulkan!");
        OnItemCollected?.Invoke();
    }
    
    public void DeactivateItem()
    {
        isActive = false;
        
        // Destroy spawned instance jika ada
        if (spawnedItemInstance != null)
        {
            GameObject.Destroy(spawnedItemInstance);
        }
    }
}

public class QuestSystem : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questName = "Collect Quest Items";
    public string questDescription = "Collect all quest items in sequence";
    
    [Header("Dynamic Description Settings")]
    [Tooltip("Gunakan dynamic description dari setiap quest item")]
    public bool useDynamicDescription = true;
    
    [Tooltip("Description default ketika quest dimulai (sebelum item pertama aktif)")]
    [TextArea(2, 4)]
    public string initialQuestDescription = "Prepare to start your quest";
    
    [Tooltip("Description ketika quest selesai")]
    [TextArea(2, 4)]
    public string completedQuestDescription = "Quest completed! All items collected!";
    
    [Header("Quest Items (In Order)")]
    public List<QuestItem> questItems = new List<QuestItem>();
    
    [Header("Quest Progress")]
    public int currentQuestIndex = 0;
    public bool questCompleted = false;
    
    [Header("Quest Events")]
    public UnityEvent OnQuestStarted;
    public UnityEvent OnQuestItemActivated;
    public UnityEvent OnQuestItemCollected;
    public UnityEvent OnQuestCompleted;
    public UnityEvent OnQuestReset;
    
    [Header("Audio")]
    public AudioClip questStartSound;
    public AudioClip itemActivatedSound;
    public AudioClip itemCollectedSound;
    public AudioClip questCompleteSound;
    
    private AudioSource audioSource;
    private static QuestSystem instance;
    
    public static QuestSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<QuestSystem>();
            return instance;
        }
    }
    
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        // Subscribe to inventory events untuk deteksi item yang diambil
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnItemAdded += OnItemAddedToInventory;
        }
        
        StartQuest();
    }
    
    void OnDestroy()
    {
        // Unsubscribe dari events
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnItemAdded -= OnItemAddedToInventory;
        }
    }
    
    public void StartQuest()
    {
        if (questCompleted) return;
        
        currentQuestIndex = 0;
        
        // Deactivate semua item dulu
        foreach (var item in questItems)
        {
            item.DeactivateItem();
            item.isCollected = false;
        }
        
        // Update initial description
        UpdateQuestDescription();
        
        // Aktifkan item pertama
        ActivateNextItem();
        
        PlaySound(questStartSound);
        OnQuestStarted?.Invoke();
        
        Debug.Log($"Quest '{questName}' dimulai!");
    }
    
    void ActivateNextItem()
    {
        if (currentQuestIndex >= questItems.Count) return;
        
        QuestItem currentItem = questItems[currentQuestIndex];
        currentItem.ActivateItem();
        
        PlaySound(itemActivatedSound);
        OnQuestItemActivated?.Invoke();
        
        Debug.Log($"Item '{currentItem.itemName}' sekarang tersedia untuk dikumpulkan!");
        
        // Update quest description jika menggunakan dynamic description
        UpdateQuestDescription();
    }
    
    void OnItemAddedToInventory(InventoryItem inventoryItem)
    {
        // Cek apakah item yang ditambahkan adalah item quest yang aktif
        if (currentQuestIndex >= questItems.Count) return;
        
        QuestItem currentQuestItem = questItems[currentQuestIndex];
        
        if (inventoryItem.itemName == currentQuestItem.itemName && currentQuestItem.isActive)
        {
            CollectCurrentQuestItem();
        }
    }
    
    void CollectCurrentQuestItem()
    {
        if (currentQuestIndex >= questItems.Count) return;
        
        QuestItem currentItem = questItems[currentQuestIndex];
        currentItem.CollectItem();
        
        PlaySound(itemCollectedSound);
        OnQuestItemCollected?.Invoke();
        
        Debug.Log($"Quest item '{currentItem.itemName}' berhasil dikumpulkan!");
        
        // Move to next item
        currentQuestIndex++;
        
        if (currentQuestIndex >= questItems.Count)
        {
            CompleteQuest();
        }
        else
        {
            // Aktifkan item selanjutnya
            ActivateNextItem();
        }
    }
    
    void CompleteQuest()
    {
        questCompleted = true;
        
        PlaySound(questCompleteSound);
        OnQuestCompleted?.Invoke();
        
        Debug.Log($"Quest '{questName}' selesai! Semua item telah dikumpulkan!");
        
        // Update quest description untuk completion
        UpdateQuestDescription();
    }
    
    void UpdateQuestDescription()
    {
        // Trigger update untuk UI dan sistem lain yang memerlukan
        // QuestUI akan menggunakan GetCurrentQuestDescription() untuk mendapatkan description terbaru
        Debug.Log($"Quest Description Updated: {GetCurrentQuestDescription()}");
    }
    
    public string GetCurrentQuestDescription()
    {
        if (!useDynamicDescription)
        {
            return questDescription; // Gunakan description static
        }
        
        if (questCompleted)
        {
            return completedQuestDescription;
        }
        
        if (currentQuestIndex >= questItems.Count)
        {
            return completedQuestDescription;
        }
        
        if (currentQuestIndex == 0 && questItems.Count > 0 && !questItems[0].isActive)
        {
            return initialQuestDescription; // Quest belum dimulai
        }
        
        // Gunakan description dari current quest item
        QuestItem currentItem = questItems[currentQuestIndex];
        if (currentItem != null && !string.IsNullOrEmpty(currentItem.questDescriptionForThisItem))
        {
            return currentItem.questDescriptionForThisItem;
        }
        
        // Fallback ke description default
        return questDescription;
    }
    
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Public methods untuk kontrol external
    public void ResetQuest()
    {
        currentQuestIndex = 0;
        questCompleted = false;
        
        // Reset semua quest items
        foreach (var item in questItems)
        {
            item.DeactivateItem();
            item.isCollected = false;
        }
        
        OnQuestReset?.Invoke();
        Debug.Log("Quest direset!");
        
        // Restart quest
        StartQuest();
    }
    
    public QuestItem GetCurrentQuestItem()
    {
        if (currentQuestIndex >= questItems.Count) return null;
        return questItems[currentQuestIndex];
    }
    
    public bool IsQuestCompleted()
    {
        return questCompleted;
    }
    
    public float GetQuestProgress()
    {
        return questItems.Count > 0 ? (float)currentQuestIndex / questItems.Count : 0f;
    }
    
    public string GetQuestStatus()
    {
        if (questCompleted) return "Completed";
        if (currentQuestIndex >= questItems.Count) return "Completed";
        
        QuestItem currentItem = GetCurrentQuestItem();
        return currentItem != null ? $"Collect: {currentItem.itemName}" : "No active quest";
    }
    
    // Debug methods
    [ContextMenu("Start Quest")]
    public void DebugStartQuest()
    {
        StartQuest();
    }
    
    [ContextMenu("Complete Current Item")]
    public void DebugCompleteCurrentItem()
    {
        if (currentQuestIndex < questItems.Count)
        {
            // Simulate collecting current item by adding to inventory
            QuestItem currentItem = questItems[currentQuestIndex];
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.AddItem(
                    currentItem.itemName,
                    currentItem.itemDescription,
                    currentItem.itemIcon
                );
            }
        }
    }
    
    [ContextMenu("Reset Quest")]
    public void DebugResetQuest()
    {
        ResetQuest();
    }
}