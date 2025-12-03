using UnityEngine;
using SnackAttack.Player;
using SnackAttack.Weapons;

namespace SnackAttack.Setup
{
    /// <summary>
    /// Automatically fixes and connects all FPS system components
    /// </summary>
    public class AutoFixSetup : MonoBehaviour
    {
        [ContextMenu("Auto-Fix Everything")]
        public void AutoFixEverything()
        {
            Debug.Log("=== Starting Auto-Fix ===");
            
            // Find Arms object (should have FPSController)
            GameObject arms = GameObject.Find("Arms");
            if (arms == null)
            {
                arms = GameObject.Find("knife");
            }
            
            if (arms == null)
            {
                Debug.LogError("Could not find 'Arms' or 'knife' object! Make sure your player object exists.");
                return;
            }
            
            FixFPSController(arms);
            FixWeaponManager(arms);
            FixAxeWeapon();
            
            Debug.Log("=== Auto-Fix Complete! ===");
            Debug.Log("Check the Inspector - all references should now be assigned.");
        }
        
        private void FixFPSController(GameObject playerObject)
        {
            FPSController fpsController = playerObject.GetComponent<FPSController>();
            if (fpsController == null)
            {
                Debug.LogWarning("FPSController not found on " + playerObject.name);
                return;
            }
            
            // Find camera
            Camera camera = playerObject.GetComponentInChildren<Camera>();
            if (camera == null)
            {
                camera = Camera.main;
            }
            
            if (camera != null)
            {
                // Ensure camera is a child of the player object for proper movement
                if (camera.transform.parent != playerObject.transform)
                {
                    Debug.LogWarning($"Camera '{camera.name}' is not a child of '{playerObject.name}'. Making it a child now...");
                    camera.transform.SetParent(playerObject.transform);
                    // Reset local position to typical FPS camera height
                    camera.transform.localPosition = new Vector3(0, 1.6f, 0);
                    camera.transform.localRotation = Quaternion.identity;
                    Debug.Log("✓ Moved camera to be child of player object");
                }
                
                SetPrivateField(fpsController, "playerCamera", camera);
                Debug.Log("✓ Assigned Camera to FPSController: " + camera.name);
            }
            else
            {
                Debug.LogWarning("Could not find Camera! Make sure Main Camera exists.");
            }
            
            // Find or create GroundCheck
            Transform groundCheck = playerObject.transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                // Search in children
                groundCheck = FindInChildren(playerObject.transform, "GroundCheck");
            }
            
            if (groundCheck == null)
            {
                // Create it
                GameObject groundCheckGO = new GameObject("GroundCheck");
                groundCheckGO.transform.SetParent(playerObject.transform);
                groundCheckGO.transform.localPosition = new Vector3(0, -1f, 0);
                groundCheck = groundCheckGO.transform;
                Debug.Log("✓ Created GroundCheck");
            }
            
            SetPrivateField(fpsController, "groundCheck", groundCheck);
            Debug.Log("✓ Assigned GroundCheck to FPSController");
        }
        
        private void FixWeaponManager(GameObject playerObject)
        {
            WeaponManager weaponManager = playerObject.GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                Debug.LogWarning("WeaponManager not found on " + playerObject.name);
                return;
            }
            
            // Find AxeWeapon
            AxeWeapon axeWeapon = FindAxeWeapon();
            if (axeWeapon != null)
            {
                SetPrivateField(weaponManager, "weapons", new BaseWeapon[] { axeWeapon });
                Debug.Log("✓ Assigned AxeWeapon to WeaponManager: " + axeWeapon.name);
            }
            else
            {
                Debug.LogWarning("Could not find AxeWeapon! Make sure it's on the knife object.");
            }
        }
        
        private void FixAxeWeapon()
        {
            AxeWeapon axeWeapon = FindAxeWeapon();
            if (axeWeapon == null)
            {
                // Try to find knife object and add AxeWeapon
                GameObject knife = GameObject.Find("knife");
                if (knife != null)
                {
                    axeWeapon = knife.AddComponent<AxeWeapon>();
                    Debug.Log("✓ Added AxeWeapon to knife object");
                }
                else
                {
                    Debug.LogWarning("Could not find 'knife' object to add AxeWeapon!");
                    return;
                }
            }
            
            // Make sure animator is set up
            Animator animator = axeWeapon.GetComponent<Animator>();
            if (animator == null)
            {
                animator = axeWeapon.gameObject.AddComponent<Animator>();
                Debug.Log("✓ Added Animator to " + axeWeapon.name);
            }
            
            // Try to find and assign animator controller
            RuntimeAnimatorController[] allControllers = Resources.FindObjectsOfTypeAll<RuntimeAnimatorController>();
            RuntimeAnimatorController controller = null;
            foreach (var c in allControllers)
            {
                if (c.name.Contains("Axe") || c.name.Contains("axe"))
                {
                    controller = c;
                    break;
                }
            }
            
            if (controller != null && animator.runtimeAnimatorController == null)
            {
                animator.runtimeAnimatorController = controller;
                Debug.Log("✓ Assigned Animator Controller: " + controller.name);
            }
        }
        
        private AxeWeapon FindAxeWeapon()
        {
            // Search for AxeWeapon in scene
            AxeWeapon[] allWeapons = FindObjectsOfType<AxeWeapon>();
            if (allWeapons.Length > 0)
            {
                return allWeapons[0];
            }
            
            // Try to find on knife object
            GameObject knife = GameObject.Find("knife");
            if (knife != null)
            {
                return knife.GetComponent<AxeWeapon>();
            }
            
            return null;
        }
        
        private Transform FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;
                
                Transform result = FindInChildren(child, name);
                if (result != null)
                    return result;
            }
            return null;
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
                Debug.LogWarning($"Could not set field '{fieldName}' in {obj.GetType().Name}");
            }
        }
    }
}
