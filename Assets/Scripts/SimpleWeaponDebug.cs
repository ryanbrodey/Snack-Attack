using UnityEngine;
using SnackAttack.Player;

public class SimpleWeaponDebug : MonoBehaviour
{
    void Update()
    {
        // Debug key presses
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("Key 1 pressed!");
            WeaponManager wm = GetComponent<WeaponManager>();
            if (wm != null)
            {
                print("WeaponManager found, trying to switch to weapon 0");
                wm.SwitchToWeapon(0);
            }
            else
            {
                print("NO WEAPONMANAGER FOUND!");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("Key 2 pressed!");
            WeaponManager wm = GetComponent<WeaponManager>();
            if (wm != null)
            {
                wm.SwitchToWeapon(1);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("Key 3 pressed!");
            WeaponManager wm = GetComponent<WeaponManager>();
            if (wm != null)
            {
                wm.SwitchToWeapon(2);
            }
        }
        
        // Debug current weapon
        if (Input.GetKeyDown(KeyCode.T))
        {
            WeaponManager wm = GetComponent<WeaponManager>();
            if (wm != null)
            {
                print($"Current weapon: {(wm.CurrentWeapon != null ? wm.CurrentWeapon.WeaponName : "NULL")}");
                print($"Weapons array length: {wm.weapons?.Length ?? 0}");
                for (int i = 0; i < (wm.weapons?.Length ?? 0); i++)
                {
                    print($"Weapon {i}: {(wm.weapons[i] != null ? wm.weapons[i].WeaponName : "NULL")}");
                }
            }
        }
    }
}





