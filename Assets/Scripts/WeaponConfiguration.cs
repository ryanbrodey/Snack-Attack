using UnityEngine;
using SnackAttack.Weapons;

[System.Serializable]
public class WeaponConfiguration
{
    [Header("Weapon Info")]
    public string weaponName;
    public int weaponIndex; // 0=Ketchup/Pistol, 1=Rifle, 2=Shotgun
    
    [Header("Camera Settings")]
    public Vector3 cameraPosition = Vector3.zero;
    public Vector3 cameraRotation = Vector3.zero;
    
    [Header("Animation")]
    public RuntimeAnimatorController animatorController;
    
    [Header("Weapon Model")]
    public GameObject weaponModel; // The actual weapon model to show
    public Transform weaponSocket; // Where to attach the weapon
    
    [Header("Weapon Script")]
    public MonoBehaviour weaponScript; // The BaseWeapon component
}

/// <summary>
/// Manages weapon configurations for dynamic FPS weapon switching
/// Handles camera positions, animations, and weapon models per weapon type
/// </summary>
[AddComponentMenu("Snack Attack/Weapon Configuration Manager")]
public class WeaponConfigurationManager : MonoBehaviour
{
    [Header("Weapon Configurations")]
    public WeaponConfiguration[] weaponConfigs;
    
    [Header("References")]
    public Camera playerCamera;
    public Transform cameraAnchor; // The CameraAnchor transform that moves
    public Animator armsAnimator;
    
    [Header("Transition Settings")]
    public bool smoothTransitions = false;
    public float transitionSpeed = 5f;
    
    // Private variables
    private int currentWeaponIndex = 0;
    private Vector3 targetCameraPosition;
    private Vector3 targetCameraRotation;
    private bool isTransitioning = false;
    
    // Events
    public System.Action<int> OnWeaponSwitched;
    
    void Start()
    {
        // Auto-find references if not set
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
            
        if (cameraAnchor == null)
        {
            // Try to find CameraAnchor
            Transform anchor = transform.Find("CameraAnchor");
            if (anchor != null)
                cameraAnchor = anchor;
        }
        
        if (armsAnimator == null)
        {
            // Try to find arms animator
            Transform arms = transform.Find("PistolArms");
            if (arms != null)
                armsAnimator = arms.GetComponent<Animator>();
        }
        
        // Initialize weapon configurations
        InitializeWeaponConfigs();
        
        // Set initial weapon
        if (weaponConfigs.Length > 0)
        {
            SwitchToWeapon(0, true); // Force immediate switch for first weapon
        }
    }
    
    void Update()
    {
        // Handle smooth camera transitions
        if (smoothTransitions && isTransitioning && cameraAnchor != null)
        {
            // Smoothly move camera anchor to target position
            cameraAnchor.localPosition = Vector3.Lerp(
                cameraAnchor.localPosition, 
                targetCameraPosition, 
                Time.deltaTime * transitionSpeed
            );
            
            cameraAnchor.localEulerAngles = Vector3.Lerp(
                cameraAnchor.localEulerAngles,
                targetCameraRotation,
                Time.deltaTime * transitionSpeed
            );
            
            // Check if we're close enough to stop transitioning
            if (Vector3.Distance(cameraAnchor.localPosition, targetCameraPosition) < 0.01f)
            {
                cameraAnchor.localPosition = targetCameraPosition;
                cameraAnchor.localEulerAngles = targetCameraRotation;
                isTransitioning = false;
            }
        }
    }
    
    void InitializeWeaponConfigs()
    {
        // Set up default configurations if not already configured
        if (weaponConfigs == null || weaponConfigs.Length == 0)
        {
            weaponConfigs = new WeaponConfiguration[3];
            
            // Ketchup/Pistol configuration (index 0)
            weaponConfigs[0] = new WeaponConfiguration
            {
                weaponName = "Ketchup Pistol",
                weaponIndex = 0,
                cameraPosition = new Vector3(-0.199f, 1.564f, 0.155f),
                cameraRotation = new Vector3(7.086f, -7.197f, -0.066f)
            };
            
            // Rifle configuration (index 1)
            weaponConfigs[1] = new WeaponConfiguration
            {
                weaponName = "Assault Rifle",
                weaponIndex = 1,
                cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
                cameraRotation = new Vector3(5.624f, -44.278f, -0.456f)
            };
            
            // Shotgun configuration (index 2)
            weaponConfigs[2] = new WeaponConfiguration
            {
                weaponName = "Shotgun",
                weaponIndex = 2,
                cameraPosition = new Vector3(-0.004f, 1.505f, 0.221f),
                cameraRotation = new Vector3(5.624f, -44.278f, -0.456f)
            };
        }
    }
    
    public bool SwitchToWeapon(int weaponIndex, bool forceImmediate = false)
    {
        if (weaponConfigs == null || weaponIndex < 0 || weaponIndex >= weaponConfigs.Length)
        {
            return false;
        }
        
        WeaponConfiguration config = weaponConfigs[weaponIndex];
        if (config == null)
        {
            return false;
        }
        
        // Update current weapon index
        currentWeaponIndex = weaponIndex;
        
        // Switch camera position
        SwitchCameraPosition(config, forceImmediate);
        
        // Switch animation controller
        SwitchAnimationController(config);
        
        // Activate/deactivate weapon models
        SwitchWeaponModels(weaponIndex);
        
        // Fire event
        OnWeaponSwitched?.Invoke(weaponIndex);
        return true;
    }
    
    void SwitchCameraPosition(WeaponConfiguration config, bool immediate = false)
    {
        if (cameraAnchor == null) return;
        
        targetCameraPosition = config.cameraPosition;
        targetCameraRotation = config.cameraRotation;
        
        if (immediate || !smoothTransitions)
        {
            // Immediate switch
            cameraAnchor.localPosition = targetCameraPosition;
            cameraAnchor.localEulerAngles = targetCameraRotation;
            isTransitioning = false;
        }
        else
        {
            // Start smooth transition
            isTransitioning = true;
        }
    }
    
    void SwitchAnimationController(WeaponConfiguration config)
    {
        if (armsAnimator == null || config.animatorController == null) return;
        
        // Switch the animator controller
        armsAnimator.runtimeAnimatorController = config.animatorController;
    }
    
    void SwitchWeaponModels(int weaponIndex)
    {
        // This will be handled by the main weapon system
    }
    
    // Public getters
    public int CurrentWeaponIndex => currentWeaponIndex;
    public WeaponConfiguration CurrentWeaponConfig => 
        (weaponConfigs != null && currentWeaponIndex >= 0 && currentWeaponIndex < weaponConfigs.Length) 
            ? weaponConfigs[currentWeaponIndex] : null;
    
    public WeaponConfiguration GetWeaponConfig(int index)
    {
        if (weaponConfigs == null || index < 0 || index >= weaponConfigs.Length)
            return null;
        return weaponConfigs[index];
    }
}
