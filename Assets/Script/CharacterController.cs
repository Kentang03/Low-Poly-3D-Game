using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;
    public float mouseSensitivity = 2f;
    
    [Header("Camera Settings")]
    public Camera fpsCamera;
    public Camera thirdPersonCamera;
    public Transform thirdPersonTarget;
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
    private bool isFirstPerson = true;
    private Vector3 thirdPersonOffset;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Initialize cameras
        InitializeCameras();
        
        // Calculate third person offset
        thirdPersonOffset = new Vector3(0, thirdPersonHeight, -thirdPersonDistance);
        
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        
        // Freeze rotation on rigidbody to prevent tipping over
        rb.freezeRotation = true;
    }
    
    void Update()
    {
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
        
        // Calculate movement direction relative to where player is looking
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        
        // Apply movement
        Vector3 moveVelocity = direction * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
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
            // Third Person Mode - Only rotate the body horizontally
            transform.Rotate(Vector3.up * mouseX);
            
            // Camera follows smoothly
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -30f, 60f);
        }
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
            fpsCamera = transform.Find("FPSCamera")?.GetComponent<Camera>();
        
        if (thirdPersonCamera == null)
            thirdPersonCamera = transform.Find("ThirdPersonCamera")?.GetComponent<Camera>();
            
        if (thirdPersonTarget == null)
            thirdPersonTarget = transform.Find("CameraTarget");
        
        // Set initial camera state
        SwitchToFPS();
    }
    
    void HandleCameraSwitch()
    {
        // Switch camera with 'C' key
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isFirstPerson)
            {
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
    }
    
    void SwitchToThirdPerson()
    {
        isFirstPerson = false;
        
        if (fpsCamera != null)
            fpsCamera.enabled = false;
            
        if (thirdPersonCamera != null)
            thirdPersonCamera.enabled = true;
    }
    
    void UpdateThirdPersonCamera()
    {
        if (thirdPersonCamera == null) return;
        
        // Calculate desired position
        Vector3 targetPosition = transform.position + transform.TransformDirection(thirdPersonOffset);
        
        // Check for obstacles between player and camera
        RaycastHit hit;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            // Adjust camera position to avoid clipping through walls
            targetPosition = hit.point - direction * 0.2f;
        }
        
        // Smooth camera movement
        thirdPersonCamera.transform.position = Vector3.Lerp(
            thirdPersonCamera.transform.position,
            targetPosition,
            Time.deltaTime * cameraTransitionSpeed
        );
        
        // Look at target
        Vector3 lookTarget = thirdPersonTarget != null ? thirdPersonTarget.position : transform.position + Vector3.up * 1.5f;
        thirdPersonCamera.transform.LookAt(lookTarget);
        
        // Apply vertical rotation based on mouse input
        thirdPersonCamera.transform.RotateAround(transform.position, transform.right, xRotation);
    }
}
