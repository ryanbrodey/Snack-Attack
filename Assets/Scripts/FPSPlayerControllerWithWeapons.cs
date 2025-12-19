using UnityEngine;
using SnackAttack.Weapons;

[System.Serializable]
public class WeaponConfigData
{
    public string weaponName;
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
    public RuntimeAnimatorController animatorController;
    public BaseWeapon weaponScript;
}

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
    public Transform cameraAnchor;
    
    [Header("Arm Models - Assign the arm GameObjects")]
    public GameObject pistolArmsModel;
    public GameObject rifleArmsModel;
    public GameObject shotgunArmsModel;
    
    [Header("UI")]
    public bool enableCrosshair = true;
    
    [Header("Weapon System")]
    public BaseWeapon[] weapons;
    public int currentWeaponIdx = 0;
    
    [Header("Weapon Controls")]
    public KeyCode semiAutoKey = KeyCode.F;
    public KeyCode fullAutoKey = KeyCode.G;
    public KeyCode[] weaponKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    
    [Header("Weapon Unlocking")]
    public bool pistolUnlocked = true;
    public bool rifleUnlocked = false;
    public bool shotgunUnlocked = false;
    public int pistolIndex = 0;
    public int rifleIndex = 1;
    public int shotgunIndex = 2;
    
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
    private Animator currentAnimator;
    
    // Crosshair system
    private SnackAttack.Player.CrosshairManager crosshairManager;
    
    // Weapon configuration data
    private WeaponConfigData[] weaponConfigs;
    
    // Arm model references
    private GameObject[] armModels;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (characterController == null)
        {
            enabled = false;
            return;
        }
        
        // Lock cursor for FPS controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Find camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
                playerCamera = Camera.main;
        }
        
        // Find camera anchor
        if (cameraAnchor == null)
        {
            cameraAnchor = transform.Find("CameraAnchor");
        }
        
        // Find arm models
        if (pistolArmsModel == null)
        {
            Transform pistolArms = transform.Find("PistolArms");
            if (pistolArms != null) pistolArmsModel = pistolArms.gameObject;
        }
        
        if (rifleArmsModel == null)
        {
            Transform rifleArms = transform.Find("RifleArms");
            if (rifleArms != null) rifleArmsModel = rifleArms.gameObject;
        }
        
        if (shotgunArmsModel == null)
        {
            Transform shotgunArms = transform.Find("ShotgunArms");
            if (shotgunArms != null) shotgunArmsModel = shotgunArms.gameObject;
        }
        
        // Store arm models for switching
        armModels = new GameObject[] { pistolArmsModel, rifleArmsModel, shotgunArmsModel };
        
        InitializeWeaponConfigurations();
        
        // Setup ground check
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
        }
        
        velocity = Vector3.zero;
        
        SetupWeapons();
    }
    
    void Update()
    {
        // Keep cursor locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        HandleInput();
        HandleWeaponInput();
        HandleMovement();
        HandleMouseLook();
        UpdateAnimations();
        UpdateWeapons();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    
    void HandleInput()
    {
        // Double-tap W for auto-run
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
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            // Dead zone to prevent drift
            if (Mathf.Abs(horizontal) < 0.2f) horizontal = 0f;
            if (Mathf.Abs(vertical) < 0.2f) vertical = 0f;
            
            // Check keys directly for better response
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
    
    void HandleWeaponInput()
    {
        // F key to attack
        if (Input.GetKeyDown(semiAutoKey))
        {
            DoAttack();
        }
        
        // Mouse click to attack
        if (Input.GetButtonDown("Fire1"))
        {
            DoAttack();
        }
        
        // Number keys switch weapons
        for (int i = 0; i < weaponKeys.Length && i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(weaponKeys[i]))
            {
                SwitchToWeapon(i);
                break;
            }
        }
    }
    
    void HandleMovement()
    {
        // Track if we were grounded last frame
        wasGroundedLastFrame = isGrounded;
        
        // Check if on ground (sphere check works better)
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        }
        else
        {
            // Fallback raycast
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                characterController.height / 2 + 0.1f, groundMask);
        }
        
        // Movement direction relative to where we're facing
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f);
        
        // Move horizontally
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);
        
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -50f);
        }
        else
        {
            // Small downward force to stay grounded
            if (velocity.y < 0)
            {
                velocity.y = -2f;
                
                // Stop jumping when we hit ground
                if (isJumping)
                {
                    isJumping = false;
                }
            }
        }
        
        // Jump when grounded
        if (jumpPressed && isGrounded && !isJumping)
        {
            // Physics formula for jump height
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
        }
        
        // Keep applying gravity while in air
        if (!isGrounded || velocity.y > 0)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        characterController.Move(velocity * Time.deltaTime);
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
        if (currentAnimator == null)
        {
            return;
        }
        
        bool moving = moveInput.magnitude > 0.1f;
        
        // Update animator based on movement
        if (HasAnimatorParameter("IsWalking"))
        {
            currentAnimator.SetBool("IsWalking", moving && !isRunning);
        }
        
        if (HasAnimatorParameter("IsRunning"))
        {
            currentAnimator.SetBool("IsRunning", moving && isRunning);
        }
        
        if (HasAnimatorParameter("IsGrounded"))
            currentAnimator.SetBool("IsGrounded", isGrounded);
        if (HasAnimatorParameter("IsJumping"))
            currentAnimator.SetBool("IsJumping", isJumping);
    }
    
    bool HasAnimatorParameter(string paramName)
    {
        if (currentAnimator == null || currentAnimator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter param in currentAnimator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
    
    void SetupWeapons()
    {
        if (weapons == null || weapons.Length == 0)
        {
            return;
        }
        
        // Start with all arms hidden
        if (pistolArmsModel != null) pistolArmsModel.SetActive(false);
        if (rifleArmsModel != null) rifleArmsModel.SetActive(false);
        if (shotgunArmsModel != null) shotgunArmsModel.SetActive(false);
        
        // Activate first weapon
        if (weapons.Length > 0)
        {
            SwitchToWeapon(currentWeaponIdx);
        }
        
        SetupCrosshair();
    }
    
    public void SwitchToWeapon(int idx)
    {
        // Check if unlocked
        if (!IsWeaponUnlocked(idx))
        {
            return;
        }
        
        if (weapons == null || idx < 0 || idx >= weapons.Length || weapons[idx] == null)
        {
            return;
        }
        
        // Hide all arms
        if (pistolArmsModel != null) pistolArmsModel.SetActive(false);
        if (rifleArmsModel != null) rifleArmsModel.SetActive(false);
        if (shotgunArmsModel != null) shotgunArmsModel.SetActive(false);
        
        // Show the right arms for this weapon
        GameObject activeArmModel = null;
        if (idx >= 0 && idx < armModels.Length)
        {
            activeArmModel = armModels[idx];
            if (activeArmModel != null)
            {
                activeArmModel.SetActive(true);
            }
        }
        
        // Get animator from active arms
        if (activeArmModel != null)
        {
            currentAnimator = activeArmModel.GetComponent<Animator>();
        }
        
        currentWeaponIdx = idx;
        currentWeapon = weapons[currentWeaponIdx];
        
        // Set camera position for this weapon
        ApplyWeaponConfiguration(idx);
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
        
        bool moving = moveInput.magnitude > 0.1f;
        currentWeapon.UpdateMovementAnimation(moving, isRunning);
    }
    
    void SetupCrosshair()
    {
        if (!enableCrosshair) return;
        
        // Add crosshair if needed
        crosshairManager = GetComponent<SnackAttack.Player.CrosshairManager>();
        if (crosshairManager == null)
        {
            crosshairManager = gameObject.AddComponent<SnackAttack.Player.CrosshairManager>();
        }
    }
    
    void InitializeWeaponConfigurations()
    {
        // Setup weapon configs
        weaponConfigs = new WeaponConfigData[3];
        
        // Load animation controllers
        RuntimeAnimatorController pistolController = Resources.Load<RuntimeAnimatorController>("Animations/PistolPlayer_Controller");
        RuntimeAnimatorController rifleController = Resources.Load<RuntimeAnimatorController>("Animations/RiflelPlayer_Controller");
        RuntimeAnimatorController shotgunController = Resources.Load<RuntimeAnimatorController>("Animations/ShotgunPlayer_Controller");
        
        // Pistol config
        weaponConfigs[0] = new WeaponConfigData
        {
            weaponName = "Ketchup Pistol",
            cameraPosition = new Vector3(-0.199f, 1.564f, 0.155f),
            cameraRotation = new Vector3(7.086f, -7.197f, -0.066f),
            animatorController = pistolController
        };
        
        // Rifle configuration (index 1)
        weaponConfigs[1] = new WeaponConfigData
        {
            weaponName = "Assault Rifle",
            cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
            cameraRotation = new Vector3(5.624f, -44.278f, -0.456f),
            animatorController = rifleController
        };
        
        // Shotgun configuration (index 2)
        weaponConfigs[2] = new WeaponConfigData
        {
            weaponName = "Shotgun",
            cameraPosition = new Vector3(-0.078f, 1.542f, 0.058f),
            cameraRotation = new Vector3(5.311f, -59.427f, 0.353f),
            animatorController = shotgunController
        };
    }
    
    void ApplyWeaponConfiguration(int weaponIndex)
    {
        if (weaponConfigs == null || weaponIndex < 0 || weaponIndex >= weaponConfigs.Length)
        {
            return;
        }
        
        WeaponConfigData config = weaponConfigs[weaponIndex];
        
        // Set camera position
        if (cameraAnchor != null)
        {
            cameraAnchor.localPosition = config.cameraPosition;
            cameraAnchor.localEulerAngles = config.cameraRotation;
        }
    }
    
    public BaseWeapon CurrentWeapon => currentWeapon;
    public int CurrentWeaponIndex => currentWeaponIdx;
    public bool IsGrounded => isGrounded;
    public Vector2 MoveInput => moveInput;
    public WeaponConfigData GetWeaponConfig(int index) => 
        (weaponConfigs != null && index >= 0 && index < weaponConfigs.Length) ? weaponConfigs[index] : null;
    
    // For shopkeeper to unlock weapons
    public void UnlockRifle()
    {
        rifleUnlocked = true;
    }
    
    public void UnlockShotgun()
    {
        shotgunUnlocked = true;
    }
    
    private bool IsWeaponUnlocked(int weaponIndex)
    {
        if (weaponIndex == pistolIndex) return pistolUnlocked;
        if (weaponIndex == rifleIndex) return rifleUnlocked;
        if (weaponIndex == shotgunIndex) return shotgunUnlocked;
        
        return true;
    }
}
