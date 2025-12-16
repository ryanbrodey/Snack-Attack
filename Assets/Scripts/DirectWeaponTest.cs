using UnityEngine;
using SnackAttack.Player;

public class DirectWeaponTest : MonoBehaviour
{
    void Update()
    {
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
        {
            print("NO WEAPON MANAGER FOUND!");
            return;
        }
        
        // Direct weapon switching test
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("=== DIRECT TEST: Switching to weapon 0 ===");
            weaponManager.SwitchToWeapon(0);
            print($"Current weapon after switch: {(weaponManager.CurrentWeapon != null ? weaponManager.CurrentWeapon.WeaponName : "NULL")}");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("=== DIRECT TEST: Switching to weapon 1 ===");
            weaponManager.SwitchToWeapon(1);
            print($"Current weapon after switch: {(weaponManager.CurrentWeapon != null ? weaponManager.CurrentWeapon.WeaponName : "NULL")}");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("=== DIRECT TEST: Switching to weapon 2 ===");
            weaponManager.SwitchToWeapon(2);
            print($"Current weapon after switch: {(weaponManager.CurrentWeapon != null ? weaponManager.CurrentWeapon.WeaponName : "NULL")}");
        }
        
        // Test current weapon info
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("=== WEAPON STATUS ===");
            print($"Current Weapon Index: {weaponManager.CurrentWeaponIndex}");
            print($"Current Weapon: {(weaponManager.CurrentWeapon != null ? weaponManager.CurrentWeapon.WeaponName : "NULL")}");
            print($"Weapons Array Length: {weaponManager.weapons?.Length ?? 0}");
            
            for (int i = 0; i < (weaponManager.weapons?.Length ?? 0); i++)
            {
                var weapon = weaponManager.weapons[i];
                print($"Weapon {i}: {(weapon != null ? weapon.WeaponName + " (Active: " + weapon.gameObject.activeInHierarchy + ")" : "NULL")}");
            }
        }
        
        // Test firing
        if (Input.GetKeyDown(KeyCode.F))
        {
            print("=== FIRING TEST ===");
            weaponManager.DoAttack();
        }
    }
}



