using UnityEngine;
using TMPro;

/// <summary>
/// UI Controller khusus untuk menampilkan timer di layar player
/// Script ini terpisah dari GameTimer untuk fleksibilitas UI
/// </summary>
public class GameTimerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public GameObject timerPanel;
    
    [Header("Display Settings")]
    public bool showTimerOnStart = true;
    public bool showMilliseconds = false;
    
    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color completedColor = Color.green;
    public Color warningColor = Color.yellow;
    
    [Header("Position Settings")]
    [Tooltip("Timer position on screen - Top Left, Top Right, Top Center")]
    public TimerPosition timerPosition = TimerPosition.TopRight;
    
    [Header("Animation")]
    public bool animateOnUpdate = false;
    public float animationScale = 1.1f;
    public float animationDuration = 0.1f;
    
    public enum TimerPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    
    private GameTimer gameTimer;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    
    void Start()
    {
        InitializeUI();
        FindGameTimer();
        SetTimerPosition();
        
        if (showTimerOnStart)
            ShowTimer();
        else
            HideTimer();
    }
    
    void Update()
    {
        UpdateTimerDisplay();
    }
    
    void InitializeUI()
    {
        // Get components
        if (timerText == null)
            timerText = GetComponentInChildren<TextMeshProUGUI>();
            
        if (timerPanel == null)
            timerPanel = gameObject;
            
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        
        // Set initial color
        if (timerText != null)
            timerText.color = normalColor;
    }
    
    void FindGameTimer()
    {
        gameTimer = GameTimer.Instance;
        if (gameTimer == null)
        {
            Debug.LogWarning("GameTimerUI: GameTimer instance not found! Timer display may not work properly.");
        }
    }
    
    void SetTimerPosition()
    {
        if (rectTransform == null) return;
        
        // Set anchor dan position berdasarkan TimerPosition
        switch (timerPosition)
        {
            case TimerPosition.TopLeft:
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.anchoredPosition = new Vector2(20f, -20f);
                break;
                
            case TimerPosition.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(0f, -20f);
                break;
                
            case TimerPosition.TopRight:
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.anchoredPosition = new Vector2(-20f, -20f);
                break;
                
            case TimerPosition.BottomLeft:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                rectTransform.anchoredPosition = new Vector2(20f, 20f);
                break;
                
            case TimerPosition.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(0.5f, 0f);
                rectTransform.anchoredPosition = new Vector2(0f, 20f);
                break;
                
            case TimerPosition.BottomRight:
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.anchoredPosition = new Vector2(-20f, 20f);
                break;
        }
    }
    
    void UpdateTimerDisplay()
    {
        if (gameTimer == null || timerText == null) return;
        
        // Update timer text
        string timeText = showMilliseconds ? gameTimer.GetFormattedTime() : gameTimer.FormatTime(gameTimer.CurrentTime);
        
        // Add label
        timerText.text = $"Time: {timeText}";
        
        // Update color based on game state
        if (gameTimer.IsGameCompleted)
        {
            timerText.color = completedColor;
        }
        else if (gameTimer.IsTimerRunning)
        {
            timerText.color = normalColor;
        }
        else
        {
            timerText.color = warningColor;
        }
        
        // Animate if enabled
        if (animateOnUpdate && gameTimer.IsTimerRunning && !gameTimer.IsGameCompleted)
        {
            AnimateTimerUpdate();
        }
    }
    
    void AnimateTimerUpdate()
    {
        // Simple pulse animation setiap detik
        float remainder = gameTimer.CurrentTime % 1f;
        if (remainder < 0.1f && animationCoroutine == null) // Trigger animation at start of each second
        {
            animationCoroutine = StartCoroutine(PulseAnimation());
        }
    }
    
    System.Collections.IEnumerator PulseAnimation()
    {
        Vector3 targetScale = originalScale * animationScale;
        
        // Scale up
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (animationDuration / 2f);
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Scale back down
        elapsedTime = 0f;
        while (elapsedTime < animationDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (animationDuration / 2f);
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        rectTransform.localScale = originalScale;
        animationCoroutine = null;
    }
    
    /// <summary>
    /// Show timer UI
    /// </summary>
    public void ShowTimer()
    {
        if (timerPanel != null)
            timerPanel.SetActive(true);
    }
    
    /// <summary>
    /// Hide timer UI
    /// </summary>
    public void HideTimer()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }
    
    /// <summary>
    /// Toggle timer visibility
    /// </summary>
    public void ToggleTimer()
    {
        if (timerPanel != null)
            timerPanel.SetActive(!timerPanel.activeSelf);
    }
    
    /// <summary>
    /// Update timer position
    /// </summary>
    public void UpdateTimerPosition(TimerPosition newPosition)
    {
        timerPosition = newPosition;
        SetTimerPosition();
    }
    
    /// <summary>
    /// Set timer text size
    /// </summary>
    public void SetTimerTextSize(float size)
    {
        if (timerText != null)
            timerText.fontSize = size;
    }
    
    /// <summary>
    /// Set timer color
    /// </summary>
    public void SetTimerColor(Color color)
    {
        normalColor = color;
        if (timerText != null && !gameTimer.IsGameCompleted)
            timerText.color = color;
    }
    
    // Event handlers
    void OnGameCompleted()
    {
        if (timerText != null)
        {
            timerText.color = completedColor;
            
            // Optional: Add completion effect
            StartCoroutine(CompletionEffect());
        }
    }
    
    System.Collections.IEnumerator CompletionEffect()
    {
        // Flash effect when game completes
        for (int i = 0; i < 3; i++)
        {
            rectTransform.localScale = originalScale * 1.2f;
            yield return new WaitForSeconds(0.1f);
            rectTransform.localScale = originalScale;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    // Integration with GameTimer events
    void OnEnable()
    {
        // Subscribe to game completion event if available
        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.OnTaskCompleted.AddListener(OnGameCompleted);
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe
        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.OnTaskCompleted.RemoveListener(OnGameCompleted);
        }
    }
    
    // Testing methods
    [ContextMenu("Test Show Timer")]
    public void TestShowTimer()
    {
        ShowTimer();
    }
    
    [ContextMenu("Test Hide Timer")]
    public void TestHideTimer()
    {
        HideTimer();
    }
    
    [ContextMenu("Test Completion Effect")]
    public void TestCompletionEffect()
    {
        StartCoroutine(CompletionEffect());
    }
}