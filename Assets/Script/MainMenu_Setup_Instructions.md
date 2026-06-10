# Main Menu Setup Instructions

## Overview
Script yang telah dibuat akan memberikan sistem main menu lengkap dengan fitur:
- Background terrain dari scene asli
- Transisi camera yang smooth dari menu ke gameplay
- Character freeze/unfreeze system
- Settings menu dengan audio dan graphics controls
- Pause menu system
- Audio management system

## Scripts Yang Dibuat:

### 1. **MainMenuManager.cs**
- Mengatur main menu UI dan transisi ke gameplay
- Mengontrol camera transition
- Mengelola freeze/unfreeze character

### 2. **GameManager.cs**
- Singleton untuk mengelola game state
- Mengatur pause/resume functionality
- Koordinasi antara berbagai manager

### 3. **SettingsManager.cs**
- Mengelola semua pengaturan game (audio, graphics, controls)
- Menyimpan settings menggunakan PlayerPrefs
- Integration dengan AudioMixer

### 4. **AudioManager.cs**
- Singleton untuk mengelola semua audio dalam game
- Support untuk music, SFX, dan UI sounds
- Audio fading effects

### 5. **CameraTransition.cs**
- Smooth camera transitions dengan animation curves
- Support untuk fade effects
- Callback system untuk transition completion

### 6. **UIButtonSound.cs**
- Automatic sound effects untuk UI buttons
- Hover dan click sounds
- Custom sound support

### 7. **PauseMenuManager.cs**
- In-game pause menu system
- ESC key handling
- Return to main menu functionality

### 8. **CharacterController.cs** (Modified)
- Ditambahkan freeze/unfreeze methods
- `canMove` variable untuk kontrol movement

## Setup Instructions:

### 1. Scene Setup:
```
- Buat scene dengan terrain sebagai background
- Pastikan character sudah ada dengan CharacterController script
- Setup Cinemachine cameras:
  * Menu Camera: Positioned untuk view terrain sebagai background
  * Gameplay Camera: Follow/Look At character untuk gameplay
```

### 2. UI Setup:
```
- Buat Canvas untuk Main Menu dengan:
  * Main Menu Panel (Play, Settings, Exit buttons)
  * Settings Panel (Audio sliders, Graphics dropdowns, dll)
  * Fade Canvas (optional, untuk transition effects)
```

### 3. GameObject Hierarchy Suggestion:
```
Scene Root
├── Terrain (Background)
├── Character
│   ├── Model
│   ├── Collider
│   ├── Rigidbody
│   └── CharacterController (script)
├── Camera System
│   ├── Menu Camera (Cinemachine)
│   ├── Gameplay Camera (Cinemachine)
│   └── Camera Transition (script)
├── UI Canvas
│   ├── Main Menu Panel
│   ├── Settings Panel
│   ├── Pause Menu Panel
│   └── Fade Canvas (optional)
├── Audio System
│   ├── AudioManager (script)
│   ├── Music Source
│   ├── SFX Source
│   └── UI Source
└── Game Managers
    ├── GameManager (script)
    ├── MainMenuManager (script)
    ├── SettingsManager (script)
    └── PauseMenuManager (script)
```

### 4. Script Assignment:
```
1. Attach MainMenuManager ke GameObject dan assign:
   - UI References (buttons, panels)
   - Camera References (menu & gameplay cameras)
   - Character Reference
   - Audio References

2. Attach GameManager ke persistent GameObject

3. Attach SettingsManager ke Settings Panel dan assign:
   - All UI controls (sliders, dropdowns, toggles)
   - Character reference untuk mouse sensitivity

4. Attach AudioManager ke persistent GameObject dan assign:
   - Audio Sources
   - Audio Clips
   - Audio Mixer

5. Attach CameraTransition ke GameObject dan assign:
   - From Camera (menu)
   - To Camera (gameplay)
   - Fade Canvas (optional)

6. Attach PauseMenuManager ke Pause Menu Panel dan assign:
   - UI References
   - Manager references

7. Add UIButtonSound ke semua buttons yang perlu sound effects
```

### 5. Cinemachine Setup:
```
Menu Camera:
- Position: Static view showing terrain/environment
- Priority: 10 (active di menu)

Gameplay Camera:
- Follow: Character Transform
- Look At: Character Transform (optional)
- Priority: 5 (active saat gameplay)
```

### 6. Audio Mixer Setup (Optional):
```
Buat Audio Mixer dengan groups:
- Master
  - Music
  - SFX
  - UI

Expose parameters:
- MasterVolume
- MusicVolume
- SFXVolume
```

## Key Features:

### Camera Transition:
- Smooth transition dari menu camera ke gameplay camera
- Configurable duration dan animation curve
- Optional fade effects

### Character Control:
- Character di-freeze saat di menu
- Automatic unfreeze saat gameplay dimulai
- ESC key untuk pause/return to menu

### Audio System:
- Background music switching (menu/gameplay)
- UI sound effects (button clicks, hovers)
- Volume controls dalam settings

### Settings System:
- Audio controls (master, music, SFX volume)
- Graphics controls (quality, resolution, fullscreen)
- Controls settings (mouse sensitivity)
- Persistent settings dengan PlayerPrefs

## Usage:

1. **Start Game**: Click Play button → Camera transitions → Character unfreezes → Gameplay begins
2. **Settings**: Click Settings → Adjust audio/graphics/controls → Changes saved automatically
3. **Pause**: Press ESC during gameplay → Pause menu appears → Can resume, go to settings, or return to main menu
4. **Audio**: Automatic music switching between menu and gameplay

## Customization:

- Modify transition duration dalam `CameraTransition.transitionDuration`
- Adjust audio volumes dalam `AudioManager`
- Customize UI sounds dengan assign custom AudioClips
- Modify camera positions untuk different viewing angles
- Add more settings options dalam `SettingsManager`

## Notes:

- Semua managers menggunakan singleton pattern untuk easy access
- Settings disimpan dengan PlayerPrefs (persistent across sessions)
- Camera transition menggunakan smooth interpolation untuk cinematic effect
- Character movement completely disabled saat di menu state
- ESC key handling untuk smooth pause/unpause experience