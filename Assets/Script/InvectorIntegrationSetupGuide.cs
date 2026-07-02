using UnityEngine;

/// <summary>
/// Setup Guide and Helper for Integrating Invector Controller with Existing Game Systems
/// 
/// SETUP INSTRUCTIONS:
/// 1. Replace the old CharacterController.cs with this integration system
/// 2. Set up the player GameObject with Invector components
/// 3. Configure the camera systems
/// 4. Update UI references
/// 5. Test all systems
/// </summary>
public class InvectorIntegrationSetupGuide : MonoBehaviour
{
    [Header("Setup Status")]
    public bool invectorControllerSetup = false;
    public bool cameraSystemSetup = false;
    public bool menuIntegrationSetup = false;
    public bool audioSystemSetup = false;
    public bool collectionSystemSetup = false;
    
    [Header("Required Components Check")]
    public GameObject playerObject;
    public InvectorControllerAdapter invectorAdapter;
    public CustomInvectorInput customInput;
    public MainMenuManager mainMenuManager;
    public GameManager gameManager;
    public PauseMenuManager pauseMenuManager;
    public SceneTransitionManager sceneTransitionManager;
    
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string setupInstructions = @"
INVECTOR INTEGRATION SETUP GUIDE (UPDATED FOR SCENE SYSTEM):

1. PLAYER SETUP:
   - Remove old CharacterController.cs from player GameObject
   - Add vThirdPersonController, vThirdPersonMotor, vThirdPersonAnimator
   - Add CustomInvectorInput (instead of vThirdPersonInput)
   - Add InvectorControllerAdapter
   - Ensure player has 'Player' tag
   - Set up Rigidbody and CapsuleCollider

2. CAMERA SETUP (NEW SCENE SYSTEM):
   - Main Menu Scene: Static camera (auto-setup)
   - Gameplay Scene: Use vThirdPersonCamera for player camera
   - No need for IntegratedCameraManager (removed)
   - Camera transitions handled by scene loading

3. SCENE SETUP:
   - Create separate MainMenu.unity scene
   - Update Build Settings with both scenes
   - Add SceneTransitionManager to gameplay scene
   - Configure scene names in scripts

4. MENU INTEGRATION:
   - MainMenuManager loads gameplay scene
   - PauseMenuManager uses SceneTransitionManager
   - No more camera transitions within same scene
   - All UI references should work as before

5. AUDIO & COLLECTION:
   - Existing audio systems work without changes
   - Collection system uses tags, works automatically
   - Test item collection with new controller

6. TESTING:
   - Test menu scene to gameplay scene transition
   - Test pause menu return to main menu
   - Test character movement and camera
   - Test item collection and audio systems
";
    
    void Start()
    {
        PerformSetupCheck();
    }
    
    [ContextMenu("Perform Setup Check")]
    public void PerformSetupCheck()
    {
        Debug.Log("=== INVECTOR INTEGRATION SETUP CHECK ===");
        
        CheckInvectorController();
        CheckCameraSystem();
        CheckMenuIntegration();
        CheckAudioSystem();
        CheckCollectionSystem();
        
        PrintSetupStatus();
    }
    
    void CheckInvectorController()
    {
        invectorControllerSetup = false;
        
        if (playerObject != null)
        {
            var controller = playerObject.GetComponent<Invector.vCharacterController.vThirdPersonController>();
            var input = playerObject.GetComponent<CustomInvectorInput>();
            var adapter = playerObject.GetComponent<InvectorControllerAdapter>();
            
            if (controller != null && input != null && adapter != null)
            {
                invectorControllerSetup = true;
                Debug.Log("✓ Invector Controller setup complete");
            }
            else
            {
                Debug.LogWarning("✗ Invector Controller setup incomplete:");
                if (controller == null) Debug.LogWarning("  - Missing vThirdPersonController");
                if (input == null) Debug.LogWarning("  - Missing CustomInvectorInput");
                if (adapter == null) Debug.LogWarning("  - Missing InvectorControllerAdapter");
            }
        }
        else
        {
            Debug.LogWarning("✗ Player GameObject not assigned");
        }
    }
    
    void CheckCameraSystem()
    {
        cameraSystemSetup = false;
        
        // Check for vThirdPersonCamera in gameplay scene
        var invectorCam = FindObjectOfType<vThirdPersonCamera>();
        
        if (invectorCam != null)
        {
            cameraSystemSetup = true;
            Debug.Log("✓ Camera system setup complete - vThirdPersonCamera found");
        }
        else
        {
            Debug.LogWarning("✗ Camera system setup incomplete:");
            Debug.LogWarning("  - Missing vThirdPersonCamera (should be in gameplay scene)");
            Debug.Log("  - Note: Main menu uses static camera, gameplay uses vThirdPersonCamera");
        }
    }
    
