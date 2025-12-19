using UnityEngine;
using SnackAttack.Player;

public class WeaponSwitchTest : MonoBehaviour
{
    private WeaponManager weaponManager;
    
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
    }
    
    void Update()
    {
        if (weaponManager == null) return;
        
        // Clear, obvious debug messages
        // Test weapon switching with number keys
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
    }
}





