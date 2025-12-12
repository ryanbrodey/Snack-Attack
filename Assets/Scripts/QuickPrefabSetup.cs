using UnityEngine;

/// <summary>
/// Quick setup script to add all necessary components to your FPS_Player_Unified prefab
/// This will automatically add and configure all the unified weapon system components
/// </summary>
[AddComponentMenu("Snack Attack/Quick Prefab Setup")]
public class QuickPrefabSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool setupOnStart = false;
    
    [Header("Status")]
    [SerializeField] private bool hasUnifiedAnimator = false;
    [SerializeField] private bool hasWeaponConfig = false;
    [SerializeField] private bool hasUnifiedSetup = false;
    [SerializeField] private bool hasTester = false;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupAllComponents();
        }
        else
        {
            CheckCurrentComponents();
        }
    }
    
    [ContextMenu("Setup All Components")]
    public void SetupAllComponents()
    {
        Debug.Log("=== QUICK PREFAB SETUP ===");
        Debug.Log("Adding all unified weapon system components...");
        
        // Add UnifiedWeaponAnimator
        if (GetComponent<UnifiedWeaponAnimator>() == null)
        {
            var unifiedAnimator = gameObject.AddComponent<UnifiedWeaponAnimator>();
            Debug.Log("✅ Added UnifiedWeaponAnimator");
            
            // Try to auto-assign the animator
            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                unifiedAnimator.unifiedAnimator = animator;
                Debug.Log("✅ Auto-assigned Animator reference");
            }
        }
        
        // Add WeaponConfigurationManager
        if (GetComponent<WeaponConfigurationManager>() == null)
        {
            gameObject.AddComponent<WeaponConfigurationManager>();
            Debug.Log("✅ Added WeaponConfigurationManager");
        }
        
        // Add UnifiedWeaponSetup
        if (GetComponent<UnifiedWeaponSetup>() == null)
        {
            gameObject.AddComponent<UnifiedWeaponSetup>();
            Debug.Log("✅ Added UnifiedWeaponSetup");
        }
        
        // Add WeaponSystemTester (optional)
        if (GetComponent<WeaponSystemTester>() == null)
        {
            gameObject.AddComponent<WeaponSystemTester>();
            Debug.Log("✅ Added WeaponSystemTester");
        }
        
        // Update FPSPlayerController references
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        if (fpsController != null)
        {
            var unifiedAnimator = GetComponent<UnifiedWeaponAnimator>();
            if (unifiedAnimator != null)
            {
                fpsController.unifiedWeaponAnimator = unifiedAnimator;
                Debug.Log("✅ Linked UnifiedWeaponAnimator to FPSController");
            }
        }
        
        CheckCurrentComponents();
        
        Debug.Log("=== SETUP COMPLETE ===");
        Debug.Log("All components have been added to your prefab!");
        Debug.Log("You can now:");
        Debug.Log("1. Create the unified animator controller");
        Debug.Log("2. Test weapon switching with keys 1, 2, 3");
        Debug.Log("3. Use the setup scripts to configure everything");
    }
    
    [ContextMenu("Check Current Components")]
    public void CheckCurrentComponents()
    {
        hasUnifiedAnimator = GetComponent<UnifiedWeaponAnimator>() != null;
        hasWeaponConfig = GetComponent<WeaponConfigurationManager>() != null;
        hasUnifiedSetup = GetComponent<UnifiedWeaponSetup>() != null;
        hasTester = GetComponent<WeaponSystemTester>() != null;
        
        Debug.Log("=== COMPONENT STATUS ===");
        Debug.Log($"UnifiedWeaponAnimator: {(hasUnifiedAnimator ? "✅ Present" : "❌ Missing")}");
        Debug.Log($"WeaponConfigurationManager: {(hasWeaponConfig ? "✅ Present" : "❌ Missing")}");
        Debug.Log($"UnifiedWeaponSetup: {(hasUnifiedSetup ? "✅ Present" : "❌ Missing")}");
        Debug.Log($"WeaponSystemTester: {(hasTester ? "✅ Present" : "❌ Missing")}");
        
        var fpsController = GetComponent<FPSPlayerControllerWithWeapons>();
        bool hasFPSController = fpsController != null;
        Debug.Log($"FPSPlayerControllerWithWeapons: {(hasFPSController ? "✅ Present" : "❌ Missing")}");
        
        if (hasFPSController && hasUnifiedAnimator)
        {
            bool isLinked = fpsController.unifiedWeaponAnimator != null;
            Debug.Log($"Components Linked: {(isLinked ? "✅ Yes" : "❌ No")}");
        }
        
        int totalComponents = (hasUnifiedAnimator ? 1 : 0) + (hasWeaponConfig ? 1 : 0) + 
                             (hasUnifiedSetup ? 1 : 0) + (hasTester ? 1 : 0);
        Debug.Log($"Setup Progress: {totalComponents}/4 components ready");
        
        if (totalComponents == 4)
        {
            Debug.Log("🎉 All components ready! Your prefab is fully set up!");
        }
    }
    
    [ContextMenu("Remove All Components")]
    public void RemoveAllComponents()
    {
        Debug.Log("Removing all unified weapon system components...");
        
        var components = new System.Type[]
        {
            typeof(UnifiedWeaponAnimator),
            typeof(WeaponConfigurationManager),
            typeof(UnifiedWeaponSetup),
            typeof(WeaponSystemTester)
        };
        
        foreach (var componentType in components)
        {
            var component = GetComponent(componentType);
            if (component != null)
            {
                DestroyImmediate(component);
                Debug.Log($"Removed {componentType.Name}");
            }
        }
        
        CheckCurrentComponents();
    }
    
    void Update()
    {
        // Quick help
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShowQuickHelp();
        }
    }
    
    void ShowQuickHelp()
    {
        Debug.Log("=== QUICK SETUP HELP ===");
        Debug.Log("F2 - Show this help");
        Debug.Log("Right-click this script and use:");
        Debug.Log("• 'Setup All Components' - Add all needed components");
        Debug.Log("• 'Check Current Components' - See what's installed");
        Debug.Log("• 'Remove All Components' - Clean slate");
        Debug.Log("");
        Debug.Log("After setup, follow the UNIFIED_ANIMATOR_SETUP_GUIDE.md");
    }
}