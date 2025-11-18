using UnityEngine;
using SnackAttack.Weapons;

namespace SnackAttack.Testing
{
    /// <summary>
    /// Simple test enemy for demonstrating weapon damage
    /// </summary>
    public class TestEnemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private float damageFlashDuration = 0.2f;
        
        private Renderer enemyRenderer;
        private bool isFlashing = false;
        
        public float Health => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;
        
        private void Awake()
        {
            currentHealth = maxHealth;
            enemyRenderer = GetComponent<Renderer>();
        }
        
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
            
            // Flash red when taking damage
            if (enemyRenderer != null && !isFlashing)
            {
                StartCoroutine(FlashDamage());
            }
            
            // Check if dead
            if (!IsAlive)
            {
                Die();
            }
        }
        
        private System.Collections.IEnumerator FlashDamage()
        {
            isFlashing = true;
            
            // Change to damage color
            if (enemyRenderer != null)
                enemyRenderer.material.color = damageColor;
            
            yield return new WaitForSeconds(damageFlashDuration);
            
            // Return to normal color
            if (enemyRenderer != null)
                enemyRenderer.material.color = normalColor;
            
            isFlashing = false;
        }
        
        private void Die()
        {
            Debug.Log($"{gameObject.name} has died!");
            
            // You can add death effects, animations, etc. here
            // For now, just disable the object
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Reset the enemy to full health
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            gameObject.SetActive(true);
            
            if (enemyRenderer != null)
                enemyRenderer.material.color = normalColor;
        }
    }
}
