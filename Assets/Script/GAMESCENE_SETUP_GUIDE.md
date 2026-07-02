# GameScene Setup Guide

## Overview
Panduan lengkap untuk setup GameScene dengan sistem spawn player yang terintegrasi dengan Invector vThirdPersonController.

## Scene Structure

### GameScene.unity
**Main gameplay scene** yang akan dimuat setelah player menekan "Play" di main menu.

```
GameScene Hierarchy:
├── Managers
│   ├── Game Manager
│   ├── Player Spawn Manager  
│   ├── Gameplay Scene Initializer
│   ├── Scene Transition Manager
│   ├── Audio Manager
│   └── Pause Menu Manager
├── Player
│   └── (vThirdPersonController setup)
├── UI
│   ├── Canvas
│   ├── Pause Menu UI
│   └── Game HUD
├── Environment
│   ├── Terrain
│   ├── Objects
│   └── Lighting
└── Spawn Points
    ├── Player Spawn Point (default)
    ├── Checkpoint 1 (optional)
    └── Checkpoint 2 (optional)
```

## Build Settings Configuration

### Scene Order in Build Settings:
1. **Scene 0**: MainMenu.unity
2. **Scene 1**: GameScene.unity

### Setup Steps:
1. Open `File > Build Settings`
2. Add scenes in correct order:
   - Drag MainMenu.unity to index 0
   - Drag GameScene.unity to index 1
3. Ensure both scenes are checked/enabled
4. Close Build Settings

## GameScene Components Setup

### 1. Required Managers

#### Game Manager:
```csharp
[Header("Game State")]
public bool isGameStarted = false;        // Will be set to true when scene loads
public bool isPaused = false;

[Header("Player Controller References")]  
public vThirdPersonController vThirdPersonController; // Auto-found
public vThirdPersonInput vThirdPersonInput;           // Auto-found

[Header("Spawn Management")]
public PlayerSpawnManager playerSpawnManager;         // Auto-found
public SceneTransitionManager sceneTransitionManager; // Auto-found
```

#### Player Spawn Manager:
```csharp
[Header("Spawn Settings")]
public Transform defaultSpawnPoint;        // Assign spawn point
public bool spawnOnSceneLoad = true;       // Enable auto-spawn
public bool enableMovementAfterSpawn = true; // Enable movement immediately

[Header("Player References")]
public vThirdPersonController playerController; // Auto-found
public vThirdPersonInput playerInput;          // Auto-found
```

#### Gameplay Scene Initializer:
```csharp
[Header("Initialization Settings")]
public bool autoInitializeOnStart = true;     // Auto-init on scene load
public float initializationDelay = 0.2f;      // Small delay for stability

[Header("Player Setup")]
public bool ensurePlayerCanMove = true;       // Ensure no movement locks
public bool setCursorForGameplay = true;      // Lock cursor for gameplay
public bool setTimeScale = true;              // Ensure Time.timeScale = 1f
```

### 2. Player Setup

#### Player GameObject Requirements:
```
Player GameObject (tag: "Player")
├── vThirdPersonController (Component)
├── vThirdPersonInput (Component)
├── vThirdPersonMotor (Component) 
├── vThirdPersonAnimator (Component)
├── Rigidbody (Constraints: Freeze Rotation X & Z)
├── CapsuleCollider
├── Animator (with Invector character animations)
└── vThirdPersonCamera (Child object with camera)
```

#### Critical Settings:
- **Rigidbody**: `constraints = FreezeRotationX | FreezeRotationZ`
- **Rigidbody**: `isKinematic = false`
- **Tag**: Must be "Player"
- **vThirdPersonController**: `enabled = true`
- **vThirdPersonInput**: `enabled = true`
- **All lock flags**: `lockMovement = false`, `lockRotation = false`

### 3. Spawn Point Setup

#### Spawn Point GameObject:
```
Player Spawn Point (tag: "SpawnPoint")
├── SpawnPoint.cs (Component)
├── Transform (Position where player spawns)
└── Gizmo visualization (green sphere + arrow)
```

#### SpawnPoint Component Settings:
```csharp
[Header("Spawn Point Settings")]
public string spawnPointName = "Default Spawn";
public bool isDefaultSpawn = true;           // Mark as default

[Header("Spawn Behavior")]  
public bool resetPlayerRotation = true;      // Reset to spawn rotation
public bool resetPlayerVelocity = true;      // Reset physics velocity

[Header("Visual Settings")]
public bool showGizmo = true;               // Show editor gizmo
public Color gizmoColor = Color.green;      // Gizmo color
public float gizmoSize = 1f;                // Gizmo size
```

## Scene Flow Process

### 1. Main Menu → GameScene Transition:
```
MainMenu: User clicks "Play" 
    ↓
MainMenuManager.StartGame()
    ↓  
MainMenuManager.LoadGameScene() 
    ↓
SceneManager.LoadSceneAsync("GameScene")
    ↓
GameScene loads with loading screen
```

### 2. GameScene Initialization:
```
GameScene.Start()
    ↓
PlayerSpawnManager.Start() → SpawnPlayerCoroutine()
    ↓ (0.1s delay)
PlayerSpawnManager.SpawnPlayer() 
    ↓
PlayerSpawnManager.EnablePlayerMovement()
    ↓
GameplaySceneInitializer.InitializeGameplayScene()
    ↓
GameManager.StartGame() → EnsurePlayerReadyToPlay()
    ↓
Player Ready - Can Move Immediately!
```

