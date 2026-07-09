# Simple Cutscene System

Sistem cutscene sederhana untuk Unity yang mendukung perpindahan kamera dengan efek fade transition.

## 🎯 Fitur Utama

- **Perpindahan Kamera Otomatis**: Menonaktifkan kamera utama dan mengaktifkan kamera cutscene
- **Fade Transition**: Efek fade in/out yang smooth dengan panel hitam
- **Timeline Integration**: Menggunakan Unity Timeline untuk animasi kamera
- **Player Control**: Menonaktifkan kontrol player selama cutscene
- **Trigger System**: Berbagai cara untuk memulai cutscene
- **Audio Support**: Pengelolaan Audio Listener otomatis

## 📁 Struktur File

```
Assets/Script/
├── CutsceneManager.cs          # Manager utama untuk cutscene
├── FadeTransition.cs           # Efek fade in/out
├── CutsceneCamera.cs           # Kamera khusus cutscene
├── CutsceneTrigger.cs          # Trigger untuk memulai cutscene
├── CutsceneTestManager.cs      # Testing dan debugging
└── Editor/
    └── CutsceneSetupWizard.cs  # Setup wizard (Editor only)
```

## 🚀 Quick Setup

### 1. Setup Otomatis dengan Quest Integration (RECOMMENDED!)

Untuk setup lengkap dengan integrasi Quest System:

1. **Buat GameObject baru** dan tambahkan `CutsceneQuestSetupWizard`
2. **Configure settings**:
   - Set cutscene names (default: QuestIntro, FirstItemFound, SecondItemFound, QuestComplete)
   - Assign references atau biarkan auto-detect
3. **Klik "Setup Complete Cutscene-Quest System"** di context menu
4. **Done!** Sistem cutscene + quest integration siap digunakan

### 2. Setup Otomatis Biasa

1. Buka **Tools > Cutscene > Setup Wizard**
2. Isi nama cutscene
3. Assign Main Camera dan Player Controller
4. Klik "Create Cutscene System"

### 3. Setup Manual

1. **Buat CutsceneManager**:
   ```csharp
   GameObject managerGO = new GameObject("CutsceneManager");
   CutsceneManager manager = managerGO.AddComponent<CutsceneManager>();
   ```

2. **Buat FadeTransition**:
   ```csharp
   GameObject fadeGO = new GameObject("FadeTransition");
   FadeTransition fade = fadeGO.AddComponent<FadeTransition>();
   ```

3. **Setup Timeline dan Camera**:
   - Buat GameObject dengan PlayableDirector
   - Buat Timeline Asset
   - Buat Cutscene Camera dengan CutsceneCamera script

## 🎮 Cara Penggunaan

### Memanggil Cutscene dari Script

```csharp
// Panggil cutscene by name
CutsceneManager.Instance.PlayCutscene("IntroScene", () => {
    Debug.Log("Cutscene selesai!");
});

// Panggil cutscene by index
CutsceneManager.Instance.PlayCutscene(0);

// Cek apakah cutscene sedang berjalan
if (CutsceneManager.Instance.IsPlayingCutscene())
{
    // Cutscene sedang berjalan
}

// Skip cutscene
CutsceneManager.Instance.SkipCutscene();
```

### Menggunakan Trigger

```csharp
// Setup CutsceneTrigger
public class MyTrigger : MonoBehaviour
{
    private void Start()
    {
        CutsceneTrigger trigger = GetComponent<CutsceneTrigger>();
        trigger.cutsceneName = "MyAwesomeCutscene";
        trigger.triggerKey = KeyCode.E;
        trigger.triggerOnce = true;
    }
}
```

## ⚙️ Konfigurasi Component

### CutsceneManager

| Property | Deskripsi |
|----------|-----------|
| `cutscenes` | List cutscene yang tersedia |
| `mainCamera` | Kamera utama yang akan dinonaktifkan |
| `fadeTransition` | Reference ke FadeTransition |
| `playerController` | Player controller yang akan dinonaktifkan |

### CutsceneData

| Property | Deskripsi |
|----------|-----------|
| `cutsceneName` | Nama unik cutscene |
| `timeline` | PlayableDirector untuk timeline |
| `cutsceneCamera` | Kamera untuk cutscene |
| `disablePlayerControl` | Nonaktifkan kontrol player |
| `fadeInDuration` | Durasi fade in (detik) |
| `fadeOutDuration` | Durasi fade out (detik) |

### FadeTransition

| Property | Deskripsi |
|----------|-----------|
| `fadeImage` | UI Image untuk efek fade |
| `fadeCurve` | Kurva animasi fade |
| `fadeColor` | Warna fade (default: hitam) |
| `defaultFadeDuration` | Durasi fade default |

### CutsceneTrigger

| Property | Deskripsi |
|----------|-----------|
| `cutsceneName` | Nama cutscene yang akan dipanggil |
| `triggerOnce` | Hanya trigger sekali |
| `triggerOnStart` | Auto trigger saat start |
| `triggerKey` | Key untuk manual trigger |
| `promptText` | Teks prompt untuk UI |

