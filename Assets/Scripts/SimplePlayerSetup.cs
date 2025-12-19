using UnityEngine;
using SnackAttack.Player;
using SnackAttack.Weapons;

namespace SnackAttack.Setup
{
    /// <summary>
    /// Simple, reliable player setup that works without editor dependencies
    /// </summary>
    public class SimplePlayerSetup : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private GameObject knifePrefab; // Drag the knife prefab from FPSAxe folder here
        [SerializeField] private RuntimeAnimatorController axeAnimatorController; // Drag the AxeAnimatorController here
        
        [Header("Setup Settings")]
        [SerializeField] private Vector3 playerStartPosition = Vector3.zero;
        [SerializeField] private Vector3 weaponPosition = new Vector3(0.5f, -0.5f, 1f);
        [SerializeField] private bool createTestEnvironment = true;
        
        [ContextMenu("Setup Player")]
        public void SetupPlayer()
        {
            if (knifePrefab == null)
            {
                return;
            }
            
            // Create main player GameObject
            GameObject player = new GameObject("Player");
            player.transform.position = playerStartPosition;
            
            // Add CharacterController
            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 1, 0);
            
            // Add FPSController
            FPSController fpsController = player.AddComponent<FPSController>();
            
            // Add WeaponManager
            WeaponManager weaponManager = player.AddComponent<WeaponManager>();
            
            // Create camera
            GameObject cameraGO = new GameObject("PlayerCamera");
            cameraGO.transform.SetParent(player.transform);
            cameraGO.transform.localPosition = new Vector3(0, 1.6f, 0);
            Camera playerCamera = cameraGO.AddComponent<Camera>();
            
            // Create ground check
            GameObject groundCheckGO = new GameObject("GroundCheck");
            groundCheckGO.transform.SetParent(player.transform);
            groundCheckGO.transform.localPosition = new Vector3(0, -1f, 0);
            
            // Set up FPS Controller references
            SetPrivateField(fpsController, "playerCamera", playerCamera);
            SetPrivateField(fpsController, "groundCheck", groundCheckGO.transform);
            
            // Setup weapon
            SetupAxeWeapon(cameraGO, weaponManager);
            
            if (createTestEnvironment)
            {
                CreateTestEnvironment();
            }
        }
        
        private void SetupAxeWeapon(GameObject cameraGO, WeaponManager weaponManager)
        {
            // Instantiate the axe
            GameObject axeInstance = Instantiate(knifePrefab);
            axeInstance.name = "Axe";
            axeInstance.transform.SetParent(cameraGO.transform);
            axeInstance.transform.localPosition = weaponPosition;
            axeInstance.transform.localRotation = Quaternion.identity;
            
            // Add AxeWeapon component
            AxeWeapon axeWeapon = axeInstance.AddComponent<AxeWeapon>();
            
            // Set up animator if controller is provided
            Animator animator = axeInstance.GetComponent<Animator>();
            if (animator != null && axeAnimatorController != null)
            {
                animator.runtimeAnimatorController = axeAnimatorController;
            }
            
            // Set up weapon manager
            SetPrivateField(weaponManager, "weapons", new BaseWeapon[] { axeWeapon });
        }
        
        private void CreateTestEnvironment()
        {
            // Create ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0, -1, 0);
            ground.transform.localScale = new Vector3(10, 1, 10);
            
            // Set ground material
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundRenderer.material.color = Color.gray;
            }
            
            // Create test enemy
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "TestEnemy";
            enemy.transform.position = new Vector3(3, 0.5f, 3);
            enemy.transform.localScale = Vector3.one;
            
            // Add TestEnemy component
            enemy.AddComponent<SnackAttack.Testing.TestEnemy>();
            
            // Set enemy material
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.red;
            }
        }
        
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
            }
        }
        
        private void OnValidate()
        {
            // Help text in inspector
            if (knifePrefab == null)
            {
            }
        }
    }
}
