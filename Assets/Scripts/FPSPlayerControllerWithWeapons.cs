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
    public Animator armsAnimator;
    public Transform cameraAnchor; // The CameraAnchor that moves for different weapons
    public UnifiedWeaponAnimator unifiedWeaponAnimator; // New unified animator system
    
    [Header("Weapon System")]
    public BaseWeapon[] weapons;
    public Transform weaponHolder;
    public int currentWeaponIdx = 0;
    
    [Header("Weapon Configuration")]
    public WeaponConfigurationManager weaponConfigManager;
    
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
    
    // Weapon configuration data
    private WeaponConfigData[] weaponConfigs;
    
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
        
        // Auto-find camera anchor
        if (cameraAnchor == null)
        {
            cameraAnchor = transform.Find("CameraAnchor");
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
        
        // Auto-find unified weapon animator
        if (unifiedWeaponAnimator == null)
        {
            unifiedWeaponAnimator = GetComponent<UnifiedWeaponAnimator>();
            if (unifiedWeaponAnimator == null)
            {
                // Try to find it in arms
                if (armsAnimator != null)
                {
                    unifiedWeaponAnimator = armsAnimator.GetComponent<UnifiedWeaponAnimator>();
                }
            }
        }
        
        // Setup weapon configuration manager
        if (weaponConfigManager == null)
        {
            weaponConfigManager = GetComponent<WeaponConfigurationManager>();
            if (weaponConfigManager == null)
            {
                weaponConfigManager = gameObject.AddComponent<WeaponConfigurationManager>();
            }
        }
        
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
        
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
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
        bool moving = moveInput.magnitude > 0.1f;
        
        // Use unified weapon animator if available
        if (unifiedWeaponAnimator != null)
        {
            unifiedWeaponAnimator.UpdateMovement(
                moving && !isRunning,  // isWalking
                moving && isRunning,   // isRunning
                isGrounded,            // isGrounded
                isJumping              // isJumping
            );
        }
        // Fallback to old system
        else if (armsAnimator != null && armsAnimator.runtimeAnimatorController != null)
        {
            // Check if parameters exist before setting them
            if (HasAnimatorParameter("IsWalking"))
                armsAnimator.SetBool("IsWalking", moving && !isRunning);
            if (HasAnimatorParameter("IsRunning"))
                armsAnimator.SetBool("IsRunning", moving && isRunning);
            if (HasAnimatorParameter("IsGrounded"))
                armsAnimator.SetBool("IsGrounded", isGrounded);
            if (HasAnimatorParameter("IsJumping"))
                armsAnimator.SetBool("IsJumping", isJumping);
        }
    }
    
    bool HasAnimatorParameter(string paramName)
    {
        if (armsAnimator == null || armsAnimator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter param in armsAnimator.parameters)
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
        
        // Update unified weapon animator
        if (unifiedWeaponAnimator != null)
        {
            unifiedWeaponAnimator.SetWeaponType(idx);
        }
        
        // Switch weapon configuration (camera position, animations, etc.)
        if (weaponConfigManager != null)
        {
            weaponConfigManager.SwitchToWeapon(idx);
        }
        else
        {
            // Fallback: Apply weapon configuration directly
            ApplyWeaponConfiguration(idx);
        }
        
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
            cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
            cameraRotation = new Vector3(5.624f, -44.278f, -0.456f),
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
        
        // Apply animation controller
        if (armsAnimator != null && config.animatorController != null)
        {
            armsAnimator.runtimeAnimatorController = config.animatorController;
            Debug.Log($"Applied animation controller: {config.animatorController.name}");
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
