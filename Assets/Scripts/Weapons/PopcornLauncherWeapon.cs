using System.Collections;
using UnityEngine;

namespace SnackAttack.Weapons
{
    // Shotgun weapon - fires multiple pellets with spread pattern
    public class PopcornLauncherWeapon : BaseWeapon
    {
        [Header("Shotgun Settings")]
        public Transform bulletSpawn;
        public GameObject pelletPrefab; // Renamed from bulletPrefab
        public float pelletSpeed = 50f; // Speed per pellet
        public int pelletsPerShot = 8; // Number of pellets per shot
        public float spreadAngle = 15f; // Spread angle in degrees
        public int maxAmmo = 8; // Shotgun shells
        public float reloadTime = 2.5f; // Reload time
        
        [Header("Shotgun Stats")]
        public float pelletDamage = 12f; // Damage per pellet
        public int penetrationCount = 2; // How many enemies each pellet can penetrate
        
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
            
            // Shotgun stats
            weaponName = "Shotgun";
            cooldown = 0.8f; // Semi-auto, slower than pistol but faster than original
            dmg = pelletDamage; // Damage per pellet
            reach = 50f; // Shorter range than rifles
            
            // Animation names (using shotgun animations)
            idleAnim = "shotgun idle";
            walkAnim = "shotgun walk";
            runAnim = "shotgun run";
            attackAnim = "shotgun shoot";
            
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
                // Look for shotgun or launcher
                Transform shotgun = transform.Find("Shotgun") ?? transform.Find("PopcornBucket") ?? transform.Find("Launcher");
                if (shotgun != null)
                {
                    bulletSpawn = shotgun.Find("BulletSpawn");
                }
                
                if (bulletSpawn == null)
                {
                    bulletSpawn = transform.Find("BulletSpawn");
                }
                
                if (bulletSpawn == null)
                {
                    Debug.LogWarning("Shotgun: BulletSpawn not found");
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
            // Consume ammo
            currentAmmo--;
            
            // Fire shotgun blast
            FireShotgunBlast();
            
            // Play shoot sound
            PlaySound(shootSound);
            
            // Complete attack after animation
            StartCoroutine(CompleteAttackAfterAnimation());
        }
        
        private void FireShotgunBlast()
        {
            if (pelletPrefab == null || bulletSpawn == null) 
            {
                Debug.LogWarning("Shotgun: Missing pellet prefab or spawn point");
                return;
            }
            
            Camera playerCamera = GetPlayerCamera();
            if (playerCamera == null) 
            {
                Debug.LogWarning("Shotgun: No player camera found");
                return;
            }
            
            Vector3 spawnPosition = bulletSpawn.position;
            Vector3 centerDirection = playerCamera.transform.forward;
            
            Vector3[] directions = new Vector3[pelletsPerShot];
            Vector3 right = playerCamera.transform.right;
            Vector3 up = playerCamera.transform.up;
            
            for (int i = 0; i < pelletsPerShot; i++)
            {
                float randomAngle = Random.Range(0f, spreadAngle * Mathf.Deg2Rad);
                float randomDirection = Random.Range(0f, 2f * Mathf.PI);
                Vector3 spreadOffset = (right * Mathf.Cos(randomDirection) + up * Mathf.Sin(randomDirection)) * Mathf.Tan(randomAngle);
                directions[i] = (centerDirection + spreadOffset).normalized;
            }
            
            for (int i = 0; i < pelletsPerShot; i++)
            {
                GameObject pellet = Instantiate(pelletPrefab, spawnPosition, Quaternion.identity);
                BulletController pelletScript = pellet.GetComponent<BulletController>();
                if (pelletScript != null)
                {
                    pelletScript.canPenetrate = true;
                    pelletScript.maxPenetrations = penetrationCount;
                    pelletScript.damage = pelletDamage;
                    pelletScript.maxRange = reach;
                    pelletScript.speed = pelletSpeed;
                    pelletScript.Initialize(directions[i], spawnPosition, pelletSpeed);
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
            // Wait for shotgun animation
            yield return new WaitForSeconds(0.5f);
            CompleteAttack();
        }
        
        private void StartReload()
        {
            if (isReloading) return;
            
            isReloading = true;
            Debug.Log("Shotgun: Reloading...");
            
            PlaySound(reloadSound);
            StartCoroutine(ReloadCoroutine());
        }
        
        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(reloadTime);
            
            currentAmmo = maxAmmo;
            isReloading = false;
            
            Debug.Log("Shotgun: Reload complete");
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
        public int PelletsPerShot => pelletsPerShot;
        public float SpreadAngle => spreadAngle;
        public float ReloadProgress => isReloading ? (reloadTime - Time.time + lastAttackTime) / reloadTime : 0f;
    }
}
