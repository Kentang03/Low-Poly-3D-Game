using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Mengelola Win Panel yang muncul ketika player berhasil mengumpulkan semua item
/// </summary>
public class WinPanelManager : MonoBehaviour
{
    [Header("Win Panel UI")]
    public GameObject winPanel;
    public TextMeshProUGUI victoryTitle;
    public TextMeshProUGUI itemsCollectedText;
    public TextMeshProUGUI timeCompletedText;
    public TextMeshProUGUI congratulationsText;
    
    [Header("Buttons")]
    public Button playAgainButton;
    public Button mainMenuButton;
    
    [Header("Animation Settings")]
    public float panelFadeInDuration = 0.5f;
    public float titleAnimationDelay = 0.3f;
    public float statsAnimationDelay = 0.6f;
    public float buttonsAnimationDelay = 1.0f;
    
    [Header("Audio")]
    public AudioClip victorySound;
    public AudioClip buttonClickSound;
    
    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";
    
    [Header("Victory Messages")]
    public string[] victoryMessages = {
        "Excellent Work!",
        "Mission Complete!",
        "Well Done!",
        "Victory!",
        "Outstanding!"
    };
    
    private CanvasGroup panelCanvasGroup;
    private AudioSource audioSource;
    private bool isShowingWinPanel = false;
    
    // Static instance
    private static WinPanelManager instance;
    public static WinPanelManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<WinPanelManager>();
            return instance;
        }
    }
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        InitializeWinPanel();
        SetupButtons();
        SetupAudio();
    }
    
    void InitializeWinPanel()
    {
        // Get or create canvas group for animations
        if (winPanel != null)
        {
            panelCanvasGroup = winPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = winPanel.AddComponent<CanvasGroup>();
                
            // Hide panel initially
            winPanel.SetActive(false);
            panelCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("WinPanelManager: Win Panel is not assigned!");
        }
    }
    
    void SetupButtons()
    {
        // Setup Play Again button
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(() => {
                PlayButtonSound();
                RestartGame();
            });
        }
        
        // Setup Main Menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() => {
                PlayButtonSound();
                GoToMainMenu();
            });
        }
    }
    
    void SetupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    /// <summary>
    /// Show win panel dengan animasi dan data
    /// </summary>
    public void ShowWinPanel()
    {
        if (isShowingWinPanel) return;
        
        isShowingWinPanel = true;
        
        // Pause game
        Time.timeScale = 0f;
        
        // Enable cursor untuk UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Get data untuk display
        ItemCollectionManager collectionManager = ItemCollectionManager.Instance;
        GameTimer gameTimer = GameTimer.Instance;
        
        // Show panel
        winPanel.SetActive(true);
        
        // Play victory sound
        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
        
        // Start animation sequence
        StartCoroutine(AnimateWinPanel(collectionManager, gameTimer));
    }
    
    IEnumerator AnimateWinPanel(ItemCollectionManager collectionManager, GameTimer gameTimer)
    {
        // Reset all UI elements alpha
        SetUIElementsAlpha(0f);
        
        // Fade in panel background
        yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, 1f, panelFadeInDuration));
        
        // Animate title
        yield return new WaitForSecondsRealtime(titleAnimationDelay);
        if (victoryTitle != null)
        {
            string randomMessage = victoryMessages[Random.Range(0, victoryMessages.Length)];
            victoryTitle.text = randomMessage;
            yield return StartCoroutine(AnimateTextElement(victoryTitle));
        }
        
        // Animate statistics
        yield return new WaitForSecondsRealtime(statsAnimationDelay - titleAnimationDelay);
        
        // Items collected info
        if (itemsCollectedText != null && collectionManager != null)
        {
            string itemInfo = $"Items Collected: {collectionManager.currentItemCount}/{collectionManager.targetItemCount} {collectionManager.itemName}";
            itemsCollectedText.text = itemInfo;
            yield return StartCoroutine(AnimateTextElement(itemsCollectedText));
        }
        
        // Time completed info
        if (timeCompletedText != null && gameTimer != null)
        {
            string timeInfo = $"Time: {gameTimer.GetTimeForStats()}";
            timeCompletedText.text = timeInfo;
            yield return StartCoroutine(AnimateTextElement(timeCompletedText));
        }
        
        // Congratulations message
        if (congratulationsText != null)
        {
            congratulationsText.text = "Congratulations on completing the challenge!";
            yield return StartCoroutine(AnimateTextElement(congratulationsText));
        }
        
        // Animate buttons
        yield return new WaitForSecondsRealtime(buttonsAnimationDelay - statsAnimationDelay);
        if (playAgainButton != null)
            yield return StartCoroutine(AnimateButtonElement(playAgainButton));
            
        if (mainMenuButton != null)
            yield return StartCoroutine(AnimateButtonElement(mainMenuButton));
    }
    
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    IEnumerator AnimateTextElement(TextMeshProUGUI textElement)
    {
        if (textElement == null) yield break;
        
        // Slide in from right with fade
        RectTransform rectTransform = textElement.GetComponent<RectTransform>();
        Vector3 originalPos = rectTransform.anchoredPosition;
        Vector3 startPos = originalPos + Vector3.right * 300f;
        
        rectTransform.anchoredPosition = startPos;
        
        float duration = 0.4f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            
            // Easing curve
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, originalPos, easedT);
            textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, easedT);
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = originalPos;
        textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, 1f);
        
        yield return new WaitForSecondsRealtime(0.1f);
    }
    
    IEnumerator AnimateButtonElement(Button button)
    {
        if (button == null) yield break;
        
        // Scale up animation
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;
        
        rectTransform.localScale = Vector3.zero;
        
        float duration = 0.3f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            
            // Bounce easing
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, easedT);
            yield return null;
        }
        
        rectTransform.localScale = originalScale;
        yield return new WaitForSecondsRealtime(0.1f);
    }
    
    void SetUIElementsAlpha(float alpha)
    {
        if (victoryTitle != null)
            victoryTitle.color = new Color(victoryTitle.color.r, victoryTitle.color.g, victoryTitle.color.b, alpha);
        if (itemsCollectedText != null)
            itemsCollectedText.color = new Color(itemsCollectedText.color.r, itemsCollectedText.color.g, itemsCollectedText.color.b, alpha);
        if (timeCompletedText != null)
            timeCompletedText.color = new Color(timeCompletedText.color.r, timeCompletedText.color.g, timeCompletedText.color.b, alpha);
        if (congratulationsText != null)
            congratulationsText.color = new Color(congratulationsText.color.r, congratulationsText.color.g, congratulationsText.color.b, alpha);
    }
    
    void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    /// <summary>
    /// Restart current game scene
    /// </summary>
    public void RestartGame()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    
    /// <summary>
    /// Go to main menu
    /// </summary>
    public void GoToMainMenu()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    /// <summary>
    /// Hide win panel (jika perlu)
    /// </summary>
    public void HideWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        isShowingWinPanel = false;
        Time.timeScale = 1f;
    }
    
    // Integration dengan ItemCollectionManager
    void OnEnable()
    {
        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.OnTaskCompleted.AddListener(ShowWinPanel);
        }
    }
    
    void OnDisable()
    {
        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.OnTaskCompleted.RemoveListener(ShowWinPanel);
        }
    }
    
    // Testing methods
    [ContextMenu("Test Show Win Panel")]
    public void TestShowWinPanel()
    {
        ShowWinPanel();
    }
    
    [ContextMenu("Test Hide Win Panel")]
    public void TestHideWinPanel()
    {
        HideWinPanel();
    }
}