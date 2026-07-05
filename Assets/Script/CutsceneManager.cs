using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
using System.Collections.Generic;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneData
    {
        public string cutsceneName;
        public PlayableDirector timeline;
        public Camera cutsceneCamera;
        public bool disablePlayerControl = true;
        public float fadeInDuration = 1f;
        public float fadeOutDuration = 1f;
    }

    [Header("Cutscene Settings")]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();
    
    [Header("References")]
    public Camera mainCamera;
    public FadeTransition fadeTransition;
    public MonoBehaviour playerController; // Assign your player controller here
    
    [Header("Audio")]
    public AudioSource cutsceneAudioSource;
    
    private CutsceneData currentCutscene;
    private bool isPlayingCutscene = false;
    private System.Action onCutsceneComplete;

    public static CutsceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure fade transition is available
        if (fadeTransition == null)
        {
            fadeTransition = FindObjectOfType<FadeTransition>();
        }

        // Setup timeline callbacks for all cutscenes
        foreach (var cutscene in cutscenes)
        {
            if (cutscene.timeline != null)
            {
                cutscene.timeline.stopped += OnTimelineComplete;
            }
        }
    }

    /// <summary>
    /// Play a cutscene by name
    /// </summary>
    public void PlayCutscene(string cutsceneName, System.Action onComplete = null)
    {
        if (isPlayingCutscene)
        {
            Debug.LogWarning("Already playing a cutscene!");
            return;
        }

        var cutscene = cutscenes.Find(c => c.cutsceneName == cutsceneName);
        if (cutscene == null)
        {
            Debug.LogError($"Cutscene '{cutsceneName}' not found!");
            return;
        }

        StartCoroutine(PlayCutsceneCoroutine(cutscene, onComplete));
    }

    /// <summary>
    /// Play a cutscene by index
    /// </summary>
    public void PlayCutscene(int index, System.Action onComplete = null)
    {
        if (index < 0 || index >= cutscenes.Count)
        {
            Debug.LogError($"Cutscene index {index} out of range!");
            return;
        }

        StartCoroutine(PlayCutsceneCoroutine(cutscenes[index], onComplete));
    }

    private IEnumerator PlayCutsceneCoroutine(CutsceneData cutscene, System.Action onComplete)
    {
        isPlayingCutscene = true;
        currentCutscene = cutscene;
        onCutsceneComplete = onComplete;

        // Disable player control
        if (cutscene.disablePlayerControl && playerController != null)
        {
            playerController.enabled = false;
        }

        // Fade out (to black)
        if (fadeTransition != null)
        {
            yield return StartCoroutine(fadeTransition.FadeOut(cutscene.fadeOutDuration));
        }

        // Switch cameras
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }

        if (cutscene.cutsceneCamera != null)
        {
            cutscene.cutsceneCamera.gameObject.SetActive(true);
        }

        // Fade in
        if (fadeTransition != null)
        {
            yield return StartCoroutine(fadeTransition.FadeIn(cutscene.fadeInDuration));
        }

        // Play timeline
        if (cutscene.timeline != null)
        {
            cutscene.timeline.Play();
        }
        else
        {
            // If no timeline, end cutscene immediately
            OnTimelineComplete(null);
        }
    }

    private void OnTimelineComplete(PlayableDirector director)
    {
        if (currentCutscene == null || !isPlayingCutscene) return;

        StartCoroutine(EndCutsceneCoroutine());
    }

    private IEnumerator EndCutsceneCoroutine()
    {
        // Fade out
        if (fadeTransition != null)
        {
            yield return StartCoroutine(fadeTransition.FadeOut(currentCutscene.fadeOutDuration));
        }

        // Switch back to main camera
        if (currentCutscene.cutsceneCamera != null)
        {
            currentCutscene.cutsceneCamera.gameObject.SetActive(false);
        }

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }

        // Fade in
        if (fadeTransition != null)
        {
            yield return StartCoroutine(fadeTransition.FadeIn(currentCutscene.fadeInDuration));
        }

        // Re-enable player control
        if (currentCutscene.disablePlayerControl && playerController != null)
        {
            playerController.enabled = true;
        }

        // Call completion callback
        onCutsceneComplete?.Invoke();

        // Cleanup
        currentCutscene = null;
        isPlayingCutscene = false;
        onCutsceneComplete = null;
    }

    /// <summary>
    /// Skip current cutscene (optional feature)
    /// </summary>
    public void SkipCutscene()
    {
        if (!isPlayingCutscene || currentCutscene == null) return;

        if (currentCutscene.timeline != null)
        {
            currentCutscene.timeline.Stop();
        }

        OnTimelineComplete(null);
    }

    /// <summary>
    /// Check if cutscene is currently playing
    /// </summary>
    public bool IsPlayingCutscene()
    {
        return isPlayingCutscene;
    }

    private void OnDestroy()
    {
        // Cleanup timeline callbacks
        foreach (var cutscene in cutscenes)
        {
            if (cutscene.timeline != null)
            {
                cutscene.timeline.stopped -= OnTimelineComplete;
            }
        }
    }
}