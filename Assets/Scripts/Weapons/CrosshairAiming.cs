using UnityEngine;

public static class CrosshairAiming
{
    /// <summary>
    /// Get the world point where the crosshair is aiming
    /// </summary>
    public static Vector3 GetAimPoint(Camera playerCamera, float maxRange = 1000f)
    {
        // Cast ray from screen center (where crosshair is)
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        
        // Try to hit something
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            Debug.Log($"Crosshair aiming at: {hit.collider.name} at {hit.point}");
            return hit.point;
        }
        
        // If nothing hit, use a point far away
        Vector3 farPoint = ray.origin + ray.direction * maxRange;
        Debug.Log($"Crosshair aiming at distant point: {farPoint}");
        return farPoint;
    }
    
    /// <summary>
    /// Get direction from bullet spawn to crosshair target
    /// </summary>
    public static Vector3 GetBulletDirection(Vector3 bulletSpawnPos, Camera playerCamera, float maxRange = 1000f)
    {
        Vector3 targetPoint = GetAimPoint(playerCamera, maxRange);
        Vector3 direction = (targetPoint - bulletSpawnPos).normalized;
        
        Debug.Log($"Bullet direction from {bulletSpawnPos} to {targetPoint}: {direction}");
        return direction;
    }
    
    /// <summary>
    /// Get multiple directions for shotgun spread
    /// </summary>
    public static Vector3[] GetShotgunDirections(Vector3 bulletSpawnPos, Camera playerCamera, int pelletCount = 8, float spreadAngle = 15f, float maxRange = 1000f)
    {
        Vector3 centerDirection = GetBulletDirection(bulletSpawnPos, playerCamera, maxRange);
        Vector3[] directions = new Vector3[pelletCount];
        
        Debug.Log($"Generating {pelletCount} shotgun pellet directions with {spreadAngle}° spread");
        
        for (int i = 0; i < pelletCount; i++)
        {
            // Create random spread around center direction
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            
            // Apply spread to center direction
            Vector3 spreadDirection = Quaternion.Euler(randomY, randomX, 0) * centerDirection;
            directions[i] = spreadDirection.normalized;
        }
        
        return directions;
    }
    
    /// <summary>
    /// Debug visualization for crosshair aiming
    /// </summary>
    public static void DrawAimDebug(Vector3 bulletSpawnPos, Camera playerCamera, float duration = 2f)
    {
        if (playerCamera == null) return;
        
        Vector3 aimPoint = GetAimPoint(playerCamera);
        Vector3 direction = GetBulletDirection(bulletSpawnPos, playerCamera);
        
        // Draw line from spawn to aim point
        Debug.DrawLine(bulletSpawnPos, aimPoint, Color.red, duration);
        
        // Draw crosshair ray
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, duration);
    }
}


