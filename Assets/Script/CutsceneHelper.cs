using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class untuk operasi cutscene yang umum digunakan
/// </summary>
public static class CutsceneHelper
{
    /// <summary>
    /// Play cutscene dengan callback
    /// </summary>
    public static void PlayCutscene(string cutsceneName, System.Action onComplete = null)
    {
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.PlayCutscene(cutsceneName, onComplete);
        }
        else
        {
            Debug.LogError("CutsceneManager not found in scene!");
        }
    }

    /// <summary>
    /// Skip cutscene yang sedang berjalan
    /// </summary>
    public static void SkipCurrentCutscene()
    {
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.SkipCutscene();
        }
    }

    /// <summary>
    /// Cek apakah cutscene sedang berjalan
    /// </summary>
    public static bool IsCutscenePlaying()
    {
        return CutsceneManager.Instance != null && CutsceneManager.Instance.IsPlayingCutscene();
    }

    /// <summary>
    /// Fade out manual
    /// </summary>
    public static void FadeOut(float duration = 1f, System.Action onComplete = null)
    {
        if (FadeTransition.Instance != null)
        {
            var fadeTransition = FadeTransition.Instance;
            fadeTransition.StartCoroutine(FadeOutCoroutine(duration, onComplete));
        }
    }

    /// <summary>
    /// Fade in manual
    /// </summary>
    public static void FadeIn(float duration = 1f, System.Action onComplete = null)
    {
        if (FadeTransition.Instance != null)
        {
            var fadeTransition = FadeTransition.Instance;
            fadeTransition.StartCoroutine(FadeInCoroutine(duration, onComplete));
        }
    }

    private static IEnumerator FadeOutCoroutine(float duration, System.Action onComplete)
    {
        yield return FadeTransition.Instance.StartCoroutine(FadeTransition.Instance.FadeOut(duration));
        onComplete?.Invoke();
    }

    private static IEnumerator FadeInCoroutine(float duration, System.Action onComplete)
    {
        yield return FadeTransition.Instance.StartCoroutine(FadeTransition.Instance.FadeIn(duration));
        onComplete?.Invoke();
    }

    /// <summary>
    /// Setup cutscene system secara otomatis
    /// </summary>
    public static bool SetupCutsceneSystem()
    {
        try
        {
            var installer = Object.FindObjectOfType<CutsceneSystemInstaller>();
            if (installer == null)
            {
                GameObject installerGO = new GameObject("CutsceneSystemInstaller");
                installer = installerGO.AddComponent<CutsceneSystemInstaller>();
            }

            installer.SetupCutsceneSystem();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to setup cutscene system: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Validate cutscene system setup
    /// </summary>
    public static bool ValidateCutsceneSystem()
    {
        bool isValid = true;
        
        if (CutsceneManager.Instance == null)
        {
            Debug.LogWarning("CutsceneManager not found in scene");
            isValid = false;
        }

        if (FadeTransition.Instance == null)
        {
            Debug.LogWarning("FadeTransition not found in scene");
            isValid = false;
        }

        var questSystem = Object.FindObjectOfType<QuestSystem>();
        if (questSystem == null)
        {
            Debug.LogWarning("QuestSystem not found - quest integration will not work");
        }

        return isValid;
    }

    /// <summary>
    /// Get all available cutscenes
    /// </summary>
    public static string[] GetAvailableCutscenes()
    {
        if (CutsceneManager.Instance == null)
            return new string[0];

        var cutscenes = CutsceneManager.Instance.cutscenes;
        string[] names = new string[cutscenes.Count];
        
        for (int i = 0; i < cutscenes.Count; i++)
        {
            names[i] = cutscenes[i].cutsceneName;
        }

        return names;
    }

    /// <summary>
    /// Quick setup untuk testing
    /// </summary>
    [System.Obsolete("Use CutsceneQuestSetupWizard.SetupComplete() instead")]
    public static void QuickSetup()
    {
        Debug.Log("Setting up cutscene system...");
        
        if (SetupCutsceneSystem())
        {
            Debug.Log("Cutscene system setup complete!");
        }
        else
        {
            Debug.LogError("Failed to setup cutscene system!");
        }
    }
}

/// <summary>
/// Component version dari CutsceneHelper untuk akses di Inspector
/// </summary>
public class CutsceneHelperComponent : MonoBehaviour
{
    [Header("Quick Actions")]
    public string cutsceneToPlay = "TestCutscene";
    
    [Header("Fade Settings")]
    public float fadeOutDuration = 1f;
    public float fadeInDuration = 1f;

    [ContextMenu("Play Cutscene")]
    public void PlayCutscene()
    {
        CutsceneHelper.PlayCutscene(cutsceneToPlay);
    }

    [ContextMenu("Skip Current Cutscene")]
    public void SkipCutscene()
    {
        CutsceneHelper.SkipCurrentCutscene();
    }

    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        CutsceneHelper.FadeOut(fadeOutDuration);
    }

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        CutsceneHelper.FadeIn(fadeInDuration);
    }

    [ContextMenu("Setup Cutscene System")]
    public void SetupSystem()
    {
        CutsceneHelper.SetupCutsceneSystem();
    }

    [ContextMenu("Validate Setup")]
    public void ValidateSystem()
    {
        bool isValid = CutsceneHelper.ValidateCutsceneSystem();
        Debug.Log($"Cutscene system validation: {(isValid ? "PASS" : "FAIL")}");
    }

    [ContextMenu("List Available Cutscenes")]
    public void ListCutscenes()
    {
        string[] cutscenes = CutsceneHelper.GetAvailableCutscenes();
        if (cutscenes.Length > 0)
        {
            Debug.Log($"Available cutscenes: {string.Join(", ", cutscenes)}");
        }
        else
        {
            Debug.Log("No cutscenes found");
        }
    }

    // Public methods untuk UI buttons
    public void PlayCutsceneByName(string name)
    {
        CutsceneHelper.PlayCutscene(name);
    }

    public void SkipCurrentCutscene()
    {
        CutsceneHelper.SkipCurrentCutscene();
    }
}