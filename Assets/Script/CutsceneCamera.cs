using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Camera))]
public class CutsceneCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public bool setAsMainCameraDuringCutscene = true;
    public CameraClearFlags clearFlags = CameraClearFlags.Skybox;
    public Color backgroundColor = Color.black;
    public int cullingMask = -1;
    
    [Header("Animation")]
    public bool animateFieldOfView = false;
    public AnimationCurve fovCurve = AnimationCurve.Linear(0, 60, 1, 60);
    public float animationDuration = 5f;
    
    [Header("Look At Target")]
    public Transform lookAtTarget;
    public bool smoothLookAt = true;
    public float lookAtSpeed = 2f;
    
    [Header("Audio")]
    public AudioListener audioListener;
    
    private Camera cam;
    private float originalFOV;
    private bool isAnimating = false;
    private float animationTimer = 0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;
        
        // Setup audio listener
        if (audioListener == null)
        {
            audioListener = GetComponent<AudioListener>();
            if (audioListener == null)
            {
                audioListener = gameObject.AddComponent<AudioListener>();
            }
        }
        
        // Disable by default
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetupCamera();
        
        if (animateFieldOfView)
        {
            StartFOVAnimation();
        }
    }

    private void OnDisable()
    {
        StopFOVAnimation();
    }

    private void Update()
    {
        HandleLookAt();
        HandleFOVAnimation();
    }

    private void SetupCamera()
    {
        if (cam == null) return;
        
        cam.clearFlags = clearFlags;
        cam.backgroundColor = backgroundColor;
        cam.cullingMask = cullingMask;
        
        if (setAsMainCameraDuringCutscene)
        {
            // Disable other main cameras
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                if (camera != cam && camera.CompareTag("MainCamera"))
                {
                    camera.enabled = false;
                }
            }
        }
        
        // Enable audio listener
        if (audioListener != null)
        {
            // Disable other audio listeners
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener != audioListener)
                {
                    listener.enabled = false;
                }
            }
            audioListener.enabled = true;
        }
    }

    private void HandleLookAt()
    {
        if (lookAtTarget == null) return;
        
        Vector3 targetDirection = lookAtTarget.position - transform.position;
        
        if (smoothLookAt)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAtSpeed * Time.deltaTime);
        }
        else
        {
            transform.LookAt(lookAtTarget);
        }
    }

    private void HandleFOVAnimation()
    {
        if (!isAnimating || !animateFieldOfView) return;
        
        animationTimer += Time.deltaTime;
        float progress = animationTimer / animationDuration;
        
        if (progress >= 1f)
        {
            progress = 1f;
            isAnimating = false;
        }
        
        float fov = fovCurve.Evaluate(progress);
        cam.fieldOfView = fov;
    }

    public void StartFOVAnimation()
    {
        if (!animateFieldOfView) return;
        
        isAnimating = true;
        animationTimer = 0f;
    }

    public void StopFOVAnimation()
    {
        isAnimating = false;
        animationTimer = 0f;
        
        if (cam != null)
        {
            cam.fieldOfView = originalFOV;
        }
    }

    public void SetLookAtTarget(Transform target)
    {
        lookAtTarget = target;
    }

    public void SetFOV(float fov)
    {
        if (cam != null)
        {
            cam.fieldOfView = fov;
        }
    }

    // Method to be called from Timeline
    public void SetCameraSettings(float fov, Vector3 position, Vector3 rotation)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        SetFOV(fov);
    }

    // Methods for Timeline Animation Tracks
    public void OnTimelinePlay()
    {
        Debug.Log($"Cutscene camera '{name}' timeline started");
    }

    public void OnTimelineStop()
    {
        Debug.Log($"Cutscene camera '{name}' timeline stopped");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw camera frustum
        if (cam != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            float fov = cam.fieldOfView * Mathf.Deg2Rad;
            float near = cam.nearClipPlane;
            float far = Mathf.Min(cam.farClipPlane, 100f); // Limit far plane for gizmo
            
            // Draw near and far planes
            float nearHeight = 2f * Mathf.Tan(fov * 0.5f) * near;
            float nearWidth = nearHeight * cam.aspect;
            
            float farHeight = 2f * Mathf.Tan(fov * 0.5f) * far;
            float farWidth = farHeight * cam.aspect;
            
            // Near plane
            Vector3[] nearCorners = {
                new Vector3(-nearWidth * 0.5f, -nearHeight * 0.5f, near),
                new Vector3(nearWidth * 0.5f, -nearHeight * 0.5f, near),
                new Vector3(nearWidth * 0.5f, nearHeight * 0.5f, near),
                new Vector3(-nearWidth * 0.5f, nearHeight * 0.5f, near)
            };
            
            // Far plane
            Vector3[] farCorners = {
                new Vector3(-farWidth * 0.5f, -farHeight * 0.5f, far),
                new Vector3(farWidth * 0.5f, -farHeight * 0.5f, far),
                new Vector3(farWidth * 0.5f, farHeight * 0.5f, far),
                new Vector3(-farWidth * 0.5f, farHeight * 0.5f, far)
            };
            
            // Draw frustum
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                
                // Near plane edges
                Gizmos.DrawLine(nearCorners[i], nearCorners[next]);
                
                // Far plane edges
                Gizmos.DrawLine(farCorners[i], farCorners[next]);
                
                // Connecting lines
                Gizmos.DrawLine(nearCorners[i], farCorners[i]);
            }
        }
        
        // Draw look at target connection
        if (lookAtTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, lookAtTarget.position);
            Gizmos.DrawWireSphere(lookAtTarget.position, 0.5f);
        }
    }
}