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
            
        }
    }
    
    void Update()
    {
        HandleShooting();
        HandleReloading();
    }
    
    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
    }
    
    void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartReload();
        }
        
        if (currentAmmo <= 0 && !isReloading && Input.GetButtonDown("Fire1"))
        {
            StartReload();
        }
    }
    
    void TryShoot()
    {
        if (isReloading)
        {
            return;
        }
        
        if (currentAmmo <= 0)
        {
            PlaySound(emptySound);
            return;
        }
        
        if (Time.time < lastShotTime + fireRate)
        {
            return;
        }
        
        Shoot();
    }
    
    void Shoot()
    {
        lastShotTime = Time.time;
        currentAmmo--;
        
        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger("Shoot");
        }
        
        if (bulletPrefab != null && bulletSpawn != null)
        {
            Camera playerCamera = GetPlayerCamera();
            Vector3 spawnPosition = bulletSpawn.position;
            Vector3 aimDirection;
            
            if (playerCamera != null)
            {
                aimDirection = CrosshairAiming.GetBulletDirectionFromSpawnToCrosshair(
                    spawnPosition, 
                    playerCamera, 
                    1000f
                );
            }
            else
            {
                aimDirection = bulletSpawn.forward;
            }
            
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            
            BulletController bulletScript = bullet.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(aimDirection, spawnPosition, bulletSpeed);
            }
            else
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = aimDirection * bulletSpeed;
                }
            }
        }
        
        PlaySound(shootSound);
    }
    
    private Camera GetPlayerCamera()
    {
        FPSPlayerController playerController = FindObjectOfType<FPSPlayerController>();
        if (playerController != null && playerController.playerCamera != null)
        {
            return playerController.playerCamera;
        }
        
        FPSPlayerControllerWithWeapons weaponsController = FindObjectOfType<FPSPlayerControllerWithWeapons>();
        if (weaponsController != null && weaponsController.playerCamera != null)
        {
            return weaponsController.playerCamera;
        }
        
        if (Camera.main != null)
        {
            return Camera.main;
        }
        
        return FindObjectOfType<Camera>();
    }
    
    void StartReload()
    {
        if (isReloading) return;
        
        isReloading = true;
        PlaySound(reloadSound);
        StartCoroutine(ReloadCoroutine());
    }
    
    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public float ReloadProgress => isReloading ? (reloadTime - Time.time + lastShotTime) / reloadTime : 0f;
}
