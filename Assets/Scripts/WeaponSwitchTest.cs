using UnityEngine;
using SnackAttack.Player;

public class WeaponSwitchTest : MonoBehaviour
{
    private WeaponManager weaponManager;
    
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
        {
            Debug.LogError("=== NO WEAPON MANAGER FOUND! ===");
        }
        else
        {
            Debug.Log("=== WEAPON MANAGER FOUND! ===");
        }
    }
    
    void Update()
    {
        if (weaponManager == null) return;
        
        // Clear, obvious debug messages
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("=== KEY 1 PRESSED - SWITCHING TO KETCHUP ===");
            weaponManager.SwitchToWeapon(0);
            LogCurrentWeapon();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("=== KEY 2 PRESSED - SWITCHING TO ASSAULT RIFLE ===");
            weaponManager.SwitchToWeapon(1);
            LogCurrentWeapon();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("=== KEY 3 PRESSED - SWITCHING TO POPCORN LAUNCHER ===");
            weaponManager.SwitchToWeapon(2);
            LogCurrentWeapon();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== WEAPON STATUS CHECK ===");
            LogAllWeapons();
        }
    }
    
    void LogCurrentWeapon()
    {
        if (weaponManager.CurrentWeapon != null)
        {
            Debug.Log($">>> CURRENT WEAPON: {weaponManager.CurrentWeapon.WeaponName} <<<");
        }
        else
        {
            Debug.Log(">>> CURRENT WEAPON: NULL <<<");
        }
    }
    
    void LogAllWeapons()
    {
        Debug.Log($"Weapon Manager Status:");
        Debug.Log($"- Current Index: {weaponManager.CurrentWeaponIndex}");
        Debug.Log($"- Weapons Array Length: {weaponManager.weapons?.Length ?? 0}");
        
        if (weaponManager.weapons != null)
        {
            for (int i = 0; i < weaponManager.weapons.Length; i++)
            {
                var weapon = weaponManager.weapons[i];
                if (weapon != null)
                {
                    Debug.Log($"- Slot {i}: {weapon.WeaponName} (GameObject: {weapon.gameObject.name}, Active: {weapon.gameObject.activeInHierarchy})");
                }
                else
                {
                    Debug.Log($"- Slot {i}: NULL");
                }
            }
        }
    }
}



