using UnityEngine;

/// <summary>
/// Reusable health component for any enemy (Kiwi, Egg, Chili, etc.).
/// Attach this to the root GameObject that should take damage.
/// Works with existing weapons that call IDamageable.TakeDamage().
///
/// NOTE:
/// This implements the global IDamageable interface defined in the
/// Player-testing scripts, which is the one used by BulletController.
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Death Behaviour")]
    [Tooltip("If true, the GameObject will be destroyed on death. If false, it will just be disabled.")]
    [SerializeField] private bool destroyOnDeath = true;

    [Tooltip("Optional death effect (e.g. particle system prefab) spawned at the enemy's position on death.")]
    [SerializeField] private GameObject deathVFX;

    [Header("Point Rewards")]
    [Tooltip("Fixed points awarded per hit (regardless of damage amount, set to 0 to disable damage rewards)")]
    [SerializeField] private int pointsPerHit = 1;
    
    [Tooltip("Bonus points awarded on kill")]
    [SerializeField] private int killBonusPoints = 50;

    public float Health => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;

    private PlayerPoints playerPoints;

    private void Awake()
    {
        // Start at full health
        currentHealth = maxHealth;
        
        // Find PlayerPoints in the scene
        FindPlayerPoints();
    }

    private void FindPlayerPoints()
    {
        // Try to find PlayerPoints on the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPoints = player.GetComponent<PlayerPoints>();
        }
        
        // Fallback: search entire scene
        if (playerPoints == null)
        {
            playerPoints = FindObjectOfType<PlayerPoints>();
        }

        if (playerPoints == null)
        {
            Debug.LogWarning($"[EnemyHealth] PlayerPoints not found! Points will not be awarded for {gameObject.name}");
        }
    }

    /// <summary>
    /// Called by bullets / weapons via the IDamageable interface.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        float previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        float actualDamage = previousHealth - currentHealth;

        Debug.Log($"{name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Award points for hitting enemy (if enabled) - fixed amount per hit
        if (playerPoints != null && actualDamage > 0 && pointsPerHit > 0)
        {
            playerPoints.points += pointsPerHit;
            Debug.Log($"[Points] Awarded {pointsPerHit} points for hitting {name}");
        }

        if (!IsAlive)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles enemy death (VFX, disabling/destroying, and any hooks).
    /// </summary>
    private void Die()
    {
        // Award kill bonus points
        if (playerPoints != null && killBonusPoints > 0)
        {
            playerPoints.points += killBonusPoints;
            Debug.Log($"[Points] Awarded {killBonusPoints} bonus points for killing {name}");
        }

        // Spawn death VFX if assigned
        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }

        // TODO: Hook into score / wave systems here if needed (e.g. notify a spawner).

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Optional helper to fully heal and re-enable the enemy (useful for pooling).
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }
}


