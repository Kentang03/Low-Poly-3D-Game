# Quest System Guide

## Overview
Quest System adalah sistem yang memungkinkan pembuatan quest berurutan dimana item selanjutnya hanya akan muncul setelah item sebelumnya dikumpulkan. Sistem ini terintegrasi dengan InventorySystem dan menyediakan event container untuk custom events serta **Dynamic Quest Descriptions** yang berubah per item.

## Fitur Utama

### 1. Sequential Item Progression
- Item quest muncul berurutan (item 2 hanya muncul setelah item 1 diambil)
- Sistem otomatis mendeteksi pengambilan item melalui InventorySystem
- Progress tracking dan validasi

### 2. Dynamic Quest Descriptions ⭐ NEW
- **Quest description berubah untuk setiap item**
- Contoh: "Cari Bakul Nasi" → "Cari Mask" → "Cari Sacred Key"
- Configurable per quest item
- Support untuk initial description dan completion description
- Automatic UI update ketika description berubah

### 3. Event Container System
Setiap quest item memiliki event container:
- `OnItemActivated` - Dipanggil ketika item menjadi aktif/muncul
- `OnItemCollected` - Dipanggil ketika item diambil player

Quest system memiliki event container:
- `OnQuestStarted` - Ketika quest dimulai
- `OnQuestItemActivated` - Ketika item quest baru muncul
- `OnQuestItemCollected` - Ketika item quest diambil
- `OnQuestCompleted` - Ketika semua item telah dikumpulkan
- `OnQuestReset` - Ketika quest direset

### 3. UI Integration
- Real-time progress display
- Current item information
- Visual progress bar
- Status updates

## Setup Instructions

### Step 1: Basic Setup
1. Buat empty GameObject dan beri nama "QuestSystem"
2. Add component `QuestSystem`
3. Add component `QuestSystemSetup` untuk auto-setup

### Step 2: Configure Quest Items dan Dynamic Descriptions
```csharp
// Di inspector QuestSystem:
1. Enable "Use Dynamic Description" untuk mengaktifkan fitur
2. Set Initial Quest Description: "Prepare for your quest..."
3. Set Completed Quest Description: "Quest completed! All items collected!"

4. Untuk setiap Quest Item:
   - Item Name: "Bakul Nasi", "Mask", "Sacred Key"
   - Item Description: deskripsi item itu sendiri
   - Quest Description For This Item: instruksi yang ditampilkan ke player
     * "Find and collect the Bakul Nasi. Look near the traditional house."
     * "Now find the Mask. Search around the ceremonial area."  
     * "Finally, locate the Sacred Key. Check the temple chamber."
   - Item Prefab: prefab yang akan spawn di scene
   - Spawn Position: transform posisi spawn
```

### Step 3: Setup UI (Optional)
1. Jalankan `QuestSystemSetup.SetupQuestSystem()` dari context menu
2. Atau setup manual:
   - Buat Canvas jika belum ada
   - Buat Quest UI Panel
   - Add component `QuestUI`
   - Assign UI references

### Step 4: Configure Events
Di inspector setiap Quest Item atau Quest System, setup UnityEvents:

**Quest Item Events:**
- `OnItemActivated`: Contoh - trigger particle effect, play sound
- `OnItemCollected`: Contoh - trigger animation, update NPC dialog

**Quest System Events:**
- `OnQuestStarted`: Setup initial game state
- `OnQuestItemActivated`: Update UI, show hints
- `OnQuestItemCollected`: Play feedback effects
- `OnQuestCompleted`: Trigger win condition, unlock areas
- `OnQuestReset`: Reset game state

## Code Examples

### Example 1: Simple Item Activation Effect
```csharp
public class QuestItemEffect : MonoBehaviour
{
    public ParticleSystem activationEffect;
    public AudioClip activationSound;
    
    // Assign this method to OnItemActivated event
    public void PlayActivationEffect()
    {
        if (activationEffect != null)
            activationEffect.Play();
            
        if (activationSound != null)
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
    }
}
```

### Example 2: Quest Progress Handler
```csharp
public class QuestProgressHandler : MonoBehaviour
{
    public GameObject questCompletePanel;
    public AudioClip questCompleteMusic;
    
    void Start()
    {
        QuestSystem questSystem = QuestSystem.Instance;
        if (questSystem != null)
        {
            questSystem.OnQuestCompleted.AddListener(OnQuestComplete);
        }
    }
    
    void OnQuestComplete()
    {
        // Show victory panel
        if (questCompletePanel != null)
            questCompletePanel.SetActive(true);
            
        // Play victory music
        if (questCompleteMusic != null)
        {
            AudioSource.PlayClipAtPoint(questCompleteMusic, Camera.main.transform.position);
        }
        
        // Stop game timer
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.CompleteGame();
        }
    }
}
```

