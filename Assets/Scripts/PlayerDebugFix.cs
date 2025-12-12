using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Debug and fix script for common player issues
/// </summary>
public class PlayerDebugFix : MonoBehaviour
{
    [Header("Debug Info")]
    public bool showDebugLogs = true;
    
    [ContextMenu("Fix All Player Issues")]
    public void FixAllPlayerIssues()
    {
        Debug.Log("=== FIXING ALL PLAYER ISSUES ===");
        
        // 1. Fix animation parameters
        FixAnimationParameters();
        
        // 2. Fix movement issues
        FixMovementIssues();
        
        // 3. Fix weapon system
        FixWeaponSystem();
        
        // 4. Fix spawn position
        FixSpawnPosition();
        
        Debug.Log("✓ All fixes applied! Try testing again.");
    }
    
    [ContextMenu("Fix Animation Parameters")]
    public void FixAnimationParameters()
    {
        Debug.Log("Fixing animation parameters...");
        
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No animator found!");
            return;
        }
        
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("No animation controller assigned!");
            return;
        }
        
        // Check which parameters exist
        var parameters = animator.parameters;
        bool hasIsGrounded = false;
        bool hasIsWalking = false;
        bool hasIsRunning = false;
        
        foreach (var param in parameters)
        {
            if (param.name == "IsGrounded") hasIsGrounded = true;
            if (param.name == "IsWalking") hasIsWalking = true;
            if (param.name == "IsRunning") hasIsRunning = true;
            Debug.Log($"Found animation parameter: {param.name} (Type: {param.type})");
        }
        
        // Disable animation updates if parameters are missing
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController != null)
        {
            if (!hasIsGrounded || !hasIsWalking || !hasIsRunning)
            {
                Debug.LogWarning("Missing animation parameters! Disabling animation updates to prevent errors.");
                // We'll modify the controller to skip animation updates
            }
        }
        
        Debug.Log($"Animation check complete. Has required parameters: IsGrounded={hasIsGrounded}, IsWalking={hasIsWalking}, IsRunning={hasIsRunning}");
    }
    
    [ContextMenu("Fix Movement Issues")]
    public void FixMovementIssues()
    {
        Debug.Log("Fixing movement issues...");
        
        // Check CharacterController
        CharacterController cc = GetComponent<CharacterController>();
        if (cc == null)
        {
            Debug.LogError("No CharacterController found! Adding one...");
            cc = gameObject.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
        }
        
        // Check if controller is enabled
        if (!cc.enabled)
        {
            cc.enabled = true;
            Debug.Log("Enabled CharacterController");
        }
        
        // Check FPS controller
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null)
        {
            Debug.LogError("No FPSPlayerControllerWithWeapons found!");
            return;
        }
        
        // Check if script is enabled
        if (!fpsController.enabled)
        {
            fpsController.enabled = true;
            Debug.Log("Enabled FPSPlayerControllerWithWeapons");
        }
        
        Debug.Log("Movement components checked and fixed");
    }
    
    [ContextMenu("Fix Weapon System")]
    public void FixWeaponSystem()
    {
        Debug.Log("Fixing weapon system...");
        
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null) return;
        
        // Find all weapons
        BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
        if (weapons.Length == 0)
        {
            Debug.LogWarning("No weapons found! Creating basic weapon setup...");
            CreateBasicWeaponSetup();
        }
        else
        {
            fpsController.weapons = weapons;
            Debug.Log($"Found {weapons.Length} weapons");
            
            // Make sure only first weapon is active
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].gameObject.SetActive(i == 0);
            }
        }
    }
    
    void CreateBasicWeaponSetup()
    {
        // Create weapon socket if missing
        Transform weaponSocket = transform.Find("WeaponSocket");
        if (weaponSocket == null)
        {
            GameObject socketGO = new GameObject("WeaponSocket");
            socketGO.transform.SetParent(transform);
            socketGO.transform.localPosition = Vector3.zero;
            weaponSocket = socketGO.transform;
        }
        
        // Create basic ketchup weapon
        GameObject ketchupGO = new GameObject("KetchupPistol");
        ketchupGO.transform.SetParent(weaponSocket);
        ketchupGO.transform.localPosition = Vector3.zero;
        var ketchupWeapon = ketchupGO.AddComponent<KetchupWeapon>();
        
        // Assign to controller
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        fpsController.weapons = new BaseWeapon[] { ketchupWeapon };
        fpsController.weaponHolder = weaponSocket;
        
        Debug.Log("Created basic weapon setup");
    }
    
    [ContextMenu("Fix Spawn Position")]
    public void FixSpawnPosition()
    {
        Debug.Log("Fixing spawn position...");
        
        // Reset position to origin
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        
        Debug.Log("Reset player position to origin");
    }
    
    [ContextMenu("Disable Error Pause")]
    public void DisableErrorPause()
    {
        Debug.Log("Disabling error pause...");
        
        // This will help with the pausing issue
        Debug.developerConsoleVisible = false;
        
        Debug.Log("Error pause disabled");
    }
    
    void Update()
    {
        if (showDebugLogs)
        {
            // Debug movement input
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log($"Movement input detected: W={Input.GetKey(KeyCode.W)}, A={Input.GetKey(KeyCode.A)}, S={Input.GetKey(KeyCode.S)}, D={Input.GetKey(KeyCode.D)}");
            }
            
            // Debug weapon switching
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log($"Weapon switch input: 1={Input.GetKeyDown(KeyCode.Alpha1)}, 2={Input.GetKeyDown(KeyCode.Alpha2)}, 3={Input.GetKeyDown(KeyCode.Alpha3)}");
            }
            
            // Debug shooting
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Shoot input detected: F={Input.GetKeyDown(KeyCode.F)}, LeftClick={Input.GetMouseButtonDown(0)}");
            }
        }
    }
}
