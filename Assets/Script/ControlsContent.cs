using UnityEngine;
using UnityEngine.UI;

public class ControlsContent : MonoBehaviour
{
    [Header("Controls Text")]
    [TextArea(20, 30)]
    public string controlsText = 
@"GAME CONTROLS

MOVEMENT
W, A, S, D - Move Forward, Left, Backward, Right
Left Shift (Hold) - Run
Space - Jump
Mouse - Look Around

CAMERA
C - Switch between First Person / Third Person View
Mouse Scroll - Zoom (Third Person only)

GAME CONTROLS
ESC - Pause Menu
Tab - Quick Pause (Alternative)
Enter - Confirm Selection
Backspace - Cancel/Back

MENU NAVIGATION
Mouse - Navigate UI
Left Click - Select Button
ESC - Return to Previous Menu

SETTINGS
All controls can be customized in the Settings menu.
Mouse sensitivity can be adjusted for your preference.

TIPS
• Use headphones for the best audio experience
• Adjust graphics settings if experiencing lag
• Save your progress regularly
• Explore the world to discover secrets!

Have fun playing!";

    [Header("UI References")]
    public Text controlsLabel;
    public ScrollRect scrollRect;
    
    void Start()
    {
        if (controlsLabel != null)
            controlsLabel.text = controlsText;
            
        // Reset scroll position to top
        if (scrollRect != null)
            scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    
    public void ResetScrollPosition()
    {
        if (scrollRect != null)
            scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    
    // Method untuk highlight control berdasarkan input
    public void HighlightControl(string controlName)
    {
        // Implementasi untuk highlight text tertentu
        // Bisa dikembangkan lebih lanjut jika dibutuhkan
        Debug.Log($"Highlighting control: {controlName}");
    }
}