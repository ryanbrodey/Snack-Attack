using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.2f; // Realistic jump height
    public float gravity = -15f; // Adjusted gravity for better feel
    public float groundCheckDistance = 0.2f;
    
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
        
        if (characterController == null)
        {
            enabled = false;
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }
        
        if (armsAnimator == null)
        {
            Transform arms = transform.Find("PistolArms");
            if (arms == null)
                arms = transform.Find("RifleArms");
            if (arms == null)
                arms = transform.Find("Arms");
            
            if (arms != null)
                armsAnimator = arms.GetComponent<Animator>();
            
            if (armsAnimator == null)
            {
                Animator[] animators = GetComponentsInChildren<Animator>();
                if (animators.Length > 0)
                    armsAnimator = animators[0];
            }
        }
        
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                Transform[] children = GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.name == "GroundCheck")
                    {
                        groundCheck = child;
                        break;
                    }
                }
            }
            
            if (groundCheck == null)
            {
                GameObject gc = new GameObject("GroundCheck");
                gc.transform.SetParent(transform);
                
                float groundCheckY = characterController.center.y - (characterController.height / 2f);
                gc.transform.localPosition = new Vector3(0, groundCheckY, 0);
                groundCheck = gc.transform;
            }
        }
        
        velocity = Vector3.zero;
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            float currentTime = Time.time;
            
            if (autoRunning)
            {
                autoRunning = false;
                wKeyPressCount = 0;
                return;
            }
            
            if (currentTime - lastWKeyTime <= doubleClickTime)
            {
                wKeyPressCount++;
                if (wKeyPressCount >= 2)
                {
                    autoRunning = true;
                    wKeyPressCount = 0;
                }
            }
            else
            {
                wKeyPressCount = 1;
            }
            
            lastWKeyTime = currentTime;
        }
        
        moveInput = Vector2.zero;
        
        if (autoRunning)
        {
            moveInput.y = 1f;
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            if (Mathf.Abs(horizontal) < 0.2f) horizontal = 0f;
            if (Mathf.Abs(vertical) < 0.2f) vertical = 0f;
            
            if (Input.GetKey(KeyCode.W)) vertical = Mathf.Max(vertical, 1f);
            if (Input.GetKey(KeyCode.S)) vertical = Mathf.Min(vertical, -1f);
            if (Input.GetKey(KeyCode.A)) horizontal = Mathf.Min(horizontal, -1f);
            if (Input.GetKey(KeyCode.D)) horizontal = Mathf.Max(horizontal, 1f);
            
            moveInput.x = horizontal;
            moveInput.y = vertical;
        }
        
        isRunning = Input.GetKey(KeyCode.LeftShift) || autoRunning;
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
    }
    
    void HandleMovement()
    {
        wasGroundedLastFrame = isGrounded;
        
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        }
        else
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                characterController.height / 2 + 0.1f, groundMask);
        }
        
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f);
        
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);
        
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -50f);
        }
        else
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
                
                if (isJumping)
                {
                    isJumping = false;
                }
            }
        }
        
        if (jumpPressed && isGrounded && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
        }
        
        if (!isGrounded || velocity.y > 0)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        characterController.Move(velocity * Time.deltaTime);
    }
    
    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        transform.Rotate(Vector3.up * mouseX);
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    void UpdateAnimations()
    {
        if (armsAnimator == null) return;
        
        float speed = moveInput.magnitude;
        armsAnimator.SetFloat("MoveSpeed", speed);
        armsAnimator.SetBool("IsRunning", isRunning && speed > 0.1f);
        armsAnimator.SetBool("IsJumping", isJumping);
    }
    

    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => velocity;
    public bool IsMoving => moveInput.magnitude > 0.1f;
    public bool IsRunning => isRunning;
    public bool IsAutoRunning => autoRunning;
    public bool IsJumping => isJumping;
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        }
    }
}
