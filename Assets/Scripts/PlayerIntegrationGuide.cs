using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Step-by-step guide to integrate your existing 3D player model with the unified weapon system
/// </summary>
public class PlayerIntegrationGuide : MonoBehaviour
{
    [Header("Integration Steps - Check off as you complete them")]
    [SerializeField] private bool step1_RemoveOldScripts = false;
    [SerializeField] private bool step2_AddWeaponSystem = false;
    [SerializeField] private bool step3_SetupCameraAnchor = false;
    [SerializeField] private bool step4_AddWeaponModels = false;
    [SerializeField] private bool step5_AddArmsAnimator = false;
    [SerializeField] private bool step6_FinalSetup = false;
    
    [Header("Required Components (Drag from Project)")]
    public GameObject ketchupWeaponPrefab;
    public GameObject rifleWeaponPrefab;
    public GameObject shotgunWeaponPrefab;
    public GameObject armsModelPrefab; // Your arms/hands model for FPS view
    
    [Header("Animation Controllers")]
    public RuntimeAnimatorController pistolAnimController;
    public RuntimeAnimatorController rifleAnimController;
    public RuntimeAnimatorController shotgunAnimController;
    
    void Start()
    {
        Debug.Log("=== PLAYER INTEGRATION GUIDE ===");
        Debug.Log("Follow the steps in order by checking the boxes in the inspector");
        Debug.Log("Or use the context menu options to run each step");
    }
    
    [ContextMenu("Step 1: Remove Old Scripts")]
    public void Step1_RemoveOldScripts()
    {
        Debug.Log("STEP 1: Removing old movement scripts...");
        
        // Remove old movement scripts that might conflict
        var oldMovement = GetComponent<MonoBehaviour>();
        if (oldMovement != null && oldMovement.GetType().Name.Contains("Movement"))
        {
            Debug.Log($"Removing old movement script: {oldMovement.GetType().Name}");
            DestroyImmediate(oldMovement);
        }
        
        // Keep CharacterController - we need it
        var charController = GetComponent<CharacterController>();
        if (charController == null)
        {
            Debug.Log("Adding CharacterController...");
            charController = gameObject.AddComponent<CharacterController>();
            charController.height = 2f;
            charController.radius = 0.5f;
            charController.center = new Vector3(0, 1, 0);
        }
        
        step1_RemoveOldScripts = true; // Mark as completed
        Debug.Log("✓ Step 1 Complete: Old scripts removed, CharacterController ready");
    }
    
    [ContextMenu("Step 2: Add Weapon System")]
    public void Step2_AddWeaponSystem()
    {
        Debug.Log("STEP 2: Adding unified weapon system...");
        
        // Add the main FPS controller with weapons
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null)
        {
            fpsController = gameObject.AddComponent<FPSPlayerControllerWithWeapons>();
            Debug.Log("Added FPSPlayerControllerWithWeapons");
        }
        
        // Add weapon configuration manager
        var configManager = GetComponent<WeaponConfigurationManager>();
        if (configManager == null)
        {
            configManager = gameObject.AddComponent<WeaponConfigurationManager>();
            Debug.Log("Added WeaponConfigurationManager");
        }
        
        // Add setup helper
        var setupHelper = GetComponent<UnifiedWeaponSystemSetup>();
        if (setupHelper == null)
        {
            setupHelper = gameObject.AddComponent<UnifiedWeaponSystemSetup>();
            Debug.Log("Added UnifiedWeaponSystemSetup");
        }
        
