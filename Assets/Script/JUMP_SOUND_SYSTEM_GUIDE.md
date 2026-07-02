# Player Jump Sound System Guide

## 📋 Overview

Sistem Jump Sound Effect yang telah dibuat memberikan feedback audio saat player melompat dan mendarat. Sistem ini terintegrasi dengan Invector Third Person Controller dan secara otomatis mendeteksi kapan player jump dan landing.

## 🎯 Komponen Sistem

### 1. PlayerJumpSoundController.cs
**Fungsi:** Mengelola sound effects untuk jump dan landing
- 🎵 Multiple jump sounds dengan random selection
- 🎯 Landing sound effect  
- 🔀 Pitch randomization untuk variasi
- ⏱️ Cooldown system untuk prevent spam
- 🔧 Auto-integration dengan Invector controller

### 2. PlayerJumpSoundSetup.cs  
**Fungsi:** Setup wizard untuk mudah konfigurasi
- 🚀 One-click setup system
- 🔍 Auto-detect player controller
- ✅ System validation
- 🧪 Testing tools

## 🚀 Setup Guide

### Step 1: Quick Setup (Recommended)
1. Pastikan Anda memiliki **vThirdPersonController** di scene
2. Buat GameObject kosong atau gunakan player GameObject
3. Add component `PlayerJumpSoundSetup.cs`
4. Klik kanan → Context Menu → **🔊 Setup Player Jump Sound System**
5. Assign AudioClip files untuk jump dan landing sounds
6. Done! ✅

### Step 2: Manual Setup (Advanced)

#### A. Setup PlayerJumpSoundController
```csharp
// 1. Pada player GameObject (yang memiliki vThirdPersonController)
// 2. Add component PlayerJumpSoundController.cs
// 3. Assign audio clips:
public AudioClip[] jumpSounds;     // Array of jump sounds
public AudioClip landingSound;     // Single landing sound
```

#### B. Configure Audio Settings
```csharp
// Volume settings
public float jumpSoundVolume = 0.7f;
public float landingSoundVolume = 0.8f;

// Pitch variation untuk natural sound
public bool randomizePitch = true;
public float minPitch = 0.9f;
public float maxPitch = 1.1f;

// Cooldown untuk prevent spam
public float jumpSoundCooldown = 0.2f;
public float landingSoundCooldown = 0.3f;
```

#### C. AudioSource Configuration
AudioSource akan dibuat otomatis dengan settings:
```csharp
audioSource.playOnAwake = false;
audioSource.spatialBlend = 0f;  // 2D sound
audioSource.volume = jumpSoundVolume;
```

## 🎮 Usage & Features

### Automatic Detection
System otomatis mendeteksi:
- **Jump Start**: Saat `playerController.isJumping` berubah dari `false` ke `true`
- **Landing**: Saat player kembali ke ground setelah airborne

### Sound Variation
- **Multiple Jump Sounds**: Assign beberapa AudioClip untuk variasi
- **Random Selection**: Setiap jump akan memilih sound secara random
- **Pitch Randomization**: Setiap sound akan memiliki pitch yang sedikit berbeda
- **Cooldown System**: Mencegah sound spam saat spam jump

### Integration dengan Invector
```csharp
// System menggunakan Invector properties:
playerController.isJumping    // Detect jump state
playerController.isGrounded   // Detect ground state

// Automatic integration, no additional code needed
```

## 🔧 Configuration Options

### Jump Sound Settings
```csharp
[Header("Jump Sound Settings")]
public AudioClip[] jumpSounds;         // Array jump sounds untuk variasi
public AudioClip landingSound;         // Single landing sound

[Header("Audio Settings")]
public float jumpSoundVolume = 0.7f;   // Volume jump sound
public float landingSoundVolume = 0.8f; // Volume landing sound
```

### Sound Variation Settings
```csharp
[Header("Sound Variation")]
public bool randomizePitch = true;     // Enable pitch randomization
public float minPitch = 0.9f;          // Minimum pitch multiplier
public float maxPitch = 1.1f;          // Maximum pitch multiplier
```

### Cooldown Settings
```csharp
[Header("Cooldown Settings")]
public float jumpSoundCooldown = 0.2f;    // Minimum time between jump sounds
public float landingSoundCooldown = 0.3f; // Minimum time between landing sounds
```

### Debug Settings
```csharp
[Header("Debug")]
public bool showDebugLogs = false;     // Enable debug console output
```

## 🧪 Testing & Debug

### Context Menu Commands
**PlayerJumpSoundController:**
- **Test Jump Sound** - Play jump sound manually
- **Test Landing Sound** - Play landing sound manually  
- **Enable Debug Mode** - Enable console debug logs
- **Disable Debug Mode** - Disable console debug logs

**PlayerJumpSoundSetup:**
- **🔊 Setup Player Jump Sound System** - Complete auto-setup
- **🎯 Find Player Controller** - Find vThirdPersonController
- **🔊 Setup Jump Sound Controller** - Add component to player
- **🎵 Configure Audio Source** - Setup AudioSource component
- **🎶 Assign Default Sounds** - Assign provided default sounds
- **🧪 Test Jump Sounds** - Test both jump and landing sounds
- **📊 Check System Status** - Check current system status

### Debug Mode
Enable debug mode untuk melihat console logs:
```csharp
PlayerJumpSoundController controller = FindObjectOfType<PlayerJumpSoundController>();
controller.SetDebugMode(true);

// Console output akan menunjukkan:
// "Player jumped!"
// "Player landed!"  
// "Played jump sound: JumpSound1"
// "Played landing sound: LandingSound"
```

