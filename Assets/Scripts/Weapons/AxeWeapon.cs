using UnityEngine;

namespace SnackAttack.Weapons
{
    // fire axe weapon - swings and hits stuff
    public class AxeWeapon : BaseWeapon
    {
    [Header("Axe Settings")]
    public LayerMask hitMask = -1;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public bool showGizmos = true;
    public string[] attackAnims = { "axe_ATK1(hit)", "axe_ATK2(hit)" };
    public int currentAttackIdx = 0;
    public float impactTime = 0.6f; // when to check for hits in animation
        
        [Header("Effects")]
        public ParticleSystem hitEffect;
        public AudioSource audioSrc;
        public AudioClip swingSound;
        public AudioClip hitSound;
        
        protected override void Awake()
        {
            base.Awake();
            
            // axe stats
            weaponName = "Fire Axe";
            cooldown = 0.5f; // fast attacks for spam
            dmg = 25f;
            reach = 2f;
            
            // axe animations
            idleAnim = "axe_IDLE";
            walkAnim = "axe_WALK";
            runAnim = "axe_RUN";
            attackAnim = "axe_ATK1(hit)";
            
            // make attack point if we dont have one
            if (attackPoint == null)
            {
                GameObject ap = new GameObject("AttackPoint");
                ap.transform.SetParent(transform);
                ap.transform.localPosition = new Vector3(0, 0, 1f); // at axe tip
                attackPoint = ap.transform;
            }
            
            // find audio source
            if (audioSrc == null)
                audioSrc = GetComponent<AudioSource>();
        }
        
        protected override void PerformAttack()
        {
            // switch between attack animations
            CycleAttackAnim();
            
            // whoosh sound
            PlaySwingSound();
            
            // handle timing since animation events dont work on readonly clips
            StartCoroutine(HandleAttackTiming());
        }
        
        // handle attack timing since animation events dont work
        private System.Collections.IEnumerator HandleAttackTiming()
        {
            float animLen = GetAnimLength();
            
            // wait for impact moment
            yield return new WaitForSeconds(animLen * impactTime);
            
            // check for hits
            OnAttackImpact();
            
            // wait for animation to finish
            yield return new WaitForSeconds(animLen * (0.95f - impactTime));
            
            // done attacking
            CompleteAttack();
        }
        
        // get how long the animation is
        private float GetAnimLength()
        {
            if (anim == null) return 1.0f;
            
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            return info.length;
        }
        
        // switch between different attack animations
        private void CycleAttackAnim()
        {
            if (attackAnims != null && attackAnims.Length > 0)
            {
                currentAttackIdx = (currentAttackIdx + 1) % attackAnims.Length;
                attackAnim = attackAnims[currentAttackIdx];
            }
        }
        
        // when axe hits something
        public override void OnAttackImpact()
        {
            DoMeleeAttack();
        }
        
        // check for hits and do damage
        private void DoMeleeAttack()
        {
            Vector3 pos = attackPoint != null ? attackPoint.position : transform.position;
            
            // find stuff to hit
            Collider[] hits = Physics.OverlapSphere(pos, attackRadius, hitMask);
            
            foreach (Collider hit in hits)
            {
                // dont hit ourselves
                if (hit.transform.IsChildOf(transform.root))
                    continue;
                
                // try to damage it
                IDamageable target = hit.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(dmg);
                    OnDamageDealt?.Invoke(dmg);
                    
                    // hit sounds and effects
                    PlayHitSound();
                    PlayHitEffect(hit.transform.position);
                }
                else
                {
                    // hit a wall or something
                    PlayHitEffect(hit.transform.position);
                }
            }
        }
        
        void PlaySwingSound()
        {
            if (audioSrc != null && swingSound != null)
            {
                audioSrc.PlayOneShot(swingSound);
            }
        }
        
        void PlayHitSound()
        {
            if (audioSrc != null && hitSound != null)
            {
                audioSrc.PlayOneShot(hitSound);
            }
        }
        
        void PlayHitEffect(Vector3 pos)
        {
            if (hitEffect != null)
            {
                hitEffect.transform.position = pos;
                hitEffect.Play();
            }
        }
        
        void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            
            // show attack range
            Vector3 pos = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, attackRadius);
            
            // show attack point
            if (attackPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(attackPoint.position, 0.1f);
            }
        }
    }
    
    // interface for stuff that can take damage
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
