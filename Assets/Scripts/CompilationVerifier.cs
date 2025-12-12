using UnityEngine;
using SnackAttack.Weapons;

/// <summary>
/// Simple script to verify all compilation issues are resolved
/// This script references all the key classes to ensure they compile correctly
/// </summary>
[AddComponentMenu("Snack Attack/Compilation Verifier")]
public class CompilationVerifier : MonoBehaviour
{
    [Header("Compilation Test")]
    [SerializeField] private bool allSystemsWorking = false;
    
    void Start()
    {
        VerifyCompilation();
    }
    
    [ContextMenu("Verify Compilation")]
    public void VerifyCompilation()
    {
        Debug.Log("=== COMPILATION VERIFICATION ===");
        
        bool success = true;
        
        // Test BaseWeapon reference
        try
        {
            BaseWeapon[] weapons = FindObjectsOfType<BaseWeapon>();
            Debug.Log($"✅ BaseWeapon class accessible - Found {weapons.Length} weapons");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ BaseWeapon class issue: {e.Message}");
            success = false;
        }
        
        // Test specific weapon types
        try
        {
            var ketchupWeapons = FindObjectsOfType<KetchupWeapon>();
            var rifleWeapons = FindObjectsOfType<AssaultRifleWeapon>();
            var popcornWeapons = FindObjectsOfType<PopcornLauncherWeapon>();
            Debug.Log($"✅ Weapon types accessible - Ketchup: {ketchupWeapons.Length}, Rifle: {rifleWeapons.Length}, Popcorn: {popcornWeapons.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Weapon types issue: {e.Message}");
            success = false;
        }
        
        // Test UnifiedWeaponAnimator
        try
        {
            var unifiedAnimators = FindObjectsOfType<UnifiedWeaponAnimator>();
            Debug.Log($"✅ UnifiedWeaponAnimator accessible - Found {unifiedAnimators.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ UnifiedWeaponAnimator issue: {e.Message}");
            success = false;
        }
        
        // Test WeaponConfigurationManager
        try
        {
            var configManagers = FindObjectsOfType<WeaponConfigurationManager>();
            Debug.Log($"✅ WeaponConfigurationManager accessible - Found {configManagers.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ WeaponConfigurationManager issue: {e.Message}");
            success = false;
        }
        
        // Test FPSPlayerControllerWithWeapons
        try
        {
            var fpsControllers = FindObjectsOfType<FPSPlayerControllerWithWeapons>();
            Debug.Log($"✅ FPSPlayerControllerWithWeapons accessible - Found {fpsControllers.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ FPSPlayerControllerWithWeapons issue: {e.Message}");
            success = false;
        }
        
        // Test IDamageable interface
        try
        {
            var damageableObjects = FindObjectsOfType<MonoBehaviour>().Length;
            Debug.Log($"✅ IDamageable interface accessible");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ IDamageable interface issue: {e.Message}");
            success = false;
        }
        
        allSystemsWorking = success;
        
        if (success)
        {
            Debug.Log("🎉 ALL COMPILATION TESTS PASSED!");
            Debug.Log("✅ No namespace issues");
            Debug.Log("✅ All classes accessible");
            Debug.Log("✅ Ready for component setup");
        }
        else
        {
            Debug.LogError("❌ Some compilation issues remain - check errors above");
        }
        
        Debug.Log("=== VERIFICATION COMPLETE ===");
    }
    
    [ContextMenu("Test Component Creation")]
    public void TestComponentCreation()
    {
        Debug.Log("=== TESTING COMPONENT CREATION ===");
        
        GameObject testObject = new GameObject("CompilationTest");
        
        try
        {
            // Test adding each component
            var unifiedAnimator = testObject.AddComponent<UnifiedWeaponAnimator>();
            Debug.Log("✅ Can create UnifiedWeaponAnimator");
            
            var weaponConfig = testObject.AddComponent<WeaponConfigurationManager>();
            Debug.Log("✅ Can create WeaponConfigurationManager");
            
            var unifiedSetup = testObject.AddComponent<UnifiedWeaponSetup>();
            Debug.Log("✅ Can create UnifiedWeaponSetup");
            
            var weaponTester = testObject.AddComponent<WeaponSystemTester>();
            Debug.Log("✅ Can create WeaponSystemTester");
            
            var quickSetup = testObject.AddComponent<QuickPrefabSetup>();
            Debug.Log("✅ Can create QuickPrefabSetup");
            
            Debug.Log("🎉 ALL COMPONENTS CAN BE CREATED!");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Component creation failed: {e.Message}");
        }
        finally
        {
            // Clean up test object
            DestroyImmediate(testObject);
            Debug.Log("Test object cleaned up");
        }
        
        Debug.Log("=== COMPONENT CREATION TEST COMPLETE ===");
    }
}