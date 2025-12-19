using UnityEngine;
using SnackAttack.Player;

public class SimpleWeaponDebug : MonoBehaviour
{
    void Update()
    {
        // Test weapon switching
        WeaponManager wm = GetComponent<WeaponManager>();
        if (wm == null) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wm.SwitchToWeapon(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wm.SwitchToWeapon(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            wm.SwitchToWeapon(2);
        }
    }
}





