# Win Condition System Guide

## 📋 Overview

Sistem Win Condition yang telah dibuat terdiri dari beberapa komponen yang bekerja sama untuk memberikan pengalaman win condition yang komprehensif dengan timer dan UI yang menarik.

## 🎯 Komponen Sistem

### 1. GameTimer.cs
**Fungsi:** Mengelola penghitungan waktu game
- ⏱️ Auto-start timer saat game dimulai  
- 🎯 Stop timer otomatis saat win condition tercapai
- 📊 Format waktu: MM:SS atau MM:SS.MS
- 🔄 Reset, pause, resume functionality

### 2. GameTimerUI.cs  
**Fungsi:** Menampilkan timer di layar player
- 📱 Posisi customizable (Top Left/Right/Center, dll)
- 🎨 Visual customization (warna, ukuran, animasi)
- ✨ Animasi pulse setiap detik (opsional)
- 🎯 Color change saat game complete

### 3. WinPanelManager.cs
**Fungsi:** Mengelola panel victory saat menang
- 🏆 Animated victory panel dengan fade in/slide effects
- 📊 Menampilkan statistik (items collected, time taken)
- 🎮 Button Play Again dan Main Menu
- 🎵 Victory sound effects
- ⏸️ Pause game otomatis saat menang

### 4. WinConditionSystemSetup.cs
**Fungsi:** Setup wizard untuk mudah setup sistem
- 🚀 One-click setup complete system
- 🔧 Auto-create missing components  
- ✅ Validation dan status checking
- 🧪 Testing tools

## 🚀 Setup Guide

### Step 1: Quick Setup (Recommended)
1. Tambahkan `WinConditionSystemSetup.cs` ke GameObject di scene
2. Klik kanan → Context Menu → **🎯 Setup Complete Win Condition System**
3. System akan auto-create semua komponen yang dibutuhkan
4. Done! ✅

### Step 2: Manual Setup (Advanced)

#### A. Setup GameTimer
```csharp
// 1. Buat GameObject "GameTimer"
// 2. Add component GameTimer.cs
// 3. Configure settings:
public bool startTimerOnStart = true;  // Auto start
public bool showMilliseconds = false;  // Format waktu
```

#### B. Setup GameTimerUI
```csharp
// 1. Di Canvas, buat GameObject "GameTimerUI"  
// 2. Add component GameTimerUI.cs
// 3. Add TextMeshPro untuk display
// 4. Configure position:
public TimerPosition timerPosition = TimerPosition.TopRight;
```

#### C. Setup WinPanel
```csharp
// 1. Di Canvas, buat GameObject "WinPanel"
// 2. Add component WinPanelManager.cs  
// 3. Create UI elements:
//    - Background panel (Image)
//    - Victory title (TextMeshPro)
//    - Statistics text (TextMeshPro) 
//    - Play Again button
//    - Main Menu button
// 4. Assign references ke WinPanelManager
```

#### D. Integration dengan ItemCollectionManager
System sudah terintegrasi otomatis via UnityEvents:
- `ItemCollectionManager.OnTaskCompleted` → Trigger win panel
- Timer berhenti otomatis saat task complete

## 🎮 Usage Examples

### Basic Usage
```csharp
// Timer sudah jalan otomatis
// Win panel muncul otomatis saat ItemCollectionManager.OnTaskCompleted

// Manual control jika diperlukan:
GameTimer.Instance.StartTimer();
GameTimer.Instance.StopTimer();  
GameTimer.Instance.ResetTimer();

// Show/hide timer UI
GameTimerUI timerUI = FindObjectOfType<GameTimerUI>();
timerUI.ShowTimer();
timerUI.HideTimer();

// Manual trigger win panel
WinPanelManager.Instance.ShowWinPanel();
```

### Customization
```csharp
// Ubah posisi timer
GameTimerUI timerUI = FindObjectOfType<GameTimerUI>();
timerUI.UpdateTimerPosition(GameTimerUI.TimerPosition.TopLeft);

// Custom timer color  
timerUI.SetTimerColor(Color.yellow);

// Custom win messages
WinPanelManager winPanel = FindObjectOfType<WinPanelManager>();
winPanel.victoryMessages = new string[] { 
    "Amazing!", "Perfect!", "Outstanding!" 
};
```

## 🔧 Configuration

### GameTimer Settings
- **startTimerOnStart**: Auto-start timer saat scene load
- **showMilliseconds**: Tampilkan milidetik (MM:SS.MS)
- **timeFormat**: Format string untuk display waktu

### GameTimerUI Settings  
- **timerPosition**: Posisi di layar (TopRight, TopLeft, dll)
- **showTimerOnStart**: Tampilkan timer UI saat start
- **animateOnUpdate**: Pulse animation setiap detik
- **normalColor/completedColor**: Warna timer normal dan saat complete

