using UnityEngine;
using SnackAttack.Player;
using SnackAttack.Weapons;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SnackAttack.Setup
{
    /// <summary>
    /// Automatically sets up the FPS player with the axe weapon
    /// </summary>
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Setup Configuration")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private GameObject knifePrefab; // The axe prefab from FPSAxe folder
        [SerializeField] private RuntimeAnimatorController axeAnimatorController;
        
        [Header("Player Settings")]
        [SerializeField] private Vector3 playerStartPosition = Vector3.zero;
        [SerializeField] private float cameraHeight = 1.6f;
        [SerializeField] private Vector3 weaponPosition = new Vector3(0.5f, -0.5f, 1f);
        [SerializeField] private Vector3 weaponRotation = new Vector3(0f, 0f, 0f);
        
        private void Start()
        {
            if (setupOnStart)
            {
                SetupPlayer();
            }
        }
        
        [ContextMenu("Setup Player")]
        public void SetupPlayer()
        {
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
            cameraGO.transform.localPosition = new Vector3(0, cameraHeight, 0);
            Camera playerCamera = cameraGO.AddComponent<Camera>();
            
            // Create ground check
            GameObject groundCheckGO = new GameObject("GroundCheck");
            groundCheckGO.transform.SetParent(player.transform);
            groundCheckGO.transform.localPosition = new Vector3(0, -1f, 0);
            
            // Set up FPS Controller references using reflection
            SetPrivateField(fpsController, "playerCamera", playerCamera);
            SetPrivateField(fpsController, "groundCheck", groundCheckGO.transform);
            
            // Setup weapon if prefab is assigned
            if (knifePrefab != null)
            {
                SetupAxeWeapon(cameraGO, weaponManager);
            }
            
            // Create ground for testing
            CreateTestGround();
            
            // Create test enemy
            CreateTestEnemy();
            
            Debug.Log("Player setup complete! Use WASD to move, Mouse to look, F to attack, Space to jump.");
        }
        
        private void SetupAxeWeapon(GameObject cameraGO, WeaponManager weaponManager)
        {
            // Instantiate the axe
            GameObject axeInstance = Instantiate(knifePrefab);
            axeInstance.name = "Axe";
            axeInstance.transform.SetParent(cameraGO.transform);
            axeInstance.transform.localPosition = weaponPosition;
            axeInstance.transform.localRotation = Quaternion.Euler(weaponRotation);
            
            // Add AxeWeapon component
            AxeWeapon axeWeapon = axeInstance.AddComponent<AxeWeapon>();
            
            // Set up animator
            Animator animator = axeInstance.GetComponent<Animator>();
            if (animator != null && axeAnimatorController != null)
            {
                animator.runtimeAnimatorController = axeAnimatorController;
            }
            
            // Set up weapon manager
            SetPrivateField(weaponManager, "weapons", new BaseWeapon[] { axeWeapon });
            
            Debug.Log("Axe weapon setup complete!");
        }
        
        private void CreateTestGround()
        {
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
        }
        
        private void CreateTestEnemy()
        {
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
            
            Debug.Log("Test enemy created at (3, 0.5, 3). Attack it to test damage!");
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
                Debug.LogWarning($"Could not find field '{fieldName}' in {obj.GetType().Name}");
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("Find Axe Prefab")]
        public void FindAxePrefab()
        {
            // Try to find the knife prefab automatically
            string[] guids = UnityEditor.AssetDatabase.FindAssets("knife t:GameObject");
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("FPSAxe"))
                {
                    knifePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    Debug.Log($"Found axe prefab at: {path}");
                    break;
                }
            }
            
            // Try to find the animator controller
            string[] controllerGuids = UnityEditor.AssetDatabase.FindAssets("AxeAnimatorController t:RuntimeAnimatorController");
            if (controllerGuids.Length > 0)
            {
                string controllerPath = UnityEditor.AssetDatabase.GUIDToAssetPath(controllerGuids[0]);
                axeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
                Debug.Log($"Found animator controller at: {controllerPath}");
            }
        }
#endif
    }
}
