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
            Debug.Log("=== Setting up Weapon System ===");
            
            // Find the player object (should be this gameObject or parent)
            GameObject player = gameObject;
            
            // Ensure we have FPSController and WeaponManager
            SetupPlayerComponents(player);
            
            // Setup the three weapons
            SetupKetchupWeapon(player);
            SetupAssaultRifle(player);
            SetupPopcornLauncher(player);
            
            Debug.Log("=== Weapon System Setup Complete! ===");
            Debug.Log("Controls:");
            Debug.Log("- 1, 2, 3: Switch weapons");
            Debug.Log("- Left Click or F: Semi-auto fire");
            Debug.Log("- G: Full-auto fire (assault rifle only)");
            Debug.Log("- R: Reload");
        }
        
        private void SetupPlayerComponents(GameObject player)
        {
            // Ensure FPSController exists
            FPSController fpsController = player.GetComponent<FPSController>();
            if (fpsController == null)
            {
                Debug.LogWarning("FPSController not found! Please add it to the player.");
            }
            
            // Ensure WeaponManager exists
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                weaponManager = player.AddComponent<WeaponManager>();
                Debug.Log("✓ Added WeaponManager component");
            }
            
            // Setup weapon array (will be populated by individual weapon setup)
            if (weaponManager.weapons == null || weaponManager.weapons.Length == 0)
            {
                weaponManager.weapons = new BaseWeapon[3];
                Debug.Log("✓ Initialized weapons array");
            }
        }
        
        private void SetupKetchupWeapon(GameObject player)
        {
            Debug.Log("Setting up Ketchup Weapon...");
            
            // Find existing ketchup weapon or create new one
            Transform ketchupTransform = FindWeaponInHierarchy(player.transform, "Ketchup");
            if (ketchupTransform == null)
            {
                Debug.LogWarning("Ketchup weapon not found in hierarchy! Please ensure it exists.");
                return;
            }
            
            // Add KetchupWeapon component
            KetchupWeapon ketchupWeapon = ketchupTransform.GetComponent<KetchupWeapon>();
            if (ketchupWeapon == null)
            {
                ketchupWeapon = ketchupTransform.gameObject.AddComponent<KetchupWeapon>();
            }
            
            // Configure the weapon
            ketchupWeapon.bulletPrefab = ketchupBulletPrefab;
            ketchupWeapon.shootSound = ketchupShootSound;
            ketchupWeapon.reloadSound = reloadSound;
            ketchupWeapon.emptySound = emptySound;
            
            // Find bullet spawn point
            Transform bulletSpawn = ketchupTransform.Find("BulletSpawn");
            if (bulletSpawn != null)
            {
                ketchupWeapon.bulletSpawn = bulletSpawn;
            }
            
            // Add to weapon manager
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.weapons[0] = ketchupWeapon;
            }
            
            Debug.Log("✓ Ketchup weapon configured (Slot 1)");
        }
        
        private void SetupAssaultRifle(GameObject player)
        {
            Debug.Log("Setting up Assault Rifle...");
            
            // Look for rifle or create weapon holder
            Transform rifleTransform = FindWeaponInHierarchy(player.transform, "Rifle") ?? 
                                     FindWeaponInHierarchy(player.transform, "AssaultRifle");
            
            if (rifleTransform == null)
            {
                // Create a weapon holder for the assault rifle
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
                
                // Create bullet spawn point
                GameObject bulletSpawnObj = new GameObject("BulletSpawn");
                bulletSpawnObj.transform.SetParent(rifleTransform);
                bulletSpawnObj.transform.localPosition = new Vector3(0, 0, 1f);
                bulletSpawnObj.transform.localRotation = Quaternion.identity;
            }
            
            // Add AssaultRifleWeapon component
            AssaultRifleWeapon rifleWeapon = rifleTransform.GetComponent<AssaultRifleWeapon>();
            if (rifleWeapon == null)
            {
                rifleWeapon = rifleTransform.gameObject.AddComponent<AssaultRifleWeapon>();
            }
            
            // Configure the weapon
            rifleWeapon.bulletPrefab = glizzyBulletPrefab;
            
            // Find or assign bullet spawn
            Transform bulletSpawn = rifleTransform.Find("BulletSpawn");
            if (bulletSpawn != null)
            {
                rifleWeapon.bulletSpawn = bulletSpawn;
            }
            
            // Add to weapon manager
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.weapons[1] = rifleWeapon;
            }
            
            Debug.Log("✓ Assault rifle configured (Slot 2)");
        }
        
        private void SetupPopcornLauncher(GameObject player)
        {
            Debug.Log("Setting up Popcorn Launcher...");
            
            // Look for popcorn bucket or create weapon holder
            Transform launcherTransform = FindWeaponInHierarchy(player.transform, "PopcornBucket") ?? 
                                         FindWeaponInHierarchy(player.transform, "Launcher");
            
            if (launcherTransform == null)
            {
                // Create a weapon holder for the launcher
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
                
                // Create bullet spawn point
                GameObject bulletSpawnObj = new GameObject("BulletSpawn");
                bulletSpawnObj.transform.SetParent(launcherTransform);
                bulletSpawnObj.transform.localPosition = new Vector3(0, 0, 1.5f);
                bulletSpawnObj.transform.localRotation = Quaternion.identity;
            }
            
            // Add PopcornLauncherWeapon component
            PopcornLauncherWeapon launcherWeapon = launcherTransform.GetComponent<PopcornLauncherWeapon>();
            if (launcherWeapon == null)
            {
                launcherWeapon = launcherTransform.gameObject.AddComponent<PopcornLauncherWeapon>();
            }
            
            // Configure the weapon
            launcherWeapon.pelletPrefab = popcornBulletPrefab;
            
            // Find or assign bullet spawn
            Transform bulletSpawn = launcherTransform.Find("BulletSpawn");
            if (bulletSpawn != null)
            {
                launcherWeapon.bulletSpawn = bulletSpawn;
            }
            
            // Add to weapon manager
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.weapons[2] = launcherWeapon;
            }
            
            Debug.Log("✓ Popcorn launcher configured (Slot 3)");
        }
        
        private Transform FindWeaponInHierarchy(Transform parent, string weaponName)
        {
            // Recursively search for weapon by name
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
            // Look for WeaponSocket in hierarchy
            Transform socket = FindWeaponInHierarchy(parent, "WeaponSocket");
            return socket;
        }
    }
}

