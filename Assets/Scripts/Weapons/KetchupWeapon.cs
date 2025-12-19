using System.Collections;
using UnityEngine;
using SnackAttack.Player;

namespace SnackAttack.Weapons
{
    // Ketchup bottle weapon - semi-automatic shooting
    public class KetchupWeapon : BaseWeapon
    {
        [Header("Stats")]
        public WeaponStats weaponStats; // reference to weapon stats for upgrades
        
        [Header("Ketchup Gun Settings")]
        public Transform bulletSpawn;
        public GameObject bulletPrefab;
        public float bulletSpeed = 40f;
        public int maxAmmo = 30;
        public float reloadTime = 2f;
        
        [Header("Audio")]
        public AudioClip shootSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
        
        // Ammo system
        private int currentAmmo;
        private bool isReloading = false;
        private AudioSource audioSource;
        
        // Camera cache for performance
        private Camera playerCameraCache;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Ketchup gun stats
            weaponName = "Ketchup Gun";
            cooldown = 0.5f; // Semi-auto rate
            dmg = 15f;
            reach = 50f; // Good range for projectile weapon
            
            // Animation names (using pistol animations)
            idleAnim = "pistol idle";
            walkAnim = "pistol walk";
            runAnim = "pistol run";
            attackAnim = "pistol shooting";
            
            // Initialize ammo
            currentAmmo = maxAmmo;
            
            // Get audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        protected override void Start()
        {
            base.Start();
            
            // try to find weapon stats if we didn't assign it manually
            if (weaponStats == null)
                weaponStats = FindObjectOfType<WeaponStats>();
            
            // Auto-find bullet spawn if not assigned
            if (bulletSpawn == null)
            {
                Transform ketchup = transform.Find("Ketchup");
                if (ketchup != null)
                {
                    bulletSpawn = ketchup.Find("BulletSpawn");
                }
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Handle reload input
            if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
            {
                StartReload();
            }
        }
        
        public override void Attack()
        {
            // Check if we can shoot
            if (isReloading)
            {
                return;
            }
            
            if (currentAmmo <= 0)
            {
                PlaySound(emptySound);
                if (!isReloading)
                    StartReload();
                return;
            }
            
            // Call base attack (handles cooldown and state)
            base.Attack();
        }
        
        protected override void PerformAttack()
        {
            // pull the current damage from weapon stats (for upgrades)
            if (weaponStats != null)
                dmg = weaponStats.pistolDamage;
            
            currentAmmo--;
            
            FireBullet();
            PlaySound(shootSound);
            
            StartCoroutine(CompleteAttackAfterAnimation());
        }
        
        private void FireBullet()
        {
            if (bulletPrefab == null || bulletSpawn == null) 
            {
                return;
            }
            
            Camera playerCamera = GetPlayerCamera();
            if (playerCamera == null) 
            {
                // Fallback
                GameObject bulletFallback = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                Rigidbody rb = bulletFallback.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = bulletSpawn.forward * bulletSpeed;
                }
                return;
            }
            
            // Spawn from gun barrel, travel toward crosshair
            Vector3 spawnPosition = bulletSpawn.position;
            Vector3 aimDirection = CrosshairAiming.GetBulletDirectionFromSpawnToCrosshair(
                spawnPosition, 
                playerCamera, 
                1000f
            );
            
            GameObject bulletObj = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            
            BulletController bulletScript = bulletObj.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(aimDirection, spawnPosition, bulletSpeed);
            }
            else
            {
                // Fallback
                Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = aimDirection * bulletSpeed;
                }
            }
        }
        
        private Camera GetPlayerCamera()
        {
            // Return cached camera if we have it
            if (playerCameraCache != null)
                return playerCameraCache;
            
            Camera cam = null;
            
            // Method 1: Try to find camera in parent hierarchy (weapons are children of camera via weaponHolder)
            cam = GetComponentInParent<Camera>();
            if (cam != null)
            {
                playerCameraCache = cam;
                return cam;
            }
            
            // Method 2: Try to find FPSPlayerController (current testing setup) and get its camera
            FPSPlayerController playerController = FindObjectOfType<FPSPlayerController>();
            if (playerController != null && playerController.playerCamera != null)
            {
                playerCameraCache = playerController.playerCamera;
                return playerController.playerCamera;
            }
            
            // Method 3: Try to find FPSPlayerControllerWithWeapons (long-term setup) and get its camera
            FPSPlayerControllerWithWeapons weaponsController = FindObjectOfType<FPSPlayerControllerWithWeapons>();
            if (weaponsController != null && weaponsController.playerCamera != null)
            {
                playerCameraCache = weaponsController.playerCamera;
                return weaponsController.playerCamera;
            }
            
            // Method 4: Try to find FPSController (namespace version) and get its PlayerCamera property
            SnackAttack.Player.FPSController fpsController = FindObjectOfType<SnackAttack.Player.FPSController>();
            if (fpsController != null && fpsController.PlayerCamera != null)
            {
                playerCameraCache = fpsController.PlayerCamera;
                return fpsController.PlayerCamera;
            }
            
            // Method 5: Fallback to Camera.main
            if (Camera.main != null)
            {
                playerCameraCache = Camera.main;
                return Camera.main;
            }
            
            // Method 6: Last resort - find any camera
            cam = FindObjectOfType<Camera>();
            if (cam != null)
            {
                playerCameraCache = cam;
            }
            
            return cam;
        }
        
        private IEnumerator CompleteAttackAfterAnimation()
        {
            yield return new WaitForSeconds(0.3f);
            CompleteAttack();
        }
        
        private void StartReload()
        {
            if (isReloading) return;
            
            isReloading = true;
            
            PlaySound(reloadSound);
            StartCoroutine(ReloadCoroutine());
        }
        
        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(reloadTime);
            
            currentAmmo = maxAmmo;
            isReloading = false;
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        // Public getters
        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public bool IsReloading => isReloading;
        public float ReloadProgress => isReloading ? (reloadTime - Time.time + lastAttackTime) / reloadTime : 0f;
    }
}
