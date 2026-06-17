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
    private bool isFirstPerson = false;
    private Vector3 thirdPersonOffset;
    
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
            fpsCamera = transform.Find("FPCamera")?.GetComponent<CinemachineCamera>();
        
        if (thirdPersonCamera == null)
            thirdPersonCamera = transform.Find("TPCamera")?.GetComponent<CinemachineCamera>();
        
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
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
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