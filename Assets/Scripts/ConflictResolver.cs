using UnityEngine;
using SnackAttack.Player;

namespace SnackAttack.Setup
{
    /// <summary>
    /// Resolves input conflicts by disabling conflicting scripts
    /// This will fix the F key and mouse click freezing issues
    /// </summary>
    public class ConflictResolver : MonoBehaviour
    {
        [Header("Conflict Resolution")]

        [Header("Status")]
        public bool conflictsResolved = false;
        
        void Start()
        {
            // Auto-resolve conflicts on start
            ResolveAllConflicts();
        }
        
        [ContextMenu("Resolve All Conflicts")]
        public void ResolveAllConflicts()
        {
            Debug.Log("🔧 [ConflictResolver] Starting conflict resolution...");
            
            // Find all conflicting components
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
            FPSController basicFPS = GetComponent<FPSController>();
            
            // Log what we found
            Debug.Log($"Found WeaponManager: {weaponManager != null}");
            Debug.Log($"Found FPSPlayerControllerWithWeapons: {fpsWithWeapons != null}");
            Debug.Log($"Found FPSController: {basicFPS != null}");
            
            // RESOLUTION STRATEGY:
            // Keep FPSPlayerControllerWithWeapons (most complete)
            // Disable WeaponManager (causes conflicts)
            // Keep basic FPSController if no FPSPlayerControllerWithWeapons
            
            if (fpsWithWeapons != null && weaponManager != null)
            {
                Debug.Log("🔧 Disabling WeaponManager to prevent input conflicts...");
                weaponManager.enabled = false;
                conflictsResolved = true;
                
                Debug.Log("✅ CONFLICT RESOLVED!");
                Debug.Log("✅ WeaponManager disabled");
                Debug.Log("✅ FPSPlayerControllerWithWeapons will handle all input");
                Debug.Log("✅ Controls: F key or Left Click = Attack, 1/2/3 = Switch weapons");
            }
            else if (weaponManager != null && basicFPS != null && fpsWithWeapons == null)
            {
                Debug.Log("✅ Using WeaponManager + FPSController setup (no conflicts detected)");
                conflictsResolved = true;
            }
            else if (fpsWithWeapons != null && basicFPS != null)
            {
                Debug.Log("✅ Using FPSPlayerControllerWithWeapons (includes movement + weapons)");
                // No need to disable FPSController as it doesn't handle weapon input
                conflictsResolved = true;
            }
            else
            {
                Debug.LogWarning("⚠️ No FPS controller found! Please add FPSController or FPSPlayerControllerWithWeapons");
            }
            
            // Clean up this script after resolving
            if (conflictsResolved)
            {
                Debug.Log("🧹 Conflict resolved! Removing ConflictResolver script...");
                if (Application.isPlaying)
                {
                    Destroy(this);
                }
                else
                {
                    DestroyImmediate(this);
                }
            }
        }
        
        [ContextMenu("Force Disable WeaponManager")]
        public void ForceDisableWeaponManager()
        {
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.enabled = false;
                Debug.Log("🔧 WeaponManager forcefully disabled!");
            }
            else
            {
                Debug.Log("No WeaponManager found to disable.");
            }
        }
        
        [ContextMenu("Force Disable FPSPlayerControllerWithWeapons")]
        public void ForceDisableFPSPlayerControllerWithWeapons()
        {
            FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
            if (fpsWithWeapons != null)
            {
                fpsWithWeapons.enabled = false;
                Debug.Log("🔧 FPSPlayerControllerWithWeapons forcefully disabled!");
            }
            else
            {
                Debug.Log("No FPSPlayerControllerWithWeapons found to disable.");
            }
        }
    }
}
