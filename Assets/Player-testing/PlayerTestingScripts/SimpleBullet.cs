using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifeTime = 3f;
    public float damage = 10f;
    public LayerMask hitLayers = -1; // What layers can this bullet hit
    public float bulletSpeed = 40f; // Speed for consistent movement
    
    [Header("Effects (Optional)")]
    public GameObject hitEffectPrefab; // Particle effect on impact
    public AudioClip hitSound; // Sound on impact
    
    private Rigidbody rb;
    private bool hasHit = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.useGravity = false;
            rb.drag = 0f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        
        BulletCollisionSetup collisionSetup = GetComponent<BulletCollisionSetup>();
        if (collisionSetup != null)
        {
            collisionSetup.SetupBulletPhysics();
        }
        
        Destroy(gameObject, lifeTime);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;
        
        GameObject hitObject = collision.gameObject;
        Vector3 hitPoint = collision.contacts[0].point;
        Vector3 hitNormal = collision.contacts[0].normal;
        
        if (((1 << hitObject.layer) & hitLayers) != 0)
        {
            HandleHit(collision);
        }
        
        DestroyBullet(hitPoint, hitNormal);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;
        
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            HandleTriggerHit(other);
        }
        
        DestroyBullet(transform.position, -transform.forward);
    }
    
    void HandleHit(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
    
    void HandleTriggerHit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
    
    void DestroyBullet(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(effect, 2f);
        }
        
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, hitPoint);
        }
        
        Destroy(gameObject);
    }
}

public interface IDamageable
{
    void TakeDamage(float damage);
}
