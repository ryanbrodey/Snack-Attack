using UnityEngine;
using SnackAttack.Player;
using SnackAttack.Weapons;

namespace SnackAttack.Setup
{
    /// <summary>
    /// Connects the FPS scripts to your existing Arms/knife setup
    /// </summary>
    public class ConnectExistingSetup : MonoBehaviour
    {
        [Header("Your Existing Objects")]
        [SerializeField] private GameObject armsObject; // Drag your "Arms" object here
        [SerializeField] private GameObject knifeObject; // Drag your "knife" object here
        [SerializeField] private Camera mainCamera; // Drag your "Main Camera" here
        
        [Header("Animator Controller")]
        [SerializeField] private RuntimeAnimatorController axeAnimatorController; // Drag AxeAnimatorController here
        
        [ContextMenu("Connect Everything")]
        public void ConnectEverything()
        {
            if (armsObject == null || knifeObject == null || mainCamera == null)
            {
                Debug.LogError("Please assign your Arms object, knife object, and Main Camera!");
                return;
            }
            
            // Add components to Arms object
            SetupArmsObject();
            
            // Add components to knife object
            SetupKnifeObject();
            
            // Create ground check
            CreateGroundCheck();
            
            // Create test ground
            CreateTestGround();
            
            // Create test enemy
            CreateTestEnemy();
            
            Debug.Log("Setup complete! Your existing Arms/knife setup is now connected to the FPS system!");
            Debug.Log("Don't forget to add Animation Events to axe_ATK1 and axe_ATK2 animations!");
        }
        
        private void SetupArmsObject()
        {
            // Add CharacterController if not present
            CharacterController cc = armsObject.GetComponent<CharacterController>();
            if (cc == null)
            {
                cc = armsObject.AddComponent<CharacterController>();
                cc.height = 2f;
                cc.radius = 0.5f;
                cc.center = new Vector3(0, 1, 0);
                Debug.Log("Added CharacterController to Arms");
            }
            
            // Add FPSController if not present
            FPSController fpsController = armsObject.GetComponent<FPSController>();
            if (fpsController == null)
            {
                fpsController = armsObject.AddComponent<FPSController>();
                Debug.Log("Added FPSController to Arms");
            }
            
            // Add WeaponManager if not present
            WeaponManager weaponManager = armsObject.GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                weaponManager = armsObject.AddComponent<WeaponManager>();
                Debug.Log("Added WeaponManager to Arms");
            }
            
            // Set up FPS Controller references
            SetPrivateField(fpsController, "playerCamera", mainCamera);
            
            // Find ground check (will be created next)
            Transform groundCheck = armsObject.transform.Find("GroundCheck");
            if (groundCheck != null)
            {
                SetPrivateField(fpsController, "groundCheck", groundCheck);
            }
        }
        
        private void SetupKnifeObject()
        {
            // Add AxeWeapon if not present
            AxeWeapon axeWeapon = knifeObject.GetComponent<AxeWeapon>();
            if (axeWeapon == null)
            {
                axeWeapon = knifeObject.AddComponent<AxeWeapon>();
                Debug.Log("Added AxeWeapon to knife");
            }
            
            // Set up animator controller
            Animator animator = knifeObject.GetComponent<Animator>();
            if (animator != null && axeAnimatorController != null)
            {
                animator.runtimeAnimatorController = axeAnimatorController;
                Debug.Log("Assigned AxeAnimatorController to knife");
            }
            else if (animator == null)
            {
                Debug.LogWarning("No Animator found on knife object!");
            }
            else if (axeAnimatorController == null)
            {
                Debug.LogWarning("Please assign the AxeAnimatorController!");
            }
            
            // Set up weapon manager reference
            WeaponManager weaponManager = armsObject.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                SetPrivateField(weaponManager, "weapons", new BaseWeapon[] { axeWeapon });
                Debug.Log("Connected axe to weapon manager");
            }
        }
        
        private void CreateGroundCheck()
        {
            // Check if ground check already exists
            Transform existingGroundCheck = armsObject.transform.Find("GroundCheck");
            if (existingGroundCheck != null)
            {
                Debug.Log("GroundCheck already exists");
                return;
            }
            
            // Create ground check
            GameObject groundCheckGO = new GameObject("GroundCheck");
            groundCheckGO.transform.SetParent(armsObject.transform);
            groundCheckGO.transform.localPosition = new Vector3(0, -1f, 0);
            
            // Set up FPS Controller reference
            FPSController fpsController = armsObject.GetComponent<FPSController>();
            if (fpsController != null)
            {
                SetPrivateField(fpsController, "groundCheck", groundCheckGO.transform);
            }
            
            Debug.Log("Created GroundCheck");
        }
        
        private void CreateTestGround()
        {
            // Check if ground already exists
            if (GameObject.Find("Ground") != null)
            {
                Debug.Log("Ground already exists");
                return;
            }
            
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0, -1, 0);
            ground.transform.localScale = new Vector3(10, 1, 10);
            
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundRenderer.material.color = Color.gray;
            }
            
            Debug.Log("Created test ground");
        }
        
        private void CreateTestEnemy()
        {
            // Check if test enemy already exists
            if (GameObject.Find("TestEnemy") != null)
            {
                Debug.Log("TestEnemy already exists");
                return;
            }
            
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy.name = "TestEnemy";
            enemy.transform.position = new Vector3(3, 0.5f, 3);
            enemy.transform.localScale = Vector3.one;
            
            enemy.AddComponent<SnackAttack.Testing.TestEnemy>();
            
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.red;
            }
            
            Debug.Log("Created test enemy at (3, 0.5, 3)");
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
        
        private void OnValidate()
        {
            // Auto-find objects if not assigned
            if (armsObject == null)
            {
                GameObject arms = GameObject.Find("Arms");
                if (arms != null)
                {
                    armsObject = arms;
                    Debug.Log("Auto-found Arms object");
                }
            }
            
            if (knifeObject == null && armsObject != null)
            {
                Transform knife = armsObject.transform.Find("knife");
                if (knife == null)
                {
                    // Try to find knife anywhere in the hierarchy
                    knife = FindDeepChild(armsObject.transform, "knife");
                }
                if (knife != null)
                {
                    knifeObject = knife.gameObject;
                    Debug.Log("Auto-found knife object");
                }
            }
            
            if (mainCamera == null && armsObject != null)
            {
                Camera cam = armsObject.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    mainCamera = cam;
                    Debug.Log("Auto-found Main Camera");
                }
            }
        }
        
        private Transform FindDeepChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;
                
                Transform result = FindDeepChild(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
