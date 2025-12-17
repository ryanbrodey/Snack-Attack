using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Setup script to help configure the unified weapon system for testing
/// This script should be attached to the FPS_Player_Unified prefab
/// </summary>
public class UnifiedWeaponSystemSetup : MonoBehaviour
{
    [Header("Animation Controllers")]
    public RuntimeAnimatorController pistolController;
    public RuntimeAnimatorController rifleController;
    public RuntimeAnimatorController shotgunController;
    
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupUnifiedWeaponSystem();
        }
    }
    
    [ContextMenu("Setup Unified Weapon System")]
    public void SetupUnifiedWeaponSystem()
    {
        Debug.Log("Setting up Unified Weapon System...");
        
        // Get the main controller
        FPSPlayerControllerWithWeapons controller = GetComponent<FPSPlayerControllerWithWeapons>();
        if (controller == null)
        {
            Debug.LogError("FPSPlayerControllerWithWeapons not found!");
            return;
        }
        
        // Get the weapon configuration manager
        WeaponConfigurationManager configManager = GetComponent<WeaponConfigurationManager>();
        if (configManager == null)
        {
            Debug.LogError("WeaponConfigurationManager not found!");
            return;
        }
        
        // Setup weapon configurations
        SetupWeaponConfigurations(configManager);
        
        // Find and setup weapons
        SetupWeapons(controller);
        
        Debug.Log("Unified Weapon System setup complete!");
        Debug.Log("Controls: 1=Ketchup Pistol, 2=Assault Rifle, 3=Popcorn Launcher/Shotgun");
        Debug.Log("Attack: Left Click or F key, Movement: WASD, Jump: Space, Run: Shift");
    }
    
    void SetupWeaponConfigurations(WeaponConfigurationManager configManager)
    {
        // Initialize weapon configurations array
        configManager.weaponConfigs = new WeaponConfiguration[3];
        
        // Load animation controllers if not assigned
        if (pistolController == null)
            pistolController = Resources.Load<RuntimeAnimatorController>("Player-testing/Animations/PistolPlayer_Controller");
        if (rifleController == null)
            rifleController = Resources.Load<RuntimeAnimatorController>("Player-testing/Animations/RiflelPlayer_Controller");
        if (shotgunController == null)
            shotgunController = Resources.Load<RuntimeAnimatorController>("Player-testing/Animations/ShotgunPlayer_Controller");
        
        // Ketchup/Pistol configuration (index 0)
        configManager.weaponConfigs[0] = new WeaponConfiguration
        {
            weaponName = "Ketchup Pistol",
            weaponIndex = 0,
            cameraPosition = new Vector3(-0.199f, 1.564f, 0.155f),
            cameraRotation = new Vector3(7.086f, -7.197f, -0.066f),
            animatorController = pistolController
        };
        
        // Rifle configuration (index 1)
        configManager.weaponConfigs[1] = new WeaponConfiguration
        {
            weaponName = "Assault Rifle",
            weaponIndex = 1,
            cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
            cameraRotation = new Vector3(5.624f, -44.278f, -0.456f),
            animatorController = rifleController
        };
        
        // Shotgun/Popcorn Launcher configuration (index 2)
        configManager.weaponConfigs[2] = new WeaponConfiguration
        {
            weaponName = "Popcorn Launcher",
            weaponIndex = 2,
            cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
            cameraRotation = new Vector3(5.624f, -44.278f, -0.456f),
            animatorController = shotgunController
        };
        
        Debug.Log("Weapon configurations setup complete");
    }
    
    void SetupWeapons(FPSPlayerControllerWithWeapons controller)
    {
        // Find all weapon scripts in children
        BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
        
        if (weapons.Length == 0)
        {
            Debug.LogWarning("No weapons found in children!");
            return;
        }
        
        // Assign weapons to controller
        controller.weapons = weapons;
        
        // Make sure all weapons are initially disabled except the first one
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == 0);
            Debug.Log($"Weapon {i}: {weapons[i].WeaponName} - Active: {i == 0}");
        }
        
        Debug.Log($"Setup {weapons.Length} weapons");
    }
    
    [ContextMenu("Test Weapon Switching")]
    public void TestWeaponSwitching()
    {
        FPSPlayerControllerWithWeapons controller = GetComponent<FPSPlayerControllerWithWeapons>();
        if (controller == null) return;
        
        Debug.Log("Testing weapon switching...");
        
        // Test switching to each weapon
        for (int i = 0; i < 3; i++)
        {
            controller.SwitchToWeapon(i);
            Debug.Log($"Switched to weapon {i}: {controller.CurrentWeapon?.WeaponName ?? "None"}");
        }
    }
    
    void Update()
    {
        // Debug info
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowHelpInfo();
        }
    }
    
    void ShowHelpInfo()
    {
        Debug.Log("=== UNIFIED WEAPON SYSTEM HELP ===");
        Debug.Log("Controls:");
        Debug.Log("1, 2, 3 - Switch weapons");
        Debug.Log("Left Click / F - Attack");
        Debug.Log("R - Reload");
        Debug.Log("WASD - Move");
        Debug.Log("Space - Jump");
        Debug.Log("Shift - Run");
        Debug.Log("H - Show this help");
        Debug.Log("Escape - Toggle cursor lock");
        
        FPSPlayerControllerWithWeapons controller = GetComponent<FPSPlayerControllerWithWeapons>();
        if (controller != null)
        {
            Debug.Log($"Current weapon: {controller.CurrentWeapon?.WeaponName ?? "None"}");
            Debug.Log($"Weapon index: {controller.CurrentWeaponIndex}");
        }
    }
}
