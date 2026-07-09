# 🏆 Quest Win Integration System

## 📋 Overview

Sistem yang mengintegrasikan **Quest System**, **Game Timer**, dan **Win Panel** untuk menampilkan win panel dengan timer ketika player menyelesaikan semua quest/item.

## ✨ Features

- ✅ **Auto Win Panel**: Win panel muncul otomatis saat quest selesai
- ✅ **Timer Display**: Menampilkan waktu completion di win panel  
- ✅ **Quest Statistics**: Menampilkan nama quest dan item collected
- ✅ **Multiple System Support**: Kompatibel dengan QuestSystem dan ItemCollectionManager
- ✅ **Easy Setup**: Setup wizard untuk konfigurasi cepat
- ✅ **Event Integration**: Rich event system untuk customization
- ✅ **Debug Tools**: Testing dan validation tools

## 🎮 How It Works

### Flow Diagram
```
Quest Started → Player Collects Items → Last Item Collected → Quest Complete Event → Timer Stops → Win Panel Shows → Display Statistics
```

### Integration Flow
```
QuestSystem.OnQuestCompleted → QuestWinIntegration.OnQuestCompleted() → GameTimer.CompleteGame() → WinPanelManager.ShowWinPanel() → Display Timer & Quest Stats
```

## 🚀 Quick Setup

### Method 1: Auto Setup (Recommended)
1. Add `QuestWinSetupWizard` component ke empty GameObject
2. Klik **"Complete Quest Win Setup"** di context menu
3. Done! Sistem siap digunakan

### Method 2: Manual Setup
1. Pastikan ada `QuestSystem` di scene
2. Pastikan ada `GameTimer` di scene  
3. Pastikan ada `WinPanelManager` dengan UI setup
4. Add `QuestWinIntegration` component ke GameObject
5. Component akan auto-setup integration di Start()

## 📁 Files Involved

### Core Integration
- **`QuestWinIntegration.cs`** - Main integration component
- **`QuestWinSetupWizard.cs`** - Setup wizard

### Existing Systems (Modified)
- **`WinPanelManager.cs`** - Updated untuk support quest data
- **`QuestSystem.cs`** - Quest system dengan OnQuestCompleted event
- **`GameTimer.cs`** - Timer system

### Documentation
- **`QUEST_WIN_INTEGRATION_README.md`** - File ini

## 🔧 Configuration

### QuestWinIntegration Settings
```csharp
[Header("Integration Settings")]
public bool autoSetupIntegration = true;        // Auto-setup di Start()
public float winPanelDelay = 1.0f;             // Delay sebelum show win panel
public bool enableDebugLogs = true;            // Enable debug logs

[Header("Quest Completion Events")]
public UnityEvent OnQuestCompletedBeforeWin;   // Event sebelum win panel
public UnityEvent OnWinPanelShown;             // Event setelah win panel shown
```

### Win Panel Display

**Quest Information:**
- Quest name dari QuestSystem
- Items collected (X/Y format)
- Quest description

**Timer Information:**
- Completion time dalam format yang readable
- Auto-format: "2m 30s" atau "45s"

**Victory Message:**
- Random victory messages
- Animated text dan button elements

## 🎯 Integration Points

### 1. Quest System Integration
```csharp
// QuestWinIntegration subscribes ke quest completion
questSystem.OnQuestCompleted.AddListener(OnQuestCompleted);

// Saat quest selesai:
void OnQuestCompleted()
{
    gameTimer.CompleteGame();              // Stop timer
    OnQuestCompletedBeforeWin?.Invoke();   // Trigger events
    ShowWinPanelDelayed();                 // Show win panel
}
```

### 2. Timer Integration  
```csharp
// Timer automatically stops saat quest complete
gameTimer.CompleteGame();

// Win panel displays timer data
string timeInfo = $"Completion Time: {gameTimer.GetTimeForStats()}";
```

### 3. Win Panel Integration
```csharp
// Win panel gets quest data
QuestCompletionData data = questWinIntegration.GetQuestCompletionData();

// Display quest information
itemsCollectedText.text = $"Quest: {data.questName}\nItems: {data.collectedItems}/{data.totalItems}";
timeCompletedText.text = $"Time: {data.timeForStats}";
```

## 🛠️ Usage Examples

### Basic Usage
```csharp
// Setup sudah otomatis jika menggunakan QuestWinSetupWizard
// Tidak perlu code tambahan - integration berjalan otomatis

// Optional: Custom events
QuestWinIntegration integration = QuestWinIntegration.Instance;
integration.OnQuestCompletedBeforeWin.AddListener(() => {
    Debug.Log("Quest completed! Playing victory cutscene...");
});
integration.OnWinPanelShown.AddListener(() => {
    Debug.Log("Win panel shown! Player can see stats.");
});
```

### Advanced Customization
```csharp
// Custom quest completion handler
public class MyQuestHandler : MonoBehaviour
{
    void Start()
    {
        QuestWinIntegration.Instance.OnQuestCompletedBeforeWin.AddListener(OnCustomQuestComplete);
    }
    
    void OnCustomQuestComplete()
    {
        // Play victory cutscene
        PlayVictoryCutscene();
        
        // Save completion stats
        SavePlayerStats();
        
        // Update achievements
        UnlockAchievements();
    }
}
```

