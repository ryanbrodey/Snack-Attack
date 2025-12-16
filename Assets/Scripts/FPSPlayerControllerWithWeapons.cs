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
            Debug.LogError("FPSPlayerController requires a CharacterController component!");
            enabled = false;
            return;
        }
        
        // Properly lock and hide cursor for FPS gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("[FPSPlayerControllerWithWeapons] Cursor locked and hidden for FPS gameplay");
        
        // Auto-find camera
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
                playerCamera = Camera.main;
        }
        
        // Auto-find camera anchor
        if (cameraAnchor == null)
        {
            cameraAnchor = transform.Find("CameraAnchor");
        }
        
        // Auto-find arm models if not assigned
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
        
        // Store arm models in array for easy access
        armModels = new GameObject[] { pistolArmsModel, rifleArmsModel, shotgunArmsModel };
        
        // Initialize weapon configurations
        InitializeWeaponConfigurations();
        
        // Create ground check
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
        }
        
        // Initialize velocity to zero to prevent immediate falling
        velocity = Vector3.zero;
        
        // Setup weapon system
        SetupWeapons();
        
        Debug.Log("FPS Player Controller with Weapons initialized!");
        Debug.Log("Controls: WASD=move, Double-tap W=auto-run, Space=jump, Mouse=look");
        Debug.Log("Weapons: 1,2,3=switch, F=semi-auto, G=full-auto, R=reload");
    }
    
    void Update()
    {
        // Ensure cursor stays locked during gameplay
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
                Debug.Log("[FPSPlayerControllerWithWeapons] Cursor unlocked and visible");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("[FPSPlayerControllerWithWeapons] Cursor locked and hidden");
            }
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
            // Get input with dead zone to prevent unwanted movement
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            // Apply dead zone to prevent joystick drift
            if (Mathf.Abs(horizontal) < 0.2f) horizontal = 0f;
            if (Mathf.Abs(vertical) < 0.2f) vertical = 0f;
            
            // Also check direct key input for more responsive controls
            if (Input.GetKey(KeyCode.W)) vertical = Mathf.Max(vertical, 1f);
            if (Input.GetKey(KeyCode.S)) vertical = Mathf.Min(vertical, -1f);
            if (Input.GetKey(KeyCode.A)) horizontal = Mathf.Min(horizontal, -1f);
            if (Input.GetKey(KeyCode.D)) horizontal = Mathf.Max(horizontal, 1f);
            
            moveInput.x = horizontal;
            moveInput.y = vertical;
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
        // Ground check - use CharacterController.isGrounded for more reliable detection
        wasGroundedLastFrame = isGrounded;
        isGrounded = characterController.isGrounded;
        
        // Additional ground check with sphere cast for better reliability
        if (!isGrounded)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        }
        
        // Landing detection
        if (isGrounded && !wasGroundedLastFrame)
        {
            isJumping = false;
            velocity.y = -2f; // Small negative value to keep grounded
        }
        
        // Jump - only allow jumping when grounded and not already jumping
        if (jumpPressed && isGrounded && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            isGrounded = false; // Immediately set to false to prevent double jumping
            Debug.Log("Jump executed!");
        }
        
        // Movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);
        
        // Gravity (only if not grounded or falling)
        if (!isGrounded || velocity.y > 0)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        // Apply vertical movement
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
        if (currentAnimator == null) return;
        
        bool moving = moveInput.magnitude > 0.1f;
        
        // Update the current arm model's animator
        if (HasAnimatorParameter("IsWalking"))
            currentAnimator.SetBool("IsWalking", moving && !isRunning);
        if (HasAnimatorParameter("IsRunning"))
            currentAnimator.SetBool("IsRunning", moving && isRunning);
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
    
    // WEAPON SYSTEM METHODS
    void SetupWeapons()
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogWarning("No weapons assigned! Please assign weapons in the Inspector.");
            return;
        }
        
        // Deactivate all arm models initially
        if (pistolArmsModel != null) pistolArmsModel.SetActive(false);
        if (rifleArmsModel != null) rifleArmsModel.SetActive(false);
        if (shotgunArmsModel != null) shotgunArmsModel.SetActive(false);
        
        // Switch to first weapon (this will activate the correct arm model)
        if (weapons.Length > 0)
        {
            SwitchToWeapon(currentWeaponIdx);
        }
        
        // Setup crosshair
        SetupCrosshair();
    }
    
    public void SwitchToWeapon(int idx)
    {
        if (weapons == null || idx < 0 || idx >= weapons.Length || weapons[idx] == null)
        {
            Debug.LogWarning($"Invalid weapon index {idx}");
            return;
        }
        
        Debug.Log($"Switching to weapon {idx + 1}: {weapons[idx].WeaponName}");
        
        // Deactivate ALL arm models
        if (pistolArmsModel != null) pistolArmsModel.SetActive(false);
        if (rifleArmsModel != null) rifleArmsModel.SetActive(false);
        if (shotgunArmsModel != null) shotgunArmsModel.SetActive(false);
        
        // Activate the correct arm model for this weapon
        GameObject activeArmModel = null;
        if (idx >= 0 && idx < armModels.Length)
        {
            activeArmModel = armModels[idx];
            if (activeArmModel != null)
            {
                activeArmModel.SetActive(true);
                Debug.Log($"Activated arm model: {activeArmModel.name}");
            }
        }
        
        // Get the animator from the active arm model
        if (activeArmModel != null)
        {
            currentAnimator = activeArmModel.GetComponent<Animator>();
            if (currentAnimator == null)
            {
                Debug.LogWarning($"No Animator found on {activeArmModel.name}!");
            }
        }
        
        // Update current weapon
        currentWeaponIdx = idx;
        currentWeapon = weapons[currentWeaponIdx];
        
        // Apply weapon configuration (camera position)
        ApplyWeaponConfiguration(idx);
        
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
    
    void SetupCrosshair()
    {
        if (!enableCrosshair) return;
        
        // Add CrosshairManager if it doesn't exist
        crosshairManager = GetComponent<SnackAttack.Player.CrosshairManager>();
        if (crosshairManager == null)
        {
            crosshairManager = gameObject.AddComponent<SnackAttack.Player.CrosshairManager>();
            Debug.Log("[FPSPlayerControllerWithWeapons] CrosshairManager added automatically");
        }
    }
    
    void InitializeWeaponConfigurations()
    {
        // Initialize weapon configuration data
        weaponConfigs = new WeaponConfigData[3];
        
        // Load animation controllers (you'll need to assign these in the inspector)
        RuntimeAnimatorController pistolController = Resources.Load<RuntimeAnimatorController>("Animations/PistolPlayer_Controller");
        RuntimeAnimatorController rifleController = Resources.Load<RuntimeAnimatorController>("Animations/RiflelPlayer_Controller");
        RuntimeAnimatorController shotgunController = Resources.Load<RuntimeAnimatorController>("Animations/ShotgunPlayer_Controller");
        
        // Ketchup/Pistol configuration (index 0)
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
        
        Debug.Log("Weapon configurations initialized");
    }
    
    void ApplyWeaponConfiguration(int weaponIndex)
    {
        if (weaponConfigs == null || weaponIndex < 0 || weaponIndex >= weaponConfigs.Length)
        {
            Debug.LogWarning($"Invalid weapon configuration index: {weaponIndex}");
            return;
        }
        
        WeaponConfigData config = weaponConfigs[weaponIndex];
        
        // Apply camera position
        if (cameraAnchor != null)
        {
            cameraAnchor.localPosition = config.cameraPosition;
            cameraAnchor.localEulerAngles = config.cameraRotation;
            Debug.Log($"Applied camera position: {config.cameraPosition} and rotation: {config.cameraRotation}");
        }
    }
    
    // Public getters
    public BaseWeapon CurrentWeapon => currentWeapon;
    public int CurrentWeaponIndex => currentWeaponIdx;
    public bool IsGrounded => isGrounded;
    public Vector2 MoveInput => moveInput;
    public WeaponConfigData GetWeaponConfig(int index) => 
        (weaponConfigs != null && index >= 0 && index < weaponConfigs.Length) ? weaponConfigs[index] : null;
}