        step2_AddWeaponSystem = true; // Mark as completed
        Debug.Log("✓ Step 2 Complete: Weapon system scripts added");
    }
    
    [ContextMenu("Step 3: Setup Camera Anchor")]
    public void Step3_SetupCameraAnchor()
    {
        Debug.Log("STEP 3: Setting up camera anchor for weapon switching...");
        
        // Find existing camera
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("No camera found! Please ensure your player has a camera.");
            return;
        }
        
        // Create camera anchor if it doesn't exist
        Transform cameraAnchor = transform.Find("CameraAnchor");
        if (cameraAnchor == null)
        {
            GameObject anchorGO = new GameObject("CameraAnchor");
            anchorGO.transform.SetParent(transform);
            cameraAnchor = anchorGO.transform;
            Debug.Log("Created CameraAnchor");
        }
        
        // Move camera under the anchor
        if (playerCamera.transform.parent != cameraAnchor)
        {
            // Store current camera position relative to player
            Vector3 currentLocalPos = transform.InverseTransformPoint(playerCamera.transform.position);
            Vector3 currentLocalRot = playerCamera.transform.localEulerAngles;
            
            // Set anchor position to current camera position
            cameraAnchor.localPosition = currentLocalPos;
            cameraAnchor.localEulerAngles = currentLocalRot;
            
            // Parent camera to anchor and reset its local transform
            playerCamera.transform.SetParent(cameraAnchor);
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
            
            Debug.Log($"Camera anchored at position: {cameraAnchor.localPosition}");
        }
        
        step3_SetupCameraAnchor = true; // Mark as completed
        Debug.Log("✓ Step 3 Complete: Camera anchor setup");
    }
    
    [ContextMenu("Step 4: Add Weapon Models")]
    public void Step4_AddWeaponModels()
    {
        Debug.Log("STEP 4: Adding weapon models and scripts...");
        
        // Create weapon socket if it doesn't exist
        Transform weaponSocket = transform.Find("WeaponSocket");
        if (weaponSocket == null)
        {
            GameObject socketGO = new GameObject("WeaponSocket");
            socketGO.transform.SetParent(transform);
            socketGO.transform.localPosition = Vector3.zero;
            weaponSocket = socketGO.transform;
            Debug.Log("Created WeaponSocket");
        }
        
        // Add weapon holders for each weapon type
        CreateWeaponHolder("KetchupPistol", weaponSocket, 0);
        CreateWeaponHolder("AssaultRifle", weaponSocket, 1);
        CreateWeaponHolder("PopcornLauncher", weaponSocket, 2);
        
        step4_AddWeaponModels = true; // Mark as completed
        Debug.Log("✓ Step 4 Complete: Weapon models added");
    }
    
    void CreateWeaponHolder(string weaponName, Transform parent, int weaponIndex)
    {
        Transform weaponHolder = parent.Find(weaponName);
        if (weaponHolder == null)
        {
            GameObject holderGO = new GameObject(weaponName);
            holderGO.transform.SetParent(parent);
            holderGO.transform.localPosition = Vector3.zero;
            holderGO.transform.localRotation = Quaternion.identity;
            weaponHolder = holderGO.transform;
            
            // Add appropriate weapon script
            BaseWeapon weaponScript = null;
            switch (weaponIndex)
            {
                case 0: // Ketchup Pistol
                    weaponScript = holderGO.AddComponent<KetchupWeapon>();
                    break;
                case 1: // Assault Rifle
                    weaponScript = holderGO.AddComponent<AssaultRifleWeapon>();
                    break;
                case 2: // Popcorn Launcher
                    weaponScript = holderGO.AddComponent<PopcornLauncherWeapon>();
                    break;
            }
            
            // Disable all weapons except the first one
            holderGO.SetActive(weaponIndex == 0);
            
            Debug.Log($"Created weapon holder: {weaponName} (Active: {weaponIndex == 0})");
        }
    }
    
    [ContextMenu("Step 5: Add Arms Animator")]
    public void Step5_AddArmsAnimator()
    {
        Debug.Log("STEP 5: Setting up arms animator...");
        
        // Look for existing arms model or create placeholder
        Transform armsModel = transform.Find("PistolArms") ?? transform.Find("Arms");
        if (armsModel == null && armsModelPrefab != null)
        {
            GameObject armsGO = Instantiate(armsModelPrefab, transform);
            armsGO.name = "PistolArms";
            armsModel = armsGO.transform;
            Debug.Log("Added arms model from prefab");
        }
        else if (armsModel == null)
        {
            // Create placeholder arms object
            GameObject armsGO = new GameObject("PistolArms");
            armsGO.transform.SetParent(transform);
            armsGO.transform.localPosition = Vector3.zero;
            armsModel = armsGO.transform;
            Debug.Log("Created placeholder arms object");
        }
        
        // Add animator if it doesn't exist
        Animator armsAnimator = armsModel.GetComponent<Animator>();
        if (armsAnimator == null)
        {
            armsAnimator = armsModel.gameObject.AddComponent<Animator>();
            Debug.Log("Added Animator to arms model");
        }
        
        // Set default animation controller
        if (pistolAnimController != null && armsAnimator.runtimeAnimatorController == null)
        {
            armsAnimator.runtimeAnimatorController = pistolAnimController;
            Debug.Log("Set pistol animation controller");
        }
        
        step5_AddArmsAnimator = true; // Mark as completed
        Debug.Log("✓ Step 5 Complete: Arms animator setup");
    }
    
    [ContextMenu("Step 6: Final Setup and Configuration")]
    public void Step6_FinalSetup()
    {
        Debug.Log("STEP 6: Final setup and configuration...");
        
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        var configManager = GetComponent<WeaponConfigurationManager>();
        var setupHelper = GetComponent<UnifiedWeaponSystemSetup>();
        
        if (fpsController == null || configManager == null)
        {
            Debug.LogError("Missing required components! Run previous steps first.");
            return;
        }
        
        // Assign references
        fpsController.playerCamera = GetComponentInChildren<Camera>();
        fpsController.cameraAnchor = transform.Find("CameraAnchor");
        fpsController.armsAnimator = GetComponentInChildren<Animator>();
        fpsController.weaponHolder = transform.Find("WeaponSocket");
        
        // Find all weapon scripts
        BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
        fpsController.weapons = weapons;
        
        // Setup weapon configuration manager
        configManager.playerCamera = fpsController.playerCamera;
        configManager.cameraAnchor = fpsController.cameraAnchor;
        configManager.armsAnimator = fpsController.armsAnimator;
        
        // Assign animation controllers to setup helper
        if (setupHelper != null)
        {
            setupHelper.pistolController = pistolAnimController;
            setupHelper.rifleController = rifleAnimController;
            setupHelper.shotgunController = shotgunAnimController;
        }
        
        // Run the unified weapon system setup
        if (setupHelper != null)
        {
            setupHelper.SetupUnifiedWeaponSystem();
        }
        
        step6_FinalSetup = true; // Mark as completed
        Debug.Log("✓ Step 6 Complete: Final setup finished!");
        Debug.Log("=== INTEGRATION COMPLETE ===");
        Debug.Log("Your 3D player model is now integrated with the unified weapon system!");
        Debug.Log("Test with keys 1, 2, 3 to switch weapons");
    }
    
    [ContextMenu("Run All Steps Automatically")]
    public void RunAllSteps()
    {
        Debug.Log("Running all integration steps automatically...");
        Step1_RemoveOldScripts();
        Step2_AddWeaponSystem();
        Step3_SetupCameraAnchor();
        Step4_AddWeaponModels();
        Step5_AddArmsAnimator();
        Step6_FinalSetup();
        Debug.Log("All steps completed! Your player is ready to test!");
    }
    
    void Update()
    {
        // Show help
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowIntegrationHelp();
        }
    }
    
    void ShowIntegrationHelp()
    {
        Debug.Log("=== INTEGRATION HELP ===");
        Debug.Log("F1 - Show this help");
        Debug.Log("Use the context menu (right-click this script) to run integration steps");
        Debug.Log("Or check the boxes in the inspector to track your progress");
        Debug.Log("Run 'Run All Steps Automatically' to do everything at once");
        
        // Show current progress
        ShowProgress();
    }
    
    [ContextMenu("Show Progress")]
    public void ShowProgress()
    {
        Debug.Log("=== INTEGRATION PROGRESS ===");
        Debug.Log($"Step 1 - Remove Old Scripts: {(step1_RemoveOldScripts ? "✓ Complete" : "○ Pending")}");
        Debug.Log($"Step 2 - Add Weapon System: {(step2_AddWeaponSystem ? "✓ Complete" : "○ Pending")}");
        Debug.Log($"Step 3 - Setup Camera Anchor: {(step3_SetupCameraAnchor ? "✓ Complete" : "○ Pending")}");
        Debug.Log($"Step 4 - Add Weapon Models: {(step4_AddWeaponModels ? "✓ Complete" : "○ Pending")}");
        Debug.Log($"Step 5 - Add Arms Animator: {(step5_AddArmsAnimator ? "✓ Complete" : "○ Pending")}");
        Debug.Log($"Step 6 - Final Setup: {(step6_FinalSetup ? "✓ Complete" : "○ Pending")}");
        
        int completedSteps = (step1_RemoveOldScripts ? 1 : 0) + 
                           (step2_AddWeaponSystem ? 1 : 0) + 
                           (step3_SetupCameraAnchor ? 1 : 0) + 
                           (step4_AddWeaponModels ? 1 : 0) + 
                           (step5_AddArmsAnimator ? 1 : 0) + 
                           (step6_FinalSetup ? 1 : 0);
        
        Debug.Log($"Progress: {completedSteps}/6 steps completed ({(completedSteps * 100 / 6)}%)");
        
        if (completedSteps == 6)
        {
            Debug.Log("🎉 All steps complete! Your weapon system is ready!");
        }
    }
}
