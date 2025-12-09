using UnityEngine;
using SnackAttack.Weapons;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerControllerWithWeapons : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -15f;
    public float groundCheckDistance = 0.2f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    
    [Header("Auto-Run Settings")]
    public float doubleClickTime = 0.3f;
    
    [Header("References")]
    public Camera playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask = 1;
    public Animator armsAnimator;
    
    [Header("Weapon System")]
    public BaseWeapon[] weapons;
    public Transform weaponHolder;
    public int currentWeaponIdx = 0;
    
    [Header("Weapon Controls")]
    public KeyCode semiAutoKey = KeyCode.F;
    public KeyCode fullAutoKey = KeyCode.G;
    public KeyCode[] weaponKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    
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
    
    // Weapon system
    private BaseWeapon currentWeapon;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (characterController == null)
        {
            Debug.LogError("FPSPlayerController requires a CharacterController component!");
            enabled = false;
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        
        // Auto-find camera
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
                playerCamera = Camera.main;
        }
        
        // Auto-find arms animator
        if (armsAnimator == null)
        {
            Transform arms = transform.Find("PistolArms");
            if (arms == null) arms = transform.Find("RifleArms");
            if (arms == null) arms = transform.Find("Arms");
            
            if (arms != null)
                armsAnimator = arms.GetComponent<Animator>();
        }
        
        // Create ground check
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
        }
        
        // Setup weapon system
        SetupWeapons();
        
        Debug.Log("FPS Player Controller with Weapons initialized!");
        Debug.Log("Controls: WASD=move, Double-tap W=auto-run, Space=jump, Mouse=look");
        Debug.Log("Weapons: 1,2,3=switch, F=semi-auto, G=full-auto, R=reload");
    }
    
    void Update()
    {
        HandleInput();
        HandleWeaponInput();
        HandleMovement();
        HandleMouseLook();
        UpdateAnimations();
        UpdateWeapons();
        
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
        
        // Get movement input
        moveInput = Vector2.zero;
        
        if (autoRunning)
        {
            moveInput.y = 1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) moveInput.y += 1f;
            if (Input.GetKey(KeyCode.S)) moveInput.y -= 1f;
            if (Input.GetKey(KeyCode.A)) moveInput.x -= 1f;
            if (Input.GetKey(KeyCode.D)) moveInput.x += 1f;
        }
        
        // Running detection
        isRunning = Input.GetKey(KeyCode.LeftShift) || autoRunning;
        
        // Jump input
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
    }
    
    void HandleWeaponInput()
    {
        // Semi-auto attack with F key
        if (Input.GetKeyDown(semiAutoKey))
        {
            DoAttack();
        }
        
        // Left mouse click for attack
        if (Input.GetButtonDown("Fire1"))
        {
            DoAttack();
        }
        
        // Number keys for weapon switching
        for (int i = 0; i < weaponKeys.Length && i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(weaponKeys[i]))
            {
                Debug.Log($"Switching to weapon {i + 1}");
                SwitchToWeapon(i);
                break;
            }
        }
    }
    
    void HandleMovement()
    {
        // Ground check
        wasGroundedLastFrame = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        
        // Landing detection
        if (isGrounded && !wasGroundedLastFrame)
        {
            isJumping = false;
        }
        
        // Jump
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            Debug.Log("Jump!");
        }
        
        // Movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);
        
        // Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        
        // Reset Y velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void UpdateAnimations()
    {
        if (armsAnimator == null) return;
        
        bool moving = moveInput.magnitude > 0.1f;
        
        armsAnimator.SetBool("IsWalking", moving && !isRunning);
        armsAnimator.SetBool("IsRunning", moving && isRunning);
        armsAnimator.SetBool("IsGrounded", isGrounded);
        armsAnimator.SetBool("IsJumping", isJumping);
    }
    
    // WEAPON SYSTEM METHODS
    void SetupWeapons()
    {
        if (weapons == null || weapons.Length == 0)
        {
            weapons = GetComponentsInChildren<BaseWeapon>(true);
        }
        
        // Create weapon holder if needed
        if (weaponHolder == null && playerCamera != null)
        {
            GameObject holder = new GameObject("WeaponHolder");
            holder.transform.SetParent(playerCamera.transform);
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;
            weaponHolder = holder.transform;
        }
        
        // Setup weapons
        foreach (BaseWeapon weapon in weapons)
        {
            if (weapon != null && weaponHolder != null)
            {
                if (weapon.transform != transform)
                {
                    weapon.transform.SetParent(weaponHolder);
                }
                weapon.gameObject.SetActive(false);
            }
        }
        
        // Switch to first weapon
        if (weapons.Length > 0)
        {
            SwitchToWeapon(currentWeaponIdx);
        }
    }
    
    public void SwitchToWeapon(int idx)
    {
        if (weapons == null || idx < 0 || idx >= weapons.Length || weapons[idx] == null)
        {
            Debug.LogWarning($"Invalid weapon index {idx}");
            return;
        }
        
        Debug.Log($"Switching to weapon: {weapons[idx].WeaponName}");
        
        // Deactivate current weapon
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        
        // Activate new weapon
        currentWeaponIdx = idx;
        currentWeapon = weapons[currentWeaponIdx];
        currentWeapon.gameObject.SetActive(true);
        
        Debug.Log($"Successfully switched to: {currentWeapon.WeaponName}");
    }
    
    public void DoAttack()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Attack();
        }
    }
    
    void UpdateWeapons()
    {
        if (currentWeapon == null) return;
        
        // Update weapon animations based on movement
        bool moving = moveInput.magnitude > 0.1f;
        currentWeapon.UpdateMovementAnimation(moving, isRunning);
    }
    
    // Public getters
    public BaseWeapon CurrentWeapon => currentWeapon;
    public int CurrentWeaponIndex => currentWeaponIdx;
    public bool IsGrounded => isGrounded;
    public Vector2 MoveInput => moveInput;
}