## 🎬 Timeline Setup

1. **Buat Timeline Asset**:
   - Right-click di Project → Create → Timeline

2. **Setup Animation Track**:
   - Drag Cutscene Camera ke Timeline
   - Buat Animation Track untuk camera movement

3. **Binding**:
   - Assign Cutscene Camera ke Timeline tracks

## 🔧 Advanced Usage

### Custom Fade Effects

```csharp
// Ganti warna fade
FadeTransition.Instance.SetFadeColor(Color.white);

// Fade manual
yield return StartCoroutine(FadeTransition.Instance.FadeOut(2f));
// Lakukan sesuatu...
yield return StartCoroutine(FadeTransition.Instance.FadeIn(1f));

// Cross fade dengan action
yield return StartCoroutine(FadeTransition.Instance.CrossFade(() => {
    // Action yang dilakukan saat fade out complete
    SceneManager.LoadScene("NewScene");
}, 1f, 1f));
```

### Cutscene Events

```csharp
public class MyScript : MonoBehaviour
{
    private void Start()
    {
        CutsceneManager.Instance.PlayCutscene("MyScene", OnCutsceneComplete);
    }

    private void OnCutsceneComplete()
    {
        // Cutscene selesai
        // Enable UI, trigger events, etc.
        GameObject.FindGameObject("QuestPanel").SetActive(true);
    }
}
```

### Camera Animation via Timeline

```csharp
// Di Timeline, gunakan Animation Track untuk animate:
// - Transform (Position, Rotation)
// - Camera (Field of View, Near/Far plane)
// - CutsceneCamera custom properties

// Atau gunakan Cinemachine untuk camera movement yang lebih advanced
```

## 🧪 Testing

1. **Gunakan CutsceneTestManager**:
   - Attach ke GameObject di scene
   - Tekan 1, 2, 3 untuk play cutscene
   - Tekan Escape untuk skip

2. **Debug Commands**:
   ```csharp
   // Test via Console
   CutsceneManager.Instance.PlayCutscene("TestScene");
   ```

## 📝 Tips dan Best Practices

### Performance
- Gunakan Object Pooling untuk cutscene cameras jika banyak
- Disable unused cameras completely
- Limit Timeline duration untuk menghindari memory leak

### Design
- Durasi fade sebaiknya 0.5-2 detik
- Pastikan audio sync dengan visual
- Test di berbagai resolusi untuk UI fade

### Workflow
- Gunakan Prefab untuk cutscene setup yang reusable
- Buat template Timeline untuk consistency
- Group related cutscene objects dalam folder

## ❗ Troubleshooting

### Cutscene tidak muncul
- Pastikan CutsceneManager.Instance tidak null
- Check apakah cutscene name sudah benar
- Verify Timeline asset assigned

### Fade tidak berfungsi
- Pastikan FadeTransition Canvas setup dengan benar
- Check UI sorting order (harus tinggi)
- Verify fadeImage reference

### Player masih bisa bergerak
- Assign playerController di CutsceneManager
- Check apakah disablePlayerControl = true
- Verify player script implementasi enable/disable

### Audio tidak sync
- Check AudioListener setup di CutsceneCamera
- Pastikan hanya 1 AudioListener yang active
- Verify audio source assignment

## 🔄 Integration dengan System Lain

### Quest System Integration (Otomatis!)

Sistem cutscene sudah terintegrasi penuh dengan Quest System yang ada. Gunakan `QuestCutsceneIntegration` untuk setup otomatis:

```csharp
// Setup otomatis dengan CutsceneQuestSetupWizard
var wizard = GetComponent<CutsceneQuestSetupWizard>();
wizard.SetupComplete(); // One-click setup!

// Atau manual dengan QuestCutsceneIntegration
var integration = GetComponent<QuestCutsceneIntegration>();
integration.questCutscenes = new QuestCutsceneIntegration.QuestCutsceneData[]
{
    // Quest intro
    new QuestCutsceneIntegration.QuestCutsceneData
    {
        eventType = QuestCutsceneIntegration.QuestEventType.QuestStarted,
        cutsceneName = "QuestIntro",
        delay = 2f
    },
    // Item collected
    new QuestCutsceneIntegration.QuestCutsceneData
    {
        eventType = QuestCutsceneIntegration.QuestEventType.ItemCollected,
        cutsceneName = "FirstItemFound",
        questItemIndex = 0
    },
    // Quest complete
    new QuestCutsceneIntegration.QuestCutsceneData
    {
        eventType = QuestCutsceneIntegration.QuestEventType.QuestCompleted,
        cutsceneName = "QuestComplete",
        delay = 1f
    }
};
```

**Fitur Quest Integration:**
- Otomatis trigger cutscene saat quest events
- Support untuk specific item events
- Distance-based triggering
- Skip already played cutscenes
- Complete visual setup wizard

### Dialog System
```csharp
public void StartDialog()
{
    CutsceneManager.Instance.PlayCutscene("DialogIntro", () => {
        DialogManager.Instance.StartDialog("npc_001");
    });
}
```

## 📄 License

Free to use dalam project Anda.