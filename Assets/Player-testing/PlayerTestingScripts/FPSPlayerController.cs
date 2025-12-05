using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.5f; // Realistic jump height
    public float gravity = -19.62f; // Realistic gravity (2x Earth gravity for better feel)
    public float groundCheckDistance = 0.4f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    
    [Header("Auto-Run Settings")]
    public float doubleClickTime = 0.3f; // Time window for double-click detection
    
    [Header("References")]
    public Camera playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask = 1;
    public Animator armsAnimator; // Reference to PistolArms animator
    
    // Private variables
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private float xRotation = 0f;
    
    // Auto-run system
    private bool autoRunning = false;
    private float lastWKeyTime = 0f;
    private int wKeyPressCount = 0;
    
    // Movement state tracking
    private Vector2 moveInput;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool jumpPressed = false;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        
        // Auto-find references if not assigned
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        
        if (armsAnimator == null)
        {
            Transform pistolArms = transform.Find("PistolArms");
            if (pistolArms != null)
                armsAnimator = pistolArms.GetComponent<Animator>();
        }
        
        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = gc.transform;
        }
        
        Debug.Log("FPS Player Controller initialized. Controls: WASD to move, Double-tap W for auto-run, Space to jump, Mouse to look around.");
    }
    
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleMouseLook();
        UpdateAnimations();
        
        // Debug info and cursor toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? 
                CursorLockMode.None : CursorLockMode.Locked;
        }
    }
    
    void HandleInput()
    {
        // Handle W key double-click for auto-run
        if (Input.GetKeyDown(KeyCode.W))
        {
            float currentTime = Time.time;
            
            // If we're auto-running and W is pressed, stop auto-run
            if (autoRunning)
            {
                autoRunning = false;
                wKeyPressCount = 0;
                Debug.Log("Auto-run: OFF (manual W press)");
                return;
            }
            
            // Check for double-click
            if (currentTime - lastWKeyTime <= doubleClickTime)
            {
                wKeyPressCount++;
                if (wKeyPressCount >= 2)
                {
                    // Activate auto-run
                    autoRunning = true;
                    wKeyPressCount = 0;
                    Debug.Log("Auto-run: ON");
                }
            }
            else
            {
                wKeyPressCount = 1;
            }
            
            lastWKeyTime = currentTime;
        }
        
        // Get movement input
        moveInput = Vector2.zero;
        
        if (autoRunning)
        {
            moveInput.y = 1f; // Always move forward when auto-running
        }
        else
        {
            // WASD input
            if (Input.GetKey(KeyCode.W)) moveInput.y += 1f;
            if (Input.GetKey(KeyCode.S)) moveInput.y -= 1f;
            if (Input.GetKey(KeyCode.A)) moveInput.x -= 1f;
            if (Input.GetKey(KeyCode.D)) moveInput.x += 1f;
        }
        
        // Running detection (Shift key or auto-run)
        isRunning = Input.GetKey(KeyCode.LeftShift) || autoRunning;
        
        // Jump input (Space bar)
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
    }
    
    void HandleMovement()
    {
        // Store previous grounded state
        wasGroundedLastFrame = isGrounded;
        
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        
        // Reset vertical velocity when grounded and falling
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to stay grounded
            
            // Reset jumping state when we land
            if (isJumping)
            {
                isJumping = false;
                Debug.Log("Landed");
            }
        }
        
        // Calculate movement direction relative to player rotation
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f); // Prevent faster diagonal movement
        
        // Apply speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);
        
        // Jumping - only allow when grounded and not already jumping
        if (jumpPressed && isGrounded && !isJumping)
        {
            // Calculate jump velocity using physics formula: v = sqrt(2 * g * h)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            Debug.Log($"Jump! Initial velocity: {velocity.y:F2}");
        }
        else if (jumpPressed && !isGrounded)
        {
            Debug.Log("Cannot jump - not grounded");
        }
        else if (jumpPressed && isJumping)
        {
            Debug.Log("Cannot jump - already jumping");
        }
        
        // Apply gravity continuously
        velocity.y += gravity * Time.deltaTime;
        
        // Apply vertical movement
        characterController.Move(velocity * Time.deltaTime);
        
        // Debug ground state changes
        if (wasGroundedLastFrame != isGrounded)
        {
            Debug.Log($"Ground state changed: {(isGrounded ? "Grounded" : "Airborne")}");
        }
    }
    
    void HandleMouseLook()
    {
        // Only handle mouse look if cursor is locked
        if (Cursor.lockState != CursorLockMode.Locked) return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate the player body left/right
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate the camera up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    void UpdateAnimations()
    {
        if (armsAnimator == null) return;
        
        // Calculate movement speed for animation
        float speed = moveInput.magnitude;
        armsAnimator.SetFloat("MoveSpeed", speed);
        armsAnimator.SetBool("IsRunning", isRunning && speed > 0.1f);
        armsAnimator.SetBool("IsJumping", isJumping);
        
        // Debug animation state
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log($"Animation State - Speed: {speed:F2}, Running: {isRunning && speed > 0.1f}, Jumping: {isJumping}");
        }
    }
    
    // Public getters for other scripts
    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => velocity;
    public bool IsMoving => moveInput.magnitude > 0.1f;
    public bool IsRunning => isRunning;
    public bool IsAutoRunning => autoRunning;
    public bool IsJumping => isJumping;
    
    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        }
    }
}
