using UnityEngine;
using SnackAttack.Player;

namespace SnackAttack.Setup
{
    /// <summary>
    /// Detects and fixes input conflicts between multiple FPS controller scripts
    /// Use this to diagnose mouse click freezing issues
    /// </summary>
    public class InputConflictFixer : MonoBehaviour
    {
        [Header("Conflict Detection")]

        [Header("Detection Results")]
        public bool hasWeaponManager = false;
        public bool hasFPSPlayerControllerWithWeapons = false;
        public bool hasConflict = false;
        
        void Start()
        {
            DetectConflicts();
        }
        
        [ContextMenu("Detect Input Conflicts")]
        public void DetectConflicts()
        {
            // Check for conflicting components
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
            
            hasWeaponManager = weaponManager != null;
            hasFPSPlayerControllerWithWeapons = fpsWithWeapons != null;
            hasConflict = hasWeaponManager && hasFPSPlayerControllerWithWeapons;
            
            Debug.Log("=== INPUT CONFLICT DETECTION ===");
            Debug.Log($"WeaponManager found: {hasWeaponManager}");
            Debug.Log($"FPSPlayerControllerWithWeapons found: {hasFPSPlayerControllerWithWeapons}");
            Debug.Log($"CONFLICT DETECTED: {hasConflict}");
            
            if (hasConflict)
            {
                Debug.LogError("🚨 INPUT CONFLICT DETECTED! Both WeaponManager and FPSPlayerControllerWithWeapons are handling mouse input!");
                Debug.LogError("This will cause weapon spam and game freezing when clicking mouse/trackpad.");
                Debug.LogError("Use 'Fix Input Conflicts' to resolve this automatically.");
            }
            else
            {
                Debug.Log("✅ No input conflicts detected.");
            }
        }
        
        [ContextMenu("Fix Input Conflicts")]
        public void FixInputConflicts()
        {
            DetectConflicts();
            
            if (!hasConflict)
            {
                Debug.Log("✅ No conflicts to fix!");
                return;
            }
            
            Debug.Log("🔧 Fixing input conflicts...");
            
            // The fix: Disable WeaponManager since FPSPlayerControllerWithWeapons is more complete
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                Debug.Log("🔧 Disabling WeaponManager to prevent input conflict...");
                weaponManager.enabled = false;
                
                Debug.Log("✅ Input conflict fixed!");
                Debug.Log("WeaponManager has been disabled. FPSPlayerControllerWithWeapons will handle all input.");
                Debug.Log("Controls: F key or Left Click = Attack, 1/2/3 = Switch weapons");
            }
        }
        
        [ContextMenu("Re-enable WeaponManager")]
        public void ReEnableWeaponManager()
        {
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.enabled = true;
                Debug.Log("WeaponManager re-enabled. WARNING: This may cause input conflicts again!");
            }
        }
        
        void Update()
        {
            // Monitor for conflicts during gameplay
            if (hasConflict && Input.GetButtonDown("Fire1"))
            {
                Debug.LogWarning("🚨 Mouse click detected with input conflict! This may cause issues.");
            }
        }
    }
}
