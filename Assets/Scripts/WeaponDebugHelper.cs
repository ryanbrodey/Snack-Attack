using UnityEngine;
using SnackAttack.Player;
using SnackAttack.Weapons;

namespace SnackAttack.Debug
{
    /// <summary>
    /// Debug helper to diagnose weapon switching issues
    /// </summary>
    public class WeaponDebugHelper : MonoBehaviour
    {
        [ContextMenu("Debug Weapon System")]
        public void DebugWeaponSystem()
        {
            Debug.Log("=== WEAPON SYSTEM DEBUG ===");
            
            // Check for WeaponManager
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                Debug.LogError("❌ NO WEAPONMANAGER FOUND! This is the problem - add WeaponManager component to this GameObject.");
                return;
            }
            
            Debug.Log("✅ WeaponManager found");
            
            // Check weapons array
            if (weaponManager.weapons == null)
            {
                Debug.LogError("❌ Weapons array is NULL! Run the WeaponSystemSetup script.");
                return;
            }
            
            Debug.Log($"✅ Weapons array exists with {weaponManager.weapons.Length} slots");
            
            // Check each weapon slot
            for (int i = 0; i < weaponManager.weapons.Length; i++)
            {
                if (weaponManager.weapons[i] == null)
                {
                    Debug.LogWarning($"⚠️ Weapon slot {i + 1} is NULL");
                }
                else
                {
                    Debug.Log($"✅ Weapon slot {i + 1}: {weaponManager.weapons[i].WeaponName}");
                }
            }
            
            // Check current weapon
            Debug.Log($"Current weapon index: {weaponManager.CurrentWeaponIndex}");
            if (weaponManager.CurrentWeapon != null)
            {
                Debug.Log($"Current weapon: {weaponManager.CurrentWeapon.WeaponName}");
            }
            else
            {
                Debug.LogWarning("⚠️ Current weapon is NULL");
            }
            
            // Check for FPSController
            var fpsController = GetComponent<SnackAttack.Player.FPSController>();
            if (fpsController == null)
            {
                Debug.LogWarning("⚠️ FPSController not found - weapon switching might not work");
            }
            else
            {
                Debug.Log("✅ FPSController found");
            }
            
            Debug.Log("=== DEBUG COMPLETE ===");
        }
        
        [ContextMenu("Test Weapon Switching")]
        public void TestWeaponSwitching()
        {
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                Debug.LogError("No WeaponManager found!");
                return;
            }
            
            Debug.Log("Testing weapon switching...");
            
            // Test switching to each weapon
            for (int i = 0; i < 3; i++)
            {
                Debug.Log($"Attempting to switch to weapon {i + 1}...");
                weaponManager.SwitchToWeapon(i);
                
                if (weaponManager.CurrentWeapon != null)
                {
                    Debug.Log($"✅ Successfully switched to: {weaponManager.CurrentWeapon.WeaponName}");
                }
                else
                {
                    Debug.LogError($"❌ Failed to switch to weapon {i + 1}");
                }
            }
        }
        
        void Update()
        {
            // Debug key presses
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Key 1 pressed!");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Key 2 pressed!");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Key 3 pressed!");
            }
        }
    }
}
