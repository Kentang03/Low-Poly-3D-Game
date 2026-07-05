using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Default Settings")]
    public float defaultFadeDuration = 1f;
    public Color fadeColor = Color.black;

    public static FadeTransition Instance { get; private set; }

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

        // Setup fade image if not assigned
        if (fadeImage == null)
        {
            SetupFadeImage();
        }

        // Start with transparent
        SetFadeAlpha(0f);
    }

    private void SetupFadeImage()
    {
        // Create Canvas if doesn't exist
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // High sorting order to appear on top
        }

        // Add CanvasScaler
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // Create fade image
        GameObject fadeImageGO = new GameObject("FadeImage");
        fadeImageGO.transform.SetParent(transform);
        
        fadeImage = fadeImageGO.AddComponent<Image>();
        fadeImage.color = fadeColor;
        
        // Make it fullscreen
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Fade to black (or specified color)
    /// </summary>
    public IEnumerator FadeOut(float duration = -1f)
    {
        if (duration < 0) duration = defaultFadeDuration;
        
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float curveValue = fadeCurve.Evaluate(progress);
            float alpha = Mathf.Lerp(startAlpha, 1f, curveValue);
            
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(1f);
    }

    /// <summary>
    /// Fade from black (or specified color) to transparent
    /// </summary>
    public IEnumerator FadeIn(float duration = -1f)
    {
        if (duration < 0) duration = defaultFadeDuration;
        
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float curveValue = fadeCurve.Evaluate(progress);
            float alpha = Mathf.Lerp(startAlpha, 0f, curveValue);
            
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);
    }

    /// <summary>
    /// Set fade alpha directly
    /// </summary>
    public void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = Mathf.Clamp01(alpha);
            fadeImage.color = color;
        }
    }

    /// <summary>
    /// Set fade color
    /// </summary>
    public void SetFadeColor(Color color)
    {
        fadeColor = color;
        if (fadeImage != null)
        {
            Color currentColor = fadeImage.color;
            color.a = currentColor.a; // Preserve current alpha
            fadeImage.color = color;
        }
    }

    /// <summary>
    /// Instant fade to black
    /// </summary>
    public void FadeToBlackInstant()
    {
        SetFadeAlpha(1f);
    }

    /// <summary>
    /// Instant fade to clear
    /// </summary>
    public void FadeToClearInstant()
    {
        SetFadeAlpha(0f);
    }

    /// <summary>
    /// Cross fade between two actions
    /// </summary>
    public IEnumerator CrossFade(System.Action onMidpoint, float fadeOutDuration = -1f, float fadeInDuration = -1f)
    {
        yield return StartCoroutine(FadeOut(fadeOutDuration));
        onMidpoint?.Invoke();
        yield return StartCoroutine(FadeIn(fadeInDuration));
    }
}