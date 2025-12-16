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
            
            // Find player camera
            Camera playerCamera = GetPlayerCamera();
            if (playerCamera == null) 
            {
                Debug.LogWarning("Shotgun: No player camera found, using forward direction");
                // Fallback to old behavior - fire straight ahead
                for (int i = 0; i < pelletsPerShot; i++)
                {
                    GameObject pellet = Instantiate(pelletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                    
                    // Add random spread
                    Vector3 spreadDirection = bulletSpawn.forward;
                    spreadDirection += Random.insideUnitSphere * (spreadAngle / 100f);
                    spreadDirection.Normalize();
                    
                    Rigidbody rb = pellet.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = spreadDirection * pelletSpeed;
                    }
                }
                return;
            }
            
            // Get spread directions for all pellets using crosshair aiming
            Vector3[] directions = CrosshairAiming.GetShotgunDirections(
                bulletSpawn.position, 
                playerCamera, 
                pelletsPerShot, 
                spreadAngle
            );
            
            Debug.Log($"Shotgun: Firing {pelletsPerShot} pellets with {spreadAngle}° spread");
            
            // Fire each pellet
            for (int i = 0; i < pelletsPerShot; i++)
            {
                GameObject pellet = Instantiate(pelletPrefab, bulletSpawn.position, Quaternion.identity);
                
                // Initialize pellet with BulletController script
                BulletController pelletScript = pellet.GetComponent<BulletController>();
                if (pelletScript != null)
                {
                    // Configure for shotgun pellet
                    pelletScript.canPenetrate = true;
                    pelletScript.maxPenetrations = penetrationCount;
                    pelletScript.damage = pelletDamage;
                    pelletScript.maxRange = reach;
                    pelletScript.speed = pelletSpeed;
                    
                    pelletScript.Initialize(directions[i], bulletSpawn.position, pelletSpeed);
                    Debug.Log($"Shotgun: Pellet {i+1} fired with BulletController script");
                }
                else
                {
                    // Fallback for pellets without UniversalBullet script
                    Rigidbody rb = pellet.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = directions[i] * pelletSpeed;
                        Debug.Log($"Shotgun: Pellet {i+1} fired with fallback Rigidbody method");
                    }
                }
            }
            
            // Debug visualization
            CrosshairAiming.DrawAimDebug(bulletSpawn.position, playerCamera, 3f);
        }
        
        private Camera GetPlayerCamera()
        {
            // Try to find camera in parent hierarchy
            Camera cam = GetComponentInParent<Camera>();
            if (cam == null)
            {
                // Try to find any camera in the scene
                cam = FindObjectOfType<Camera>();
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