### 3. Player State After Load:
- ✅ **Position**: At spawn point
- ✅ **Rotation**: Facing spawn direction
- ✅ **Movement**: Enabled and responsive
- ✅ **Input**: vThirdPersonInput active
- ✅ **Camera**: Following player
- ✅ **Physics**: Rigidbody not frozen
- ✅ **Cursor**: Locked for gameplay
- ✅ **Time**: Normal time scale (1.0)

## Setup Using Wizard (Recommended)

### Auto Setup Steps:
1. **Open Wizard**: `Tools > Kiro > Spawn System Setup Wizard`
2. **Set Position**: Enter spawn position (or use current player position)
3. **Configure Options**: 
   - ✅ Create Spawn Manager
   - ✅ Create Scene Initializer  
   - ✅ Setup Existing Player
4. **Click**: "Auto Setup Complete System"
5. **Test**: Play the scene

### Wizard Features:
- **Scene Analysis**: Shows current components status
- **One-Click Setup**: Creates all required components
- **Auto-Configuration**: Links all references automatically
- **Validation**: Verifies setup is correct

## Manual Setup (Alternative)

### Step 1: Create Spawn Point
1. Create empty GameObject at desired spawn location
2. Name: "Player Spawn Point"  
3. Add `SpawnPoint.cs` component
4. Set tag to "SpawnPoint"
5. Configure spawn settings

### Step 2: Add Spawn Manager
1. Create empty GameObject: "Player Spawn Manager"
2. Add `PlayerSpawnManager.cs` component
3. Assign spawn point to `defaultSpawnPoint`
4. Link player controller references if known

### Step 3: Add Scene Initializer  
1. Create empty GameObject: "Gameplay Scene Initializer"
2. Add `GameplaySceneInitializer.cs` component  
3. Enable `autoInitializeOnStart`
4. Set `initializationDelay = 0.2f`

### Step 4: Configure Game Manager
1. Find existing GameManager in scene
2. Assign PlayerSpawnManager reference
3. Link player controller components
4. Verify SceneTransitionManager reference

## Validation & Testing

### Debug Validation Methods:

#### PlayerSpawnManager:
- Right-click in Inspector → "Spawn Player"

#### GameplaySceneInitializer:
- Right-click in Inspector → "Validate Gameplay Setup"  
- Right-click in Inspector → "Initialize Gameplay Scene"

#### GameManager:
- Right-click in Inspector → "Validate Controller Setup"

#### Setup Wizard:
- "Validate Current Setup" button

### Expected Console Output:
```
=== INITIALIZING GAMEPLAY SCENE ===
✅ GameManager found
✅ PlayerSpawnManager found
✅ Player spawn system initialized
✅ Player controller setup for immediate gameplay
✅ Game state configured for gameplay
🎮 GAMEPLAY SCENE READY - Player can move!

Player spawned at: Player Spawn Point
✅ Player movement enabled - ready to play!
🎉 GAMEPLAY READY! Player can move and play.
```

### Test Checklist:
- [ ] Scene loads without errors
- [ ] Player spawns at spawn point
- [ ] Player can move immediately (WASD + Mouse)
- [ ] Camera follows player properly  
- [ ] Sprint and jump work (Shift + Space)
- [ ] ESC opens pause menu
- [ ] Pause menu "Return to Main Menu" works
- [ ] No movement restrictions or frozen components

## Common Issues & Solutions

### Issue 1: Player Not Spawning
**Symptoms**: Player not at spawn point or missing
**Solutions**:
- Verify spawn point has "SpawnPoint" tag
- Check PlayerSpawnManager.defaultSpawnPoint is assigned
- Ensure spawnOnSceneLoad = true
- Look for error messages in console

### Issue 2: Player Can't Move  
**Symptoms**: Player spawns but doesn't respond to input
**Solutions**:
- Check vThirdPersonController.enabled = true
- Verify vThirdPersonInput.enabled = true  
- Ensure lockMovement = false and lockRotation = false
- Check Time.timeScale = 1f
- Verify rigidbody is not kinematic

### Issue 3: Camera Issues
**Symptoms**: Camera doesn't follow or weird angles
**Solutions**:
- Ensure vThirdPersonCamera is child of player
- Check camera target assignment in vThirdPersonInput
- Verify camera initialization after spawn
- Check camera collision layers

### Issue 4: Build Settings Problems
**Symptoms**: Scene transition fails
**Solutions**:  
- Verify GameScene is in Build Settings at index 1
- Check scene name matches "GameScene" exactly
- Ensure scene is enabled/checked in build settings
- Verify MainMenu scene is at index 0

## Performance Considerations

### Optimization Tips:
- Use spawn delay (0.1-0.2s) to prevent frame drops
- Cache component references to avoid FindObjectOfType calls
- Disable unnecessary systems during spawn process
- Use object pooling for frequently spawned items

### Memory Management:
- Don't destroy managers between scene transitions
- Use DontDestroyOnLoad for persistent managers
- Clean up temporary spawn objects
- Manage texture and mesh memory usage

## Final Notes

### Scene Requirements:
- Must be named "GameScene.unity" 
- Must be added to Build Settings at index 1
- Must have all required manager components
- Must have properly configured player with Invector components

### Player Guarantee:
- **Immediate Movement**: Player can move as soon as scene loads
- **No Locks**: No input locks, rigidbody freezing, or movement restrictions
- **Proper State**: All components enabled and configured correctly
- **Ready to Play**: Full gameplay functionality available immediately

This setup ensures your GameScene will load the player at the spawn point with full movement capability from the moment the scene starts!