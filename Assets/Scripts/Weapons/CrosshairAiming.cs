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
    /// Get bullet direction directly from camera forward (crosshair is always centered)
    /// This is more accurate and explicit - uses camera's forward direction where crosshair points
    /// </summary>
    public static Vector3 GetBulletDirectionFromCamera(Camera playerCamera)
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("CrosshairAiming: Camera is null, returning Vector3.forward");
            return Vector3.forward;
        }
        
        // Crosshair is always at screen center, so camera forward is the exact direction
        Vector3 direction = playerCamera.transform.forward;
        
        Debug.Log($"Bullet direction from camera forward: {direction}");
        return direction;
    }
    
    /// <summary>
    /// Get bullet direction from spawn position to crosshair target point
    /// This accounts for bullet spawn offset from camera center - uses raycast from screen center
    /// to get exact crosshair aim point, then calculates direction from bullet spawn to that point
    /// </summary>
    public static Vector3 GetBulletDirectionFromSpawnToCrosshair(Vector3 bulletSpawnPos, Camera playerCamera, float maxRange = 1000f)
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("CrosshairAiming: Camera is null, returning Vector3.forward");
            return Vector3.forward;
        }
        
        // Cast ray from screen center (where crosshair is) to get exact aim point
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        
        Vector3 targetPoint;
        
        // Try to hit something - this gives us the exact point where crosshair is aiming
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If nothing hit, use a point far away along the ray
            targetPoint = ray.origin + ray.direction * maxRange;
        }
        
        // Calculate direction from bullet spawn to the exact crosshair aim point
        Vector3 direction = (targetPoint - bulletSpawnPos).normalized;
        
        Debug.Log($"Bullet direction from spawn {bulletSpawnPos} to crosshair target {targetPoint}: {direction}");
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
    /// Get multiple directions for shotgun spread using camera forward (more accurate)
    /// </summary>
    public static Vector3[] GetShotgunDirectionsFromCamera(Camera playerCamera, int pelletCount = 8, float spreadAngle = 15f)
    {
        Vector3 centerDirection = GetBulletDirectionFromCamera(playerCamera);
        Vector3[] directions = new Vector3[pelletCount];
        
        Debug.Log($"Generating {pelletCount} shotgun pellet directions with {spreadAngle}° spread from camera forward");
        
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
    /// Get multiple directions for shotgun spread from spawn position to crosshair
    /// Accounts for bullet spawn offset from camera center
    /// </summary>
    public static Vector3[] GetShotgunDirectionsFromSpawnToCrosshair(Vector3 bulletSpawnPos, Camera playerCamera, int pelletCount = 8, float spreadAngle = 15f, float maxRange = 1000f)
    {
        Vector3 centerDirection = GetBulletDirectionFromSpawnToCrosshair(bulletSpawnPos, playerCamera, maxRange);
        Vector3[] directions = new Vector3[pelletCount];
        
        Debug.Log($"Generating {pelletCount} shotgun pellet directions with {spreadAngle}° spread from spawn to crosshair");
        
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


