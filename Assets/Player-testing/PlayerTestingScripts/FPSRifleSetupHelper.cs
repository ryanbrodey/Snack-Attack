using UnityEngine;

/// <summary>
/// Helper script to fix FPS_Player_Rifle setup issues.
/// Attach this to the FPS_Player_Rifle GameObject in the scene and click "Fix Setup" in the Inspector.
/// </summary>
public class FPSRifleSetupHelper : MonoBehaviour
{
    [ContextMenu("Fix Setup")]
    public void FixSetup()
    {
        GameObject player = gameObject;
        
        // 1. Add CharacterController if missing
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
            cc.skinWidth = 0.08f;
            cc.minMoveDistance = 0.001f;
            Debug.Log("✓ Added CharacterController");
        }
        else
        {
            // Ensure correct settings
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
            Debug.Log("✓ Updated CharacterController settings");
        }
        
        // 2. Add FPSPlayerController if missing
        FPSPlayerController fpsController = player.GetComponent<FPSPlayerController>();
        if (fpsController == null)
        {
            fpsController = player.AddComponent<FPSPlayerController>();
            Debug.Log("✓ Added FPSPlayerController");
        }
        
        // 3. Find and assign camera
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            Debug.LogWarning("Could not find camera in children, using Camera.main");
        }
        
        if (playerCamera != null)
        {
            // Ensure camera is a child of player (for proper movement)
            if (playerCamera.transform.parent != player.transform)
            {
                // Check if it's under CameraAnchor
                Transform cameraAnchor = player.transform.Find("CameraAnchor");
                if (cameraAnchor != null && playerCamera.transform.IsChildOf(cameraAnchor))
                {
                    // Camera is already properly parented under CameraAnchor -> Player hierarchy
                    Debug.Log("✓ Camera is properly parented under CameraAnchor");
                }
                else
                {
                    // Make camera a direct child of player
                    playerCamera.transform.SetParent(player.transform);
                    playerCamera.transform.localPosition = new Vector3(0, 1.643f, 0.062f);
                    playerCamera.transform.localRotation = Quaternion.identity;
                    Debug.Log("✓ Moved camera to be child of player");
                }
            }
            
            // Set camera reference using reflection
            var controllerType = typeof(FPSPlayerController);
            var cameraField = controllerType.GetField("playerCamera", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (cameraField != null)
            {
                cameraField.SetValue(fpsController, playerCamera);
                Debug.Log($"✓ Assigned Camera '{playerCamera.name}' to FPSPlayerController");
            }
        }
        
        // 4. Find or create GroundCheck
        Transform groundCheck = player.transform.Find("GroundCheck");
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(player.transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
            Debug.Log("✓ Created GroundCheck");
        }
        
        // Set ground check reference
        var groundCheckField = typeof(FPSPlayerController).GetField("groundCheck", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (groundCheckField != null)
        {
            groundCheckField.SetValue(fpsController, groundCheck);
            Debug.Log("✓ Assigned GroundCheck to FPSPlayerController");
        }
        
        // 5. Fix player position - ensure it's on the ground
        // CharacterController center is at (0, 1, 0) with height 2, so bottom is at y=0
        // If player is floating, set Y position to 0 (assuming ground is at y=0)
        if (player.transform.position.y > 0.1f)
        {
            Vector3 pos = player.transform.position;
            pos.y = 0f;
            player.transform.position = pos;
            Debug.Log($"✓ Reset player Y position to 0 (was {player.transform.position.y})");
        }
        
        // 6. Find arms animator
        Animator armsAnimator = player.GetComponentInChildren<Animator>();
        if (armsAnimator != null)
        {
            var armsAnimatorField = typeof(FPSPlayerController).GetField("armsAnimator", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (armsAnimatorField != null)
            {
                armsAnimatorField.SetValue(fpsController, armsAnimator);
                Debug.Log($"✓ Assigned Animator '{armsAnimator.name}' to FPSPlayerController");
            }
        }
        
        Debug.Log("=== FPS_Player_Rifle Setup Complete ===");
        Debug.Log("The player should now work correctly. Test by pressing Play.");
    }
}

