using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public GameObject settingsPanel;
    public Button backFromSettingsButton;
    
    [Header("Camera References")]
    public CinemachineCamera menuCamera;
    public CinemachineCamera gameplayCamera;
    public CameraTransition cameraTransition;
    public float transitionDuration = 2f;
    
    [Header("Character Reference")]
    public CharacterController playerController;
    public GameObject player;
    
    [Header("Audio")]
    public AudioSource buttonClickSound;
    public AudioSource backgroundMusic;
    
    [Header("References")]
    public AudioManager audioManager;
    public GameManager gameManager;
    
    private bool isInMenu = true;
    private bool isTransitioning = false;
    
    void Start()
    {
        InitializeMainMenu();
        SetupButtonListeners();
        InitializeReferences();
        
        // Pastikan cursor visible di menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void InitializeReferences()
    {
        // Get references jika belum di-assign
        if (audioManager == null)
            audioManager = AudioManager.Instance;
            
        if (gameManager == null)
            gameManager = GameManager.Instance;
            
        if (cameraTransition == null)
            cameraTransition = FindObjectOfType<CameraTransition>();
            
        // Setup camera transition jika ada
        if (cameraTransition != null)
        {
            cameraTransition.SetCameras(menuCamera, gameplayCamera);
            cameraTransition.transitionDuration = transitionDuration;
        }
    }
    
    void InitializeMainMenu()
    {
        // Pastikan menu panel aktif
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        // Pastikan settings panel tidak aktif
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        // Set camera menu sebagai aktif
        if (menuCamera != null)
            menuCamera.enabled = true;
            
        // Set camera gameplay sebagai tidak aktif
        if (gameplayCamera != null)
            gameplayCamera.enabled = false;
            
        // Freeze character movement
        if (playerController != null)
        {
            playerController.FreezeCharacter();
        }
        else
        {
            FreezePlayer(true);
        }
        
        // Set game state
        isInMenu = true;
        Time.timeScale = 1f;
    }
    
    void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(() => StartCoroutine(StartGame()));
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
            
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(CloseSettings);
    }
    
    public IEnumerator StartGame()
    {
        if (isTransitioning) yield break;
        
        isTransitioning = true;
        
        // Play button click sound dan transition sound
        PlayButtonSound();
        if (audioManager != null)
            audioManager.PlayTransition();
        
        // Disable main menu UI
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        // Start camera transition
        if (cameraTransition != null)
        {
            cameraTransition.StartTransitionWithCallback(() => {
                OnTransitionComplete();
            });
            
            // Wait for transition to complete
            yield return new WaitUntil(() => !cameraTransition.IsTransitioning);
        }
        else
        {
            // Fallback to manual transition
            yield return StartCoroutine(TransitionToGameplay());
        }
    }
    
    void OnTransitionComplete()
    {
        // Enable player movement
        if (playerController != null)
        {
            playerController.UnfreezeCharacter();
        }
        else
        {
            FreezePlayer(false);
        }
        
        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Change music to gameplay
        if (audioManager != null)
            audioManager.PlayGameplayMusic();
            
        // Notify game manager
        if (gameManager != null)
            gameManager.StartGame();
        
        // Set game state
        isInMenu = false;
        isTransitioning = false;
    }
    
    IEnumerator TransitionToGameplay()
    {
        // Smooth transition between cameras
        if (menuCamera != null && gameplayCamera != null)
        {
            float elapsedTime = 0f;
            
            // Get starting positions and rotations
            Vector3 startPos = menuCamera.transform.position;
            Quaternion startRot = menuCamera.transform.rotation;
            
            Vector3 endPos = gameplayCamera.transform.position;
            Quaternion endRot = gameplayCamera.transform.rotation;
            
            while (elapsedTime < transitionDuration)
            {
                float progress = elapsedTime / transitionDuration;
                progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth curve
                
                // Interpolate position and rotation
                menuCamera.transform.position = Vector3.Lerp(startPos, endPos, progress);
                menuCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, progress);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Ensure final position is exact
            menuCamera.transform.position = endPos;
            menuCamera.transform.rotation = endRot;
            
            // Switch to gameplay camera
            menuCamera.enabled = false;
            gameplayCamera.enabled = true;
        }
    }
    
    public void OpenSettings()
    {
        PlayButtonSound();
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    public void CloseSettings()
    {
        PlayButtonSound();
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
    
    public void ExitGame()
    {
        PlayButtonSound();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void FreezePlayer(bool freeze)
    {
        if (playerController != null)
        {
            playerController.enabled = !freeze;
        }
        
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (freeze)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
        }
    }
    
    void PlayButtonSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }
        else if (buttonClickSound != null)
        {
            buttonClickSound.Play();
        }
    }
    
    // Method untuk kembali ke main menu (bisa dipanggil dari pause menu)
    public void ReturnToMainMenu()
    {
        StartCoroutine(TransitionToMainMenu());
    }
    
    IEnumerator TransitionToMainMenu()
    {
        if (isTransitioning) yield break;
        
        isTransitioning = true;
        
        // Freeze player
        FreezePlayer(true);
        
        // Transition camera back to menu
        if (gameplayCamera != null && menuCamera != null)
        {
            float elapsedTime = 0f;
            
            Vector3 startPos = gameplayCamera.transform.position;
            Quaternion startRot = gameplayCamera.transform.rotation;
            
            Vector3 endPos = menuCamera.transform.position;
            Quaternion endRot = menuCamera.transform.rotation;
            
            // Switch to menu camera for transition
            gameplayCamera.enabled = false;
            menuCamera.enabled = true;
            
            while (elapsedTime < transitionDuration)
            {
                float progress = elapsedTime / transitionDuration;
                progress = Mathf.SmoothStep(0f, 1f, progress);
                
                menuCamera.transform.position = Vector3.Lerp(startPos, endPos, progress);
                menuCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, progress);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        // Show main menu UI
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        isInMenu = true;
        isTransitioning = false;
    }
    
    void Update()
    {
        // ESC key untuk pause/menu (hanya jika tidak sedang transisi dan tidak di menu)
        if (Input.GetKeyDown(KeyCode.Escape) && !isTransitioning && !isInMenu)
        {
            // Ini bisa dikembangkan untuk pause menu
            ReturnToMainMenu();
        }
    }
}