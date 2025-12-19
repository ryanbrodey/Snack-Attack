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
            rb.useGravity = false;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.mass = 0.01f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        
        if (col != null)
        {
            col.isTrigger = true;
            
            if (col is SphereCollider)
            {
                SphereCollider sphereCol = (SphereCollider)col;
                sphereCol.material = null;
            }
            else if (col is BoxCollider)
            {
                BoxCollider boxCol = (BoxCollider)col;
                boxCol.material = null;
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capCol = (CapsuleCollider)col;
                capCol.material = null;
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
            
            float velocityDot = Vector3.Dot(rb.velocity.normalized, targetDirection);
            if (velocityDot < 0.5f || rb.velocity.magnitude < speed * 0.5f)
            {
                rb.velocity = desiredVelocity;
            }
            else
            {
                rb.velocity = desiredVelocity;
            }
            
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.drag = 0f;
        }
        else
        {
            // Fallback: move using transform
            transform.position += targetDirection * speed * Time.fixedDeltaTime;
        }
    }
    
    public void Initialize(Vector3 direction, Vector3 origin, float bulletSpeed = -1f)
    {
        if (direction == Vector3.zero)
        {
            direction = Vector3.forward;
        }
        
        direction = direction.normalized;
        
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
        if (ignoreCollisions)
        {
            return;
        }
        
        int playerLayer = LayerMask.NameToLayer("Player");
        if (collision.gameObject.layer == playerLayer)
        {
            return;
        }
        
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        if (collision.gameObject.layer == bulletLayer)
        {
            return;
        }
        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (!canPenetrate)
        {
            HandleHit(collision.gameObject, collision.contacts[0].point);
            DestroyBullet();
            return;
        }
        
        if (hasHit)
        {
            DestroyBullet();
            return;
        }
        
        HandleHit(collision.gameObject, collision.contacts[0].point);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (ignoreCollisions)
        {
            return;
        }
        
        int playerLayer = LayerMask.NameToLayer("Player");
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        if (other.gameObject.layer == playerLayer || other.gameObject.layer == bulletLayer)
        {
            return;
        }
        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (!canPenetrate)
        {
            HandleHit(other.gameObject, other.ClosestPoint(transform.position));
            DestroyBullet();
            return;
        }
        
        if (hasHit)
        {
            DestroyBullet();
            return;
        }
        
        HandleHit(other.gameObject, other.ClosestPoint(transform.position));
    }
    
    void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        
        if (damageable == null)
        {
            Transform parent = hitObject.transform.parent;
            while (parent != null && damageable == null)
            {
                damageable = parent.GetComponent<IDamageable>();
                parent = parent.parent;
            }
        }
        
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        
        if (canPenetrate && penetrationCount < maxPenetrations)
        {
            penetrationCount++;
            return;
        }
        
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


