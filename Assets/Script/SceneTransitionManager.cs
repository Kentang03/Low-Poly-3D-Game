using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles scene transitions from gameplay back to main menu
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scene References")]
    public string mainMenuSceneName = "MainMenu";
    
    [Header("Loading UI")]
    public GameObject loadingScreen;
    public UnityEngine.UI.Slider loadingBar;
    
    [Header("Audio")]
    public AudioManager audioManager;
    
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneTransitionManager>();
                
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneTransitionManager");
                    instance = go.AddComponent<SceneTransitionManager>();
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
        if (audioManager == null)
            audioManager = AudioManager.Instance;
            
        // Hide loading screen at start
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
    
    /// <summary>
    /// Return to main menu scene
    /// </summary>
    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadMainMenuScene());
    }
    
    /// <summary>
    /// Restart current scene
    /// </summary>
    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneCoroutine(currentScene));
    }
    
    /// <summary>
    /// Load specific scene by name
    /// </summary>
    /// <param name="sceneName">Name of scene to load</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    IEnumerator LoadMainMenuScene()
    {
        yield return StartCoroutine(LoadSceneCoroutine(mainMenuSceneName));
    }
    
    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Show loading screen
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        
        // Reset loading bar
        if (loadingBar != null)
            loadingBar.value = 0f;
        
        // Wait a frame
        yield return null;
        
        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Don't allow scene activation until loading is complete
        asyncLoad.allowSceneActivation = false;
        
        // Update loading bar while loading
        while (!asyncLoad.isDone)
        {
            // Loading progress goes from 0 to 0.9
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (loadingBar != null)
                loadingBar.value = progress;
            
            // Scene is ready to activate
            if (asyncLoad.progress >= 0.9f)
            {
                // Show 100% loading
                if (loadingBar != null)
                    loadingBar.value = 1f;
                    
                // Wait a moment
                yield return new WaitForSeconds(0.5f);
                
                // Activate the scene
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Hide loading screen after scene loads
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
}