using UnityEngine;

/// <summary>
/// Script to help setup MainMenu scene with proper camera and lighting
/// </summary>
public class MainMenuSceneSetup : MonoBehaviour
{
    [Header("Main Menu Setup")]
    [Tooltip("Position of the main menu camera")]
    public Vector3 menuCameraPosition = new Vector3(0, 2, -5);
    
    [Tooltip("Rotation of the main menu camera")]
    public Vector3 menuCameraRotation = new Vector3(10, 0, 0);
    
    [Tooltip("Field of view for menu camera")]
    public float menuCameraFOV = 60f;
    
    [Header("Background")]
    public GameObject backgroundPrefab;
    public Material skyboxMaterial;
    
    [Header("Lighting")]
    public Light mainLight;
    public Color ambientLight = new Color(0.2f, 0.2f, 0.3f);
    
    void Start()
    {
        SetupMenuCamera();
        SetupLighting();
        SetupBackground();
    }
    
    void SetupMenuCamera()
    {
        Camera menuCamera = Camera.main;
        if (menuCamera == null)
        {
            // Create main camera if not exists
            GameObject cameraGO = new GameObject("Main Camera");
            menuCamera = cameraGO.AddComponent<Camera>();
            cameraGO.tag = "MainCamera";
            
            // Add audio listener
            cameraGO.AddComponent<AudioListener>();
        }
        
        // Set camera position and rotation
        menuCamera.transform.position = menuCameraPosition;
        menuCamera.transform.eulerAngles = menuCameraRotation;
        
        // Set camera properties
        menuCamera.fieldOfView = menuCameraFOV;
        menuCamera.clearFlags = CameraClearFlags.Skybox;
        
        // Set camera for UI
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = menuCamera;
        }
    }
    
    void SetupLighting()
    {
        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLight;
        
        // Setup main directional light
        if (mainLight == null)
        {
            // Try to find existing light
            mainLight = FindObjectOfType<Light>();
            
            if (mainLight == null)
            {
                // Create new directional light
                GameObject lightGO = new GameObject("Directional Light");
                mainLight = lightGO.AddComponent<Light>();
            }
        }
        
        if (mainLight != null)
        {
            mainLight.type = LightType.Directional;
            mainLight.intensity = 1.0f;
            mainLight.color = Color.white;
            mainLight.shadows = LightShadows.Soft;
            
            // Position the light
            mainLight.transform.rotation = Quaternion.Euler(30, 30, 0);
        }
        
        // Set skybox if available
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
    }
    
    void SetupBackground()
    {
        if (backgroundPrefab != null)
        {
            // Instantiate background prefab
            GameObject background = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity);
            background.name = "Menu Background";
        }
    }
}