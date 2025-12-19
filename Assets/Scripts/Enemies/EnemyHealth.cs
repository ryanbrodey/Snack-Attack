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

        FindPlayerPoints();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    private void FindPlayerPoints()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerPoints = player.GetComponent<PlayerPoints>();

        if (playerPoints == null)
            playerPoints = FindObjectOfType<PlayerPoints>();
    }

    // Called by bullets/weapons
    public void TakeDamage(float damage)
    {
        if (!IsAlive)
        {
            return;
        }

        float previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        float actualDamage = previousHealth - currentHealth;

        // Hit sound
        if (actualDamage > 0f)
        {
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }

        // Points
        if (playerPoints != null && actualDamage > 0 && pointsPerHit > 0)
        {
            playerPoints.points += pointsPerHit;
        }

        if (!IsAlive)
        {
            Die();
        }
    }

    private void Die()
    {
        // Death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Kill points
        if (playerPoints != null && killBonusPoints > 0)
        {
            playerPoints.points += killBonusPoints;
        }

        // Death sound
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // VFX
        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, 2f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        gameObject.SetActive(true);
    }
}
