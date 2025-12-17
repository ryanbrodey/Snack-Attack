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
        
        Debug.Log($"[Bullet] Awake - Rigidbody: {(rb != null ? "Found" : "MISSING")}, Collider: {(col != null ? col.GetType().Name : "MISSING")}, Layer: {gameObject.layer}");
        
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
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Better collision detection for fast bullets
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Only freeze rotation
            
            Debug.Log($"[Bullet] Physics setup - CollisionDetectionMode: {rb.collisionDetectionMode}, Layer: {gameObject.layer}");
        }
        
        // Set collider to trigger during ignore period to prevent physics interactions
        if (col != null)
        {
            col.isTrigger = true; // Start as trigger to prevent physics collisions
            
            // Remove any physics material that might cause bouncing
            if (col is SphereCollider)
            {
                SphereCollider sphereCol = (SphereCollider)col;
                sphereCol.material = null;
                Debug.Log($"[Bullet] SphereCollider - Radius: {sphereCol.radius}, IsTrigger: {col.isTrigger}, Layer: {gameObject.layer}");
            }
            else if (col is BoxCollider)
            {
                BoxCollider boxCol = (BoxCollider)col;
                boxCol.material = null;
                Debug.Log($"[Bullet] BoxCollider - Size: {boxCol.size}, IsTrigger: {col.isTrigger}, Layer: {gameObject.layer}");
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capCol = (CapsuleCollider)col;
                capCol.material = null;
                Debug.Log($"[Bullet] CapsuleCollider - Radius: {capCol.radius}, Height: {capCol.height}, IsTrigger: {col.isTrigger}, Layer: {gameObject.layer}");
            }
        }
        else
        {
            Debug.LogError("[Bullet] No Collider component found on bullet! Bullets will not detect collisions!");
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
                Debug.Log($"[Bullet] Collisions now enabled. Collider isTrigger: {col.isTrigger}, Layer: {gameObject.layer}");
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
        
        Debug.Log($"[Bullet] Initialized - Direction: {direction}, Speed: {speed}, Position: {origin}, Damage: {damage}, CollisionIgnoreTime: {collisionIgnoreTime}");
        
        // Ensure physics is set up
        SetupPhysics();
        
        if (rb != null)
        {
            rb.velocity = targetDirection * speed;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            Debug.Log($"[Bullet] Rigidbody velocity set to: {rb.velocity}");
        }
        else
        {
            Debug.LogError("[Bullet] Initialize: Rigidbody is NULL!");
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Bullet] OnCollisionEnter called - Object: {collision.gameObject.name}, Layer: {collision.gameObject.layer}, IgnoreCollisions: {ignoreCollisions}");
        
        // Ignore collisions during ignore period
        if (ignoreCollisions)
        {
            Debug.Log($"[Bullet] Ignoring collision (still in ignore period: {Time.time - spawnTime:F3}s / {collisionIgnoreTime}s)");
            return;
        }
        
        // Ignore player collisions
        int playerLayer = LayerMask.NameToLayer("Player");
        if (collision.gameObject.layer == playerLayer)
        {
            Debug.Log($"[Bullet] Ignoring player collision: {collision.gameObject.name}");
            return;
        }
        
        // Ignore other bullets
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        if (collision.gameObject.layer == bulletLayer)
        {
            Debug.Log($"[Bullet] Ignoring bullet collision: {collision.gameObject.name}");
            return;
        }
        
        Debug.Log($"[Bullet] Processing collision with: {collision.gameObject.name} (Layer: {collision.gameObject.layer}) at point: {collision.contacts[0].point}");
        
        // Quick check: Is this an enemy?
        bool isEnemy = collision.gameObject.GetComponent<IDamageable>() != null;
        if (!isEnemy)
        {
            IDamageable parentDamageable = collision.gameObject.GetComponentInParent<IDamageable>();
            isEnemy = parentDamageable != null;
        }
        
        if (isEnemy)
        {
            Debug.Log($"[Bullet] *** ENEMY DETECTED: {collision.gameObject.name} has IDamageable! ***");
        }
        else
        {
            Debug.LogWarning($"[Bullet] ⚠️ Hit NON-ENEMY object: {collision.gameObject.name}. Bullet will stop here and won't reach enemies behind it!");
            Debug.LogWarning($"[Bullet] ⚠️ Make sure you're shooting directly at the enemy, not at environment objects in front of it!");
        }
        
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
        Debug.Log($"[Bullet] OnTriggerEnter called - Object: {other.gameObject.name}, Layer: {other.gameObject.layer}, IgnoreCollisions: {ignoreCollisions}");
        
        // Only handle triggers if collisions are enabled (after ignore period)
        if (ignoreCollisions)
        {
            Debug.Log($"[Bullet] Ignoring trigger (still in ignore period: {Time.time - spawnTime:F3}s / {collisionIgnoreTime}s)");
            return;
        }
        
        // Ignore player and bullet triggers
        int playerLayer = LayerMask.NameToLayer("Player");
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        if (other.gameObject.layer == playerLayer || other.gameObject.layer == bulletLayer)
        {
            Debug.Log($"[Bullet] Ignoring trigger from player/bullet: {other.gameObject.name}");
            return;
        }
        
        Debug.Log($"[Bullet] Processing trigger with: {other.gameObject.name} (Layer: {other.gameObject.layer})");
        
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
        Debug.Log($"[Bullet] HandleHit called - Object: {hitObject.name}, HitPoint: {hitPoint}");
        Debug.Log($"[Bullet] Searching for IDamageable on: {hitObject.name}");
        
        // Damage system - search in hit object AND parent objects
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        bool foundOnDirect = damageable != null;
        
        if (foundOnDirect)
        {
            Debug.Log($"[Bullet] Found IDamageable directly on: {hitObject.name}");
        }
        else
        {
            Debug.Log($"[Bullet] No IDamageable on {hitObject.name}, searching parent hierarchy...");
            
            // If not found on hit object, search parent hierarchy
            Transform parent = hitObject.transform.parent;
            int depth = 0;
            while (parent != null && damageable == null)
            {
                depth++;
                Debug.Log($"[Bullet] Checking parent level {depth}: {parent.name}");
                damageable = parent.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Debug.Log($"[Bullet] Found IDamageable on parent level {depth}: {parent.name}");
                    break;
                }
                parent = parent.parent;
            }
            
            if (damageable == null)
            {
                Debug.LogWarning($"[Bullet] No IDamageable found on {hitObject.name} or any of its {depth} parent levels!");
                Debug.LogWarning($"[Bullet] Full hierarchy: {GetGameObjectPath(hitObject)}");
            }
        }
        
        if (damageable != null)
        {
            Debug.Log($"[Bullet] DEALING {damage} DAMAGE to {hitObject.name} (found via {(foundOnDirect ? "direct" : "parent")})");
            damageable.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning($"[Bullet] Cannot deal damage - No IDamageable component found!");
        }
        
        // Penetration logic (only for shotgun pellets)
        if (canPenetrate && penetrationCount < maxPenetrations)
        {
            penetrationCount++;
            Debug.Log($"[Bullet] Penetration {penetrationCount}/{maxPenetrations} - continuing");
            // Continue traveling - don't destroy yet
            return;
        }
        
        // Regular bullets (non-penetrating) always destroy immediately
        hasHit = true;
        DestroyBullet();
    }
    
    string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
    
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}


