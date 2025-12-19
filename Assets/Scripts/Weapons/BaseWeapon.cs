using UnityEngine;

namespace SnackAttack.Weapons
{
    // base weapon class - all weapons inherit from this
    public abstract class BaseWeapon : MonoBehaviour
    {
        [Header("Weapon Stats")]
        public string weaponName = "Base Weapon";
        public float cooldown = 0.3f; // fast attacks for spam clicking
        public float dmg = 10f;
        public float reach = 2f;
        
        [Header("Animations")]
        public Animator anim;
        public UnifiedWeaponAnimator unifiedAnimator; // Reference to unified animator
        public string idleAnim = "Idle";
        public string attackAnim = "Attack";
        public string walkAnim = "Walk";
        public string runAnim = "Run";
        
        // state stuff
        protected bool canAttack = true;
        protected bool attacking = false;
        protected float lastAttackTime;
        protected float lastAnimationTime;
        protected string currentAnimationState;
        
        // events for other scripts
        public System.Action OnAttackStarted;
        public System.Action OnAttackCompleted;
        public System.Action<float> OnDamageDealt;
        
        // getters
        public string WeaponName => weaponName;
        public bool CanAttack => canAttack && !attacking;
        public bool IsAttacking => attacking;
        public float Damage => dmg;
        public float Range => reach;
        
        protected virtual void Awake()
        {
            // find animator if we dont have one
            if (anim == null)
                anim = GetComponent<Animator>();
                
            // Auto-find unified animator
            if (unifiedAnimator == null)
            {
                // Try to find it in parent objects
                unifiedAnimator = GetComponentInParent<UnifiedWeaponAnimator>();
                if (unifiedAnimator == null)
                {
                    // Try to find it in the player
                    FPSPlayerControllerWithWeapons player = FindObjectOfType<FPSPlayerControllerWithWeapons>();
                    if (player != null)
                    {
                        unifiedAnimator = player.GetComponent<UnifiedWeaponAnimator>();
                    }
                }
            }
        }
        
        protected virtual void Start()
        {
            // Force reset attack state
            StopAllCoroutines();
            attacking = false;
            canAttack = true;
            lastAttackTime = 0f;
            
            // start in idle
            PlayIdle();
        }
        
        protected virtual void Update()
        {
            // cooldown timer
            if (!canAttack && Time.time >= lastAttackTime + cooldown)
            {
                canAttack = true;
            }
            
            // Backspace to reset weapon if it gets stuck
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ForceResetState();
            }
        }
        
        // try to attack
        public virtual void Attack()
        {
            if (!CanAttack) return;
            
            StartAttack();
        }
        
        // begin attack
        protected virtual void StartAttack()
        {
            attacking = true;
            canAttack = false;
            lastAttackTime = Time.time;
            
            PlayAttack();
            OnAttackStarted?.Invoke();
            
            // do the actual attack
            PerformAttack();
        }
        
        // finish attack
        protected virtual void CompleteAttack()
        {
            attacking = false;
            OnAttackCompleted?.Invoke();
            
            // Reset animation state and return to idle
            currentAnimationState = "";
            PlayIdle();
        }
        
        // override this for weapon specific attacks
        protected abstract void PerformAttack();
        
        // update animations based on movement
        public virtual void UpdateMovementAnimation(bool moving, bool running)
        {
            if (attacking) return; // dont interrupt attacks
            
            // Determine what animation should be playing
            string targetAnimation;
            if (moving)
            {
                targetAnimation = running ? runAnim : walkAnim;
            }
            else
            {
                targetAnimation = idleAnim;
            }
            
            // Only change animation if it's different and enough time has passed
            if (targetAnimation != currentAnimationState && 
                Time.time - lastAnimationTime > 0.1f)
            {
                currentAnimationState = targetAnimation;
                lastAnimationTime = Time.time;
                
                if (moving)
                {
                    if (running)
                        PlayRun();
                    else
                        PlayWalk();
                }
                else
                {
                    PlayIdle();
                }
            }
        }
        
        // animation helpers
        protected virtual void PlayIdle()
        {
            if (anim != null && !string.IsNullOrEmpty(idleAnim))
            {
                anim.Play(idleAnim);
                currentAnimationState = idleAnim;
                lastAnimationTime = Time.time;
            }
        }
        
        protected virtual void PlayAttack()
        {
            // Use unified animator if available
            if (unifiedAnimator != null)
            {
                unifiedAnimator.TriggerAttack();
                currentAnimationState = attackAnim;
                lastAnimationTime = Time.time;
                return;
            }
            
            // Fallback to individual animator
            if (anim != null && !string.IsNullOrEmpty(attackAnim))
            {
                // Try using trigger first
                if (anim.parameters != null)
                {
                    foreach (var param in anim.parameters)
                    {
                        if (param.name == "Attack" && param.type == AnimatorControllerParameterType.Trigger)
                        {
                            anim.SetTrigger("Attack");
                            currentAnimationState = attackAnim;
                            lastAnimationTime = Time.time;
                            return;
                        }
                    }
                }
                
                // Fallback to direct play
                anim.Play(attackAnim);
                currentAnimationState = attackAnim;
                lastAnimationTime = Time.time;
            }
        }
        
        protected virtual void PlayWalk()
        {
            if (anim != null && !string.IsNullOrEmpty(walkAnim))
            {
                anim.Play(walkAnim);
                currentAnimationState = walkAnim;
                lastAnimationTime = Time.time;
            }
        }
        
        protected virtual void PlayRun()
        {
            if (anim != null && !string.IsNullOrEmpty(runAnim))
            {
                anim.Play(runAnim);
                currentAnimationState = runAnim;
                lastAnimationTime = Time.time;
            }
        }
        
        // called by animation events
        public virtual void OnAttackAnimationComplete()
        {
            CompleteAttack();
        }
        
        // called when weapon hits something
        public virtual void OnAttackImpact()
        {
            // override this in weapon classes
        }
        
        // manual reset if weapon gets stuck
        [ContextMenu("Force Reset Weapon State")]
        public void ForceResetState()
        {
            StopAllCoroutines();
            attacking = false;
            canAttack = true;
            lastAttackTime = 0f;
            currentAnimationState = "";
            PlayIdle();
        }
    }
}
