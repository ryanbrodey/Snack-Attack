using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifeTime = 3f;
    public float damage = 10f;
    public LayerMask hitLayers = -1; // What layers can this bullet hit
    
    [Header("Effects (Optional)")]
    public GameObject hitEffectPrefab; // Particle effect on impact
    public AudioClip hitSound; // Sound on impact
    
    private Rigidbody rb;
    private bool hasHit = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Destroy after lifetime to prevent bullet buildup
        Destroy(gameObject, lifeTime);
        
        Debug.Log($"Ketchup bullet spawned with {damage} damage, {lifeTime}s lifetime");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return; // Prevent multiple collision calls
        hasHit = true;
        
        GameObject hitObject = collision.gameObject;
        Vector3 hitPoint = collision.contacts[0].point;
        Vector3 hitNormal = collision.contacts[0].normal;
        
        Debug.Log($"Ketchup bullet hit: {hitObject.name} at {hitPoint}");
        
        // Check if we hit something on our hit layers
        if (((1 << hitObject.layer) & hitLayers) != 0)
        {
            HandleHit(collision);
        }
        
        // Always destroy bullet on any collision for realistic behavior
        DestroyBullet(hitPoint, hitNormal);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;
        
        Debug.Log($"Ketchup bullet triggered: {other.name}");
        
        // Handle trigger collisions (useful for enemy hit detection)
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            HandleTriggerHit(other);
        }
        
        DestroyBullet(transform.position, -transform.forward);
    }
    
    void HandleHit(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        Vector3 hitPoint = collision.contacts[0].point;
        
        // Try to damage the target
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"Dealt {damage} damage to {hitObject.name}");
        }
        
        // Check for specific enemy components
        var enemy = hitObject.GetComponent<MonoBehaviour>();
        if (enemy != null && (enemy.GetType().Name.Contains("AI") || enemy.GetType().Name.Contains("Enemy")))
        {
            Debug.Log($"Hit enemy: {hitObject.name}");
            // You can add specific enemy hit logic here
        }
        
        // Check for common enemy tags
        if (hitObject.CompareTag("Enemy"))
        {
            Debug.Log($"Hit tagged enemy: {hitObject.name}");
        }
    }
    
    void HandleTriggerHit(Collider other)
    {
        // Similar to HandleHit but for trigger colliders
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"Dealt {damage} damage to {other.name} via trigger");
        }
    }
    
    void DestroyBullet(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Spawn hit effect if available
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(effect, 2f); // Clean up effect after 2 seconds
        }
        
        // Play hit sound if available
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, hitPoint);
        }
        
        Debug.Log("Ketchup bullet destroyed on impact");
        
        // Destroy the bullet immediately
        Destroy(gameObject);
    }
}

// Interface for objects that can take damage
public interface IDamageable
{
    void TakeDamage(float damage);
}