### WinPanelManager Settings
- **panelFadeInDuration**: Durasi fade in panel (0.5s default)
- **titleAnimationDelay**: Delay animasi title (0.3s)
- **statsAnimationDelay**: Delay animasi statistik (0.6s)  
- **mainMenuSceneName**: Nama scene main menu ("MainMenu")
- **gameSceneName**: Nama scene game ("GameScene")

## 🎯 Integration Points

### Dengan ItemCollectionManager
```csharp
// Sudah auto-integrated via UnityEvents
ItemCollectionManager.Instance.OnTaskCompleted.AddListener(ShowWinPanel);
ItemCollectionManager.Instance.OnTaskCompleted.AddListener(GameTimer.Instance.CompleteGame);
```

### Dengan Scene Management
```csharp
// Win panel punya built-in scene transitions
winPanel.RestartGame();    // Reload current scene
winPanel.GoToMainMenu();   // Load main menu scene
```

### Dengan Audio System  
```csharp
// WinPanelManager mendukung audio
public AudioClip victorySound;      // Sound saat menang
public AudioClip buttonClickSound;  // Sound button click
```

## 🧪 Testing & Debug

### Context Menu Commands
**GameTimer:**
- Start Timer
- Stop Timer  
- Reset Timer
- Complete Game

**GameTimerUI:**
- Test Show Timer
- Test Hide Timer
- Test Completion Effect

**WinPanelManager:**
- Test Show Win Panel
- Test Hide Win Panel

**WinConditionSystemSetup:**
- 🎯 Setup Complete Win Condition System
- ⏱️ Setup Game Timer
- 📱 Setup Game Timer UI  
- 🏆 Setup Win Panel
- 🧪 Test Win Condition
- 📊 System Status

### Debug Methods
```csharp
// Check system status
WinConditionSystemSetup setup = FindObjectOfType<WinConditionSystemSetup>();
setup.CheckSystemStatus();

// Force test win condition
setup.TestWinCondition();

// Manual trigger untuk testing
ItemCollectionManager.Instance.OnTaskCompleted?.Invoke();
```

## 📝 Checklist Setup

- [ ] ItemCollectionManager exists in scene
- [ ] GameTimer component added  
- [ ] GameTimerUI component added dengan TextMeshPro
- [ ] WinPanel UI created dengan semua elements
- [ ] WinPanelManager component configured
- [ ] Canvas exists for UI elements
- [ ] Scene names configured correctly (MainMenu, GameScene)
- [ ] Audio clips assigned (optional)
- [ ] Test win condition works

## ⚠️ Troubleshooting

### Timer tidak muncul
- Pastikan GameTimerUI memiliki timerText assigned
- Check showTimerOnStart = true
- Pastikan Canvas ada dan aktif

### Win panel tidak muncul  
- Check ItemCollectionManager.OnTaskCompleted event
- Pastikan WinPanelManager.winPanel assigned
- Check console untuk error messages

### Button tidak berfungsi
- Pastikan scene names benar di WinPanelManager
- Check button onClick events ter-assign
- Pastikan ada EventSystem di scene

### Timer tidak akurat
- Pastikan hanya ada satu GameTimer instance  
- Check Time.timeScale tidak di-modify
- Pastikan GameTimer.IsTimerRunning = true

## 🎨 UI Layout Tips

### Recommended UI Structure:
```
Canvas (Screen Space - Overlay)
├── GameTimerUI (Top Right)
│   └── TimerText (TextMeshPro)  
└── WinPanel (Full Screen)
    └── Content (Center)
        ├── VictoryTitle (TextMeshPro)
        ├── ItemsCollectedText (TextMeshPro)
        ├── TimeCompletedText (TextMeshPro)  
        ├── CongratsText (TextMeshPro)
        ├── PlayAgainButton
        └── MainMenuButton
```

### Sizing Guidelines:
- Timer UI: 200x50 pixels
- Win Panel: Full screen dengan content 600x400  
- Buttons: 120x40 pixels
- Font sizes: Title(36), Stats(20), Buttons(16)

## 🔮 Future Enhancements

Sistem ini mudah untuk di-extend:

- **Statistics**: Tambah best time, attempts count
- **Achievements**: Integration dengan achievement system  
- **Leaderboards**: Save/load high scores
- **Animations**: More elaborate victory animations
- **Sound**: Background music transitions
- **Localization**: Multi-language support

## ✅ Completed Features

✅ Game timer dengan start/stop/reset  
✅ Timer UI display di layar player  
✅ Win panel dengan animasi smooth  
✅ Items collected dan time statistics  
✅ Play Again dan Main Menu buttons  
✅ Auto-integration dengan ItemCollectionManager  
✅ Setup wizard untuk easy setup  
✅ Comprehensive testing tools  
✅ Full documentation dan guide

System siap digunakan! 🎉