### Manual Testing
```csharp
// Test sounds manually dalam script
PlayerJumpSoundController controller = FindObjectOfType<PlayerJumpSoundController>();
controller.TestJumpSound();    // Play jump sound
controller.TestLandingSound(); // Play landing sound
```

## 🎵 Audio Recommendations

### Jump Sounds
- **Duration**: 0.1 - 0.5 seconds untuk responsiveness
- **Format**: WAV atau OGG untuk quality
- **Volume**: Medium intensity, tidak terlalu loud
- **Variety**: 2-5 different sounds untuk variation
- **Character**: Light, energetic sounds (whoosh, hop, etc.)

### Landing Sounds
- **Duration**: 0.1 - 0.3 seconds
- **Character**: Grounded, impact sounds (thud, step, etc.)  
- **Volume**: Slightly louder dari jump sound
- **Timing**: Immediate response saat landing

### Example Sound Types
**Jump Sounds:**
- Whoosh/wind sounds
- Light vocal grunts  
- Fabric/clothing rustle
- Air displacement sounds

**Landing Sounds:**
- Footstep impacts
- Ground contact sounds
- Cloth settling sounds
- Light thud/impact sounds

## 🔧 Advanced Customization

### Adding Sounds at Runtime
```csharp
PlayerJumpSoundController controller = FindObjectOfType<PlayerJumpSoundController>();

// Add new jump sound
controller.AddJumpSound(newJumpClip);

// Change landing sound
controller.SetLandingSound(newLandingClip);
```

### Manual Sound Triggers
```csharp
// If you need to trigger sounds manually from other scripts
PlayerJumpSoundController controller = FindObjectOfType<PlayerJumpSoundController>();

// Manual jump sound
controller.OnPlayerJump();

// Manual landing sound  
controller.OnPlayerLand();
```

### Integration dengan Event Systems
```csharp
// You can call these methods from UnityEvents
public void OnPlayerJump()    // For UnityEvent integration
public void OnPlayerLand()    // For UnityEvent integration
```

## 🔍 Troubleshooting

### Sound tidak play
- **Check AudioSource**: Pastikan ada AudioSource component
- **Check AudioClips**: Pastikan jump sounds dan landing sound assigned
- **Check Volume**: Pastikan volume tidak 0
- **Check Audio Listener**: Pastikan ada Audio Listener di scene

### Sound terlalu sering/spam
- **Increase Cooldown**: Naikkan `jumpSoundCooldown` dan `landingSoundCooldown`
- **Check Input**: Pastikan tidak ada multiple input sources

### Player Controller tidak terdeteksi  
- **Check vThirdPersonController**: Pastikan component ada di scene
- **Check Script Location**: PlayerJumpSoundController harus di GameObject yang sama dengan vThirdPersonController atau system akan auto-find

### Landing sound tidak play
- **Check Ground Detection**: Pastikan Invector ground detection bekerja
- **Check isGrounded**: Verify `playerController.isGrounded` changes correctly
- **Enable Debug Mode**: Check console untuk landing detection logs

## ⚙️ System Requirements

- **Unity Version**: Compatible dengan Unity yang support Invector
- **Dependencies**: 
  - Invector Third Person Controller
  - AudioSource component  
  - AudioListener dalam scene
- **Audio Format**: WAV, OGG, MP3 supported
- **Performance**: Minimal impact, efficient sound management

## 📝 Checklist Setup

- [ ] vThirdPersonController exists in scene
- [ ] PlayerJumpSoundController added to player GameObject
- [ ] AudioSource component configured
- [ ] Jump sounds assigned (Array AudioClip)
- [ ] Landing sound assigned (Single AudioClip)  
- [ ] Audio Listener exists in scene
- [ ] Volume levels configured appropriately
- [ ] Cooldown settings configured
- [ ] Test jump and landing sounds work
- [ ] Debug mode tested (optional)

## 🎯 Integration Points

### Dengan Audio Manager
```csharp
// Jika Anda memiliki AudioManager, bisa integrate:
AudioManager.Instance.PlaySFX(jumpSound);
AudioManager.Instance.PlaySFX(landingSound);
```

### Dengan Settings System
```csharp  
// Volume bisa dikontrol dari settings
public void SetJumpSoundVolume(float volume)
{
    PlayerJumpSoundController controller = FindObjectOfType<PlayerJumpSoundController>();
    controller.jumpSoundVolume = volume;
}
```

### Dengan Animation Events
Jika diperlukan, bisa juga trigger dari Animation Events:
```csharp
// In Animation Event, call:
FindObjectOfType<PlayerJumpSoundController>().OnPlayerJump();
```

## ✅ Completed Features

✅ **Automatic jump detection** dengan Invector integration  
✅ **Multiple jump sounds** dengan random selection  
✅ **Landing sound effects** dengan ground detection  
✅ **Pitch randomization** untuk natural variation  
✅ **Cooldown system** untuk prevent audio spam  
✅ **Easy setup wizard** dengan auto-configuration  
✅ **Debug tools** untuk testing dan troubleshooting  
✅ **Comprehensive documentation** dan setup guide  
✅ **Context menu integration** untuk easy testing  
✅ **Runtime sound management** dengan add/change sounds  

System jump sound effects siap digunakan! 🎵