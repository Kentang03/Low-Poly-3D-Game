# Settings Panel Setup Guide

## Overview
Settings panel sekarang menggunakan layout 2-kolom dengan navigasi di kiri dan konten di kanan.

## UI Layout Structure

```
┌─────────────────────────────────────────────────┐
│                   SETTINGS                      │
├──────────────────┬──────────────────────────────┤
│   NAVIGATION     │        CONTENT AREA          │
│   (Left Side)    │       (Right Side)           │
│                  │                              │
│  [🔊 Audio]     │  ┌─Audio Settings Panel──┐   │
│  [ 🎮 Controls] │  │ Master Volume: ████▒▒│   │
│                  │  │ Music Volume:  ███▒▒▒│   │
│                  │  │ SFX Volume:    ████▒▒│   │
│                  │  └──────────────────────┘   │
│                  │                              │
│      [Back]      │     OR                       │
│                  │                              │
│                  │  ┌─Controls Settings────┐   │
│                  │  │ Mouse Sensitivity: 3.0│   │
│                  │  │ Camera Speed: 5.0     │   │
│                  │  │ ☐ Invert Y Axis      │   │
│                  │  │ Controls Instructions │   │
│                  │  └──────────────────────┘   │
└──────────────────┴──────────────────────────────┘
```

## GameObject Hierarchy

```
Settings Panel
├── Left Panel (Navigation)
│   ├── Audio Settings Button
│   ├── Controls Button  
│   └── Back Button
└── Right Panel (Content Area)
    ├── Audio Settings Panel
    │   ├── Master Volume Slider
    │   ├── Music Volume Slider
    │   ├── SFX Volume Slider
    │   └── Volume Labels
    └── Controls Settings Panel
        ├── Mouse Sensitivity Slider
        ├── Camera Speed Slider
        ├── Invert Y Toggle
        ├── Invert X Toggle
        └── Controls Instructions Text
```

## Script Components Required

### 1. MainMenuManager.cs (Updated)
**New Fields:**
```csharp
[Header("Settings Sub-Panels (Left Side Navigation)")]
public Button audioSettingsButton;
public Button controlsButton;

[Header("Settings Content Panels (Right Side Display)")]
public GameObject audioSettingsPanel;
public GameObject controlsSettingsPanel;
```

**New Methods:**
- `ShowAudioSettings()` - Display audio panel on right
- `ShowControlsSettings()` - Display controls panel on right
- `HideAllSettingsContentPanels()` - Hide all right panels

### 2. AudioSettingsManager.cs (New)
- Manages audio volume sliders
- Saves/loads audio preferences
- Integrates with AudioManager

### 3. ControlsSettingsManager.cs (New)
- Manages control sensitivity settings
- Displays control instructions
- Integrates with camera system

## Setup Instructions

### Step 1: Create UI Layout
1. **Settings Panel**:
   - Create main settings panel GameObject
   - Add horizontal layout group or manual positioning

2. **Left Navigation Panel**:
   - Create Audio Settings Button
   - Create Controls Button
   - Create Back Button
   - Add vertical layout group

3. **Right Content Area**:
   - Create Audio Settings Panel
   - Create Controls Settings Panel
   - Both panels occupy same space (only one active at a time)

### Step 2: Audio Settings Panel
```
Audio Settings Panel
├── Master Volume
│   ├── Label: "Master Volume"
│   ├── Slider (0.0 - 1.0)
│   └── Percentage Label
├── Music Volume
│   ├── Label: "Music Volume"
│   ├── Slider (0.0 - 1.0)
│   └── Percentage Label
└── SFX Volume
    ├── Label: "SFX Volume"
    ├── Slider (0.0 - 1.0)
    └── Percentage Label
```

### Step 3: Controls Settings Panel
```
Controls Settings Panel
├── Mouse Sensitivity
│   ├── Label: "Mouse Sensitivity"
│   ├── Slider (1.0 - 10.0)
│   └── Value Label
├── Camera Speed
│   ├── Label: "Camera Speed" 
│   ├── Slider (1.0 - 10.0)
│   └── Value Label
├── Invert Options
│   ├── Toggle: "Invert Y Axis"
│   └── Toggle: "Invert X Axis"
└── Controls Instructions
    └── Text Area with control mappings
```

### Step 4: Component Assignment
1. **MainMenuManager**:
   - Assign all buttons and panels
   - Link navigation buttons to content panels

2. **AudioSettingsManager**:
   - Attach to Audio Settings Panel
   - Assign all sliders and labels
   - Link to AudioManager

3. **ControlsSettingsManager**:
   - Attach to Controls Settings Panel
   - Assign all sliders, toggles, and text
   - Configure default values

### Step 5: Inspector Setup Checklist

#### MainMenuManager:
- [ ] Settings Panel assigned
- [ ] Audio Settings Button assigned (left panel)
- [ ] Controls Button assigned (left panel)
- [ ] Back From Settings Button assigned
- [ ] Audio Settings Panel assigned (right panel)
- [ ] Controls Settings Panel assigned (right panel)

#### AudioSettingsManager:
- [ ] Master Volume Slider assigned
- [ ] Music Volume Slider assigned
- [ ] SFX Volume Slider assigned
- [ ] All volume labels assigned (optional)
- [ ] AudioManager reference assigned

#### ControlsSettingsManager:
- [ ] Mouse Sensitivity Slider assigned
- [ ] Camera Speed Slider assigned
- [ ] Invert Y Toggle assigned
- [ ] Invert X Toggle assigned
- [ ] Controls Instructions Text assigned

## Behavior Flow

### Navigation:
1. **Open Settings**: Shows settings panel + audio panel by default
2. **Click Audio Button**: Hides all content panels → Shows audio panel
3. **Click Controls Button**: Hides all content panels → Shows controls panel
4. **Click Back**: Closes settings → Returns to main menu

### Settings Persistence:
- All settings auto-save to PlayerPrefs
- Settings load automatically on start
- Reset buttons available for defaults

## Visual Polish (Optional)

### Button Highlighting:
- Selected navigation button can be highlighted
- Use button color states for visual feedback

### Smooth Transitions:
- Fade in/out content panels
- Scale or slide animations for panel changes

### Responsive Layout:
- Adjust for different screen sizes
- Maintain proper spacing and proportions

This system provides a clean, organized settings interface that's easy to navigate and extend with additional settings categories in the future!