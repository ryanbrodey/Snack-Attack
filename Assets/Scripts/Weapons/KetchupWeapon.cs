using System.Collections;
using UnityEngine;

namespace SnackAttack.Weapons
{
    // Ketchup bottle weapon - semi-automatic shooting
    public class KetchupWeapon : BaseWeapon
    {
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
            attackAnim = "Shooting";
            
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
                Transform ketchup = transform.Find("Ketchup");
                if (ketchup != null)
                {
                    bulletSpawn = ketchup.Find("BulletSpawn");
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
                // Auto-reload when empty
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
            
            // Spawn bullet
            if (bulletPrefab != null && bulletSpawn != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = bulletSpawn.forward * bulletSpeed;
                }
                
                // Shot fired successfully
            }
            else
            {
                // Missing bullet prefab or spawn point
            }
            
            // Play shoot sound
            PlaySound(shootSound);
            
            // Complete attack immediately (projectile weapon)
            StartCoroutine(CompleteAttackAfterAnimation());
        }
        
        private IEnumerator CompleteAttackAfterAnimation()
        {
            // Wait a short time for animation to play
            yield return new WaitForSeconds(0.3f);
            CompleteAttack();
        }
        
        private void StartReload()
        {
            if (isReloading) return;
            
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
        
        // Public getters for UI or other systems
        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public bool IsReloading => isReloading;
        public float ReloadProgress => isReloading ? (reloadTime - Time.time + lastAttackTime) / reloadTime : 0f;
    }
}
