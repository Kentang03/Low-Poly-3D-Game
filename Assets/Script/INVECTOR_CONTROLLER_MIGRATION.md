# Invector Controller Migration Guide

## Overview
GameManager dan PauseMenuManager telah diupdate untuk menggunakan `vThirdPersonController` dan `vThirdPersonInput` dari Invector secara langsung, menggantikan sistem `CharacterController` lama dan `InvectorControllerAdapter`.

## Key Changes Made

### 1. GameManager.cs Updates

#### New References:
```csharp
[Header("Player Controller References")]
public vThirdPersonController vThirdPersonController;  // Invector controller
public vThirdPersonInput vThirdPersonInput;            // Invector input
public InvectorControllerAdapter invectorAdapter;      // Optional adapter

[Header("Legacy Support (Deprecated)")]
public CharacterController legacyPlayerController;     // Backward compatibility
```

#### Updated Methods:

##### StartGame():
- **Primary**: Enables `vThirdPersonController` and `vThirdPersonInput`
- **Fallback**: Uses `InvectorControllerAdapter` if direct components not available
- **Legacy**: Falls back to `CharacterController` for compatibility

##### SetPauseState():
- **Primary**: Disables `vThirdPersonInput` when paused
- **Direct Control**: Sets `vThirdPersonController.input = Vector3.zero` when paused
- **Safety**: Stops sprinting when paused

##### New Helper Methods:
- `GetActiveControllerType()`: Returns string describing active controller
- `GetPlayerTransform()`: Returns transform of active player controller
- `ValidateControllerSetup()`: Debug method to check controller setup

### 2. PauseMenuManager.cs Updates

#### New References:
```csharp
[Header("Player Controller References")]
public vThirdPersonController vThirdPersonController;  // Invector controller
public vThirdPersonInput vThirdPersonInput;            // Invector input
public InvectorControllerAdapter invectorAdapter;      // Optional adapter
```

#### Updated Methods:

##### FreezePlayerMovement():
- **Primary Method**: Disables `vThirdPersonInput` and zeroes controller input
- **Fallback**: Uses `InvectorControllerAdapter.SetCanMove()`
- **Legacy**: Direct `CharacterController.enabled` control

## Migration Benefits

### 1. Direct Integration:
- No intermediate adapter layer needed
- Direct access to Invector functionality
- Better performance and control

### 2. Cleaner Code:
- Fewer dependencies on custom adapters
- Direct use of Invector's intended API
- More maintainable codebase

### 3. Better Control:
- Precise input management during pause
- Proper movement state control
- Access to all Invector features

## Setup Instructions

### 1. Player GameObject Setup:
```
Player GameObject
├── vThirdPersonController (Component)
├── vThirdPersonInput (Component)
├── vThirdPersonMotor (Component)
├── vThirdPersonAnimator (Component)
├── Rigidbody
├── CapsuleCollider
└── Animator
```

### 2. Inspector Assignment:

#### GameManager:
- **vThirdPersonController**: Assign player's vThirdPersonController component
- **vThirdPersonInput**: Assign player's vThirdPersonInput component
- **invectorAdapter**: Optional - leave null if using direct integration
- **legacyPlayerController**: Optional - for backward compatibility

#### PauseMenuManager:
- Same assignments as GameManager
- Auto-finds components if not assigned

### 3. Camera Setup:
- Use `vThirdPersonCamera` from Invector
- Assign in `vThirdPersonInput` component
- Camera auto-initializes with player target

## Controller Priority System

The system uses a priority hierarchy for controller selection:

### 1. Primary (Preferred):
- `vThirdPersonController` + `vThirdPersonInput`
- Direct Invector integration

### 2. Secondary (Fallback):
- `InvectorControllerAdapter`
- Custom adapter layer

### 3. Tertiary (Legacy):
- `CharacterController`
- Deprecated, for compatibility only

## Validation & Debugging

### GameManager Validation:
```csharp
// In Inspector, right-click GameManager
// Select "Validate Controller Setup"
```

**Output Example:**
```
=== CONTROLLER SETUP VALIDATION ===
vThirdPersonController: ✅ Found
vThirdPersonInput: ✅ Found
InvectorAdapter: ❌ Not Found
Legacy Controller: ✅ Not Found

Active Controller Type: Invector vThirdPersonController
🎉 SETUP COMPLETE: Using Invector vThirdPersonController
```

### Debug Information:
- `GetActiveControllerType()`: Check which controller is active
- `GetPlayerTransform()`: Get player transform reference
- Console logs during pause/resume operations

## Common Issues & Solutions

### Issue 1: Controller Not Found
**Problem**: "No suitable player controller found"
**Solution**: 
1. Ensure player has `vThirdPersonController` component
2. Assign references in GameManager inspector
3. Check component is enabled

### Issue 2: Input Not Responding
**Problem**: Player doesn't move after pause/resume
**Solution**:
1. Verify `vThirdPersonInput` is assigned
2. Check input component is enabled after resume
3. Ensure Time.timeScale = 1f after resume

### Issue 3: Camera Issues
**Problem**: Camera doesn't follow player
**Solution**:
1. Assign `vThirdPersonCamera` in scene
2. Check camera target is set to player transform
3. Verify camera initialization in `vThirdPersonInput`

## Testing Checklist

### Basic Functionality:
- [ ] Player movement works in gameplay
- [ ] Camera follows player correctly
- [ ] Sprint/jump functions work

### Pause System:
- [ ] ESC key pauses game
- [ ] Player movement stops when paused
- [ ] Time.timeScale = 0 when paused
- [ ] UI appears and cursor is visible
- [ ] ESC key resumes game
- [ ] Player movement resumes correctly
- [ ] Time.timeScale = 1 when resumed

### Scene Transitions:
- [ ] Main menu to gameplay transition works
- [ ] Pause menu "Return to Main Menu" works
- [ ] Player controller state preserved across transitions

### Controller Validation:
- [ ] GameManager.ValidateControllerSetup() shows correct status
- [ ] No error messages in console
- [ ] GetActiveControllerType() returns expected value

## Performance Considerations

### Optimizations:
- Direct component access (no adapter overhead)
- Efficient input state management
- Minimal component searching with caching

### Memory Usage:
- References cached at initialization
- No repeated FindObjectOfType calls during gameplay
- Clean component lifecycle management

This migration provides a more robust, performant, and maintainable integration with the Invector character controller system.