using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameStarted = false;
    public bool isPaused = false;
    
    [Header("References")]
    public MainMenuManager mainMenuManager;
    public CharacterController playerController;
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void InitializeGame()
    {
        // Set initial game state
        isGameStarted = false;
        isPaused = false;
        
        // Pastikan game dimulai dengan main menu
        if (mainMenuManager == null)
            mainMenuManager = FindObjectOfType<MainMenuManager>();
    }
    
    public void StartGame()
    {
        isGameStarted = true;
        isPaused = false;
        
        // Enable player controller
        if (playerController != null)
            playerController.enabled = true;
            
        Debug.Log("Game Started!");
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Game Paused!");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Game Resumed!");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        // Reset game state
        isGameStarted = false;
        isPaused = false;
        
        // Return to main menu
        if (mainMenuManager != null)
            mainMenuManager.ReturnToMainMenu();
            
        Debug.Log("Game Restarted!");
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void Update()
    {
        // Handle pause with ESC key during gameplay
        if (Input.GetKeyDown(KeyCode.Escape) && isGameStarted && !isPaused)
        {
            PauseGame();
        }
    }
}