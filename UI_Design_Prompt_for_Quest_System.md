# UI Design Prompt for Quest System

## Design Request: Quest System UI for Unity Game

I need you to create a modern, clean, and user-friendly UI design for a quest system in a Unity game. The UI should have an Indonesian cultural theme with elements that match traditional Indonesian aesthetics.

### 📋 UI Components Required:

**Main Quest Panel:**
- **Position:** Top-left corner of screen (40% width, 30% height)
- **Background:** Semi-transparent dark panel with subtle gradient
- **Style:** Modern UI with Indonesian cultural accents (batik patterns, traditional colors)

**1. Quest Title Section (Top 20% of panel)**
- Large, bold text displaying quest name
- Font: Sans-serif, readable, size ~18px
- Color: White or light gold
- Example text: "Collect Sacred Items"

**2. Quest Description Section (Middle 40% of panel)**
- Multi-line text area for dynamic descriptions
- Font: Smaller sans-serif, size ~12px  
- Color: Light gray/white
- Text should wrap properly
- **Dynamic content examples:**
  - Initial: "Welcome! Prepare to collect the sacred items of Indonesia."
  - Active: "Find and collect the Bakul Nasi. Look near the traditional house."
  - Progress: "Now search for the ceremonial Mask around the temple area."
  - Final: "Finally, locate the Sacred Key in the chamber."

**3. Quest Status Section (Middle 20% of panel)**
- Status indicator text
- Font: Medium size ~14px
- **Color coding:**
  - Yellow: "In Progress" / "Collect: [Item Name]"
  - Cyan/Blue: "Completed"
- Example: "Status: Collect Bakul Nasi"

**4. Progress Bar Section (Bottom 20% of panel)**
- Horizontal progress slider
- Background: Dark gray/black (80% opacity)
- Fill color: Green (incomplete) → Cyan (completed)
- Height: ~20px with rounded corners
- Progress text below: "1/3" format, centered, small font

**5. Current Item Display Panel (Optional sub-panel)**
- Smaller panel that appears when item is active
- Position: Below main panel or integrated within
- Contains:
  - Item icon/image (64x64px)
  - Item name (bold, medium font)
  - Item description (small font, 2-3 lines)
- Background: Slightly transparent, matching main panel

### 🎨 Visual Style Guidelines:

**Color Scheme (Indonesian Cultural Theme):**
- Primary: Deep brown/mahogany (#8B4513, #A0522D)
- Secondary: Gold/yellow accents (#FFD700, #DAA520) 
- Background: Dark transparent (#000000 50-70% opacity)
- Text: White (#FFFFFF), Light gold (#F5DEB3)
- Progress: Green (#32CD32) → Cyan (#00FFFF)
- Accent: Traditional Indonesian colors (maroon, gold, dark green)

**Design Elements:**
- Subtle batik pattern overlay on panel background
- Rounded corners (radius: 8-12px)
- Drop shadow for depth
- Optional: Traditional Indonesian border patterns
- Minimalist icons with Indonesian cultural elements

**Typography:**
- Modern, clean sans-serif fonts
- Good readability on dark backgrounds
- Hierarchical font sizes (Title > Status > Description > Progress)

**Layout Structure:**
```
┌─────────────────────────────────────┐
│  🏺 COLLECT SACRED ITEMS           │ ← Title
├─────────────────────────────────────┤
│ Find and collect the Bakul Nasi.   │ ← Dynamic Description
│ Look near the traditional house.    │   (2-3 lines, wrappable)
│ The rice basket holds ancient power.│
├─────────────────────────────────────┤
│ Status: Collect Bakul Nasi         │ ← Status (color coded)
├─────────────────────────────────────┤
│ ████████░░░░░░░░░░░░  1/3          │ ← Progress bar + counter  
└─────────────────────────────────────┘

Optional Current Item Panel:
┌───────────────┐
│ [🏺] Bakul Nasi │ ← Item icon + name
│ Traditional    │ ← Item description
│ rice basket    │
└───────────────┘
```

### 📱 Responsive Requirements:

- Scalable UI that works on different screen resolutions
- Anchored to top-left with proper margins (10-20px from edges)
- Text should remain readable at different UI scales
- Panel should not overlap with other game UI elements

### 🎯 Functional Requirements:

**Visual Feedback:**
- Smooth progress bar animations when items are collected
- Color transitions for status changes
- Subtle scale/pulse animations for quest completion
- Panel slide-in animation when quest starts

**States to Design:**
1. **Initial State:** Quest just started, first item not yet active
2. **Active State:** Current item available, description shows instructions
3. **Progress State:** Item collected, moving to next item
4. **Completion State:** All items collected, celebration visuals

### 🔧 Technical Notes:
- UI built using Unity's Canvas system
- Uses TextMeshPro for text rendering
- Slider component for progress bar
- Image components for backgrounds and icons
- Support for Unity's UI animation system

### 💡 Additional Enhancements (Optional):
- Particle effects around panel borders during active states
- Subtle glow effects for important elements
- Traditional Indonesian music visualization elements
- Cultural iconography (gamelan, temples, traditional patterns)
- Floating quest objectives with pointer arrows

Please create a modern, polished UI design that balances functionality with Indonesian cultural aesthetics, ensuring excellent user experience and clear information hierarchy.