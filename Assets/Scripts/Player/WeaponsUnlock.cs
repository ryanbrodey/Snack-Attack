using UnityEngine;

public class WeaponUnlocks : MonoBehaviour
{
    [Header("Unlocked?")]
    public bool pistolUnlocked = true;
    public bool shotgunUnlocked = false;
    public bool rifleUnlocked = false;

    public bool IsUnlocked(int slot)
    {
        return slot switch
        {
            1 => pistolUnlocked,
            2 => shotgunUnlocked,
            3 => rifleUnlocked,
            _ => false
        };
    }

    public void UnlockShotgun() => shotgunUnlocked = true;
    public void UnlockRifle() => rifleUnlocked = true;
}
