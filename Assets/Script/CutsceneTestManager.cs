using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

public class CutsceneTestManager : MonoBehaviour
{
    [Header("Test UI")]
    public Button[] cutsceneButtons;
    public Text statusText;
    public Button skipButton;
    
    [Header("Test Settings")]
    public KeyCode testKey1 = KeyCode.Alpha1;
    public KeyCode testKey2 = KeyCode.Alpha2;
    public KeyCode testKey3 = KeyCode.Alpha3;
    public KeyCode skipKey = KeyCode.Escape;
    
    private CutsceneManager cutsceneManager;

    private void Start()
    {
        cutsceneManager = CutsceneManager.Instance;
        
        if (cutsceneManager == null)
        {
            Debug.LogError("CutsceneManager not found!");
            if (statusText != null)
                statusText.text = "ERROR: CutsceneManager not found!";
            return;
        }

        SetupUI();
        UpdateStatus();
    }

    private void SetupUI()
    {
        // Setup cutscene buttons
        if (cutsceneButtons != null && cutsceneButtons.Length > 0)
        {
            for (int i = 0; i < cutsceneButtons.Length; i++)
            {
                if (cutsceneButtons[i] != null)
                {
                    int index = i; // Capture for closure
                    cutsceneButtons[i].onClick.AddListener(() => PlayCutsceneByIndex(index));
                    
                    // Update button text
                    Text buttonText = cutsceneButtons[i].GetComponentInChildren<Text>();
                    if (buttonText != null && index < cutsceneManager.cutscenes.Count)
                    {
                        buttonText.text = $"Play: {cutsceneManager.cutscenes[index].cutsceneName}";
                    }
                }
            }
        }

        // Setup skip button
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCutscene);
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateStatus();
    }

    private void HandleInput()
    {
        if (cutsceneManager == null) return;

        // Test keys for playing cutscenes
        if (Input.GetKeyDown(testKey1))
        {
            PlayCutsceneByIndex(0);
        }
        else if (Input.GetKeyDown(testKey2))
        {
            PlayCutsceneByIndex(1);
        }
        else if (Input.GetKeyDown(testKey3))
        {
            PlayCutsceneByIndex(2);
        }

        // Skip key
        if (Input.GetKeyDown(skipKey))
        {
            SkipCutscene();
        }
    }

    private void PlayCutsceneByIndex(int index)
    {
        if (cutsceneManager == null || index >= cutsceneManager.cutscenes.Count)
        {
            Debug.LogWarning($"Cannot play cutscene at index {index}");
            return;
        }

        string cutsceneName = cutsceneManager.cutscenes[index].cutsceneName;
        Debug.Log($"Playing cutscene: {cutsceneName}");
        
        cutsceneManager.PlayCutscene(index, () => {
            Debug.Log($"Cutscene '{cutsceneName}' completed!");
        });
    }

    private void PlayCutsceneByName(string name)
    {
        if (cutsceneManager == null)
        {
            Debug.LogWarning("CutsceneManager not available");
            return;
        }

        Debug.Log($"Playing cutscene: {name}");
        cutsceneManager.PlayCutscene(name, () => {
            Debug.Log($"Cutscene '{name}' completed!");
        });
    }

    private void SkipCutscene()
    {
        if (cutsceneManager == null)
        {
            Debug.LogWarning("CutsceneManager not available");
            return;
        }

        if (cutsceneManager.IsPlayingCutscene())
        {
            Debug.Log("Skipping cutscene...");
            cutsceneManager.SkipCutscene();
        }
    }

    private void UpdateStatus()
    {
        if (statusText == null || cutsceneManager == null) return;

        string status = cutsceneManager.IsPlayingCutscene() ? "PLAYING CUTSCENE" : "Ready";
        int cutsceneCount = cutsceneManager.cutscenes.Count;
        
        statusText.text = $"Status: {status}\nCutscenes Available: {cutsceneCount}";
    }

    // Public methods for UI buttons or other scripts
    public void PlayFirstCutscene()
    {
        PlayCutsceneByIndex(0);
    }

    public void PlaySecondCutscene()
    {
        PlayCutsceneByIndex(1);
    }

    public void PlayThirdCutscene()
    {
        PlayCutsceneByIndex(2);
    }

    private void OnGUI()
    {
        if (cutsceneManager == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Cutscene Test Controls", GUI.skin.box);
        
        GUILayout.Space(10);
        
        for (int i = 0; i < cutsceneManager.cutscenes.Count && i < 3; i++)
        {
            KeyCode key = i == 0 ? testKey1 : i == 1 ? testKey2 : testKey3;
            string cutsceneName = cutsceneManager.cutscenes[i].cutsceneName;
            
            if (GUILayout.Button($"({key}) Play: {cutsceneName}"))
            {
                PlayCutsceneByIndex(i);
            }
        }
        
        GUILayout.Space(10);
        
        GUI.enabled = cutsceneManager.IsPlayingCutscene();
        if (GUILayout.Button($"({skipKey}) Skip Cutscene"))
        {
            SkipCutscene();
        }
        GUI.enabled = true;
        
        GUILayout.Space(10);
        
        string status = cutsceneManager.IsPlayingCutscene() ? "PLAYING" : "IDLE";
        GUILayout.Label($"Status: {status}");
        
        GUILayout.EndArea();
    }
}