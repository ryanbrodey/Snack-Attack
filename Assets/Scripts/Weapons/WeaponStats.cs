using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("Base Damage")]
    public int pistolDamage = 1;
    public int shotgunDamage = 2;
    public int rifleDamage = 3;

    [Header("Damage Increase Per Upgrade")]
    public int pistolUpgradeAmount = 1;
    public int shotgunUpgradeAmount = 1;
    public int rifleUpgradeAmount = 1;

    public void UpgradePistol()
    {
        pistolDamage += pistolUpgradeAmount;
    }

    public void UpgradeShotgun()
    {
        shotgunDamage += shotgunUpgradeAmount;
    }

    public void UpgradeRifle()
    {
        rifleDamage += rifleUpgradeAmount;
    }
}
