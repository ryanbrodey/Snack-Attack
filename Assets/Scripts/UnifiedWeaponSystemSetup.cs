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
        // Get the main controller
        FPSPlayerControllerWithWeapons controller = GetComponent<FPSPlayerControllerWithWeapons>();
        if (controller == null) return;
        
        WeaponConfigurationManager configManager = GetComponent<WeaponConfigurationManager>();
        if (configManager == null) return;
        
        SetupWeaponConfigurations(configManager);
        SetupWeapons(controller);
    }
    
    void SetupWeaponConfigurations(WeaponConfigurationManager configManager)
    {
        configManager.weaponConfigs = new WeaponConfiguration[3];
        
        // Load animation controllers
        if (pistolController == null)
            pistolController = Resources.Load<RuntimeAnimatorController>("Player-testing/Animations/PistolPlayer_Controller");
        if (rifleController == null)
            rifleController = Resources.Load<RuntimeAnimatorController>("Player-testing/Animations/RiflelPlayer_Controller");
        if (shotgunController == null)
            shotgunController = Resources.Load<RuntimeAnimatorController>("Player-testing/Animations/ShotgunPlayer_Controller");
        
        // Weapon configs
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
        
        configManager.weaponConfigs[2] = new WeaponConfiguration
        {
            weaponName = "Popcorn Launcher",
            weaponIndex = 2,
            cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
            cameraRotation = new Vector3(5.624f, -44.278f, -0.456f),
            animatorController = shotgunController
        };
    }
    
    void SetupWeapons(FPSPlayerControllerWithWeapons controller)
    {
        BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
        
        if (weapons.Length == 0) return;
        
        controller.weapons = weapons;
        
        // Only first weapon active initially
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == 0);
        }
    }
    
    [ContextMenu("Test Weapon Switching")]
    public void TestWeaponSwitching()
    {
        FPSPlayerControllerWithWeapons controller = GetComponent<FPSPlayerControllerWithWeapons>();
        if (controller == null) return;
        
        // Test switching
        for (int i = 0; i < 3; i++)
        {
            controller.SwitchToWeapon(i);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowHelpInfo();
        }
    }
    
    void ShowHelpInfo()
    {
        // Help info for testing
    }
}