### Testing
```csharp
// Test quest completion
QuestWinIntegration.Instance.TestQuestComplete();

// Test dari QuestWinSetupWizard
QuestWinSetupWizard wizard = FindObjectOfType<QuestWinSetupWizard>();
wizard.TestQuestCompletion();
```

## 📊 Data Structure

### QuestCompletionData
```csharp
public class QuestCompletionData
{
    public string questName;           // Nama quest
    public int totalItems;             // Total item di quest
    public int collectedItems;         // Item yang sudah dikumpulkan
    public string questDescription;    // Deskripsi quest
    
    public float completionTime;       // Waktu completion (seconds)
    public string formattedTime;       // Format: "02:35"
    public string timeForStats;        // Format: "2m 35s"
}
```

## 🎨 Customization Options

### Win Panel Display
- Modify victory messages di `WinPanelManager.victoryMessages`
- Customize animation timing dan effects
- Add custom UI elements untuk additional stats

### Event Handlers
```csharp
// Pre-win panel events (cutscenes, effects, etc.)
integration.OnQuestCompletedBeforeWin.AddListener(PlayVictoryCutscene);
integration.OnQuestCompletedBeforeWin.AddListener(SaveProgress);

// Post-win panel events (achievements, transitions, etc.)  
integration.OnWinPanelShown.AddListener(UnlockAchievements);
integration.OnWinPanelShown.AddListener(UpdateLeaderboard);
```

### Timer Customization
```csharp
// Customize timer format di GameTimer
gameTimer.showMilliseconds = true;  // Show milliseconds
gameTimer.timeFormat = "hh:mm:ss";  // Hour format
```

## 🧪 Testing Guide

### Setup Testing
1. Run `QuestWinSetupWizard.CompleteQuestWinSetup()`
2. Validate dengan `QuestWinSetupWizard.ValidateAllSystems()`
3. Test dengan `QuestWinSetupWizard.TestQuestCompletion()`

### Runtime Testing
1. Start play mode
2. Complete quest items sampai selesai
3. Verify win panel muncul dengan timer dan stats
4. Check console logs untuk debug information

### Integration Testing
```csharp
[ContextMenu("Test Integration")]
public void TestIntegration()
{
    // 1. Setup systems
    QuestWinSetupWizard.Instance.CompleteQuestWinSetup();
    
    // 2. Start quest
    QuestSystem.Instance.StartQuest();
    
    // 3. Complete quest (simulate)
    QuestSystem.Instance.CompleteQuest(); // This should trigger win panel
}
```

## 🔍 Troubleshooting

### Win Panel Tidak Muncul
- Check `QuestWinIntegration` ada di scene
- Verify `QuestWinIntegration.SetupIntegration()` sudah dipanggil  
- Check console untuk error messages
- Validate `WinPanelManager` ada dan configured

### Timer Tidak Muncul
- Check `GameTimer` ada di scene dan running
- Verify `GameTimer.CompleteGame()` dipanggil saat quest selesai
- Check `GameTimer.GetTimeForStats()` returns valid data

### Quest Data Tidak Tampil
- Verify `QuestSystem` ada dan configured dengan item
- Check `QuestSystem.OnQuestCompleted` event working
- Validate quest completion flow dengan debug logs

### Common Issues
```csharp
// Issue: Integration tidak setup
// Solution: Call setup manually
QuestWinIntegration.Instance.SetupIntegration();

// Issue: Systems tidak ditemukan  
// Solution: Use setup wizard
QuestWinSetupWizard.Instance.CompleteQuestWinSetup();

// Issue: Events tidak trigger
// Solution: Check event subscriptions
integration.ValidateIntegration();
```

## 📈 Performance Notes

### Optimization
- Integration hanya subscribe ke events yang diperlukan
- Win panel animation menggunakan efficient coroutines
- Timer update hanya saat running (tidak continuous)

### Memory Management  
- Event subscriptions di-cleanup di OnDestroy()
- Win panel elements di-reuse (tidak recreate)
- Quest data structure lightweight

## 🔄 Backward Compatibility

### ItemCollectionManager Support
Sistem tetap support `ItemCollectionManager` untuk backward compatibility:
```csharp
// Old system tetap bekerja
ItemCollectionManager.Instance.OnTaskCompleted.AddListener(ShowWinPanel);

// New system prioritas lebih tinggi
QuestWinIntegration > QuestSystem > ItemCollectionManager
```

### Migration Path
1. Existing projects dapat continue menggunakan `ItemCollectionManager`
2. Add `QuestWinIntegration` untuk enhanced features
3. Gradually migrate ke `QuestSystem` untuk full features

## 📚 Related Documentation

- **Quest System**: `QUEST_SYSTEM_SUMMARY.md`
- **Cutscene System**: `CUTSCENE_SYSTEM_README.md`
- **Win Panel**: Check `WinPanelManager.cs` comments

## 🎉 Conclusion

Quest Win Integration System menyediakan seamless integration antara quest completion dan victory presentation. Dengan setup yang mudah dan customization yang flexible, sistem ini memungkinkan:

✅ **Automatic Flow**: Quest complete → Timer stop → Win panel → Stats display  
✅ **Rich Data**: Quest info, timer, achievements, dll  
✅ **Easy Setup**: One-click setup wizard  
✅ **Customizable**: Event system untuk custom behaviors  
✅ **Backward Compatible**: Works dengan existing systems  

**Ready untuk production!** 🚀

---

**Happy questing dengan automatic win panels!** 🏆⏰