using UnityEngine;
using UnityEngine.UI;

public class CreditsContent : MonoBehaviour
{
    [Header("Credits Text")]
    [TextArea(20, 30)]
    public string creditsText = 
@"GAME CREDITS

DEVELOPMENT TEAM
Game Designer: [Your Name]
Programmer: [Your Name]
3D Artist: [Your Name]
Sound Designer: [Your Name]

SPECIAL THANKS
Unity Technologies
Community Contributors
Beta Testers

ASSETS USED
[List any third-party assets]

MUSIC & SOUND EFFECTS
[List music credits]

COPYRIGHT
© 2026 [Your Studio Name]
All Rights Reserved

Thank you for playing!

Made with Unity Engine
Built with passion and coffee ☕";

    [Header("UI References")]
    public Text creditsLabel;
    public ScrollRect scrollRect;
    
    void Start()
    {
        if (creditsLabel != null)
            creditsLabel.text = creditsText;
            
        // Reset scroll position to top
        if (scrollRect != null)
            scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    
    public void ResetScrollPosition()
    {
        if (scrollRect != null)
            scrollRect.normalizedPosition = new Vector2(0, 1);
    }
}