using UnityEngine;

/// <summary>
/// Reusable health component for any enemy (Kiwi, Egg, Chili, etc.).
/// Attach this to the root GameObject that should take damage.
/// Works with existing weapons that call IDamageable.TakeDamage().
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Death Behaviour")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private GameObject deathVFX;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Point Rewards")]
    [SerializeField] private int pointsPerHit = 1;
    [SerializeField] private int killBonusPoints = 50;

    public float Health => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;

    private PlayerPoints playerPoints;
    private AudioSource audioSource;

    private void Awake()
    {
        currentHealth = maxHealth;

        Debug.Log($"[EnemyHealth] {name} initialized with {maxHealth} HP");

        FindPlayerPoints();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D audio
            Debug.Log($"[EnemyHealth] AudioSource added to {name}");
        }
    }

    private void FindPlayerPoints()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerPoints = player.GetComponent<PlayerPoints>();

        if (playerPoints == null)
            playerPoints = FindObjectOfType<PlayerPoints>();

        if (playerPoints == null)
            Debug.LogWarning($"[EnemyHealth] PlayerPoints NOT FOUND for {name}");
    }

    /// <summary>
    /// Called by bullets / weapons via the IDamageable interface.
    /// </summary>
    public void TakeDamage(float damage)
    {
        Debug.Log($"[EnemyHealth] {name} TakeDamage called with {damage}");

        if (!IsAlive)
        {
            Debug.Log($"[EnemyHealth] {name} already dead, ignoring damage");
            return;
        }

        float previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        float actualDamage = previousHealth - currentHealth;

        Debug.Log($"[EnemyHealth] {name} HIT! Took {actualDamage} damage. HP now {currentHealth}/{maxHealth}");

        // 🔊 HIT SOUND
        if (actualDamage > 0f)
        {
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
                Debug.Log($"[EnemyHealth] Hit sound played on {name}");
            }
            else
            {
                Debug.LogWarning($"[EnemyHealth] Hit sound is NULL on {name}");
            }
        }

        // 🎯 POINTS
        if (playerPoints != null && actualDamage > 0 && pointsPerHit > 0)
        {
            playerPoints.points += pointsPerHit;
            Debug.Log($"[EnemyHealth] Awarded {pointsPerHit} hit points for {name}");
        }

        if (!IsAlive)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[EnemyHealth] {name} DIED");

        // 💀 DEATH ANIMATION
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
            Debug.Log($"[EnemyHealth] Death animation triggered on {name}");
        }

        // 🏆 KILL POINTS
        if (playerPoints != null && killBonusPoints > 0)
        {
            playerPoints.points += killBonusPoints;
            Debug.Log($"[EnemyHealth] Awarded {killBonusPoints} kill points for {name}");
        }

        // 🔊 DEATH SOUND
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            Debug.Log($"[EnemyHealth] Death sound played on {name}");
        }
        else
        {
            Debug.LogWarning($"[EnemyHealth] Death sound is NULL on {name}");
        }

        // 💥 VFX
        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
            Debug.Log($"[EnemyHealth] Death VFX spawned for {name}");
        }

        if (destroyOnDeath)
        {
            Debug.Log($"[EnemyHealth] Destroying {name}");
            Destroy(gameObject, 2f); // wait for death animation to play
        }
        else
        {
            Debug.Log($"[EnemyHealth] Disabling {name}");
            gameObject.SetActive(false);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        gameObject.SetActive(true);
        Debug.Log($"[EnemyHealth] {name} health reset to {maxHealth}");
    }
}
