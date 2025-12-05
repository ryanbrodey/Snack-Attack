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
        // Check for shoot input (left mouse button or Fire1)
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
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
        if (currentAmmo <= 0 && !isReloading && (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0)))
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
        
        // Spawn bullet
        if (bulletPrefab != null && bulletSpawn != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = bulletSpawn.forward * bulletSpeed;
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
