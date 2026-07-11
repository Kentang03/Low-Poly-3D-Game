# Character Audio System - Quick Start

## 🎵 Apa yang Ditambahkan?

Sistem audio untuk karakter Anda yang mencakup:
- ✅ Sound effect lompat (jump)
- ✅ Sound effect mendarat (landing)
- ✅ Footsteps untuk walk, run, dan sprint
- ✅ Auto-detection kecepatan movement
- ✅ Random pitch variation untuk suara natural

## 🚀 Quick Setup (3 Langkah)

### 1. Buka Setup Wizard
```
Menu: Invector → Character Audio Setup Wizard
```

### 2. Pilih Character GameObject
- Drag character Anda (yang memiliki vThirdPersonInput) ke field "Character GameObject"
- Klik "Add Character Audio Controller"

### 3. Assign Audio Clips
- Drag audio clips ke fields yang sesuai
- Atau gunakan "Select Folder and Auto-Assign" untuk auto-detect dari folder

**Done!** 🎉

## 📁 File Structure

```
Assets/ControllerPlayer/Scripts/CharacterController/
├── CharacterAudioController.cs          // Main script
├── vThirdPersonController.cs            // Modified untuk integration
├── vThirdPersonInput.cs                 // Modified untuk auto-init
├── Editor/
│   ├── CharacterAudioControllerEditor.cs  // Custom inspector
│   └── CharacterAudioSetupWizard.cs      // Setup wizard
├── AUDIO_README.md                      // Quick start (file ini)
└── CHARACTER_AUDIO_SETUP.md             // Dokumentasi lengkap
```

## 🎮 Cara Kerja

System otomatis:
- Detect saat karakter melompat → play jump sound
- Detect saat karakter mendarat → play landing sound
- Detect movement speed:
  - `inputMagnitude < 0.5` → walk footsteps
  - `inputMagnitude ≥ 0.5` → run footsteps
  - `isSprinting = true` → sprint footsteps

## ⚙️ Configuration

### Volumes (0-1)
- Jump: 0.7
- Landing: 0.6
- Walk: 0.5
- Run: 0.6
- Sprint: 0.7

### Step Intervals (seconds)
- Walk: 0.5s
- Run: 0.35s
- Sprint: 0.25s

Semua bisa diatur di Inspector!

## 🎨 Rekomendasi Audio

### Format
- **Type**: WAV atau OGG
- **Sample Rate**: 44100 Hz
- **Bit Depth**: 16-bit
- **Channels**: Mono (lebih ringan)

### Durasi
- Jump: 0.2 - 0.5 detik
- Landing: 0.2 - 0.4 detik
- Footsteps: 0.1 - 0.3 detik

### Jumlah Variasi
- Minimal: 2 clips per movement type
- Recommended: 3-4 clips per movement type
- Optimal: 5+ clips untuk variasi maksimal

## 🆓 Download Audio Gratis

1. **Freesound.org** - Search: "footstep", "jump"
2. **Mixkit.co** - Kategori: Game Sounds
3. **Zapsplat.com** - Free dengan atribusi
4. **Unity Asset Store** - Search: "footstep" filter by "Free"

## 🧪 Testing

### Di Editor (Play Mode)
1. Start Play Mode
2. Pilih character di Hierarchy
3. Di Inspector → Character Audio Controller
4. Klik "Test Jump Sound" atau "Test Landing Sound"

### In-Game
- **WASD** → Dengar footsteps
- **Shift** → Sprint, footsteps lebih cepat
- **Space** → Jump sound
- **Landing** → Auto-play landing sound

## ⚠️ Troubleshooting

| Problem | Solution |
|---------|----------|
| Tidak ada suara | Cek AudioListener ada di Main Camera |
| Footsteps tidak main | Cek karakter isGrounded = true |
| Volume terlalu pelan | Adjust volume di Inspector |
| Footsteps terlalu cepat | Increase Step Interval values |

## 📖 Dokumentasi Lengkap

Baca `CHARACTER_AUDIO_SETUP.md` untuk:
- Setup advanced
- Custom scripting
- Surface-based footsteps
- Audio Mixer integration
- Performance optimization

## 💡 Tips

1. **Use Multiple Clips**: 3-4 variasi per movement type untuk suara lebih natural
2. **Enable Random Pitch**: Membuat variasi tambahan otomatis
3. **Adjust Intervals**: Sesuaikan dengan animation speed karakter Anda
4. **Test In-Game**: Selalu test dengan gameplay actual, bukan hanya di editor

## 🔧 Customization via Code

```csharp
// Get reference
CharacterAudioController audio = GetComponent<CharacterAudioController>();

// Change volumes
audio.walkVolume = 0.3f;
audio.runVolume = 0.5f;

// Change intervals
audio.walkStepInterval = 0.6f;

// Manual control
audio.PlayJumpSound();
audio.StopAllAudio();
```

## ✅ Checklist Setup

- [ ] Character Audio Controller added
- [ ] Jump sound assigned
- [ ] Landing sound assigned
- [ ] Walk footsteps assigned (min 2 clips)
- [ ] Run footsteps assigned (min 2 clips)
- [ ] Sprint footsteps assigned (optional)
- [ ] Tested in Play Mode
- [ ] Volumes adjusted
- [ ] Intervals adjusted
- [ ] Tested in actual gameplay

## 🆘 Need Help?

1. Open Setup Wizard: `Invector → Character Audio Setup Wizard`
2. Read full documentation: `CHARACTER_AUDIO_SETUP.md`
3. Check Inspector tooltips
4. Check Unity Console for errors

---

**System Version**: 1.0  
**Compatible with**: Invector Third Person Controller  
**Unity Version**: 2021.3+
