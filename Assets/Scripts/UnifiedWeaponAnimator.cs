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
    private const string MOVE_SPEED = "MoveSpeed";
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_GROUNDED = "IsGrounded";
    private const string IS_JUMPING = "IsJumping";
    private const string ATTACK_TRIGGER = "Attack";
    private const string RELOAD_TRIGGER = "Reload";
    
    // Animation state tracking
    private bool hasWeaponTypeParam;
    private bool hasMoveSpeedParam;
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
        CheckAnimatorParameters();
        SetWeaponType(currentWeaponType);
    }
    
    void CheckAnimatorParameters()
    {
        if (unifiedAnimator == null || unifiedAnimator.runtimeAnimatorController == null)
        {
            return;
        }
        
        foreach (AnimatorControllerParameter param in unifiedAnimator.parameters)
        {
            switch (param.name)
            {
                case WEAPON_TYPE:
                    hasWeaponTypeParam = true;
                    break;
                case MOVE_SPEED:
                    hasMoveSpeedParam = true;
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
    }
    
    // Switch weapon type (0=pistol, 1=rifle, 2=shotgun)
    public void SetWeaponType(int weaponType)
    {
        if (unifiedAnimator == null) return;
        
        currentWeaponType = weaponType;
        
        if (hasWeaponTypeParam)
        {
            unifiedAnimator.SetInteger(WEAPON_TYPE, weaponType);
        }
    }
    
    // Update movement animations
    public void UpdateMovement(bool isWalking, bool isRunning, bool isGrounded, bool isJumping)
    {
        if (unifiedAnimator == null) return;
        
        // Calculate movement speed for MoveSpeed parameter
        // 0 = idle, 0.5 = walking, 1.0 = running
        float moveSpeed = 0f;
        if (isRunning)
            moveSpeed = 1.0f;
        else if (isWalking)
            moveSpeed = 0.5f;
        
        if (hasMoveSpeedParam)
            unifiedAnimator.SetFloat(MOVE_SPEED, moveSpeed);
        
        if (hasWalkingParam)
            unifiedAnimator.SetBool(IS_WALKING, isWalking);
            
        if (hasRunningParam)
            unifiedAnimator.SetBool(IS_RUNNING, isRunning);
            
        if (hasGroundedParam)
            unifiedAnimator.SetBool(IS_GROUNDED, isGrounded);
            
        if (hasJumpingParam)
            unifiedAnimator.SetBool(IS_JUMPING, isJumping);
    }
    
    // Trigger attack animation
    public void TriggerAttack()
    {
        if (unifiedAnimator == null) return;
        
        if (hasAttackParam)
        {
            unifiedAnimator.SetTrigger(ATTACK_TRIGGER);
        }
    }
    
    // Trigger reload animation
    public void TriggerReload()
    {
        if (unifiedAnimator == null) return;
        
        if (hasReloadParam)
        {
            unifiedAnimator.SetTrigger(RELOAD_TRIGGER);
        }
    }
    
    public AnimatorStateInfo GetCurrentStateInfo()
    {
        if (unifiedAnimator == null) return new AnimatorStateInfo();
        return unifiedAnimator.GetCurrentAnimatorStateInfo(0);
    }
    
    public bool IsInState(string stateName)
    {
        if (unifiedAnimator == null) return false;
        return unifiedAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    
    // Public getters
    public int CurrentWeaponType => currentWeaponType;
    public Animator Animator => unifiedAnimator;
    public bool HasWeaponTypeParameter => hasWeaponTypeParam;
    
    // Animation event callbacks
    public void OnAttackComplete()
    {
    }
    
    public void OnReloadComplete()
    {
    }
}