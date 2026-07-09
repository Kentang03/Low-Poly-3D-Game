using UnityEngine;
using Unity.Cinemachine;
public class CharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;
    public float mouseSensitivity = 2f;
    
    [Header("Camera Settings")]
    public CinemachineCamera fpsCamera;
    public CinemachineCamera thirdPersonCamera;
    public CinemachineCamera freeLookCamera;  // New Cinemachine Camera with Orbital Follow
    public float thirdPersonDistance = 5f;
    public float thirdPersonHeight = 2f;
    public float cameraTransitionSpeed = 5f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private Rigidbody rb;
    private bool isGrounded;
    private float xRotation = 0f;
    public bool isFirstPerson = false;
    private Vector3 thirdPersonOffset;

    public GameObject playerModel;
    
    [Header("Game State")]
    public bool canMove = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Initialize cameras
        InitializeCameras();
        
        // Calculate third person offset
        thirdPersonOffset = new Vector3(0, thirdPersonHeight, -thirdPersonDistance);
        
        // JANGAN lock cursor di Start - akan di-handle oleh MainMenuManager
        // Cursor akan di-lock hanya saat gameplay dimulai
        
        // Freeze rotation on rigidbody to prevent tipping over
        rb.freezeRotation = true;
        
        isFirstPerson = false;
    }
    
    void Update()
    {
        // Jika tidak bisa bergerak (sedang di menu), skip semua input
        if (!canMove) return;
        
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Handle camera switching
        HandleCameraSwitch();
        
        // Handle input
        HandleMovement();
        HandleMouseLook();
        HandleJump();
        
        // Update third person camera position
        if (!isFirstPerson)
        {
            UpdateThirdPersonCamera();
        }
    }
    
    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Determine if running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Get camera direction for relative movement
        Vector3 cameraForward = GetCameraForward();
        Vector3 cameraRight = GetCameraRight();
        
        // Calculate movement direction relative to camera
        Vector3 direction = (cameraRight * horizontal + cameraForward * vertical).normalized;
        
        // Apply movement
        Vector3 moveVelocity = direction * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
        
        // Rotate player to face movement direction (optional)
        if (direction.magnitude > 0.1f && !isFirstPerson)
        {
            // Smoothly rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerModel.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    
    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        if (isFirstPerson)
        {
            // FPS Mode - Rotate the body horizontally
            transform.Rotate(Vector3.up * mouseX);
            
            // Rotate the camera vertically
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            fpsCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            // Third Person Mode - FreeLook camera handles mouse input automatically
            // No manual mouse handling needed for FreeLook camera
            
            // Optional: Still rotate character based on camera direction during movement
            // This is now handled in HandleMovement()
        }
    }
    
    // Helper methods to get camera direction
    Vector3 GetCameraForward()
    {
        if (isFirstPerson && fpsCamera != null)
        {
            return fpsCamera.transform.forward;
        }
        else if (freeLookCamera != null)
        {
            // For new Cinemachine, use the camera transform directly
            // The OrbitalFollow component handles the orbital movement
            Vector3 forward = freeLookCamera.transform.forward;
            forward.y = 0; // Remove vertical component for ground movement
            return forward.normalized;
        }
        else if (thirdPersonCamera != null)
        {
            Vector3 forward = thirdPersonCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }
        
        // Fallback to world forward
        return Vector3.forward;
    }
    
    Vector3 GetCameraRight()
    {
        if (isFirstPerson && fpsCamera != null)
        {
            return fpsCamera.transform.right;
        }
        else if (freeLookCamera != null)
        {
            // For new Cinemachine, use the camera transform directly
            Vector3 right = freeLookCamera.transform.right;
            right.y = 0; // Keep it horizontal
            return right.normalized;
        }
        else if (thirdPersonCamera != null)
        {
            Vector3 right = thirdPersonCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }
        
        // Fallback to world right
        return Vector3.right;
    }
    
    // Optional: Get camera input values for more precise control
    Vector2 GetCameraInput()
    {
        if (freeLookCamera != null)
        {
            // Get input from Input Axis Controller if available
            var inputAxisController = freeLookCamera.GetComponent<CinemachineInputAxisController>();
            if (inputAxisController != null)
            {
                // Access input values if needed for advanced camera control
                return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
        }
        return Vector2.zero;
    }
    
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }
    
    void InitializeCameras()
    {
        // If cameras are not assigned, try to find them
        if (fpsCamera == null)
            fpsCamera = transform.Find("FPCamera")?.GetComponent<CinemachineCamera>();
        
        if (thirdPersonCamera == null)
            thirdPersonCamera = transform.Find("TPCamera")?.GetComponent<CinemachineCamera>();
            
        // Manual assignment is recommended for FreeLook camera
        // Auto-detection as fallback
        if (freeLookCamera == null)
        {
            // Find FreeLook camera by name or tag
            GameObject freeLookObj = GameObject.Find("FreeLook Camera");
            if (freeLookObj != null)
                freeLookCamera = freeLookObj.GetComponent<CinemachineCamera>();
                
            // Or find by CinemachineOrbitalFollow component
            if (freeLookCamera == null)
            {
                CinemachineOrbitalFollow orbitalFollow = FindObjectOfType<CinemachineOrbitalFollow>();
                if (orbitalFollow != null)
                    freeLookCamera = orbitalFollow.GetComponent<CinemachineCamera>();
            }
        }
        
        // Set initial camera state - prioritize FreeLook if available
        if (freeLookCamera != null)
        {
            SwitchToFreeLook();
        }
        else
        {
            SwitchToThirdPerson();
        }
    }
    
    void HandleCameraSwitch()
    {
        // Switch camera with 'C' key
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isFirstPerson)
            {
                if (freeLookCamera != null)
                    SwitchToFreeLook();
                else
                    SwitchToThirdPerson();
            }
            else
            {
                SwitchToFPS();
            }
        }
    }
    
    void SwitchToFPS()
    {
        isFirstPerson = true;
        
        if (fpsCamera != null)
            fpsCamera.enabled = true;
            
        if (thirdPersonCamera != null)
            thirdPersonCamera.enabled = false;
            
        if (freeLookCamera != null)
            freeLookCamera.enabled = false;
    }
    
    void SwitchToThirdPerson()
    {
        isFirstPerson = false;
        
        if (fpsCamera != null)
            fpsCamera.enabled = false;
            
        if (thirdPersonCamera != null)
            thirdPersonCamera.enabled = true;
            
        if (freeLookCamera != null)
            freeLookCamera.enabled = false;
    }
    
    void SwitchToFreeLook()
    {
        isFirstPerson = false;
        
        if (fpsCamera != null)
            fpsCamera.enabled = false;
            
        if (thirdPersonCamera != null)
            thirdPersonCamera.enabled = false;
            
        if (freeLookCamera != null)
            freeLookCamera.enabled = true;
    }
    
    void UpdateThirdPersonCamera()
    {
        return; 
    }

    
    // Public methods untuk mengontrol movement dari MainMenuManager
    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
        
        if (!canMove)
        {
            // Stop semua movement
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    public void FreezeCharacter()
    {
        SetCanMove(false);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        // Unlock cursor saat character di-freeze (menu state)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void UnfreezeCharacter()
    {
        SetCanMove(true);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        // Lock cursor saat character bisa bergerak (gameplay state)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Method untuk mengontrol cursor secara manual
    public void SetCursorLocked(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}