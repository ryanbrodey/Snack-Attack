using System.Collections;
using UnityEngine;

public class KetchupGunTest : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.5f; // Time between shots
    public float bulletSpeed = 40f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    
    [Header("References")]
    public Animator armsAnimator;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    
    [Header("Audio (Optional)")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    
    // Private variables
    private float lastShotTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;
    private AudioSource audioSource;
    
    void Start()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        
        // Auto-find references if not assigned
        if (armsAnimator == null)
            armsAnimator = GetComponent<Animator>();
            
        if (bulletSpawn == null)
        {
            // Try to find BulletSpawn in hierarchy
            Transform ketchup = transform.Find("Ketchup");
            if (ketchup != null)
            {
                bulletSpawn = ketchup.Find("BulletSpawn");
            }
            
            if (bulletSpawn == null)
            {
                Debug.LogWarning("BulletSpawn not found! Please assign it in the inspector or ensure it exists under Ketchup.");
            }
        }
        
        Debug.Log($"Ketchup Gun initialized with {maxAmmo} rounds. Left click to shoot, R to reload.");
    }
    
    void Update()
    {
        HandleShooting();
        HandleReloading();
        
        // Debug info
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log($"Ammo: {currentAmmo}/{maxAmmo}, Reloading: {isReloading}");
        }
    }
    
    void HandleShooting()
    {
        // Check for shoot input (left mouse button - Fire1 is the same as mouse button 0)
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
    }
    
    void HandleReloading()
    {
        // Manual reload with R key
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartReload();
        }
        
        // Auto-reload when empty and trying to shoot
        if (currentAmmo <= 0 && !isReloading && Input.GetButtonDown("Fire1"))
        {
            StartReload();
        }
    }
    
    void TryShoot()
    {
        // Check if we can shoot
        if (isReloading)
        {
            Debug.Log("Cannot shoot while reloading!");
            return;
        }
        
        if (currentAmmo <= 0)
        {
            PlaySound(emptySound);
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }
        
        if (Time.time < lastShotTime + fireRate)
        {
            Debug.Log($"Weapon cooling down... {(lastShotTime + fireRate - Time.time):F1}s remaining");
            return;
        }
        
        // Perform the shot
        Shoot();
    }
    
    void Shoot()
    {
        // Update timing and ammo
        lastShotTime = Time.time;
        currentAmmo--;
        
        // Play shooting animation
        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger("Shoot");
        }
        
        // Spawn bullet with crosshair aiming
        if (bulletPrefab != null && bulletSpawn != null)
        {
            // Find player camera for crosshair aiming
            Camera playerCamera = GetPlayerCamera();
            Vector3 spawnPosition = bulletSpawn.position;
            Vector3 aimDirection;
            
            if (playerCamera != null)
            {
                // Use proper crosshair aiming - direction from bullet spawn to crosshair target
                aimDirection = CrosshairAiming.GetBulletDirectionFromSpawnToCrosshair(
                    spawnPosition, 
                    playerCamera, 
                    1000f
                );
            }
            else
            {
                // Fallback to bullet spawn forward if no camera found
                Debug.LogWarning("KetchupGunTest: No player camera found, using bullet spawn forward");
                aimDirection = bulletSpawn.forward;
            }
            
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            
            // Use BulletController if available (better physics)
            BulletController bulletScript = bullet.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(aimDirection, spawnPosition, bulletSpeed);
            }
            else
            {
                // Fallback to direct Rigidbody velocity
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = aimDirection * bulletSpeed;
                }
            }
            
            Debug.Log($"Ketchup shot fired! Ammo remaining: {currentAmmo}/{maxAmmo}");
        }
        else
        {
            Debug.LogWarning("Missing bullet prefab or bullet spawn point!");
        }
        
        // Play shoot sound
        PlaySound(shootSound);
    }
    
    private Camera GetPlayerCamera()
    {
        // Try to find player camera
        Camera cam = null;
        
        // Method 1: Try to find FPSPlayerController
        FPSPlayerController playerController = FindObjectOfType<FPSPlayerController>();
        if (playerController != null && playerController.playerCamera != null)
        {
            return playerController.playerCamera;
        }
        
        // Method 2: Try to find FPSPlayerControllerWithWeapons
        FPSPlayerControllerWithWeapons weaponsController = FindObjectOfType<FPSPlayerControllerWithWeapons>();
        if (weaponsController != null && weaponsController.playerCamera != null)
        {
            return weaponsController.playerCamera;
        }
        
        // Method 3: Fallback to Camera.main
        if (Camera.main != null)
        {
            return Camera.main;
        }
        
        // Method 4: Find any camera
        cam = FindObjectOfType<Camera>();
        return cam;
    }
    
    void StartReload()
    {
        if (isReloading) return;
        
        isReloading = true;
        Debug.Log($"Reloading ketchup gun... ({reloadTime}s)");
        
        // Play reload sound
        PlaySound(reloadSound);
        
        // Start reload coroutine
        StartCoroutine(ReloadCoroutine());
    }
    
    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
        
        Debug.Log($"Reload complete! {maxAmmo} rounds loaded.");
    }
    
    void PlaySound(AudioClip clip)
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
    public float ReloadProgress => isReloading ? (reloadTime - Time.time + lastShotTime) / reloadTime : 0f;
}
