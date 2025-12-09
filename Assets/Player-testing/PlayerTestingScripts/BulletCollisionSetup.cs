using UnityEngine;

[System.Serializable]
public class BulletCollisionSetup : MonoBehaviour
{
    [Header("Bullet Physics Configuration")]
    [Tooltip("Configure this on your bullet prefab")]
    public bool setupOnStart = true;
    
    [Header("Collision Settings")]
    public LayerMask collisionLayers = -1; // What layers this bullet can hit
    public bool ignorePlayer = true;
    public bool ignoreBullets = true;
    
    [Header("Physics Settings")]
    public bool useGravity = false;
    public float drag = 0f;
    public float angularDrag = 0f;
    
    private Rigidbody rb;
    private Collider col;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupBulletPhysics();
        }
    }
    
    [ContextMenu("Setup Bullet Physics")]
    public void SetupBulletPhysics()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        if (rb == null)
        {
            Debug.LogError("BulletCollisionSetup: No Rigidbody found! Adding one...");
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        if (col == null)
        {
            Debug.LogError("BulletCollisionSetup: No Collider found! Adding SphereCollider...");
            col = gameObject.AddComponent<SphereCollider>();
            ((SphereCollider)col).radius = 0.05f; // Small bullet collider
        }
        
        // Configure Rigidbody
        rb.useGravity = useGravity;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth movement
        
        // Configure Collider
        col.isTrigger = false; // Use collision detection, not trigger
        
        // Set up layer-based collision
        SetupLayerCollisions();
        
        Debug.Log($"Bullet physics configured: Gravity={useGravity}, Drag={drag}, Collides with layers: {GetLayerNames(collisionLayers)}");
    }
    
    void SetupLayerCollisions()
    {
        // Get current layer
        int bulletLayer = gameObject.layer;
        
        // If ignorePlayer is true, disable collision with Player layer
        if (ignorePlayer)
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            if (playerLayer != -1)
            {
                Physics.IgnoreLayerCollision(bulletLayer, playerLayer, true);
                Debug.Log($"Bullet layer {bulletLayer} will ignore Player layer {playerLayer}");
            }
        }
        
        // If ignoreBullets is true, disable bullet-to-bullet collision
        if (ignoreBullets)
        {
            int bulletLayerMask = LayerMask.NameToLayer("Bullet");
            if (bulletLayerMask != -1)
            {
                Physics.IgnoreLayerCollision(bulletLayer, bulletLayerMask, true);
                Debug.Log($"Bullet layer {bulletLayer} will ignore other bullets");
            }
        }
    }
    
    string GetLayerNames(LayerMask layerMask)
    {
        string layerNames = "";
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask & (1 << i)) != 0)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    layerNames += layerName + ", ";
                }
            }
        }
        return layerNames.TrimEnd(',', ' ');
    }
    
    // Call this to check if bullet should collide with a specific object
    public bool ShouldCollideWith(GameObject other)
    {
        int otherLayer = other.layer;
        return (collisionLayers & (1 << otherLayer)) != 0;
    }
}


