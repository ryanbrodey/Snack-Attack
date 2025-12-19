using UnityEngine;
using SnackAttack.Player;

public class DirectWeaponTest : MonoBehaviour
{
    void Update()
    {
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
        {
            return;
        }
        
        // Direct weapon switching test
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponManager.SwitchToWeapon(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponManager.SwitchToWeapon(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponManager.SwitchToWeapon(2);
        }
        
        // Check current weapon info with T key
        if (Input.GetKeyDown(KeyCode.T))
        {
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            weaponManager.DoAttack();
        }
    }
}





