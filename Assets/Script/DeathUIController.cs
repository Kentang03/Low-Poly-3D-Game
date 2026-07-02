using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller untuk Death UI Panel
/// Script ini bisa di-attach ke Canvas atau Death Panel GameObject
/// </summary>
public class DeathUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Text deathMessageText;
    [SerializeField] private Text checkpointInfoText;
    [SerializeField] private Text countdownText;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("UI Settings")]
    [SerializeField] private bool fadeInOut = true;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private bool showCountdown = true;
    
    private CanvasGroup canvasGroup;
    private DeathManager deathManager;
    private bool isVisible = false;
    
    void Start()
    {
        InitializeUI();
        SetupReferences();
        SetupButtons();
    }
    
    void InitializeUI()
    {
        // Find death panel jika tidak diassign
        if (deathPanel == null)
        {
            deathPanel = transform.Find("DeathPanel")?.gameObject;
            if (deathPanel == null)
            {
                // Create death panel if doesn't exist
                CreateDeathPanel();
            }
        }
        
        // Setup canvas group untuk fading
        if (fadeInOut)
        {
            canvasGroup = deathPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = deathPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Initially hide the panel
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }
    
    void CreateDeathPanel()
    {
        // Create basic death panel structure
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            // Create canvas if doesn't exist
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvasComp = canvasGO.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            canvas = canvasGO;
        }
        
        // Create death panel
        deathPanel = new GameObject("DeathPanel");
        deathPanel.transform.SetParent(canvas.transform, false);
        
        // Add background image
        Image backgroundImage = deathPanel.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        
        // Set to full screen
        RectTransform panelRect = deathPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        CreateUIElements();
        
        Debug.Log("Created basic DeathPanel UI structure");
    }
    
    void CreateUIElements()
    {
        // Create death message text
        GameObject messageTextGO = new GameObject("DeathMessageText");
        messageTextGO.transform.SetParent(deathPanel.transform, false);
        deathMessageText = messageTextGO.AddComponent<Text>();
        deathMessageText.text = "You Died!";
        deathMessageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        deathMessageText.fontSize = 48;
        deathMessageText.color = Color.white;
        deathMessageText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform messageRect = messageTextGO.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0.5f, 0.7f);
        messageRect.anchorMax = new Vector2(0.5f, 0.7f);
        messageRect.sizeDelta = new Vector2(400, 100);
        
        // Create checkpoint info text
        GameObject checkpointTextGO = new GameObject("CheckpointInfoText");
        checkpointTextGO.transform.SetParent(deathPanel.transform, false);
        checkpointInfoText = checkpointTextGO.AddComponent<Text>();
        checkpointInfoText.text = "Respawn at: Checkpoint";
        checkpointInfoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        checkpointInfoText.fontSize = 24;
        checkpointInfoText.color = Color.gray;
        checkpointInfoText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform checkpointRect = checkpointTextGO.GetComponent<RectTransform>();
        checkpointRect.anchorMin = new Vector2(0.5f, 0.5f);
        checkpointRect.anchorMax = new Vector2(0.5f, 0.5f);
        checkpointRect.sizeDelta = new Vector2(300, 50);
        
        // Create countdown text
        if (showCountdown)
        {
            GameObject countdownTextGO = new GameObject("CountdownText");
            countdownTextGO.transform.SetParent(deathPanel.transform, false);
            countdownText = countdownTextGO.AddComponent<Text>();
            countdownText.text = "Respawning in 3...";
            countdownText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            countdownText.fontSize = 20;
            countdownText.color = Color.yellow;
            countdownText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform countdownRect = countdownTextGO.GetComponent<RectTransform>();
            countdownRect.anchorMin = new Vector2(0.5f, 0.4f);
            countdownRect.anchorMax = new Vector2(0.5f, 0.4f);
            countdownRect.sizeDelta = new Vector2(200, 30);
        }
        
        // Create respawn button
        GameObject respawnButtonGO = new GameObject("RespawnButton");
        respawnButtonGO.transform.SetParent(deathPanel.transform, false);
        respawnButton = respawnButtonGO.AddComponent<Button>();
        
        // Add button image
        Image buttonImage = respawnButtonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.8f, 0.8f); // Blue-ish
        
        // Add button text
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(respawnButtonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "Respawn Now";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 20;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonRect = respawnButtonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.3f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.3f);
        buttonRect.sizeDelta = new Vector2(150, 40);
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
    }
    
    void SetupReferences()
    {
        // Find DeathManager
        deathManager = DeathManager.Instance;
        if (deathManager == null)
        {
            deathManager = FindObjectOfType<DeathManager>();
        }
    }
    
    void SetupButtons()
    {
        // Setup respawn button
        if (respawnButton != null)
        {
            respawnButton.onClick.RemoveAllListeners();
            respawnButton.onClick.AddListener(OnRespawnButtonClicked);
        }
        
        // Setup main menu button if exists
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
    }
    
    /// <summary>
    /// Show death UI panel
    /// </summary>
    public void ShowDeathUI()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            isVisible = true;
            
            if (fadeInOut && canvasGroup != null)
            {
                StartCoroutine(FadeIn());
            }
        }
    }
    
    /// <summary>
    /// Hide death UI panel
    /// </summary>
    public void HideDeathUI()
    {
        if (deathPanel != null)
        {
            if (fadeInOut && canvasGroup != null)
            {
                StartCoroutine(FadeOut());
            }
            else
            {
                deathPanel.SetActive(false);
            }
            
            isVisible = false;
        }
    }
    
    /// <summary>
    /// Update death message
    /// </summary>
    /// <param name="message">Message to display</param>
    public void SetDeathMessage(string message)
    {
        if (deathMessageText != null)
        {
            deathMessageText.text = message;
        }
    }
    
    /// <summary>
    /// Update checkpoint info
    /// </summary>
    /// <param name="checkpointName">Checkpoint name to display</param>
    public void SetCheckpointInfo(string checkpointName)
    {
        if (checkpointInfoText != null)
        {
            checkpointInfoText.text = $"Respawn at: {checkpointName}";
        }
    }
    
    /// <summary>
    /// Update countdown text
    /// </summary>
    /// <param name="countdown">Countdown time</param>
    public void SetCountdownText(string countdown)
    {
        if (countdownText != null)
        {
            countdownText.text = countdown;
        }
    }
    
    void OnRespawnButtonClicked()
    {
        Debug.Log("Respawn button clicked");
        
        if (deathManager != null)
        {
            deathManager.ManualRespawn();
        }
    }
    
    void OnMainMenuButtonClicked()
    {
        Debug.Log("Main menu button clicked");
        
        // Return to main menu (you can customize this)
        MainMenuManager mainMenu = FindObjectOfType<MainMenuManager>();
        if (mainMenu != null)
        {
            // Implementation depends on your main menu system
            Debug.Log("Returning to main menu...");
        }
    }
    
    System.Collections.IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 0f;
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    
    System.Collections.IEnumerator FadeOut()
    {
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 1f;
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        deathPanel.SetActive(false);
    }
    
    /// <summary>
    /// Get UI references untuk DeathManager
    /// </summary>
    public void AssignToDeathManager()
    {
        if (deathManager != null)
        {
            // Use reflection or modify DeathManager untuk accept UI references
            Debug.Log("Assigned UI references to DeathManager");
        }
    }
    
    public bool IsVisible => isVisible;
}