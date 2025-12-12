using UnityEngine;

/// <summary>
/// Simple test script to verify the unified weapon system is working
/// Attach to any GameObject in the scene
/// </summary>
[AddComponentMenu("Snack Attack/Weapon System Tester")]
public class WeaponSystemTester : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableTesting = true;
    public KeyCode testKey = KeyCode.T;
    
    private FPSPlayerControllerWithWeapons playerController;
    private UnifiedWeaponAnimator unifiedAnimator;
    
    void Start()
    {
        // Find the player controller
        playerController = FindObjectOfType<FPSPlayerControllerWithWeapons>();
        if (playerController != null)
        {
            unifiedAnimator = playerController.unifiedWeaponAnimator;
            Debug.Log("WeaponSystemTester: Found player controller and unified animator");
        }
        else
        {
            Debug.LogWarning("WeaponSystemTester: Could not find FPSPlayerControllerWithWeapons!");
        }
    }
    
    void Update()
    {
        if (!enableTesting || playerController == null) return;
        
        // Test weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestWeaponSwitch(0, "Ketchup Gun");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestWeaponSwitch(1, "Assault Rifle");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestWeaponSwitch(2, "Popcorn Shotgun");
        }
        
        // Manual test trigger
        if (Input.GetKeyDown(testKey))
        {
            RunFullSystemTest();
        }
        
        // Display current status
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplaySystemInfo();
        }
    }
    
    void TestWeaponSwitch(int weaponIndex, string weaponName)
    {
        Debug.Log($"=== Testing Weapon Switch to {weaponName} (Index: {weaponIndex}) ===");
        
        if (playerController.weapons != null && weaponIndex < playerController.weapons.Length)
        {
            playerController.SwitchToWeapon(weaponIndex);
            
            // Verify the switch
            if (playerController.CurrentWeaponIndex == weaponIndex)
            {
                Debug.Log($"✅ Weapon switch successful! Current weapon: {playerController.CurrentWeapon?.WeaponName}");
                
                if (unifiedAnimator != null)
                {
                    Debug.Log($"✅ Unified animator weapon type: {unifiedAnimator.CurrentWeaponType}");
                }
            }
            else
            {
                Debug.LogError($"❌ Weapon switch failed! Expected index: {weaponIndex}, Actual: {playerController.CurrentWeaponIndex}");
            }
        }
        else
        {
            Debug.LogError($"❌ Weapon at index {weaponIndex} not found!");
        }
    }
    
    void RunFullSystemTest()
    {
        Debug.Log("=== RUNNING FULL WEAPON SYSTEM TEST ===");
        
        if (playerController == null)
        {
            Debug.LogError("❌ Player controller not found!");
            return;
        }
        
        // Test 1: Check weapon array
        Debug.Log($"Test 1: Weapon Array - Found {playerController.weapons?.Length ?? 0} weapons");
        if (playerController.weapons != null)
        {
            for (int i = 0; i < playerController.weapons.Length; i++)
            {
                var weapon = playerController.weapons[i];
                Debug.Log($"  Weapon {i}: {weapon?.WeaponName ?? "NULL"} - Active: {weapon?.gameObject.activeInHierarchy ?? false}");
            }
        }
        
        // Test 2: Check unified animator
        Debug.Log($"Test 2: Unified Animator - {(unifiedAnimator != null ? "Found" : "Missing")}");
        if (unifiedAnimator != null)
        {
            Debug.Log($"  Current weapon type: {unifiedAnimator.CurrentWeaponType}");
            Debug.Log($"  Has WeaponType parameter: {unifiedAnimator.HasWeaponTypeParameter}");
        }
        
        // Test 3: Check camera anchor
        Debug.Log($"Test 3: Camera Anchor - {(playerController.cameraAnchor != null ? "Found" : "Missing")}");
        if (playerController.cameraAnchor != null)
        {
            Debug.Log($"  Position: {playerController.cameraAnchor.localPosition}");
            Debug.Log($"  Rotation: {playerController.cameraAnchor.localEulerAngles}");
        }
        
        // Test 4: Test weapon switching
        Debug.Log("Test 4: Testing weapon switching...");
        StartCoroutine(TestWeaponSwitchingSequence());
    }
    
    System.Collections.IEnumerator TestWeaponSwitchingSequence()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Test switching to each weapon
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"  Switching to weapon {i}...");
            playerController.SwitchToWeapon(i);
            yield return new WaitForSeconds(1f);
            
            // Verify switch
            bool success = playerController.CurrentWeaponIndex == i;
            Debug.Log($"  Weapon {i} switch: {(success ? "✅ SUCCESS" : "❌ FAILED")}");
        }
        
        Debug.Log("=== WEAPON SYSTEM TEST COMPLETE ===");
    }
    
    void DisplaySystemInfo()
    {
        Debug.Log("=== CURRENT SYSTEM STATUS ===");
        
        if (playerController != null)
        {
            Debug.Log($"Current Weapon Index: {playerController.CurrentWeaponIndex}");
            Debug.Log($"Current Weapon Name: {playerController.CurrentWeapon?.WeaponName ?? "None"}");
            Debug.Log($"Is Grounded: {playerController.IsGrounded}");
            Debug.Log($"Move Input: {playerController.MoveInput}");
        }
        
        if (unifiedAnimator != null)
        {
            Debug.Log($"Unified Animator Weapon Type: {unifiedAnimator.CurrentWeaponType}");
            
            if (unifiedAnimator.Animator != null)
            {
                var stateInfo = unifiedAnimator.GetCurrentStateInfo();
                Debug.Log($"Current Animation State: {stateInfo.shortNameHash}");
            }
        }
    }
    
    void OnGUI()
    {
        if (!enableTesting) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Weapon System Tester", GUI.skin.box);
        GUILayout.Label($"Press 1, 2, 3 to switch weapons");
        GUILayout.Label($"Press {testKey} for full system test");
        GUILayout.Label($"Press I for system info");
        
        if (playerController != null)
        {
            GUILayout.Label($"Current: {playerController.CurrentWeapon?.WeaponName ?? "None"}");
            GUILayout.Label($"Index: {playerController.CurrentWeaponIndex}");
        }
        
        GUILayout.EndArea();
    }
}