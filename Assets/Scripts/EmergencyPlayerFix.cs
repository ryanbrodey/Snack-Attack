using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Emergency fix to get basic player working by disabling problematic features
/// </summary>
public class EmergencyPlayerFix : MonoBehaviour
{
    [ContextMenu("Emergency Fix - Disable Animations")]
    public void EmergencyFixDisableAnimations()
    {
        Debug.Log("=== EMERGENCY FIX: DISABLING ANIMATIONS ===");
        
        // Disable all animators to prevent errors
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (Animator anim in animators)
        {
            anim.enabled = false;
            Debug.Log($"Disabled animator on {anim.gameObject.name}");
        }
        
        Debug.Log("✓ All animators disabled - game should not pause anymore");
    }
    
    [ContextMenu("Emergency Fix - Disable Full Auto")]
    public void EmergencyFixDisableFullAuto()
    {
        Debug.Log("=== EMERGENCY FIX: DISABLING FULL AUTO ===");
        
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController != null)
        {
            // Change the full auto key to something unused to prevent G key issues
            fpsController.fullAutoKey = KeyCode.None;
            Debug.Log("Disabled full auto key (G key)");
        }
        
        Debug.Log("✓ Full auto disabled");
    }
    
    [ContextMenu("Emergency Fix - Simple Weapon Setup")]
    public void EmergencyFixSimpleWeaponSetup()
    {
        Debug.Log("=== EMERGENCY FIX: SIMPLE WEAPON SETUP ===");
        
        // Remove all existing weapons
        BaseWeapon[] existingWeapons = GetComponentsInChildren<BaseWeapon>();
        foreach (BaseWeapon weapon in existingWeapons)
        {
            if (weapon != null)
            {
                DestroyImmediate(weapon.gameObject);
            }
        }
        
        // Create simple weapon setup
        CreateSimpleWeapon();
        
        Debug.Log("✓ Simple weapon setup complete");
    }
    
    void CreateSimpleWeapon()
    {
        // Find or create weapon socket
        Transform weaponSocket = transform.Find("WeaponSocket");
        if (weaponSocket == null)
        {
            GameObject socketGO = new GameObject("WeaponSocket");
            socketGO.transform.SetParent(transform);
            socketGO.transform.localPosition = Vector3.zero;
            weaponSocket = socketGO.transform;
        }
        
        // Create simple ketchup weapon
        GameObject weaponGO = new GameObject("SimpleKetchupWeapon");
        weaponGO.transform.SetParent(weaponSocket);
        weaponGO.transform.localPosition = Vector3.zero;
        
        // Add simple weapon script
        var simpleWeapon = weaponGO.AddComponent<SimpleTestWeapon>();
        
        // Assign to controller
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController != null)
        {
            fpsController.weapons = new BaseWeapon[] { simpleWeapon };
            fpsController.weaponHolder = weaponSocket;
            fpsController.currentWeaponIdx = 0;
        }
        
        Debug.Log("Created simple test weapon");
    }
    
    [ContextMenu("Emergency Fix - Disable Error Pause")]
    public void EmergencyFixDisableErrorPause()
    {
        Debug.Log("=== EMERGENCY FIX: DISABLE ERROR PAUSE ===");
        
        // Turn off error pause in editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.pauseStateChanged += (UnityEditor.PauseState state) =>
        {
            if (state == UnityEditor.PauseState.Paused)
            {
                UnityEditor.EditorApplication.isPaused = false;
                Debug.Log("Auto-unpaused game");
            }
        };
        #endif
        
        Debug.Log("✓ Error pause disabled");
    }
    
    [ContextMenu("Run All Emergency Fixes")]
    public void RunAllEmergencyFixes()
    {
        Debug.Log("=== RUNNING ALL EMERGENCY FIXES ===");
        
        EmergencyFixDisableAnimations();
        EmergencyFixDisableFullAuto();
        EmergencyFixSimpleWeaponSetup();
        EmergencyFixDisableErrorPause();
        
        Debug.Log("=== ALL EMERGENCY FIXES COMPLETE ===");
        Debug.Log("Try testing now - should work without pausing!");
    }
}

/// <summary>
/// Simple test weapon that doesn't use animations
/// </summary>
public class SimpleTestWeapon : BaseWeapon
{
    protected override void Awake()
    {
        // Don't call base.Awake() to avoid animation setup
        weaponName = "Simple Test Weapon";
        cooldown = 0.5f;
        dmg = 15f;
        reach = 50f;
    }
    
    protected override void Start()
    {
        // Don't call base.Start() to avoid animation issues
        Debug.Log($"[{weaponName}] Simple weapon started - no animations");
    }
    
    protected override void Update()
    {
        // Simple cooldown without animation calls
        if (!canAttack && Time.time >= lastAttackTime + cooldown)
        {
            canAttack = true;
        }
    }
    
    protected override void PerformAttack()
    {
        Debug.Log($"[{weaponName}] BANG! Simple attack performed");
        
        // Simple attack without complex systems
        attacking = true;
        canAttack = false;
        lastAttackTime = Time.time;
        
        // Complete attack immediately
        Invoke(nameof(CompleteSimpleAttack), 0.1f);
    }
    
    void CompleteSimpleAttack()
    {
        attacking = false;
        Debug.Log($"[{weaponName}] Attack completed");
    }
    
    public override void UpdateMovementAnimation(bool moving, bool running)
    {
        // Do nothing - no animations
    }
}
