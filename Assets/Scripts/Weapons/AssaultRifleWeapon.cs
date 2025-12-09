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
                    Debug.LogWarning($"[{weaponName}] BulletSpawn not found! Please assign it in the inspector.");
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
                Debug.Log($"[{weaponName}] Cannot shoot while reloading!");
                return;
            }
            
            if (currentAmmo <= 0)
            {
                PlaySound(emptySound);
                Debug.Log($"[{weaponName}] Out of ammo! Press R to reload.");
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
            
            Debug.Log($"[{weaponName}] Full-auto mode activated!");
            isFullAutoFiring = true;
            cooldown = fullAutoRate; // Switch to full-auto rate
            
            // Start continuous firing coroutine
            fullAutoCoroutine = StartCoroutine(FullAutoFiring());
        }
        
        private void StopFullAuto()
        {
            if (!isFullAutoFiring) return;
            
            Debug.Log($"[{weaponName}] Full-auto mode deactivated!");
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
            
            // Spawn bullet
            if (bulletPrefab != null && bulletSpawn != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = bulletSpawn.forward * bulletSpeed;
                }
                
                Debug.Log($"[{weaponName}] Shot fired! Ammo: {currentAmmo}/{maxAmmo}");
            }
            else
            {
                Debug.LogWarning($"[{weaponName}] Missing bullet prefab or spawn point!");
            }
            
            // Play shoot sound
            PlaySound(shootSound);
            
            // Trigger animation
            if (anim != null)
            {
                anim.SetTrigger("Shoot");
            }
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
            Debug.Log($"[{weaponName}] Reloading... ({reloadTime}s)");
            
            PlaySound(reloadSound);
            StartCoroutine(ReloadCoroutine());
        }
        
        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(reloadTime);
            
            currentAmmo = maxAmmo;
            isReloading = false;
            
            Debug.Log($"[{weaponName}] Reload complete! {maxAmmo} rounds loaded.");
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
