using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller untuk animasi kancil yang bekerja dengan KancilGuide
/// </summary>
[RequireComponent(typeof(Animator))]
public class KancilAnimationController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string isWalkingParameter = "IsWalking";
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string isIdleParameter = "IsIdle";
    [SerializeField] private string jumpTrigger = "Jump";
    [SerializeField] private string lookAroundTrigger = "LookAround";
    
    [Header("Animation Settings")]
    [SerializeField] private float animationSmoothTime = 0.1f;
    [SerializeField] private bool useSpeedBlending = true;
    [SerializeField] private float maxAnimationSpeed = 5f;
    
    [Header("Behavior Settings")]
    [SerializeField] private bool randomIdleAnimations = true;
    [SerializeField] private float idleAnimationInterval = 5f;
    [SerializeField] private float lookAroundChance = 0.3f;
    
    // Components
    private Animator animator;
    private KancilGuide kancilGuide;
    
    // Animation state
    private bool isCurrentlyWalking = false;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private Coroutine idleAnimationCoroutine;
    
    // Animation parameter hashes (untuk performa)
    private int isWalkingHash;
    private int speedHash;
    private int isIdleHash;
    private int jumpHash;
    private int lookAroundHash;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        kancilGuide = GetComponent<KancilGuide>();
        
        // Cache parameter hashes
        CacheAnimationHashes();
    }
    
    private void Start()
    {
        // Subscribe to KancilGuide events
        if (kancilGuide != null)
        {
            kancilGuide.OnStateChanged += OnKancilStateChanged;
        }
        
        // Start idle animation routine
        if (randomIdleAnimations)
        {
            StartIdleAnimationRoutine();
        }
        
        // Set initial animation state
        SetAnimationState(false, 0f);
    }
    
    private void Update()
    {
        UpdateAnimationSpeed();
    }
    
    private void CacheAnimationHashes()
    {
        isWalkingHash = Animator.StringToHash(isWalkingParameter);
        speedHash = Animator.StringToHash(speedParameter);
        isIdleHash = Animator.StringToHash(isIdleParameter);
        jumpHash = Animator.StringToHash(jumpTrigger);
        lookAroundHash = Animator.StringToHash(lookAroundTrigger);
    }
    
    private void OnKancilStateChanged(KancilGuide.KancilState newState)
    {
        switch (newState)
        {
            case KancilGuide.KancilState.Idle:
                SetAnimationState(false, 0f);
                StartIdleAnimationRoutine();
                break;
                
            case KancilGuide.KancilState.MovingToCheckpoint:
            case KancilGuide.KancilState.ReturningToPlayer:
                SetAnimationState(true, GetMovementSpeed());
                StopIdleAnimationRoutine();
                break;
                
            case KancilGuide.KancilState.WaitingAtCheckpoint:
                SetAnimationState(false, 0f);
                TriggerLookAround();
                StartIdleAnimationRoutine();
                break;
                
            case KancilGuide.KancilState.WaitingForPlayer:
                SetAnimationState(false, 0f);
                TriggerLookAround();
                StartIdleAnimationRoutine();
                break;
        }
    }
    
    private void UpdateAnimationSpeed()
    {
        if (useSpeedBlending && isCurrentlyWalking)
        {
            float actualSpeed = GetMovementSpeed();
            targetSpeed = actualSpeed;
        }
        
        // Smooth speed transition
        if (Mathf.Abs(currentSpeed - targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, animationSmoothTime * Time.deltaTime * 10f);
            
            if (HasParameter(speedHash))
            {
                animator.SetFloat(speedHash, currentSpeed / maxAnimationSpeed);
            }
        }
    }
    
    private float GetMovementSpeed()
    {
        if (kancilGuide != null)
        {
            UnityEngine.AI.NavMeshAgent agent = kancilGuide.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                return agent.velocity.magnitude;
            }
        }
        return 0f;
    }
    
    private void SetAnimationState(bool walking, float speed)
    {
        isCurrentlyWalking = walking;
        targetSpeed = speed;
        
        // Set walking parameter
        if (HasParameter(isWalkingHash))
        {
            animator.SetBool(isWalkingHash, walking);
        }
        
        // Set idle parameter (opposite of walking)
        if (HasParameter(isIdleHash))
        {
            animator.SetBool(isIdleHash, !walking);
        }
        
        // Set initial speed
        if (HasParameter(speedHash))
        {
            animator.SetFloat(speedHash, speed / maxAnimationSpeed);
        }
    }
    
    private void TriggerLookAround()
    {
        if (randomIdleAnimations && Random.value < lookAroundChance)
        {
            if (HasParameter(lookAroundHash))
            {
                animator.SetTrigger(lookAroundHash);
            }
        }
    }
    
    private void StartIdleAnimationRoutine()
    {
        if (idleAnimationCoroutine == null && randomIdleAnimations)
        {
            idleAnimationCoroutine = StartCoroutine(IdleAnimationRoutine());
        }
    }
    
    private void StopIdleAnimationRoutine()
    {
        if (idleAnimationCoroutine != null)
        {
            StopCoroutine(idleAnimationCoroutine);
            idleAnimationCoroutine = null;
        }
    }
    
    private IEnumerator IdleAnimationRoutine()
    {
        while (!isCurrentlyWalking)
        {
            yield return new WaitForSeconds(idleAnimationInterval + Random.Range(-1f, 2f));
            
            if (!isCurrentlyWalking)
            {
                // Random idle animation
                if (Random.value < lookAroundChance)
                {
                    TriggerLookAround();
                }
            }
        }
        
        idleAnimationCoroutine = null;
    }
    
    // Public methods untuk trigger animasi khusus
    public void TriggerJump()
    {
        if (HasParameter(jumpHash))
        {
            animator.SetTrigger(jumpHash);
        }
    }
    
    public void TriggerCustomAnimation(string triggerName)
    {
        int hash = Animator.StringToHash(triggerName);
        if (HasParameter(hash))
        {
            animator.SetTrigger(hash);
        }
    }
    
    public void SetCustomBool(string paramName, bool value)
    {
        int hash = Animator.StringToHash(paramName);
        if (HasParameter(hash))
        {
            animator.SetBool(hash, value);
        }
    }
    
    public void SetCustomFloat(string paramName, float value)
    {
        int hash = Animator.StringToHash(paramName);
        if (HasParameter(hash))
        {
            animator.SetFloat(hash, value);
        }
    }
    
    // Helper method untuk mengecek apakah parameter ada
    private bool HasParameter(int hash)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.nameHash == hash)
                return true;
        }
        return false;
    }
    
    private bool HasParameter(string name)
    {
        return HasParameter(Animator.StringToHash(name));
    }
    
    // Getters untuk debugging
    public bool IsWalking() => isCurrentlyWalking;
    public float GetCurrentSpeed() => currentSpeed;
    public float GetTargetSpeed() => targetSpeed;
    
    // Event handlers untuk animasi events (dipanggil dari Animation Events)
    public void OnWalkAnimationStart()
    {
        Debug.Log("Kancil walk animation started");
    }
    
    public void OnWalkAnimationEnd()
    {
        Debug.Log("Kancil walk animation ended");
    }
    
    public void OnIdleAnimationStart()
    {
        Debug.Log("Kancil idle animation started");
    }
    
    public void OnFootstep()
    {
        // Bisa ditambahkan sound effect footstep di sini
        Debug.Log("Kancil footstep");
    }
    
    private void OnDestroy()
    {
        // Unsubscribe dari events
        if (kancilGuide != null)
        {
            kancilGuide.OnStateChanged -= OnKancilStateChanged;
        }
        
        StopIdleAnimationRoutine();
    }
    
    // Debug info untuk inspector
    [System.Serializable]
    public class AnimationDebugInfo
    {
        public bool isWalking;
        public float currentSpeed;
        public float targetSpeed;
        public bool hasAnimator;
        public bool hasController;
        public string currentStateName;
    }
    
    public AnimationDebugInfo GetDebugInfo()
    {
        AnimationDebugInfo info = new AnimationDebugInfo();
        info.isWalking = isCurrentlyWalking;
        info.currentSpeed = currentSpeed;
        info.targetSpeed = targetSpeed;
        info.hasAnimator = animator != null;
        info.hasController = animator != null && animator.runtimeAnimatorController != null;
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            info.currentStateName = "Layer0: " + stateInfo.shortNameHash.ToString();
        }
        else
        {
            info.currentStateName = "No controller";
        }
        
        return info;
    }
}