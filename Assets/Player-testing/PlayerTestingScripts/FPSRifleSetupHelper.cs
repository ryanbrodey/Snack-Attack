using UnityEngine;

public class FPSRifleSetupHelper : MonoBehaviour
{
    [ContextMenu("Fix Setup")]
    public void FixSetup()
    {
        GameObject player = gameObject;
        
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
            cc.skinWidth = 0.08f;
            cc.minMoveDistance = 0.001f;
        }
        else
        {
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
        }
        
        FPSPlayerController fpsController = player.GetComponent<FPSPlayerController>();
        if (fpsController == null)
        {
            fpsController = player.AddComponent<FPSPlayerController>();
        }
        

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        if (playerCamera != null)
        {
            if (playerCamera.transform.parent != player.transform)
            {
                Transform cameraAnchor = player.transform.Find("CameraAnchor");
                if (cameraAnchor == null || !playerCamera.transform.IsChildOf(cameraAnchor))
                {
                    playerCamera.transform.SetParent(player.transform);
                    playerCamera.transform.localPosition = new Vector3(0, 1.643f, 0.062f);
                    playerCamera.transform.localRotation = Quaternion.identity;
                }
            }
            
            var controllerType = typeof(FPSPlayerController);
            var cameraField = controllerType.GetField("playerCamera", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (cameraField != null)
            {
                cameraField.SetValue(fpsController, playerCamera);
            }
        }
        
        Transform groundCheck = player.transform.Find("GroundCheck");
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(player.transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
        }
        
        var groundCheckField = typeof(FPSPlayerController).GetField("groundCheck", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (groundCheckField != null)
        {
            groundCheckField.SetValue(fpsController, groundCheck);
        }
        
        if (player.transform.position.y > 0.1f)
        {
            Vector3 pos = player.transform.position;
            pos.y = 0f;
            player.transform.position = pos;
        }
        
        Animator armsAnimator = player.GetComponentInChildren<Animator>();
        if (armsAnimator != null)
        {
            var armsAnimatorField = typeof(FPSPlayerController).GetField("armsAnimator", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (armsAnimatorField != null)
            {
                armsAnimatorField.SetValue(fpsController, armsAnimator);
            }
        }
    }
}








