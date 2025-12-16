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

    public float Health => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;

    private void Awake()
    {
        // Start at full health
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Called by bullets / weapons via the IDamageable interface.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        Debug.Log($"{name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

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


