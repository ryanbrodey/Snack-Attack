using UnityEngine;

/// <summary>
/// Unified Weapon Animator System
/// Manages a single animator controller that handles all weapon types
/// Uses animator parameters to switch between weapon states
/// </summary>
[AddComponentMenu("Snack Attack/Unified Weapon Animator")]
public class UnifiedWeaponAnimator : MonoBehaviour
{
    [Header("Unified Animator")]
    public Animator unifiedAnimator;
    public RuntimeAnimatorController unifiedController;
    
    [Header("Weapon States")]
    public int currentWeaponType = 0; // 0=Pistol/Ketchup, 1=Rifle, 2=Shotgun
    
    // Animator parameter names
    private const string WEAPON_TYPE = "WeaponType";
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_GROUNDED = "IsGrounded";
    private const string IS_JUMPING = "IsJumping";
    private const string ATTACK_TRIGGER = "Attack";
    private const string RELOAD_TRIGGER = "Reload";
    
    // Animation state tracking
    private bool hasWeaponTypeParam;
    private bool hasWalkingParam;
    private bool hasRunningParam;
    private bool hasGroundedParam;
    private bool hasJumpingParam;
    private bool hasAttackParam;
    private bool hasReloadParam;
    
    void Awake()
    {
        // Auto-find unified animator if not assigned
        if (unifiedAnimator == null)
        {
            unifiedAnimator = GetComponent<Animator>();
        }
        
        // Set up the unified controller
        if (unifiedController != null && unifiedAnimator != null)
        {
            unifiedAnimator.runtimeAnimatorController = unifiedController;
        }
    }
    
    void Start()
    {
        // Check which parameters exist in the animator
        CheckAnimatorParameters();
        
        // Set initial weapon type
        SetWeaponType(currentWeaponType);
        
        Debug.Log($"UnifiedWeaponAnimator initialized with weapon type: {currentWeaponType}");
    }
    
    void CheckAnimatorParameters()
    {
        if (unifiedAnimator == null || unifiedAnimator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("UnifiedWeaponAnimator: No animator or controller found!");
            return;
        }
        
        // Check which parameters exist
        foreach (AnimatorControllerParameter param in unifiedAnimator.parameters)
        {
            switch (param.name)
            {
                case WEAPON_TYPE:
                    hasWeaponTypeParam = true;
                    break;
                case IS_WALKING:
                    hasWalkingParam = true;
                    break;
                case IS_RUNNING:
                    hasRunningParam = true;
                    break;
                case IS_GROUNDED:
                    hasGroundedParam = true;
                    break;
                case IS_JUMPING:
                    hasJumpingParam = true;
                    break;
                case ATTACK_TRIGGER:
                    hasAttackParam = true;
                    break;
                case RELOAD_TRIGGER:
                    hasReloadParam = true;
                    break;
            }
        }
        
        Debug.Log($"Animator Parameters Found - WeaponType: {hasWeaponTypeParam}, Walking: {hasWalkingParam}, Running: {hasRunningParam}, Attack: {hasAttackParam}");
    }
    
    /// <summary>
    /// Switch to a specific weapon type
    /// 0 = Pistol/Ketchup Gun
    /// 1 = Assault Rifle
    /// 2 = Shotgun/Popcorn Launcher
    /// </summary>
    public void SetWeaponType(int weaponType)
    {
        if (unifiedAnimator == null) return;
        
        currentWeaponType = weaponType;
        
        if (hasWeaponTypeParam)
        {
            unifiedAnimator.SetInteger(WEAPON_TYPE, weaponType);
            Debug.Log($"Set weapon type to: {weaponType}");
        }
        else
        {
            Debug.LogWarning("WeaponType parameter not found in animator!");
        }
    }
    
    /// <summary>
    /// Update movement animations
    /// </summary>
    public void UpdateMovement(bool isWalking, bool isRunning, bool isGrounded, bool isJumping)
    {
        if (unifiedAnimator == null) return;
        
        if (hasWalkingParam)
            unifiedAnimator.SetBool(IS_WALKING, isWalking);
            
        if (hasRunningParam)
            unifiedAnimator.SetBool(IS_RUNNING, isRunning);
            
        if (hasGroundedParam)
            unifiedAnimator.SetBool(IS_GROUNDED, isGrounded);
            
        if (hasJumpingParam)
            unifiedAnimator.SetBool(IS_JUMPING, isJumping);
    }
    
    /// <summary>
    /// Trigger attack animation
    /// </summary>
    public void TriggerAttack()
    {
        if (unifiedAnimator == null) return;
        
        if (hasAttackParam)
        {
            unifiedAnimator.SetTrigger(ATTACK_TRIGGER);
            Debug.Log($"Triggered attack for weapon type: {currentWeaponType}");
        }
        else
        {
            Debug.LogWarning("Attack trigger not found in animator!");
        }
    }
    
    /// <summary>
    /// Trigger reload animation
    /// </summary>
    public void TriggerReload()
    {
        if (unifiedAnimator == null) return;
        
        if (hasReloadParam)
        {
            unifiedAnimator.SetTrigger(RELOAD_TRIGGER);
            Debug.Log($"Triggered reload for weapon type: {currentWeaponType}");
        }
    }
    
    /// <summary>
    /// Get current animation state info
    /// </summary>
    public AnimatorStateInfo GetCurrentStateInfo()
    {
        if (unifiedAnimator == null) return new AnimatorStateInfo();
        return unifiedAnimator.GetCurrentAnimatorStateInfo(0);
    }
    
    /// <summary>
    /// Check if animator is in a specific state
    /// </summary>
    public bool IsInState(string stateName)
    {
        if (unifiedAnimator == null) return false;
        return unifiedAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    
    // Public getters
    public int CurrentWeaponType => currentWeaponType;
    public Animator Animator => unifiedAnimator;
    public bool HasWeaponTypeParameter => hasWeaponTypeParam;
    
    // Animation event callbacks (called from animation events)
    public void OnAttackComplete()
    {
        // This can be called from animation events
        Debug.Log("Attack animation completed");
    }
    
    public void OnReloadComplete()
    {
        // This can be called from animation events
        Debug.Log("Reload animation completed");
    }
}