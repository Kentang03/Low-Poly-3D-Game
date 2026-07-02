using UnityEngine;
using UnityEngine.UI;

public class StoneSpawnerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private StoneSpawner stoneSpawner;
    [SerializeField] private Slider forceSlider;
    [SerializeField] private Slider angleSlider;
    [SerializeField] private Slider lifetimeSlider;
    [SerializeField] private Button spawnButton;
    [SerializeField] private Button autoSpawnToggle;
    [SerializeField] private Text forceValueText;
    [SerializeField] private Text angleValueText;
    [SerializeField] private Text lifetimeValueText;
    [SerializeField] private Text autoSpawnStatusText;
    
    [Header("Direction Controls")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    [Header("Settings")]
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 30f;
    [SerializeField] private float minLifetime = 1f;
    [SerializeField] private float maxLifetime = 30f;
    [SerializeField] private bool showUI = true;
    
    private bool isAutoSpawnActive = false;

    void Start()
    {
        if (stoneSpawner == null)
            stoneSpawner = FindObjectOfType<StoneSpawner>();
            
        SetupUI();
        UpdateUI();
    }

    void SetupUI()
    {
        // Setup sliders
        if (forceSlider != null)
        {
            forceSlider.minValue = minForce;
            forceSlider.maxValue = maxForce;
            forceSlider.value = 15f; // Default value
            forceSlider.onValueChanged.AddListener(OnForceChanged);
        }
        
        if (angleSlider != null)
        {
            angleSlider.minValue = 0f;
            angleSlider.maxValue = 90f;
            angleSlider.value = 45f; // Default value
            angleSlider.onValueChanged.AddListener(OnAngleChanged);
        }
        
        if (lifetimeSlider != null)
        {
            lifetimeSlider.minValue = minLifetime;
            lifetimeSlider.maxValue = maxLifetime;
            lifetimeSlider.value = 10f; // Default value
            lifetimeSlider.onValueChanged.AddListener(OnLifetimeChanged);
        }
        
        // Setup buttons
        if (spawnButton != null)
            spawnButton.onClick.AddListener(OnSpawnButtonClicked);
            
        if (autoSpawnToggle != null)
            autoSpawnToggle.onClick.AddListener(OnAutoSpawnToggle);
        
        // Direction buttons
        if (forwardButton != null)
            forwardButton.onClick.AddListener(() => SetDirection(Vector3.forward));
        if (backButton != null)
            backButton.onClick.AddListener(() => SetDirection(Vector3.back));
        if (leftButton != null)
            leftButton.onClick.AddListener(() => SetDirection(Vector3.left));
        if (rightButton != null)
            rightButton.onClick.AddListener(() => SetDirection(Vector3.right));
    }

    void Update()
    {
        UpdateUI();
        
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Space))
            OnSpawnButtonClicked();
            
        if (Input.GetKeyDown(KeyCode.A))
            OnAutoSpawnToggle();
    }

    void UpdateUI()
    {
        if (forceValueText != null && forceSlider != null)
            forceValueText.text = $"Force: {forceSlider.value:F1}";
            
        if (angleValueText != null && angleSlider != null)
            angleValueText.text = $"Angle: {angleSlider.value:F0}°";
            
        if (lifetimeValueText != null && lifetimeSlider != null)
            lifetimeValueText.text = $"Lifetime: {lifetimeSlider.value:F1}s";
            
        if (autoSpawnStatusText != null)
            autoSpawnStatusText.text = isAutoSpawnActive ? "Auto: ON" : "Auto: OFF";
    }

    void OnForceChanged(float value)
    {
        if (stoneSpawner != null)
            stoneSpawner.SetThrowForce(value);
    }

    void OnAngleChanged(float value)
    {
        if (stoneSpawner != null)
            stoneSpawner.SetThrowAngle(value);
    }

    void OnLifetimeChanged(float value)
    {
        if (stoneSpawner != null)
            stoneSpawner.SetStoneLifetime(value);
    }

    void OnSpawnButtonClicked()
    {
        if (stoneSpawner != null)
            stoneSpawner.SpawnStone();
    }

    void OnAutoSpawnToggle()
    {
        if (stoneSpawner != null)
        {
            stoneSpawner.ToggleAutoSpawn();
            isAutoSpawnActive = !isAutoSpawnActive;
        }
    }

    void SetDirection(Vector3 direction)
    {
        if (stoneSpawner != null)
            stoneSpawner.SetThrowDirection(direction);
    }

    void OnGUI()
    {
        if (!showUI) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Stone Spawner Controls", GUI.skin.GetStyle("label"));
        GUILayout.Space(10);
        
        if (GUILayout.Button("Spawn Stone (SPACE)"))
            OnSpawnButtonClicked();
            
        if (GUILayout.Button(isAutoSpawnActive ? "Stop Auto Spawn (A)" : "Start Auto Spawn (A)"))
            OnAutoSpawnToggle();
        
        GUILayout.Space(10);
        GUILayout.Label("Direction Controls:");
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Forward")) SetDirection(Vector3.forward);
        if (GUILayout.Button("Back")) SetDirection(Vector3.back);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Left")) SetDirection(Vector3.left);
        if (GUILayout.Button("Right")) SetDirection(Vector3.right);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        if (stoneSpawner != null)
        {
            GUILayout.Label($"Force: {(forceSlider?.value ?? 15f):F1}");
            float newForce = GUILayout.HorizontalSlider(forceSlider?.value ?? 15f, minForce, maxForce);
            if (forceSlider != null) forceSlider.value = newForce;
            
            GUILayout.Label($"Angle: {(angleSlider?.value ?? 45f):F0}°");
            float newAngle = GUILayout.HorizontalSlider(angleSlider?.value ?? 45f, 0f, 90f);
            if (angleSlider != null) angleSlider.value = newAngle;
            
            GUILayout.Label($"Lifetime: {(lifetimeSlider?.value ?? 10f):F1}s");
            float newLifetime = GUILayout.HorizontalSlider(lifetimeSlider?.value ?? 10f, minLifetime, maxLifetime);
            if (lifetimeSlider != null) lifetimeSlider.value = newLifetime;
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public void ToggleUI()
    {
        showUI = !showUI;
    }
}