using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Properties")]
    public float speed = 50f;
    public float damage = 15f;
    public float lifetime = 5f;
    public float maxRange = 100f;
    
    [Header("Collision")]
    public LayerMask hitLayers = -1;
    public bool canPenetrate = false;
    public int maxPenetrations = 1;
    
    private Vector3 startPosition;
    private Vector3 targetDirection;
    private Rigidbody rb;
    private int penetrationCount = 0;
    private bool hasHit = false;
    
    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        
        // Configure physics
        if (rb != null)
        {
            rb.useGravity = false;
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
        
        Debug.Log($"BulletController spawned: Speed={speed}, Damage={damage}, Range={maxRange}");
    }
    
    void FixedUpdate()
    {
        // Check if traveled too far
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            Debug.Log("Bullet reached max range, destroying");
            DestroyBullet();
            return;
        }
        
        // Move bullet
        if (rb != null)
        {
            rb.velocity = targetDirection * speed;
        }
    }
    
    public void Initialize(Vector3 direction, Vector3 origin, float bulletSpeed = -1f)
    {
        targetDirection = direction.normalized;
        startPosition = origin;
        
        if (bulletSpeed > 0)
            speed = bulletSpeed;
            
        // Orient bullet to face direction
        transform.LookAt(transform.position + targetDirection);
        
        Debug.Log($"Bullet initialized: Direction={direction}, Speed={speed}");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (hasHit && !canPenetrate) return;
        
        Debug.Log($"Bullet hit: {collision.gameObject.name}");
        HandleHit(collision.gameObject, collision.contacts[0].point);
    }
    
    void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        // Damage system
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"Bullet dealt {damage} damage to {hitObject.name}");
        }
        else
        {
            Debug.Log($"Hit {hitObject.name} but no IDamageable component found");
        }
        
        // Penetration logic for shotgun pellets
        if (canPenetrate && penetrationCount < maxPenetrations)
        {
            penetrationCount++;
            Debug.Log($"Bullet penetrated {hitObject.name}, continuing ({penetrationCount}/{maxPenetrations})");
            return; // Continue traveling
        }
        
        // Stop bullet
        hasHit = true;
        DestroyBullet();
    }
    
    void DestroyBullet()
    {
        Debug.Log("Bullet destroyed");
        Destroy(gameObject);
    }
}


