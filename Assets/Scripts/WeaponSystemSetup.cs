using UnityEngine;
using SnackAttack.Player;
using SnackAttack.Weapons;

namespace SnackAttack.Setup
{
    /// <summary>
    /// Helper script to set up the new weapon system on existing player prefabs
    /// </summary>
    public class WeaponSystemSetup : MonoBehaviour
    {
        [Header("Weapon Prefabs")]
        public GameObject ketchupBulletPrefab;
        public GameObject glizzyBulletPrefab; // For assault rifle
        public GameObject popcornBulletPrefab;
        
        [Header("Audio Clips")]
        public AudioClip ketchupShootSound;
        public AudioClip rifleShootSound;
        public AudioClip launcherShootSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
        public AudioClip explosionSound;
        
        [Header("Effects")]
        public GameObject explosionEffect;
        
        [ContextMenu("Setup Weapon System")]
        public void SetupWeaponSystem()
        {
            GameObject player = gameObject;
            
            SetupPlayerComponents(player);
            
            // Setup weapons
            SetupKetchupWeapon(player);
            SetupAssaultRifle(player);
            SetupPopcornLauncher(player);
        }
        
        private void SetupPlayerComponents(GameObject player)
        {
            FPSController fpsController = player.GetComponent<FPSController>();
            
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                weaponManager = player.AddComponent<WeaponManager>();
            }
            
            if (weaponManager.weapons == null || weaponManager.weapons.Length == 0)
            {
                weaponManager.weapons = new BaseWeapon[3];
            }
        }
        
        private void SetupKetchupWeapon(GameObject player)
        {
            // Find ketchup weapon
            Transform ketchupTransform = FindWeaponInHierarchy(player.transform, "Ketchup");
            if (ketchupTransform == null)
            {
                return;
            }
            
            KetchupWeapon ketchupWeapon = ketchupTransform.GetComponent<KetchupWeapon>();
            if (ketchupWeapon == null)
            {
                ketchupWeapon = ketchupTransform.gameObject.AddComponent<KetchupWeapon>();
            }
            
            ketchupWeapon.bulletPrefab = ketchupBulletPrefab;
            ketchupWeapon.shootSound = ketchupShootSound;
            ketchupWeapon.reloadSound = reloadSound;
            ketchupWeapon.emptySound = emptySound;
            
            Transform bulletSpawn = ketchupTransform.Find("BulletSpawn");
            if (bulletSpawn != null)
            {
                ketchupWeapon.bulletSpawn = bulletSpawn;
            }
            
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.weapons[0] = ketchupWeapon;
            }
        }
        
        private void SetupAssaultRifle(GameObject player)
        {
            // Find rifle
            Transform rifleTransform = FindWeaponInHierarchy(player.transform, "Rifle") ?? 
                                     FindWeaponInHierarchy(player.transform, "AssaultRifle");
            
            if (rifleTransform == null)
            {
                GameObject rifleHolder = new GameObject("AssaultRifle");
                Transform weaponSocket = FindWeaponSocket(player.transform);
                if (weaponSocket != null)
                {
                    rifleHolder.transform.SetParent(weaponSocket);
                }
                else
                {
                    rifleHolder.transform.SetParent(player.transform);
                }
                rifleHolder.transform.localPosition = Vector3.zero;
                rifleHolder.transform.localRotation = Quaternion.identity;
                rifleTransform = rifleHolder.transform;
                
                GameObject bulletSpawnObj = new GameObject("BulletSpawn");
                bulletSpawnObj.transform.SetParent(rifleTransform);
                bulletSpawnObj.transform.localPosition = new Vector3(0, 0, 1f);
                bulletSpawnObj.transform.localRotation = Quaternion.identity;
            }
            
            AssaultRifleWeapon rifleWeapon = rifleTransform.GetComponent<AssaultRifleWeapon>();
            if (rifleWeapon == null)
            {
                rifleWeapon = rifleTransform.gameObject.AddComponent<AssaultRifleWeapon>();
            }
            
            rifleWeapon.bulletPrefab = glizzyBulletPrefab;
            
            Transform bulletSpawn = rifleTransform.Find("BulletSpawn");
            if (bulletSpawn != null)
            {
                rifleWeapon.bulletSpawn = bulletSpawn;
            }
            
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.weapons[1] = rifleWeapon;
            }
        }
        
        private void SetupPopcornLauncher(GameObject player)
        {
            // Find popcorn launcher
            Transform launcherTransform = FindWeaponInHierarchy(player.transform, "PopcornBucket") ?? 
                                         FindWeaponInHierarchy(player.transform, "Launcher");
            
            if (launcherTransform == null)
            {
                GameObject launcherHolder = new GameObject("PopcornLauncher");
                Transform weaponSocket = FindWeaponSocket(player.transform);
                if (weaponSocket != null)
                {
                    launcherHolder.transform.SetParent(weaponSocket);
                }
                else
                {
                    launcherHolder.transform.SetParent(player.transform);
                }
                launcherHolder.transform.localPosition = Vector3.zero;
                launcherHolder.transform.localRotation = Quaternion.identity;
                launcherTransform = launcherHolder.transform;
                
                GameObject bulletSpawnObj = new GameObject("BulletSpawn");
                bulletSpawnObj.transform.SetParent(launcherTransform);
                bulletSpawnObj.transform.localPosition = new Vector3(0, 0, 1.5f);
                bulletSpawnObj.transform.localRotation = Quaternion.identity;
            }
            
            PopcornLauncherWeapon launcherWeapon = launcherTransform.GetComponent<PopcornLauncherWeapon>();
            if (launcherWeapon == null)
            {
                launcherWeapon = launcherTransform.gameObject.AddComponent<PopcornLauncherWeapon>();
            }
            
            launcherWeapon.pelletPrefab = popcornBulletPrefab;
            
            Transform bulletSpawn = launcherTransform.Find("BulletSpawn");
            if (bulletSpawn != null)
            {
                launcherWeapon.bulletSpawn = bulletSpawn;
            }
            
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.weapons[2] = launcherWeapon;
            }
        }
        
        private Transform FindWeaponInHierarchy(Transform parent, string weaponName)
        {
            Transform found = parent.Find(weaponName);
            if (found != null) return found;
            
            foreach (Transform child in parent)
            {
                found = FindWeaponInHierarchy(child, weaponName);
                if (found != null) return found;
            }
            
            return null;
        }
        
        private Transform FindWeaponSocket(Transform parent)
        {
            return FindWeaponInHierarchy(parent, "WeaponSocket");
        }
    }
}

