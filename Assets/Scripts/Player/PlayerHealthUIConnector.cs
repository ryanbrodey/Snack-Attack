using UnityEngine;

/// <summary>
/// Automatically connects the Hearts UI GameObjects to the PlayerHealth component at runtime.
/// This solves the "cross-scene reference" issue where prefabs can't directly reference scene objects.
/// 
/// HOW TO USE:
/// 1. Attach this script to the HeartsUI GameObject in your scene (under Canvas)
/// 2. Make sure the player has the "Player" tag
/// 3. Make sure HeartsUI has children named: "Heart", "Heart (1)", "Heart (2)", "Heart (3)", "Heart (4)"
/// </summary>
public class PlayerHealthUIConnector : MonoBehaviour
{
    [Header("Optional - Leave Empty for Auto-Find")]
    [Tooltip("Optional: Manually assign the player if auto-find doesn't work")]
    public GameObject player;

    void Start()
    {
        ConnectHeartsToPlayer();
    }

    void ConnectHeartsToPlayer()
    {
        // Find the player if not manually assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Debug.LogError("[PlayerHealthUIConnector] Player GameObject with 'Player' tag not found in scene!");
            return;
        }

        // Get the PlayerHealth component from the player
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("[PlayerHealthUIConnector] PlayerHealth component not found on player! Make sure the player has the PlayerHealth script attached.");
            return;
        }

        // This script should be attached to the HeartsUI GameObject
        Transform heartsContainer = transform;

        // Initialize the hearts array with 5 elements
        playerHealth.hearts = new GameObject[5];

        // Find and assign each heart GameObject by name
        playerHealth.hearts[0] = FindChildByName(heartsContainer, "Heart");
        playerHealth.hearts[1] = FindChildByName(heartsContainer, "Heart (1)");
        playerHealth.hearts[2] = FindChildByName(heartsContainer, "Heart (2)");
        playerHealth.hearts[3] = FindChildByName(heartsContainer, "Heart (3)");
        playerHealth.hearts[4] = FindChildByName(heartsContainer, "Heart (4)");

        // Verify all hearts were found and log results
        bool allFound = true;
        for (int i = 0; i < playerHealth.hearts.Length; i++)
        {
            if (playerHealth.hearts[i] == null)
            {
                Debug.LogError($"[PlayerHealthUIConnector] Heart at index {i} not found! Expected child name: {GetExpectedHeartName(i)}");
                allFound = false;
            }
            else
            {
                Debug.Log($"[PlayerHealthUIConnector] Connected Heart {i}: {playerHealth.hearts[i].name}");
            }
        }

        if (allFound)
        {
            Debug.Log("<color=green>[PlayerHealthUIConnector] ✓ Successfully connected all 5 hearts to PlayerHealth!</color>");
        }
        else
        {
            Debug.LogWarning("[PlayerHealthUIConnector] Some hearts were not found. Check the names of the heart GameObjects under HeartsUI.");
        }
    }

    /// <summary>
    /// Helper method to find a child GameObject by name
    /// </summary>
    GameObject FindChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Helper to get the expected heart name for error messages
    /// </summary>
    string GetExpectedHeartName(int index)
    {
        if (index == 0) return "Heart";
        return $"Heart ({index})";
    }
}

