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
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            FPSPlayerControllerWithWeapons fpsWithWeapons = GetComponent<FPSPlayerControllerWithWeapons>();
            
            hasWeaponManager = weaponManager != null;
            hasFPSPlayerControllerWithWeapons = fpsWithWeapons != null;
            hasConflict = hasWeaponManager && hasFPSPlayerControllerWithWeapons;
        }
        
        [ContextMenu("Fix Input Conflicts")]
        public void FixInputConflicts()
        {
            DetectConflicts();
            
            if (!hasConflict) return;
            
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.enabled = false;
            }
        }
        
        [ContextMenu("Re-enable WeaponManager")]
        public void ReEnableWeaponManager()
        {
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.enabled = true;
            }
        }
    }
}
