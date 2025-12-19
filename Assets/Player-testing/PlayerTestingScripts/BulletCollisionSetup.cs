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
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
            ((SphereCollider)col).radius = 0.05f;
        }
        
        rb.useGravity = useGravity;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        col.isTrigger = false;
        
        SetupLayerCollisions();
    }
    
    void SetupLayerCollisions()
    {
        int bulletLayer = gameObject.layer;
        
        if (ignorePlayer)
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            if (playerLayer != -1)
            {
                Physics.IgnoreLayerCollision(bulletLayer, playerLayer, true);
            }
        }
        
        if (ignoreBullets)
        {
            int bulletLayerMask = LayerMask.NameToLayer("Bullet");
            if (bulletLayerMask != -1)
            {
                Physics.IgnoreLayerCollision(bulletLayer, bulletLayerMask, true);
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
    
    public bool ShouldCollideWith(GameObject other)
    {
        int otherLayer = other.layer;
        return (collisionLayers & (1 << otherLayer)) != 0;
    }
}







