using UnityEngine;

public class LayerSetupHelper : MonoBehaviour
{
    [Header("Layer Configuration")]
    [Tooltip("Run this to automatically configure physics layers for FPS gameplay")]
    
    [Header("Layer Names")]
    public string playerLayerName = "Player";
    public string enemyLayerName = "Enemy";
    public string environmentLayerName = "Environment";
    public string bulletLayerName = "Bullet";
    public string groundLayerName = "Ground";
    
    [ContextMenu("Setup Physics Layers")]
    public void SetupPhysicsLayers()
    {
        Debug.Log("=== Setting up Physics Layers for FPS Game ===");
        
        // Get or create layer indices
        int playerLayer = GetOrCreateLayer(playerLayerName);
        int enemyLayer = GetOrCreateLayer(enemyLayerName);
        int environmentLayer = GetOrCreateLayer(environmentLayerName);
        int bulletLayer = GetOrCreateLayer(bulletLayerName);
        int groundLayer = GetOrCreateLayer(groundLayerName);
        int defaultLayer = 0; // Default layer
        
        // Configure collision matrix
        ConfigureLayerCollisions(playerLayer, enemyLayer, environmentLayer, bulletLayer, groundLayer, defaultLayer);
        
        Debug.Log("=== Physics Layer Setup Complete ===");
        Debug.Log($"Player Layer: {playerLayer}");
        Debug.Log($"Enemy Layer: {enemyLayer}");
        Debug.Log($"Environment Layer: {environmentLayer}");
        Debug.Log($"Bullet Layer: {bulletLayer}");
        Debug.Log($"Ground Layer: {groundLayer}");
        
        PrintCollisionMatrix();
    }
    
    int GetOrCreateLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' not found. Please create it manually in Tags & Layers.");
            Debug.LogWarning("Go to Edit → Project Settings → Tags and Layers → Layers");
        }
        return layer;
    }
    
    void ConfigureLayerCollisions(int player, int enemy, int environment, int bullet, int ground, int defaultLayer)
    {
        // Player collisions
        if (player != -1)
        {
            Physics.IgnoreLayerCollision(player, bullet, true);  // Player doesn't collide with bullets
            Physics.IgnoreLayerCollision(player, player, true);  // Players don't collide with each other
        }
        
        // Bullet collisions
        if (bullet != -1)
        {
            Physics.IgnoreLayerCollision(bullet, bullet, true);  // Bullets don't collide with each other
            if (player != -1) Physics.IgnoreLayerCollision(bullet, player, true);  // Bullets don't hit player
            
            // Bullets DO collide with:
            if (enemy != -1) Physics.IgnoreLayerCollision(bullet, enemy, false);       // Enemies
            if (environment != -1) Physics.IgnoreLayerCollision(bullet, environment, false); // Environment
            if (ground != -1) Physics.IgnoreLayerCollision(bullet, ground, false);     // Ground
            Physics.IgnoreLayerCollision(bullet, defaultLayer, false);                 // Default objects
        }
        
        // Enemy collisions
        if (enemy != -1)
        {
            if (player != -1) Physics.IgnoreLayerCollision(enemy, player, false);  // Enemies can hit player
            Physics.IgnoreLayerCollision(enemy, enemy, true);   // Enemies don't collide with each other
        }
        
        // Environment and Ground should collide with everything by default
        
        Debug.Log("Collision matrix configured:");
        Debug.Log("- Bullets will hit: Enemies, Environment, Ground, Default objects");
        Debug.Log("- Bullets will NOT hit: Player, Other bullets");
        Debug.Log("- Player will NOT hit: Bullets, Other players");
        Debug.Log("- Enemies will NOT hit: Other enemies");
    }
    
    void PrintCollisionMatrix()
    {
        Debug.Log("\n=== Current Collision Matrix ===");
        string[] layerNames = {"Default", playerLayerName, enemyLayerName, environmentLayerName, bulletLayerName, groundLayerName};
        
        foreach (string layer1 in layerNames)
        {
            int layerIndex1 = LayerMask.NameToLayer(layer1);
            if (layerIndex1 == -1 && layer1 != "Default") continue;
            if (layer1 == "Default") layerIndex1 = 0;
            
            foreach (string layer2 in layerNames)
            {
                int layerIndex2 = LayerMask.NameToLayer(layer2);
                if (layerIndex2 == -1 && layer2 != "Default") continue;
                if (layer2 == "Default") layerIndex2 = 0;
                
                bool collides = !Physics.GetIgnoreLayerCollision(layerIndex1, layerIndex2);
                Debug.Log($"{layer1} <-> {layer2}: {(collides ? "COLLIDES" : "IGNORES")}");
            }
        }
    }
    
    [ContextMenu("Apply Layers to Scene Objects")]
    public void ApplyLayersToSceneObjects()
    {
        Debug.Log("=== Applying Layers to Scene Objects ===");
        
        // Find and set player layer
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = FindObjectOfType<FPSPlayerController>()?.gameObject;
        if (player != null)
        {
            SetLayerRecursively(player, LayerMask.NameToLayer(playerLayerName));
            Debug.Log($"Set {player.name} to Player layer");
        }
        
        // Find and set ground objects
        GameObject[] planes = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject plane in planes)
        {
            SetLayerRecursively(plane, LayerMask.NameToLayer(groundLayerName));
            Debug.Log($"Set {plane.name} to Ground layer");
        }
        
        // Find objects with "Environment" in name
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("environment") || obj.name.ToLower().Contains("wall"))
            {
                SetLayerRecursively(obj, LayerMask.NameToLayer(environmentLayerName));
                Debug.Log($"Set {obj.name} to Environment layer");
            }
        }
        
        Debug.Log("=== Layer Application Complete ===");
    }
    
    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (layer == -1) return;
        
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}





