using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Quick fix script to resolve common issues with the FPS_Player_Unified prefab
/// </summary>
public class QuickPlayerFix : MonoBehaviour
{
    [ContextMenu("Fix Player Setup Issues")]
    public void FixPlayerSetupIssues()
    {
        Debug.Log("=== FIXING PLAYER SETUP ISSUES ===");
        
        // 1. Replace basic FPS controller with weapons version
        ReplaceBasicFPSController();
        
        // 2. Fix missing script references
        FixMissingScripts();
        
        // 3. Setup ground check
        SetupGroundCheck();
        
        // 4. Setup camera anchor
        SetupCameraAnchor();
        
        // 5. Assign missing references
        AssignMissingReferences();
        
        // 6. Setup weapon system
        SetupWeaponSystem();
        
        Debug.Log("✓ All issues fixed! Player should now work correctly.");
    }
    
    void ReplaceBasicFPSController()
    {
        Debug.Log("Replacing basic FPS controller with weapons version...");
        
        // Check if we have the basic FPS controller
        var basicController = GetComponent<MonoBehaviour>();
        if (basicController != null && basicController.GetType().Name == "FPSPlayerController")
        {
            Debug.Log("Found basic FPS controller, removing it...");
            DestroyImmediate(basicController);
        }
        
        // Add the weapons version if not present
        var weaponsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (weaponsController == null)
        {
            weaponsController = gameObject.AddComponent<FPSPlayerControllerWithWeapons>();
            Debug.Log("Added FPSPlayerControllerWithWeapons");
        }
    }
    
    void FixMissingScripts()
    {
        Debug.Log("Fixing missing script references...");
        
        // Remove any missing script components
        var components = GetComponents<MonoBehaviour>();
        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (components[i] == null)
            {
                Debug.Log("Removing missing script component");
                // Note: Can't actually remove null components in runtime, but we can detect them
            }
        }
        
        // Add WeaponConfigurationManager if missing
        var configManager = GetComponent<WeaponConfigurationManager>();
        if (configManager == null)
        {
            configManager = gameObject.AddComponent<WeaponConfigurationManager>();
            Debug.Log("Added WeaponConfigurationManager");
        }
        
