using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraTransition : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineCamera fromCamera;
    public CinemachineCamera toCamera;
    public float transitionDuration = 2f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Transition Effects")]
    public bool useFadeEffect = false;
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 0.5f;
    
    private bool isTransitioning = false;
    
    public bool IsTransitioning => isTransitioning;
    
    public void StartTransition()
    {
        if (fromCamera != null && toCamera != null && !isTransitioning)
        {
            StartCoroutine(TransitionBetweenCameras());
        }
    }
    
    public void StartTransitionWithCallback(System.Action onComplete)
    {
        if (fromCamera != null && toCamera != null && !isTransitioning)
        {
            StartCoroutine(TransitionBetweenCamerasWithCallback(onComplete));
        }
    }
    
    IEnumerator TransitionBetweenCameras()
    {
        yield return StartCoroutine(TransitionBetweenCamerasWithCallback(null));
    }
    
    IEnumerator TransitionBetweenCamerasWithCallback(System.Action onComplete)
    {
        isTransitioning = true;
        
        // Fade out jika menggunakan fade effect
        if (useFadeEffect && fadeCanvas != null)
        {
            yield return StartCoroutine(FadeOut());
        }
        
        // Get starting position and rotation
        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;
        
        // Get target position and rotation
        Vector3 targetPosition = toCamera.transform.position;
        Quaternion targetRotation = toCamera.transform.rotation;
        
        // Perform smooth transition
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            float progress = elapsedTime / transitionDuration;
            float curveProgress = transitionCurve.Evaluate(progress);
            
            // Interpolate position and rotation
            fromCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, curveProgress);
            fromCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, curveProgress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure exact final position
        fromCamera.transform.position = targetPosition;
        fromCamera.transform.rotation = targetRotation;
        
        // Switch to target camera
        fromCamera.enabled = false;
        toCamera.enabled = true;
        
        // Fade in jika menggunakan fade effect
        if (useFadeEffect && fadeCanvas != null)
        {
            yield return StartCoroutine(FadeIn());
        }
        
        isTransitioning = false;
        
        // Call completion callback
        onComplete?.Invoke();
    }
    
    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startAlpha = fadeCanvas.alpha;
        
        while (elapsedTime < fadeDuration)
        {
            float progress = elapsedTime / fadeDuration;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, 1f, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        fadeCanvas.alpha = 1f;
    }
    
    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float startAlpha = fadeCanvas.alpha;
        
        while (elapsedTime < fadeDuration)
        {
            float progress = elapsedTime / fadeDuration;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, 0f, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        fadeCanvas.alpha = 0f;
    }
    
    // Method untuk set camera references secara dinamis
    public void SetCameras(CinemachineCamera from, CinemachineCamera to)
    {
        fromCamera = from;
        toCamera = to;
    }
    
    // Method untuk instant switch tanpa transition
    public void InstantSwitch()
    {
        if (fromCamera != null && toCamera != null)
        {
            fromCamera.enabled = false;
            toCamera.enabled = true;
        }
    }
}