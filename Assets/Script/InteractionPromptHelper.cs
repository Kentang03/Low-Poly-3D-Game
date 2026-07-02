using UnityEngine;
using TMPro;

public class InteractionPromptHelper : MonoBehaviour
{
    [Header("Prompt Settings")]
    public Canvas worldCanvas;
    public GameObject promptPrefab;
    
    [ContextMenu("Create Interaction Prompt")]
    public void CreateInteractionPrompt()
    {
        // Create world space canvas if it doesn't exist
        if (worldCanvas == null)
        {
            GameObject canvasGO = new GameObject("WorldCanvas");
            worldCanvas = canvasGO.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.worldCamera = Camera.main;
            
            // Add CanvasScaler for proper scaling
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        // Create prompt GameObject
        GameObject promptGO = new GameObject("InteractionPrompt");
        promptGO.transform.SetParent(worldCanvas.transform);
        
        // Add RectTransform
        RectTransform rect = promptGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        
        // Add background panel
        UnityEngine.UI.Image background = promptGO.AddComponent<UnityEngine.UI.Image>();
        background.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black
        
        // Create text child
        GameObject textGO = new GameObject("PromptText");
        textGO.transform.SetParent(promptGO.transform);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "Press E to interact";
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        // Position prompt above ItemReceiver
        promptGO.transform.position = transform.position + Vector3.up * 2f;
        promptGO.transform.LookAt(Camera.main.transform);
        
        // Make it a prefab-ready setup
        promptGO.SetActive(false);
        
        Debug.Log("Interaction Prompt created! Assign it to ItemReceiver's interactionPrompt field.");
    }
}