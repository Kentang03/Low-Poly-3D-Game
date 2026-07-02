# Scene Setup Guide - Main Menu System

## Overview
Sistem menu telah dirombak untuk menggunakan scene terpisah. Main menu sekarang berada di scene sendiri dan gameplay berada di scene yang berbeda.

## Scene Structure

### 1. Main Menu Scene
- **Nama Scene**: `MainMenu.unity`
- **Fungsi**: Menampilkan main menu, settings, credits, dan controls
- **Scripts yang diperlukan**:
  - `MainMenuManager.cs`
  - `MainMenuSceneSetup.cs`
  - `AudioManager.cs`

### 2. Gameplay Scene
- **Nama Scene**: `MainScene.unity` (atau sesuai kebutuhan)
- **Fungsi**: Scene gameplay utama
- **Scripts yang diperlukan**:
  - `GameManager.cs`
  - `PauseMenuManager.cs`
  - `SceneTransitionManager.cs`
  - Script player controller (vThirdPersonCamera.cs)

## Setup Instructions

### 1. Create Main Menu Scene
1. Buat scene baru: `File > New Scene`
2. Save sebagai `MainMenu.unity` di folder `Assets/Scenes/`
3. Setup basic scene:
   - Tambahkan `Main Camera`
   - Tambahkan `Directional Light`
   - Tambahkan `Canvas` untuk UI

### 2. Setup Main Menu UI
1. Buat GameObject kosong sebagai `MainMenuManager`
2. Attach script `MainMenuManager.cs`
3. Buat UI elements:
   - Main Menu Panel
   - Play Button
   - Settings Button
   - Credits Button
   - Controls Button
   - Exit Button
   - Settings Panel (dengan back button)
   - Credits Panel (dengan back button)
   - Controls Panel (dengan back button)

### 3. Setup Loading Screen
1. Buat Loading Screen UI:
   - Panel dengan background
   - Loading text
   - Progress bar (Slider)
2. Assign ke MainMenuManager script

### 4. Setup Gameplay Scene
1. Tambahkan `SceneTransitionManager` prefab ke scene
2. Setup `PauseMenuManager` dengan reference ke `SceneTransitionManager`
3. Pastikan camera menggunakan `vThirdPersonCamera.cs`

### 5. Build Settings
1. Buka `File > Build Settings`
2. Tambahkan scenes dalam urutan:
   - MainMenu.unity (index 0)
   - MainScene.unity (index 1)
3. Set MainMenu sebagai scene pertama

## Script Changes

### MainMenuManager.cs
- Dihapus dependency ke CameraTransition
- Ditambah sistem loading screen
- Menggunakan SceneManager untuk load gameplay scene

### PauseMenuManager.cs
- Menggunakan SceneTransitionManager untuk kembali ke main menu
- Dihapus dependency ke MainMenuManager

### Scripts yang Dihapus
- `CameraTransition.cs` - tidak diperlukan lagi
- `IntegratedCameraManager.cs` - tidak diperlukan lagi
- Setup guides lama (.md files)

## Camera System

### Main Menu Camera
- Static camera untuk menampilkan main menu
- Setup otomatis via `MainMenuSceneSetup.cs`

### Gameplay Camera
- Menggunakan `vThirdPersonCamera.cs` dari Invector
- Setup manual di gameplay scene

## Audio System
- `AudioManager` tetap digunakan
- Setup untuk main menu dan gameplay audio

## Testing
1. Play dari Main Menu scene
2. Test transisi ke gameplay scene
3. Test pause menu dan return to main menu
4. Test semua UI panels

## Troubleshooting

### Loading Issues
- Pastikan scene names sesuai di script
- Check Build Settings order
- Verify SceneTransitionManager setup

### Camera Issues
- Ensure vThirdPersonCamera.cs properly assigned
- Check camera references in gameplay scene

### UI Issues
- Verify all UI references assigned in inspector
- Check Canvas render mode settings