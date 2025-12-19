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
            
            // Find all conflicting components
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
            FPSController basicFPS = GetComponent<FPSController>();
            
            
            // RESOLUTION STRATEGY:
            // Keep FPSPlayerControllerWithWeapons (most complete)
            // Disable WeaponManager (causes conflicts)
            // Keep basic FPSController if no FPSPlayerControllerWithWeapons
            
            if (fpsWithWeapons != null && weaponManager != null)
            {
                weaponManager.enabled = false;
                conflictsResolved = true;
            }
            else if (weaponManager != null && basicFPS != null && fpsWithWeapons == null)
            {
                conflictsResolved = true;
            }
            else if (fpsWithWeapons != null && basicFPS != null)
            {
                conflictsResolved = true;
            }
            
            // Clean up this script after resolving
            if (conflictsResolved)
            {
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
            }
        }
        
        [ContextMenu("Force Disable FPSPlayerControllerWithWeapons")]
        public void ForceDisableFPSPlayerControllerWithWeapons()
        {
            FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
            if (fpsWithWeapons != null)
            {
                fpsWithWeapons.enabled = false;
            }
        }
    }
}
