using UnityEngine;

public static class CrosshairAiming
{
    // Get the world point where the crosshair is aiming
    public static Vector3 GetAimPoint(Camera playerCamera, float maxRange = 1000f)
    {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            return hit.point;
        }
        
        Vector3 farPoint = ray.origin + ray.direction * maxRange;
        return farPoint;
    }
    
    // Get direction from bullet spawn to crosshair target
    public static Vector3 GetBulletDirection(Vector3 bulletSpawnPos, Camera playerCamera, float maxRange = 1000f)
    {
        Vector3 targetPoint = GetAimPoint(playerCamera, maxRange);
        Vector3 direction = (targetPoint - bulletSpawnPos).normalized;
        
        return direction;
    }
    
    // Get bullet direction from camera forward
    public static Vector3 GetBulletDirectionFromCamera(Camera playerCamera)
    {
        if (playerCamera == null)
        {
            return Vector3.forward;
        }
        
        Vector3 direction = playerCamera.transform.forward;
        
        return direction;
    }
    
    // Get bullet direction from spawn position to crosshair target
    public static Vector3 GetBulletDirectionFromSpawnToCrosshair(Vector3 bulletSpawnPos, Camera playerCamera, float maxRange = 1000f)
    {
        if (playerCamera == null)
        {
            return Vector3.forward;
        }
        
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        
        Vector3 targetPoint;
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * maxRange;
        }
        
        Vector3 direction = (targetPoint - bulletSpawnPos).normalized;
        
        return direction;
    }
    
    // Get multiple directions for shotgun spread
    public static Vector3[] GetShotgunDirections(Vector3 bulletSpawnPos, Camera playerCamera, int pelletCount = 8, float spreadAngle = 15f, float maxRange = 1000f)
    {
        Vector3 centerDirection = GetBulletDirection(bulletSpawnPos, playerCamera, maxRange);
        Vector3[] directions = new Vector3[pelletCount];
        
        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            
            Vector3 spreadDirection = Quaternion.Euler(randomY, randomX, 0) * centerDirection;
            directions[i] = spreadDirection.normalized;
        }
        
        return directions;
    }
    
    // Get multiple directions for shotgun spread from camera forward
    public static Vector3[] GetShotgunDirectionsFromCamera(Camera playerCamera, int pelletCount = 8, float spreadAngle = 15f)
    {
        Vector3 centerDirection = GetBulletDirectionFromCamera(playerCamera);
        Vector3[] directions = new Vector3[pelletCount];
        
        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            
            Vector3 spreadDirection = Quaternion.Euler(randomY, randomX, 0) * centerDirection;
            directions[i] = spreadDirection.normalized;
        }
        
        return directions;
    }
    
    // Get multiple directions for shotgun spread from spawn to crosshair
    public static Vector3[] GetShotgunDirectionsFromSpawnToCrosshair(Vector3 bulletSpawnPos, Camera playerCamera, int pelletCount = 8, float spreadAngle = 15f, float maxRange = 1000f)
    {
        Vector3 centerDirection = GetBulletDirectionFromSpawnToCrosshair(bulletSpawnPos, playerCamera, maxRange);
        Vector3[] directions = new Vector3[pelletCount];
        
        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            
            Vector3 spreadDirection = Quaternion.Euler(randomY, randomX, 0) * centerDirection;
            directions[i] = spreadDirection.normalized;
        }
        
        return directions;
    }
    
    // Debug visualization
    public static void DrawAimDebug(Vector3 bulletSpawnPos, Camera playerCamera, float duration = 2f)
    {
        if (playerCamera == null) return;
        
        Vector3 aimPoint = GetAimPoint(playerCamera);
        Vector3 direction = GetBulletDirection(bulletSpawnPos, playerCamera);
        
        Debug.DrawLine(bulletSpawnPos, aimPoint, Color.red, duration);
        
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, duration);
    }
}