### Example 3: Dynamic Quest Item Spawning
```csharp
public class DynamicQuestSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] itemPrefabs;
    
    void Start()
    {
        QuestSystem questSystem = QuestSystem.Instance;
        if (questSystem != null)
        {
            questSystem.OnQuestItemActivated.AddListener(OnItemActivated);
        }
    }
    
    void OnItemActivated()
    {
        // Custom logic untuk spawn item di posisi random
        QuestItem currentItem = QuestSystem.Instance.GetCurrentQuestItem();
        if (currentItem != null && spawnPoints.Length > 0)
        {
            Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            currentItem.spawnPosition = randomSpawn;
        }
    }
}
```

### Example 4: Dynamic Quest Descriptions ⭐ NEW
```csharp
public class DynamicQuestManager : MonoBehaviour
{
    void Start()
    {
        QuestSystem questSystem = QuestSystem.Instance;
        if (questSystem != null)
        {
            // Setup dynamic descriptions
            questSystem.useDynamicDescription = true;
            questSystem.initialQuestDescription = "Prepare for your Indonesian cultural quest!";
            questSystem.completedQuestDescription = "Amazing! You've collected all sacred items!";
            
            // Setup per-item descriptions
            if (questSystem.questItems.Count >= 3)
            {
                questSystem.questItems[0].questDescriptionForThisItem = 
                    "Find the Bakul Nasi (rice basket). Look near the traditional house.";
                questSystem.questItems[1].questDescriptionForThisItem = 
                    "Now search for the ceremonial Mask. Check around the temple area.";
                questSystem.questItems[2].questDescriptionForThisItem = 
                    "Finally, locate the Sacred Key. It should be in the final chamber.";
            }
            
            // Subscribe ke events untuk log description changes
            questSystem.OnQuestItemActivated.AddListener(() => {
                Debug.Log($"New quest: {questSystem.GetCurrentQuestDescription()}");
            });
        }
    }
}
```

## Integration dengan System Lain

### InventorySystem Integration
Quest system otomatis terintegrasi dengan InventorySystem:
- Mendeteksi item yang diambil melalui `OnItemAdded` event
- Validasi item quest yang sesuai
- Progress otomatis ke item selanjutnya

### GameTimer Integration
```csharp
// Di OnQuestCompleted event, bisa trigger:
GameTimer.Instance.CompleteGame(); // Stop timer
```

### Win Condition Integration
```csharp
public class QuestWinCondition : MonoBehaviour
{
    void Start()
    {
        QuestSystem.Instance.OnQuestCompleted.AddListener(() => {
            WinPanelManager.Instance.ShowWinPanel();
        });
    }
}
```

## Best Practices

### 1. Event Configuration
- Gunakan UnityEvents untuk flexibility
- Separate concerns - buat script terpisah untuk setiap jenis effect
- Test semua events di Play Mode

### 2. Item Prefab Setup
- Pastikan prefab punya CollectibleItem component
- Set proper collider dan trigger
- Add visual/audio feedback

### 3. Performance Considerations
- Jangan spawn terlalu banyak item sekaligus
- Destroy item setelah collected
- Use object pooling untuk effects

### 4. Testing & Debugging
- Gunakan context menu untuk testing:
  - `Start Quest`
  - `Complete Current Item`  
  - `Reset Quest`
- Monitor console logs untuk debugging
- Use `QuestSystemSetup.ValidateQuestSetup()` untuk validasi

## Troubleshooting

### Quest tidak mulai
- Pastikan InventorySystem ada di scene
- Check apakah QuestSystem.Start() dipanggil
- Validate prefab references

### Item tidak terdeteksi saat diambil
- Pastikan CollectibleItem component ada di prefab
- Check itemName matching antara quest dan collectible
- Validate InventorySystem integration

### UI tidak update
- Pastikan QuestUI component ada dan aktif
- Check UI references di inspector
- Validate Canvas setup

### Events tidak trigger
- Check UnityEvent assignment di inspector
- Pastikan listener masih subscribe
- Test dengan Debug.Log() di event methods

## Advanced Usage

### Custom Quest Types
Extend QuestSystem untuk quest types lain:
```csharp
public class TimedQuest : QuestSystem
{
    public float timeLimit = 60f;
    
    protected override void StartQuest()
    {
        base.StartQuest();
        StartCoroutine(QuestTimer());
    }
    
    IEnumerator QuestTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        if (!IsQuestCompleted())
        {
            // Handle quest failure
            OnQuestFailed?.Invoke();
        }
    }
}
```

### Multi-Step Quest Chains
```csharp
public class QuestChainManager : MonoBehaviour
{
    public QuestSystem[] questChain;
    private int currentQuestIndex = 0;
    
    void Start()
    {
        foreach (var quest in questChain)
        {
            quest.OnQuestCompleted.AddListener(OnQuestCompleted);
        }
        
        StartCurrentQuest();
    }
    
    void OnQuestCompleted()
    {
        currentQuestIndex++;
        if (currentQuestIndex < questChain.Length)
        {
            StartCurrentQuest();
        }
        else
        {
            // All quests completed
            OnAllQuestsCompleted?.Invoke();
        }
    }
}
```

Sistem ini memberikan foundation yang solid untuk quest berbasis item collection dengan flexibility tinggi melalui event system.