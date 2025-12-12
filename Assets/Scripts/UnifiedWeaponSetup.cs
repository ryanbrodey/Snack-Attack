using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Helper script to quickly set up the unified weapon system
/// Attach this to your FPS_Player_Unified prefab and click "Setup Unified Weapon System" in the inspector
/// </summary>
[AddComponentMenu("Snack Attack/Unified Weapon Setup")]
public class UnifiedWeaponSetup : MonoBehaviour
{
    [Header("Setup References")]
    public RuntimeAnimatorController unifiedController;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    [ContextMenu("Setup Unified Weapon System")]
    public void SetupUnifiedWeaponSystem()
    {
        Debug.Log("=== Setting up Unified Weapon System ===");
        
        // Get the FPS controller
        FPSPlayerControllerWithWeapons fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController == null)
        {
            Debug.LogError("FPSPlayerControllerWithWeapons not found!");
            return;
        }
        
        // Get or add UnifiedWeaponAnimator
        UnifiedWeaponAnimator unifiedAnimator = GetComponent<UnifiedWeaponAnimator>();
        if (unifiedAnimator == null)
        {
            unifiedAnimator = gameObject.AddComponent<UnifiedWeaponAnimator>();
            Debug.Log("Added UnifiedWeaponAnimator component");
        }
        
        // Set up the unified animator
        if (fpsController.armsAnimator != null)
        {
            unifiedAnimator.unifiedAnimator = fpsController.armsAnimator;
            Debug.Log("Assigned arms animator to unified system");
        }
        
        if (unifiedController != null)
        {
            unifiedAnimator.unifiedController = unifiedController;
            Debug.Log("Assigned unified controller");
        }
        
        // Link the unified animator to the FPS controller
        fpsController.unifiedWeaponAnimator = unifiedAnimator;
        
        // Set up weapon references in the unified animator for each weapon
        SetupWeaponReferences();
        
        Debug.Log("=== Unified Weapon System Setup Complete! ===");
        Debug.Log("Next steps:");
        Debug.Log("1. Create the UnifiedWeaponController animator controller");
        Debug.Log("2. Set up the animator parameters and states as described in the guide");
        Debug.Log("3. Assign the controller to the unifiedController field");
        Debug.Log("4. Test weapon switching with keys 1, 2, 3");
    }
    
    void SetupWeaponReferences()
    {
        // Find all weapons and set their unified animator reference
        BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
        UnifiedWeaponAnimator unifiedAnimator = GetComponent<UnifiedWeaponAnimator>();
        
        foreach (BaseWeapon weapon in weapons)
        {
            weapon.unifiedAnimator = unifiedAnimator;
            Debug.Log($"Set unified animator reference for: {weapon.WeaponName}");
        }
        
        Debug.Log($"Set up {weapons.Length} weapon references");
    }
    
    [ContextMenu("Debug Weapon System")]
    public void DebugWeaponSystem()
    {
        Debug.Log("=== Weapon System Debug Info ===");
        
        FPSPlayerControllerWithWeapons fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController != null)
        {
            Debug.Log($"FPS Controller found: {fpsController.name}");
            Debug.Log($"Current weapon index: {fpsController.CurrentWeaponIndex}");
            Debug.Log($"Current weapon: {(fpsController.CurrentWeapon != null ? fpsController.CurrentWeapon.WeaponName : "None")}");
            Debug.Log($"Arms animator: {(fpsController.armsAnimator != null ? "Found" : "Missing")}");
            Debug.Log($"Unified animator: {(fpsController.unifiedWeaponAnimator != null ? "Found" : "Missing")}");
        }
        
        UnifiedWeaponAnimator unifiedAnimator = GetComponent<UnifiedWeaponAnimator>();
        if (unifiedAnimator != null)
        {
            Debug.Log($"Unified Animator - Current weapon type: {unifiedAnimator.CurrentWeaponType}");
            Debug.Log($"Has WeaponType parameter: {unifiedAnimator.HasWeaponTypeParameter}");
        }
        
        BaseWeapon[] weapons = GetComponentsInChildren<BaseWeapon>(true);
        Debug.Log($"Found {weapons.Length} weapons:");
        for (int i = 0; i < weapons.Length; i++)
        {
            Debug.Log($"  {i}: {weapons[i].WeaponName} - Active: {weapons[i].gameObject.activeInHierarchy}");
        }
    }
    
    void Update()
    {
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            DebugWeaponSystem();
        }
    }
}