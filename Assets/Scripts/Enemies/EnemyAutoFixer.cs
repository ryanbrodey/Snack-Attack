using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Automatically fixes enemy configurations in the scene
/// </summary>
public class EnemyAutoFixer : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("The player the enemies should chase")]
    public Transform playerTarget;
    
    [Tooltip("How far enemies will chase the player")]
    public float chaseRange = 1000f;
    
    [Tooltip("Movement speed for all enemies")]
    public float movementSpeed = 5f;
    
    [Tooltip("Auto-find player by tag 'Player'")]
    public bool autoFindPlayer = true;

    void Start()
    {
        if (autoFindPlayer && playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Fix All Enemies in Scene")]
    public void FixAllEnemiesInScene()
    {
        // Auto-find player if needed
        if (playerTarget == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // Try finding by name
                player = GameObject.Find("FPS_Player_Unified_v2");
            }
            if (player != null)
            {
                playerTarget = player.transform;
                Debug.Log($"Found player: {player.name}");
            }
            else
            {
                Debug.LogError("Could not find player! Please assign manually.");
                return;
            }
        }

        int fixedCount = 0;

        // Fix all KiwiAI enemies
        KiwiAI[] kiwis = FindObjectsOfType<KiwiAI>();
        foreach (KiwiAI kiwi in kiwis)
        {
            kiwi.player = playerTarget;
            kiwi.chaseRange = chaseRange;
            
            NavMeshAgent agent = kiwi.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = movementSpeed;
            }
            
            EnemyHealth health = kiwi.GetComponent<EnemyHealth>();
            if (health != null && health.Health <= 0)
            {
                health.ResetHealth();
            }
            
            EditorUtility.SetDirty(kiwi);
            Debug.Log($"Fixed Kiwi: {kiwi.gameObject.name} - Player: {playerTarget.name}, Chase: {chaseRange}, Speed: {movementSpeed}");
            fixedCount++;
        }

        // Fix all ChiliAI enemies
        ChiliAI[] chilis = FindObjectsOfType<ChiliAI>();
        foreach (ChiliAI chili in chilis)
        {
            chili.player = playerTarget;
            chili.chaseRange = chaseRange;
            
            NavMeshAgent agent = chili.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = movementSpeed;
            }
            
            EnemyHealth health = chili.GetComponent<EnemyHealth>();
            if (health != null && health.Health <= 0)
            {
                health.ResetHealth();
            }
            
            EditorUtility.SetDirty(chili);
            Debug.Log($"Fixed Chili: {chili.gameObject.name} - Player: {playerTarget.name}, Chase: {chaseRange}, Speed: {movementSpeed}");
            fixedCount++;
        }

        // Fix all EggAI enemies
        EggAI[] eggs = FindObjectsOfType<EggAI>();
        foreach (EggAI egg in eggs)
        {
            egg.player = playerTarget;
            egg.chaseRange = chaseRange;
            
            NavMeshAgent agent = egg.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = movementSpeed;
            }
            
            EnemyHealth health = egg.GetComponent<EnemyHealth>();
            if (health != null && health.Health <= 0)
            {
                health.ResetHealth();
            }
            
            EditorUtility.SetDirty(egg);
            Debug.Log($"Fixed Egg: {egg.gameObject.name} - Player: {playerTarget.name}, Chase: {chaseRange}, Speed: {movementSpeed}");
            fixedCount++;
        }

        Debug.Log($"<color=green>Successfully fixed {fixedCount} enemies!</color>");
        
        // Mark scene as dirty so Unity knows to save changes
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyAutoFixer))]
public class EnemyAutoFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EnemyAutoFixer fixer = (EnemyAutoFixer)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Click the button below to automatically fix all enemies in the scene!", MessageType.Info);
        
        if (GUILayout.Button("Fix All Enemies", GUILayout.Height(40)))
        {
            fixer.FixAllEnemiesInScene();
        }
    }
}
#endif

