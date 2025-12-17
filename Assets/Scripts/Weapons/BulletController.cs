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
    public float collisionIgnoreTime = 0.2f; // Ignore collisions for this duration after spawn
    
    private Vector3 startPosition;
    private Vector3 targetDirection = Vector3.zero;
    private Rigidbody rb;
    private Collider col;
    private int penetrationCount = 0;
    private bool hasHit = false;
    private bool isInitialized = false;
    private float spawnTime;
    private bool ignoreCollisions = true;
    private float lastVelocityCheck = 0f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        SetupPhysics();
        SetupCollisionLayers();
    }
    
    void SetupPhysics()
    {
        if (rb != null)
        {
            rb.useGravity = false; // CRITICAL: No gravity
            rb.drag = 0f; // No air resistance
            rb.angularDrag = 0f;
            rb.mass = 0.01f; // Very light mass to prevent physics interactions
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth movement
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Better collision detection for fast bullets
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Only freeze rotation
        }
        
        // Set collider to trigger during ignore period to prevent physics interactions
        if (col != null)
        {
            col.isTrigger = true; // Start as trigger to prevent physics collisions
            
            // Remove any physics material that might cause bouncing
            if (col is SphereCollider)
            {
                ((SphereCollider)col).material = null;
            }
            else if (col is BoxCollider)
            {
                ((BoxCollider)col).material = null;
            }
            else if (col is CapsuleCollider)
            {
                ((CapsuleCollider)col).material = null;
            }
        }
    }
    
    void SetupCollisionLayers()
    {
        // Ignore collisions with player layer
        int bulletLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer != -1)
        {
            Physics.IgnoreLayerCollision(bulletLayer, playerLayer, true);
        }
        
        // Ignore collisions with other bullets
        int bulletLayerMask = LayerMask.NameToLayer("Bullet");
        if (bulletLayerMask != -1 && bulletLayer != bulletLayerMask)
        {
            Physics.IgnoreLayerCollision(bulletLayer, bulletLayerMask, true);
        }
    }
    
    void Start()
    {
        startPosition = transform.position;
        spawnTime = Time.time;
        Destroy(gameObject, lifetime);
    }
    
    void FixedUpdate()
    {
        if (!isInitialized || targetDirection == Vector3.zero)
            return;
        
        // Enable collisions after ignore period
        if (ignoreCollisions && Time.time - spawnTime >= collisionIgnoreTime)
        {
            ignoreCollisions = false;
            // Switch from trigger to collision after ignore period
            if (col != null)
            {
                col.isTrigger = false;
            }
        }
        
        // Check if traveled too far
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            DestroyBullet();
            return;
        }
        
        // Maintain velocity - keep it constant and straight
        if (rb != null)
        {
            Vector3 desiredVelocity = targetDirection * speed;
            
            // Check if velocity is wrong (backwards, stopped, or wrong direction)
            float velocityDot = Vector3.Dot(rb.velocity.normalized, targetDirection);
            if (velocityDot < 0.5f || rb.velocity.magnitude < speed * 0.5f)
            {
                // Velocity is wrong - force it to correct value
                rb.velocity = desiredVelocity;
            }
            else
            {
                // Maintain correct velocity
                rb.velocity = desiredVelocity;
            }
            
            // Ensure physics settings stay correct
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.drag = 0f;
        }
        else
        {
            // Fallback: move using transform if rigidbody fails
            transform.position += targetDirection * speed * Time.fixedDeltaTime;
        }
    }
    
    public void Initialize(Vector3 direction, Vector3 origin, float bulletSpeed = -1f)
    {
        // Validate direction
        if (direction == Vector3.zero)
        {
            Debug.LogError("BulletController.Initialize: Direction is ZERO!");
            direction = Vector3.forward;
        }
        
        // Normalize and validate direction
        direction = direction.normalized;
        
        // Check for backwards direction (shouldn't happen, but safety check)
        if (Vector3.Dot(direction, Vector3.forward) < -0.5f)
        {
            Debug.LogWarning($"BulletController.Initialize: Direction appears backwards! {direction}");
        }
        
        targetDirection = direction;
        startPosition = origin;
        isInitialized = true;
        hasHit = false;
        spawnTime = Time.time;
        ignoreCollisions = true;
        lastVelocityCheck = Time.time;
        
        if (bulletSpeed > 0)
            speed = bulletSpeed;
        
        transform.position = origin;
        transform.rotation = Quaternion.LookRotation(targetDirection);
        
        // Ensure physics is set up
        SetupPhysics();
        
        if (rb != null)
        {
            rb.velocity = targetDirection * speed;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions during ignore period
        if (ignoreCollisions)
            return;
        
        // Ignore player collisions
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            return;
        
        // Ignore other bullets
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            return;
        
        // Stop bullet immediately - no bouncing
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // For non-penetrating bullets, destroy immediately on ANY collision
        if (!canPenetrate)
        {
            HandleHit(collision.gameObject, collision.contacts[0].point);
            DestroyBullet();
            return;
        }
        
        // Penetrating bullets (shotgun) can continue
        if (hasHit)
        {
            DestroyBullet();
            return;
        }
        
        HandleHit(collision.gameObject, collision.contacts[0].point);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Only handle triggers if collisions are enabled (after ignore period)
        if (ignoreCollisions)
            return;
        
        // Ignore player and bullet triggers
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || 
            other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            return;
        
        // Stop bullet immediately
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // For non-penetrating bullets, destroy immediately on ANY trigger
        if (!canPenetrate)
        {
            HandleHit(other.gameObject, other.ClosestPoint(transform.position));
            DestroyBullet();
            return;
        }
        
        // Penetrating bullets (shotgun) can continue
        if (hasHit)
        {
            DestroyBullet();
            return;
        }
        
        // For triggers, use the trigger's position as hit point
        HandleHit(other.gameObject, other.ClosestPoint(transform.position));
    }
    
    void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        // Damage system
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        
        // Penetration logic (only for shotgun pellets)
        if (canPenetrate && penetrationCount < maxPenetrations)
        {
            penetrationCount++;
            // Continue traveling - don't destroy yet
            return;
        }
        
        // Regular bullets (non-penetrating) always destroy immediately
        hasHit = true;
        DestroyBullet();
    }
    
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}


