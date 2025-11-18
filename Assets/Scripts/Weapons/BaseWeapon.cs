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
        public string idleAnim = "Idle";
        public string attackAnim = "Attack";
        public string walkAnim = "Walk";
        public string runAnim = "Run";
        
        // state stuff
        protected bool canAttack = true;
        protected bool attacking = false;
        protected float lastAttackTime;
        
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
        }
        
        protected virtual void Start()
        {
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
            PlayIdle();
        }
        
        // override this for weapon specific attacks
        protected abstract void PerformAttack();
        
        // update animations based on movement
        public virtual void UpdateMovementAnimation(bool moving, bool running)
        {
            if (attacking) return; // dont interrupt attacks
            
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
        
        // animation helpers
        protected virtual void PlayIdle()
        {
            if (anim != null && !string.IsNullOrEmpty(idleAnim))
                anim.Play(idleAnim);
        }
        
        protected virtual void PlayAttack()
        {
            if (anim != null && !string.IsNullOrEmpty(attackAnim))
                anim.Play(attackAnim);
        }
        
        protected virtual void PlayWalk()
        {
            if (anim != null && !string.IsNullOrEmpty(walkAnim))
                anim.Play(walkAnim);
        }
        
        protected virtual void PlayRun()
        {
            if (anim != null && !string.IsNullOrEmpty(runAnim))
                anim.Play(runAnim);
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
    }
}
