using UnityEngine;
using SnackAttack.Player;

/// <summary>
/// Automatically disables WeaponManager when FPSPlayerControllerWithWeapons is present
/// This prevents input conflicts that cause game freezing
/// </summary>
public class DeprecateWeaponManager : MonoBehaviour
{
    [Header("Auto-Fix Input Conflicts")]
    [TextArea(5, 8)]
    [SerializeField] private string info = @"
This script automatically fixes input conflicts by:
1. Detecting if both WeaponManager and FPSPlayerControllerWithWeapons exist
2. Disabling WeaponManager to prevent conflicts
3. Ensuring only one input handler is active

This fixes the F key and mouse click freezing issues!";

    void Awake()
    {
        FixInputConflicts();
    }
    
    void FixInputConflicts()
    {
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
        
        if (weaponManager != null && fpsWithWeapons != null)
        {
            Debug.LogWarning("🚨 [DeprecateWeaponManager] INPUT CONFLICT DETECTED!");
            Debug.LogWarning("Both WeaponManager and FPSPlayerControllerWithWeapons are present.");
            Debug.LogWarning("Disabling WeaponManager to prevent F key and mouse click conflicts...");
            
            weaponManager.enabled = false;
            
            Debug.Log("✅ [DeprecateWeaponManager] Conflict resolved!");
            Debug.Log("✅ WeaponManager disabled");
            Debug.Log("✅ FPSPlayerControllerWithWeapons will handle all input");
            Debug.Log("✅ Controls: F key or Left Click = Attack");
            
            // Remove this script after fixing
            Destroy(this);
        }
        else if (weaponManager != null)
        {
            Debug.Log("✅ [DeprecateWeaponManager] WeaponManager active (no conflicts)");
            Destroy(this);
        }
        else if (fpsWithWeapons != null)
        {
            Debug.Log("✅ [DeprecateWeaponManager] FPSPlayerControllerWithWeapons active (no conflicts)");
            Destroy(this);
        }
        else
        {
            Debug.LogWarning("⚠️ [DeprecateWeaponManager] No FPS controller found!");
            Destroy(this);
        }
    }
}