    void CheckMenuIntegration()
    {
        menuIntegrationSetup = false;
        
        if (gameManager != null && pauseMenuManager != null)
        {
            // Check if managers have proper references for new scene system
            var gameManagerHasAdapter = gameManager.invectorAdapter != null;
            var pauseMenuHasTransition = pauseMenuManager.sceneTransitionManager != null;
            
            // Check for scene transition manager
            var sceneTransition = FindObjectOfType<SceneTransitionManager>();
            
            if (gameManagerHasAdapter && pauseMenuHasTransition && sceneTransition != null)
            {
                menuIntegrationSetup = true;
                Debug.Log("✓ Menu integration setup complete (scene system)");
            }
            else
            {
                Debug.LogWarning("✗ Menu integration setup incomplete:");
                if (!gameManagerHasAdapter) Debug.LogWarning("  - GameManager missing invectorAdapter reference");
                if (!pauseMenuHasTransition) Debug.LogWarning("  - PauseMenuManager missing sceneTransitionManager reference");
                if (sceneTransition == null) Debug.LogWarning("  - SceneTransitionManager not found in scene");
            }
        }
        else
        {
            Debug.LogWarning("✗ Menu managers not found or assigned");
        }
    }
    
    void CheckAudioSystem()
    {
        audioSystemSetup = false;
        
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioSystemSetup = true;
            Debug.Log("✓ Audio system found - should work without changes");
        }
        else
        {
            Debug.LogWarning("✗ AudioManager not found");
        }
    }
    
    void CheckCollectionSystem()
    {
        collectionSystemSetup = false;
        
        var collectionManager = FindObjectOfType<ItemCollectionManager>();
        var collectibles = FindObjectsOfType<CollectibleItem>();
        
        if (collectionManager != null && collectibles.Length > 0)
        {
            collectionSystemSetup = true;
            Debug.Log($"✓ Collection system found - {collectibles.Length} collectible items");
        }
        else
        {
            Debug.LogWarning("✗ Collection system not complete:");
            if (collectionManager == null) Debug.LogWarning("  - Missing ItemCollectionManager");
            if (collectibles.Length == 0) Debug.LogWarning("  - No CollectibleItem objects found");
        }
    }
    
    void PrintSetupStatus()
    {
        Debug.Log("\n=== SETUP STATUS ===");
        Debug.Log($"Invector Controller: {(invectorControllerSetup ? "✓" : "✗")}");
        Debug.Log($"Camera System: {(cameraSystemSetup ? "✓" : "✗")}");
        Debug.Log($"Menu Integration: {(menuIntegrationSetup ? "✓" : "✗")}");
        Debug.Log($"Audio System: {(audioSystemSetup ? "✓" : "✗")}");
        Debug.Log($"Collection System: {(collectionSystemSetup ? "✓" : "✗")}");
        
        bool allComplete = invectorControllerSetup && cameraSystemSetup && 
                          menuIntegrationSetup && audioSystemSetup && collectionSystemSetup;
        
        if (allComplete)
        {
            Debug.Log("\n🎉 ALL SYSTEMS READY! Integration complete.");
        }
        else
        {
            Debug.Log("\n⚠️ Some systems need attention. Check warnings above.");
        }
    }
    
    [ContextMenu("Auto Setup References")]
    public void AutoSetupReferences()
    {
        Debug.Log("Auto-setting up references for new scene system...");
        
        // Find and assign components
        if (invectorAdapter == null)
            invectorAdapter = FindObjectOfType<InvectorControllerAdapter>();
            
        if (customInput == null)
            customInput = FindObjectOfType<CustomInvectorInput>();
            
        if (mainMenuManager == null)
            mainMenuManager = FindObjectOfType<MainMenuManager>();
            
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
            
        if (pauseMenuManager == null)
            pauseMenuManager = FindObjectOfType<PauseMenuManager>();
            
        if (sceneTransitionManager == null)
            sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();
        
        Debug.Log("Auto-setup complete. Run 'Perform Setup Check' to verify.");
        Debug.Log("Note: Some components may only be available in specific scenes (MainMenu vs Gameplay)");
    }
}