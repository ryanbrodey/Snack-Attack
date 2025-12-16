using System.Collections;
using UnityEngine;

namespace SnackAttack.Weapons
{
    // Assault rifle weapon - semi-auto (F) and full-auto (G) modes
    public class AssaultRifleWeapon : BaseWeapon
    {
        [Header("Assault Rifle Settings")]
        public Transform bulletSpawn;
        public GameObject bulletPrefab;
        public float bulletSpeed = 50f;
        public int maxAmmo = 30;
        public float reloadTime = 2.5f;
        
        [Header("Fire Modes")]
        public float semiAutoRate = 0.3f; // F key fire rate
        public float fullAutoRate = 0.1f; // G key fire rate (much faster)
        
        [Header("Audio")]
        public AudioClip shootSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
        
        // Ammo and firing system
        private int currentAmmo;
        private bool isReloading = false;
        private bool isFullAutoFiring = false;
        private AudioSource audioSource;
        private Coroutine fullAutoCoroutine;
        
        // Camera cache for performance
        private Camera playerCameraCache;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Assault rifle stats
            weaponName = "Assault Rifle";
            cooldown = semiAutoRate; // Default to semi-auto rate
            dmg = 20f; // Higher damage than ketchup gun
            reach = 75f; // Longer range
            
            // Animation names (using rifle animations)
            idleAnim = "rifle idle";
            walkAnim = "rifle walk";
            runAnim = "rifle run";
            attackAnim = "rifle shoot"; // Will need to check actual animation names
            
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
            
            // Auto-find bullet spawn if not assigned
            if (bulletSpawn == null)
            {
                // Look for common bullet spawn names
                bulletSpawn = transform.Find("BulletSpawn");
                if (bulletSpawn == null)
                {
                    Transform weapon = transform.Find("Rifle") ?? transform.Find("AssaultRifle");
                    if (weapon != null)
                    {
                        bulletSpawn = weapon.Find("BulletSpawn");
                    }
                }
                
                if (bulletSpawn == null)
                {
                    // BulletSpawn not found
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
            
            // Handle full-auto firing with G key
            if (Input.GetKeyDown(KeyCode.G) && !isReloading)
            {
                StartFullAuto();
            }
            
            if (Input.GetKeyUp(KeyCode.G) || isReloading)
            {
                StopFullAuto();
            }
        }
        
        public override void Attack()
        {
            // This handles F key and mouse click (semi-auto)
            if (isFullAutoFiring) return; // Don't interrupt full-auto
            
            // Check if we can shoot
            if (isReloading)
            {
                // Cannot shoot while reloading
                return;
            }
            
            if (currentAmmo <= 0)
            {
                PlaySound(emptySound);
                // Out of ammo
                if (!isReloading)
                    StartReload();
                return;
            }
            
            // Set to semi-auto rate and fire
            cooldown = semiAutoRate;
            base.Attack();
        }
        
        private void StartFullAuto()
        {
            if (isFullAutoFiring || isReloading) return;
            
            // Full-auto mode activated
            isFullAutoFiring = true;
            cooldown = fullAutoRate; // Switch to full-auto rate
            
            // Start continuous firing coroutine
            fullAutoCoroutine = StartCoroutine(FullAutoFiring());
        }
        
        private void StopFullAuto()
        {
            if (!isFullAutoFiring) return;
            
            // Full-auto mode deactivated
            isFullAutoFiring = false;
            cooldown = semiAutoRate; // Switch back to semi-auto rate
            
            if (fullAutoCoroutine != null)
            {
                StopCoroutine(fullAutoCoroutine);
                fullAutoCoroutine = null;
            }
        }
        
        private IEnumerator FullAutoFiring()
        {
            while (isFullAutoFiring && !isReloading)
            {
                if (currentAmmo <= 0)
                {
                    PlaySound(emptySound);
                    StartReload();
                    break;
                }
                
                if (CanAttack)
                {
                    // Fire a shot
                    FireBullet();
                }
                
                yield return new WaitForSeconds(fullAutoRate);
            }
            
            isFullAutoFiring = false;
        }
        
        protected override void PerformAttack()
        {
            FireBullet();
            
            // Complete attack quickly for rapid fire
            StartCoroutine(CompleteAttackAfterAnimation());
        }
        
        private void FireBullet()
        {
            // Consume ammo
            currentAmmo--;
            
            // Update attack timing
            attacking = true;
            canAttack = false;
            lastAttackTime = Time.time;
            
            // Fire bullet with crosshair aiming
            if (bulletPrefab == null || bulletSpawn == null) 
            {
                Debug.LogWarning("AssaultRifleWeapon: Missing bullet prefab or spawn point");
                return;
            }
            
            // Find player camera
            Camera playerCamera = GetPlayerCamera();
            if (playerCamera == null) 
            {
                Debug.LogWarning("AssaultRifleWeapon: No player camera found, using forward direction");
                // Fallback to old behavior
                GameObject bulletFallback = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                Rigidbody rb = bulletFallback.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = bulletSpawn.forward * bulletSpeed;
                }
                PlaySound(shootSound);
                return;
            }
            
            // Spawn from gun barrel, but travel in camera forward direction (where crosshair points)
            Vector3 spawnPosition = bulletSpawn.position;
            Vector3 aimDirection = playerCamera.transform.forward; // Crosshair direction
            
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
            
            // Play shoot sound
            PlaySound(shootSound);
            
            // Trigger animation
            if (anim != null)
            {
                anim.SetTrigger("Shoot");
            }
            
            // Debug visualization (only for semi-auto to avoid spam)
            if (!isFullAutoFiring)
            {
                CrosshairAiming.DrawAimDebug(bulletSpawn.position, playerCamera, 1f);
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
            yield return new WaitForSeconds(0.1f); // Quick recovery for rapid fire
            CompleteAttack();
        }
        
        private void StartReload()
        {
            if (isReloading) return;
            
            // Stop full-auto if reloading
            StopFullAuto();
            
            isReloading = true;
            // Reloading weapon
            
            PlaySound(reloadSound);
            StartCoroutine(ReloadCoroutine());
        }
        
        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(reloadTime);
            
            currentAmmo = maxAmmo;
            isReloading = false;
            
            // Reload complete
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
        public bool IsFullAutoFiring => isFullAutoFiring;
        public float ReloadProgress => isReloading ? (reloadTime - Time.time + lastAttackTime) / reloadTime : 0f;
    }
}
