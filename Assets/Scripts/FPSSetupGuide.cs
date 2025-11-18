using UnityEngine;
using SnackAttack.Player;

namespace SnackAttack.Setup
{
    /// <summary>
    /// This script provides guidance on how to set up the FPS system
    /// Attach this to any GameObject to see the setup instructions in the Inspector
    /// </summary>
    public class FPSSetupGuide : MonoBehaviour
    {
        [Header("FPS System Setup Guide")]
        [TextArea(20, 30)]
        [SerializeField] private string setupInstructions = @"
=== FPS SYSTEM SETUP GUIDE ===

1. PLAYER SETUP:
   - Create an empty GameObject named 'Player'
   - Add CharacterController component
   - Add FPSController script
   - Add WeaponManager script
   
2. CAMERA SETUP:
   - Create a child GameObject named 'PlayerCamera'
   - Add Camera component
   - Position at (0, 1.6, 0) relative to Player
   - Assign this camera to FPSController's 'Player Camera' field
   
3. GROUND CHECK SETUP:
   - Create a child GameObject named 'GroundCheck'
   - Position at (0, -1, 0) relative to Player
   - Assign to FPSController's 'Ground Check' field
   
4. WEAPON SETUP:
   - Drag the 'knife' prefab from FPSAxe folder into the scene
   - Make it a child of PlayerCamera
   - Position appropriately (try: 0.5, -0.5, 1)
   - Add AxeWeapon script to the knife GameObject
   - Assign the knife's Animator to the AxeWeapon's 'Weapon Animator' field
   
5. ANIMATION SETUP:
   - Create an Animator Controller for the axe
   - Add animation states: Idle, Attack, Walk, Run
   - Set up transitions between states
   - Add Animation Events to call OnAttackImpact() and OnAttackAnimationComplete()
   
6. GROUND SETUP:
   - Create a plane or cube for the ground
   - Set its layer to 'Default' or create a 'Ground' layer
   - Make sure the Ground Layer Mask in FPSController matches
   
7. TESTING:
   - Add TestEnemy script to cubes or other objects
   - Set their layer to include them in the weapon's Attack Layer Mask
   
CONTROLS:
- WASD: Move
- Mouse: Look around
- Space: Attack
- Shift: Run
- Escape: Toggle cursor lock
- 1-4: Switch weapons (when multiple weapons are added)
- Mouse Wheel: Cycle weapons

EXTENDING THE SYSTEM:
- Create new weapon classes inheriting from BaseWeapon
- Override PerformAttack() method for custom attack logic
- Add new weapons to WeaponManager's weapons array
- Create appropriate animation controllers for each weapon type
        ";
        
        [Header("Quick Setup")]
        [SerializeField] private bool autoSetupPlayer = false;
        [SerializeField] private GameObject knifePrefab;
        
        private void Start()
        {
            if (autoSetupPlayer)
            {
                QuickSetupPlayer();
            }
        }
        
        [ContextMenu("Print Setup Instructions")]
        private void PrintSetupInstructions()
        {
            Debug.Log(setupInstructions);
        }
        
        [ContextMenu("Quick Setup Player")]
        private void QuickSetupPlayer()
        {
            // This method can be called from the context menu to quickly set up a basic player
            GameObject player = new GameObject("Player");
            
            // Add CharacterController
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
            
            // Add FPSController
            var fpsController = player.AddComponent<FPSController>();
            
            // Add WeaponManager
            player.AddComponent<WeaponManager>();
            
            // Create camera
            GameObject cameraGO = new GameObject("PlayerCamera");
            cameraGO.transform.SetParent(player.transform);
            cameraGO.transform.localPosition = new Vector3(0, 1.6f, 0);
            Camera cam = cameraGO.AddComponent<Camera>();
            
            // Create ground check
            GameObject groundCheckGO = new GameObject("GroundCheck");
            groundCheckGO.transform.SetParent(player.transform);
            groundCheckGO.transform.localPosition = new Vector3(0, -1f, 0);
            
            // Set up FPS Controller references using reflection (since fields are private)
            var fpsControllerType = typeof(SnackAttack.Player.FPSController);
            var cameraField = fpsControllerType.GetField("playerCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var groundCheckField = fpsControllerType.GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (cameraField != null) cameraField.SetValue(fpsController, cam);
            if (groundCheckField != null) groundCheckField.SetValue(fpsController, groundCheckGO.transform);
            
            // Position player at origin
            player.transform.position = Vector3.zero;
            
            Debug.Log("Basic Player setup complete! Don't forget to add weapons and set up the ground.");
        }
    }
}