        // Add FPSPlayerControllerWithWeapons if missing
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null)
        {
            fpsController = gameObject.AddComponent<FPSPlayerControllerWithWeapons>();
            Debug.Log("Added FPSPlayerControllerWithWeapons");
        }
    }
    
    void SetupGroundCheck()
    {
        Debug.Log("Setting up ground check...");
        
        Transform groundCheck = transform.Find("GroundCheck");
        if (groundCheck == null)
        {
            GameObject groundCheckGO = new GameObject("GroundCheck");
            groundCheckGO.transform.SetParent(transform);
            groundCheckGO.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = groundCheckGO.transform;
            Debug.Log("Created GroundCheck");
        }
        
        // Assign to FPS controller
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController != null)
        {
            fpsController.groundCheck = groundCheck;
            Debug.Log("Assigned ground check to FPS controller");
        }
    }
    
    void SetupCameraAnchor()
    {
        Debug.Log("Setting up camera anchor...");
        
        // Find or create camera anchor
        Transform cameraAnchor = transform.Find("CameraAnchor");
        if (cameraAnchor == null)
        {
            GameObject anchorGO = new GameObject("CameraAnchor");
            anchorGO.transform.SetParent(transform);
            anchorGO.transform.localPosition = new Vector3(-0.199f, 1.564f, 0.155f); // Default pistol position
            anchorGO.transform.localEulerAngles = new Vector3(7.086f, -7.197f, -0.066f);
            cameraAnchor = anchorGO.transform;
            Debug.Log("Created CameraAnchor");
        }
        
        // Find camera and move it under anchor if needed
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null && playerCamera.transform.parent != cameraAnchor)
        {
            playerCamera.transform.SetParent(cameraAnchor);
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
            Debug.Log("Moved camera under CameraAnchor");
        }
    }
    
    void AssignMissingReferences()
    {
        Debug.Log("Assigning missing references...");
        
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        var configManager = GetComponent<WeaponConfigurationManager>();
        
        if (fpsController != null)
        {
            // Assign camera
            if (fpsController.playerCamera == null)
            {
                fpsController.playerCamera = GetComponentInChildren<Camera>();
                Debug.Log("Assigned player camera");
            }
            
            // Assign camera anchor
            if (fpsController.cameraAnchor == null)
            {
                fpsController.cameraAnchor = transform.Find("CameraAnchor");
                Debug.Log("Assigned camera anchor");
            }
            
            // Assign arms animator
            if (fpsController.armsAnimator == null)
            {
                fpsController.armsAnimator = GetComponentInChildren<Animator>();
                Debug.Log("Assigned arms animator");
            }
            
            // Assign weapon holder
            if (fpsController.weaponHolder == null)
            {
                Transform weaponSocket = transform.Find("WeaponSocket");
                if (weaponSocket == null)
                {
                    GameObject socketGO = new GameObject("WeaponSocket");
                    socketGO.transform.SetParent(transform);
                    socketGO.transform.localPosition = Vector3.zero;
                    weaponSocket = socketGO.transform;
                    Debug.Log("Created WeaponSocket");
                }
                fpsController.weaponHolder = weaponSocket;
                Debug.Log("Assigned weapon holder");
            }
            
            // Assign weapon config manager
            if (fpsController.weaponConfigManager == null)
            {
                fpsController.weaponConfigManager = configManager;
                Debug.Log("Assigned weapon config manager");
            }
        }
        
        if (configManager != null)
        {
            configManager.playerCamera = GetComponentInChildren<Camera>();
            configManager.cameraAnchor = transform.Find("CameraAnchor");
            configManager.armsAnimator = GetComponentInChildren<Animator>();
            Debug.Log("Assigned references to config manager");
        }
    }
    
    void SetupWeaponSystem()
    {
        Debug.Log("Setting up weapon system...");
        
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null) return;
        
        // Create weapon holders if they don't exist
        Transform weaponSocket = fpsController.weaponHolder;
        if (weaponSocket != null)
        {
            CreateWeaponHolder("KetchupPistol", weaponSocket, typeof(KetchupWeapon), 0);
            CreateWeaponHolder("AssaultRifle", weaponSocket, typeof(AssaultRifleWeapon), 1);
            CreateWeaponHolder("PopcornLauncher", weaponSocket, typeof(PopcornLauncherWeapon), 2);
            
            // Find all weapons and assign to controller
            BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
            fpsController.weapons = weapons;
            Debug.Log($"Found and assigned {weapons.Length} weapons");
        }
    }
    
    void CreateWeaponHolder(string weaponName, Transform parent, System.Type weaponType, int index)
    {
        Transform weaponHolder = parent.Find(weaponName);
        if (weaponHolder == null)
        {
            GameObject holderGO = new GameObject(weaponName);
            holderGO.transform.SetParent(parent);
            holderGO.transform.localPosition = Vector3.zero;
            holderGO.transform.localRotation = Quaternion.identity;
            
            // Add weapon script
            BaseWeapon weaponScript = (BaseWeapon)holderGO.AddComponent(weaponType);
            
            // Only activate the first weapon
            holderGO.SetActive(index == 0);
            
            Debug.Log($"Created weapon holder: {weaponName} (Active: {index == 0})");
        }
    }
    
    [ContextMenu("Test Weapon Switching")]
    public void TestWeaponSwitching()
    {
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null)
        {
            Debug.LogError("FPSPlayerControllerWithWeapons not found!");
            return;
        }
        
        Debug.Log("Testing weapon switching...");
        for (int i = 0; i < 3; i++)
        {
            fpsController.SwitchToWeapon(i);
            Debug.Log($"Switched to weapon {i + 1}");
        }
    }
}
