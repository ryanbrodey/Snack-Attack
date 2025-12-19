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
        int playerLayer = GetOrCreateLayer(playerLayerName);
        int enemyLayer = GetOrCreateLayer(enemyLayerName);
        int environmentLayer = GetOrCreateLayer(environmentLayerName);
        int bulletLayer = GetOrCreateLayer(bulletLayerName);
        int groundLayer = GetOrCreateLayer(groundLayerName);
        int defaultLayer = 0;
        
        ConfigureLayerCollisions(playerLayer, enemyLayer, environmentLayer, bulletLayer, groundLayer, defaultLayer);
        PrintCollisionMatrix();
    }
    
    int GetOrCreateLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        return layer;
    }
    
    void ConfigureLayerCollisions(int player, int enemy, int environment, int bullet, int ground, int defaultLayer)
    {
        if (player != -1)
        {
            Physics.IgnoreLayerCollision(player, bullet, true);
            Physics.IgnoreLayerCollision(player, player, true);
        }
        
        if (bullet != -1)
        {
            Physics.IgnoreLayerCollision(bullet, bullet, true);
            if (player != -1) Physics.IgnoreLayerCollision(bullet, player, true);
            
            if (enemy != -1) Physics.IgnoreLayerCollision(bullet, enemy, false);
            if (environment != -1) Physics.IgnoreLayerCollision(bullet, environment, false);
            if (ground != -1) Physics.IgnoreLayerCollision(bullet, ground, false);
            Physics.IgnoreLayerCollision(bullet, defaultLayer, false);
        }
        
        if (enemy != -1)
        {
            if (player != -1) Physics.IgnoreLayerCollision(enemy, player, false);
            Physics.IgnoreLayerCollision(enemy, enemy, true);
        }
    }
    
    void PrintCollisionMatrix()
    {
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
            }
        }
    }
    
    [ContextMenu("Apply Layers to Scene Objects")]
    public void ApplyLayersToSceneObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = FindObjectOfType<FPSPlayerController>()?.gameObject;
        if (player != null)
        {
            SetLayerRecursively(player, LayerMask.NameToLayer(playerLayerName));
        }
        
        GameObject[] planes = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject plane in planes)
        {
            SetLayerRecursively(plane, LayerMask.NameToLayer(groundLayerName));
        }
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("environment") || obj.name.ToLower().Contains("wall"))
            {
                SetLayerRecursively(obj, LayerMask.NameToLayer(environmentLayerName));
            }
        }
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







