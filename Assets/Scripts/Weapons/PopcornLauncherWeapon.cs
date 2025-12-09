using System.Collections;
using UnityEngine;

namespace SnackAttack.Weapons
{
    // Popcorn rocket launcher - OP high damage, long range weapon
    public class PopcornLauncherWeapon : BaseWeapon
    {
        [Header("Popcorn Launcher Settings")]
        public Transform bulletSpawn;
        public GameObject bulletPrefab;
        public float bulletSpeed = 30f; // Slower but more powerful
        public int maxAmmo = 8; // Lower ammo count for balance
        public float reloadTime = 3f; // Longer reload time
        
        [Header("OP Stats")]
        public float explosionRadius = 5f; // Area damage
        public LayerMask explosionLayers = -1;
        
        [Header("Audio")]
        public AudioClip shootSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
        public AudioClip explosionSound;
        
        [Header("Effects")]
        public GameObject explosionEffect;
        
        // Ammo system
        private int currentAmmo;
        private bool isReloading = false;
        private AudioSource audioSource;
        
        protected override void Awake()
        {
            base.Awake();
            
            // OP Popcorn launcher stats
            weaponName = "Popcorn Rocket Launcher";
            cooldown = 1.5f; // Slow but devastating
            dmg = 75f; // VERY high damage
            reach = 100f; // Longest range
            
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
                // Look for popcorn bucket or launcher
                Transform popcorn = transform.Find("PopcornBucket") ?? transform.Find("Launcher");
                if (popcorn != null)
                {
                    bulletSpawn = popcorn.Find("BulletSpawn");
                }
                
                if (bulletSpawn == null)
                {
                    bulletSpawn = transform.Find("BulletSpawn");
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
        }
        
        public override void Attack()
        {
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
            
            // Call base attack (handles cooldown and state)
            base.Attack();
        }
        
        protected override void PerformAttack()
        {
            // Consume ammo
            currentAmmo--;
            
            // Spawn explosive popcorn projectile
            if (bulletPrefab != null && bulletSpawn != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                
                // Add explosive behavior to the bullet
                PopcornProjectile projectile = bullet.GetComponent<PopcornProjectile>();
                if (projectile == null)
                {
                    projectile = bullet.AddComponent<PopcornProjectile>();
                }
                
                // Configure the projectile
                projectile.damage = dmg;
                projectile.explosionRadius = explosionRadius;
                projectile.explosionLayers = explosionLayers;
                projectile.explosionEffect = explosionEffect;
                projectile.explosionSound = explosionSound;
                projectile.launcher = this;
                
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = bulletSpawn.forward * bulletSpeed;
                }
                
                Debug.Log($"[{weaponName}] POPCORN ROCKET FIRED! Ammo: {currentAmmo}/{maxAmmo}");
            }
            else
            {
                Debug.LogWarning($"[{weaponName}] Missing bullet prefab or spawn point!");
            }
            
            // Play shoot sound
            PlaySound(shootSound);
            
            // Complete attack after animation
            StartCoroutine(CompleteAttackAfterAnimation());
        }
        
        private IEnumerator CompleteAttackAfterAnimation()
        {
            // Wait longer for heavy weapon animation
            yield return new WaitForSeconds(0.8f);
            CompleteAttack();
        }
        
        private void StartReload()
        {
            if (isReloading) return;
            
            isReloading = true;
            Debug.Log($"[{weaponName}] Reloading rocket launcher... ({reloadTime}s)");
            
            PlaySound(reloadSound);
            StartCoroutine(ReloadCoroutine());
        }
        
        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(reloadTime);
            
            currentAmmo = maxAmmo;
            isReloading = false;
            
            Debug.Log($"[{weaponName}] Reload complete! {maxAmmo} rockets loaded.");
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
    
    // Special projectile component for explosive popcorn rockets
    public class PopcornProjectile : MonoBehaviour
    {
        [HideInInspector] public float damage = 75f;
        [HideInInspector] public float explosionRadius = 5f;
        [HideInInspector] public LayerMask explosionLayers = -1;
        [HideInInspector] public GameObject explosionEffect;
        [HideInInspector] public AudioClip explosionSound;
        [HideInInspector] public PopcornLauncherWeapon launcher;
        
        private bool hasExploded = false;
        
        void Start()
        {
            // Auto-destroy after 5 seconds if it doesn't hit anything
            Destroy(gameObject, 5f);
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (hasExploded) return;
            
            Explode(collision.contacts[0].point);
        }
        
        private void Explode(Vector3 explosionPoint)
        {
            hasExploded = true;
            
            Debug.Log($"[PopcornProjectile] EXPLOSION at {explosionPoint}!");
            
            // Play explosion sound
            if (explosionSound != null && launcher != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, explosionPoint);
            }
            
            // Spawn explosion effect
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, explosionPoint, Quaternion.identity);
            }
            
            // Deal area damage
            Collider[] hitColliders = Physics.OverlapSphere(explosionPoint, explosionRadius, explosionLayers);
            
            foreach (Collider hit in hitColliders)
            {
                // Don't damage the player who fired it
                if (hit.transform.IsChildOf(launcher.transform.root))
                    continue;
                
                // Calculate distance-based damage
                float distance = Vector3.Distance(explosionPoint, hit.transform.position);
                float damageMultiplier = 1f - (distance / explosionRadius);
                float finalDamage = damage * damageMultiplier;
                
                // Try to damage the target
                IDamageable target = hit.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(finalDamage);
                    Debug.Log($"[PopcornProjectile] Dealt {finalDamage} explosion damage to {hit.name}");
                }
                
                // Add explosion force to rigidbodies
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(500f, explosionPoint, explosionRadius);
                }
            }
            
            // Destroy the projectile
            Destroy(gameObject);
        }
        
        void OnDrawGizmosSelected()
        {
            // Show explosion radius in editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
