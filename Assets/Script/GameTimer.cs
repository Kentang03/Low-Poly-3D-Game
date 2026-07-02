using UnityEngine;
using TMPro;

/// <summary>
/// Mengelola game timer untuk tracking waktu bermain
/// </summary>
public class GameTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerDisplay;
    
    [Header("Timer Settings")]
    public bool startTimerOnStart = true;
    public bool showMilliseconds = false;
    
    [Header("Display Format")]
    public string timeFormat = "mm:ss";
    public string timeFormatWithMilliseconds = "mm:ss.ff";
    
    // Timer state
    private float currentTime = 0f;
    private bool isTimerRunning = false;
    private bool isGameCompleted = false;
    
    // Static instance untuk easy access
    private static GameTimer instance;
    public static GameTimer Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameTimer>();
            return instance;
        }
    }
    
    // Properties
    public float CurrentTime => currentTime;
    public bool IsTimerRunning => isTimerRunning;
    public bool IsGameCompleted => isGameCompleted;
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        // Initialize timer display
        if (timerDisplay == null)
            timerDisplay = GetComponent<TextMeshProUGUI>();
            
        // Auto-start timer if enabled
        if (startTimerOnStart)
            StartTimer();
            
        UpdateTimerDisplay();
    }
    
    void Update()
    {
        if (isTimerRunning && !isGameCompleted)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    
    /// <summary>
    /// Start the game timer
    /// </summary>
    public void StartTimer()
    {
        isTimerRunning = true;
        Debug.Log("Game Timer Started!");
    }
    
    /// <summary>
    /// Stop the game timer
    /// </summary>
    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log($"Game Timer Stopped at: {GetFormattedTime()}");
    }
    
    /// <summary>
    /// Pause/Resume timer
    /// </summary>
    public void PauseTimer()
    {
        isTimerRunning = !isTimerRunning;
        Debug.Log($"Game Timer {(isTimerRunning ? "Resumed" : "Paused")}");
    }
    
    /// <summary>
    /// Reset timer to zero
    /// </summary>
    public void ResetTimer()
    {
        currentTime = 0f;
        isTimerRunning = false;
        isGameCompleted = false;
        UpdateTimerDisplay();
        Debug.Log("Game Timer Reset!");
    }
    
    /// <summary>
    /// Complete the game (stops timer and marks as completed)
    /// </summary>
    public void CompleteGame()
    {
        if (!isGameCompleted)
        {
            isGameCompleted = true;
            StopTimer();
            Debug.Log($"Game Completed! Final Time: {GetFormattedTime()}");
        }
    }
    
    /// <summary>
    /// Get formatted time string
    /// </summary>
    public string GetFormattedTime()
    {
        return FormatTime(currentTime);
    }
    
    /// <summary>
    /// Format time in readable string
    /// </summary>
    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        
        if (showMilliseconds)
        {
            return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    /// <summary>
    /// Update timer display UI
    /// </summary>
    void UpdateTimerDisplay()
    {
        if (timerDisplay != null)
        {
            timerDisplay.text = GetFormattedTime();
            
            // Change color when game completed
            if (isGameCompleted)
            {
                timerDisplay.color = Color.green;
            }
        }
    }
    
    /// <summary>
    /// Get time in different formats for statistics
    /// </summary>
    public string GetTimeForStats()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        
        if (minutes > 0)
            return $"{minutes}m {seconds}s";
        else
            return $"{seconds}s";
    }
    
    // Event methods untuk integration dengan sistem lain
    void OnEnable()
    {
        // Subscribe ke ItemCollectionManager jika ada
        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.OnTaskCompleted.AddListener(CompleteGame);
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe
        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.OnTaskCompleted.RemoveListener(CompleteGame);
        }
    }
    
    // Methods untuk testing di Inspector
    [ContextMenu("Start Timer")]
    public void StartTimerDebug()
    {
        StartTimer();
    }
    
    [ContextMenu("Stop Timer")]
    public void StopTimerDebug()
    {
        StopTimer();
    }
    
    [ContextMenu("Reset Timer")]
    public void ResetTimerDebug()
    {
        ResetTimer();
    }
    
    [ContextMenu("Complete Game")]
    public void CompleteGameDebug()
    {
        CompleteGame();
    }
}